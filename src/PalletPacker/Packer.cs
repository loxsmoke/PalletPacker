using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq.Extensions;

namespace PalletPacker
{
    /// <summary>
    /// Based on algorithm from
    /// http://www.dtic.mil/dtic/tr/fulltext/u2/a391201.pdf
    /// Packs a set of boxes into a "bix box" pallet using 2d packing layer by layer.
    /// </summary>
    public class Packer
    {
        /// <summary>
        /// Packed pallet data.
        /// </summary>
        public class PackedPallet
        {
            /// <summary>
            /// Dimensions of the pallet.
            /// </summary>
            public Point3D PalletDimensions;
            /// <summary>
            /// The list of boxes that could not be packed.
            /// </summary>
            public List<Box> NotPackedBoxes;
            /// <summary>
            /// The list of boxes that were packed.
            /// </summary>
            public List<Box> PackedBoxes;
            /// <summary>
            /// Total volume of packed boxes.
            /// </summary>
            public long PackedVolume;
            /// <summary>
            /// True if all boxes were packed
            /// </summary>
            public bool AllBoxesPacked => NotPackedBoxes.Count == 0;
            /// <summary>
            /// Pack the specified box.
            /// </summary>
            /// <param name="box"></param>
            public void PackBox(Box box)
            {
                PackedBoxes.Add(box);
                NotPackedBoxes.Remove(box);
                PackedVolume += box.Dimensions.Volume;
            }
        }

        /// <summary>
        /// The box selected for packing.
        /// </summary>
        public class SelectedBox
        {
            /// <summary>
            /// Found box.
            /// </summary>
            public Box Box;
            /// <summary>
            /// Packed box dimensions. If orientation of the box did not change during packing
            /// this value is the same as box Dimensions field.
            /// </summary>
            public Point3D PackedDimensions;
            /// <summary>
            /// Absolute difference between ideal box and currently best box.
            /// Parameter used in next best suited box search. Trying to minimize it.
            /// </summary>
            public Point3D DeltaFromIdeal;
            /// <summary>
            /// True if current box fits in layer. Boxes that fit in layer are better
            /// than not fitting ones.
            /// </summary>
            public bool FitsInLayer;

            /// <summary>
            /// Check if the new box evaluation parameters are better than current ones.  
            /// </summary>
            /// <param name="newFitsInLayer"></param>
            /// <param name="newDelta"></param>
            /// <returns>True if new box is better. False if not.</returns>
            public bool IsBetterFit(bool newFitsInLayer, Point3D newDelta)
            {
                if (!FitsInLayer && newFitsInLayer) return true;
                if (FitsInLayer != newFitsInLayer) return false;

                // check if new delta is better
                return newDelta.Y < DeltaFromIdeal.Y ||
                       newDelta.Y == DeltaFromIdeal.Y &&
                       (newDelta.X < DeltaFromIdeal.X ||
                        newDelta.X == DeltaFromIdeal.X &&
                        newDelta.Z < DeltaFromIdeal.Z);
            }
        }

        /// <summary>
        /// Packing layer.
        /// </summary>
        public class Layer
        {
            /// <summary>
            /// Layer thickness. Y dimension of the layer.
            /// </summary>
            public long LayerY;
            /// <summary>
            /// The total sum of differences of all boxes between this layer thickness and
            /// dimension of the box that is closest to the layer thickness.
            /// </summary>
            public long MinDifferenceTotal;
            /// </inheritdoc>
            public override string ToString()
            {
                return $"dim={LayerY} val={MinDifferenceTotal}";
            }
        }

        /// <summary>
        /// The list of all possible packing layers created from each possible box dimension
        /// </summary>
        private List<Layer> layers;

        private bool quit;
        public int iterations;
        private long subLayerThickness, sublayerZlimit;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boxList"></param>
        /// <param name="palletSize">Pallet dimensions.</param>
        /// <returns></returns>
        public PackedPallet Pack(List<Box> boxList, Point3D palletSize)
        {
            if (boxList == null || boxList.Count == 0) return null;
            quit = false;
            iterations = 0;
            PackedPallet bestPackedPallet = null;
            // Try packing pallet in all orientations
            foreach (var rotatedPallet in palletSize.AllRotations)
            {
                layers = CreateLayers(boxList, rotatedPallet);
                // Try packing pallet starting with different layer
                foreach (var layer in layers)
                {
                    if (quit) break;
                    ++iterations;
                    var packedPallet = PackPallet(layer, boxList, rotatedPallet);
                    if (quit) break;
                    if (bestPackedPallet == null ||
                        packedPallet.PackedVolume > bestPackedPallet.PackedVolume)
                    {
                        bestPackedPallet = packedPallet;
                    }
                    if (bestPackedPallet.AllBoxesPacked) break;
                }

                if (bestPackedPallet.AllBoxesPacked) break;

                // If pallet is a cube then rotating it makes no sense. 
                // Just exit after first packing attempt 
                if (palletSize.IsCube) break;
            }
            return bestPackedPallet;
        }

        /// <summary>
        /// Generate the list of all possible layers that can fit in the pallet.
        /// All box dimensions are used when generating layers. For example if 
        /// box dimensions are 1,2,3 then three layers could be created and if box
        /// dimensions are 1,2,2 then only two layers are possible.
        /// </summary>
        /// <param name="boxList"></param>
        /// <param name="pallet"></param>
        /// <returns>The list of layers</returns>
        public static List<Layer> CreateLayers(List<Box> boxList, Point3D pallet)
        {
            var layers = new List<Layer>();
            var layerThickness = new HashSet<long>();
            foreach (var box in boxList)
            {
                // Try all coordinates of the box
                foreach (var checkDim in box.Dimensions.YDimensionVariants)
                {
                    // If box does not fit in the pallet then ignore that box
                    if (checkDim.Y > pallet.Y ||
                        ((checkDim.X > pallet.X ||
                          checkDim.Z > pallet.Z) &&
                         (checkDim.Z > pallet.X ||
                          checkDim.X > pallet.Z))) continue;
                    // Do not add layers of the same thickness
                    if (layerThickness.Contains(checkDim.Y)) continue;

                    layers.Add(new Layer()
                    {
                        LayerY = checkDim.Y,
                        MinDifferenceTotal = CalculateMinDiffTotal(boxList, box, checkDim.Y)
                    });
                    layerThickness.Add(checkDim.Y);
                }
            }
            return layers.OrderBy(layer => layer.MinDifferenceTotal).ToList();
        }

        /// <summary>
        /// Pack boxes into a pallet starting with specified layer thickness.
        /// </summary>
        /// <param name="startLayer">The layer to start with</param>
        /// <param name="boxList">All boxes to pack</param>
        /// <param name="palletDimensions">Dimensions of the pallet</param>
        /// <returns>Packed pallet object</returns>
        PackedPallet PackPallet(Layer startLayer, List<Box> boxList, Point3D palletDimensions)
        {
            var pallet = new PackedPallet()
            {
                PalletDimensions = palletDimensions,
                NotPackedBoxes = boxList.Select(box => box.Clone()).ToList(),
                PackedBoxes = new List<Box>()
            };

            // Start building layers at Y=0
            var layerBottomY = 0L;
            var layerThickness = startLayer.LayerY;
            do
            {
                subLayerThickness = 0;
                var oldLayerThickness = layerThickness;
                layerThickness = PackLayer(pallet, layerBottomY, layerThickness, palletDimensions.Y - layerBottomY, palletDimensions.Z);
                if (pallet.AllBoxesPacked) break;
                if (subLayerThickness != 0 && !quit)
                {
                    // Pack the special case where layer starts at oldLayerThickness but later
                    // becomes thicker to accomodate taller boxes
                    PackLayer(pallet, layerBottomY + oldLayerThickness,
                        subLayerThickness,
                        layerThickness - oldLayerThickness,
                        sublayerZlimit);
                    if (pallet.AllBoxesPacked) break;
                }
                // This layer was packed. Raise the level by packed layer thickness
                layerBottomY += layerThickness;
                // Find the next layer to pack
                layerThickness = FindLayer(pallet.NotPackedBoxes, palletDimensions.SubtractY(layerBottomY));
                if (layerThickness == 0) break;
            } while (!quit);
            return pallet;
        }

        /// <summary>
        /// Find the next layer to pack given the list of remaining boxes and the space 
        /// in the pallet.
        /// This function tries to find the layer thickness that would have the maximum "smoothness".
        /// i.e. the difference between selected layer thickness and total diff 
        /// between layer thickness and remaining boxes would be minimal.
        /// </summary>
        /// <param name="notPackedBoxes">The list of boxes that still need packing</param>
        /// <param name="maxSpaceRemaining">The space that is still available</param>
        /// <returns>The next layer thickness or 0 if suitable boxes do not exist</returns>
        long FindLayer(List<Box> notPackedBoxes, Point3D maxSpaceRemaining)
        {
            var layerThickness = 0L;
            var minDiffTotal = long.MaxValue;
            foreach (var box in notPackedBoxes)
            {
                // Try box rotated in all dimensions
                foreach (var checkDim in box.Dimensions.YDimensionVariants)
                {
                    if (checkDim.Y <= maxSpaceRemaining.Y &&
                        (checkDim.X <= maxSpaceRemaining.X && checkDim.Z <= maxSpaceRemaining.Z ||
                         checkDim.Z < maxSpaceRemaining.X && checkDim.X <= maxSpaceRemaining.Z))
                    {
                        var diffTotal = CalculateMinDiffTotal(notPackedBoxes, box, checkDim.Y);
                        if (diffTotal < minDiffTotal)
                        {
                            minDiffTotal = diffTotal;
                            layerThickness = checkDim.Y;
                        }
                    }
                }
            }
            return layerThickness > maxSpaceRemaining.Y ? 0 : layerThickness;
        }

        /// <summary>
        /// Calculate the total of absolute minimum differences between layerThickness and 
        /// X,Y and Z dimensions of all boxes
        /// </summary>
        /// <param name="boxList">The list of boxes to check</param>
        /// <param name="excludeBox">Exclude this box from checking</param>
        /// <param name="layerThickness">The value we are comparing to</param>
        /// <returns>The total of minimum absolute differences</returns>
        static long CalculateMinDiffTotal(List<Box> boxList, Box excludeBox, long layerThickness)
        {
            if (boxList.Count <= 1) return 0;
            return boxList
                .Where(box => box != excludeBox)
                .Sum(box => box.Dimensions.MinDiff(layerThickness));
        }

        /// <summary>
        /// Pack one layer. If during packing some boxes become taller than layerThickness then 
        /// increase the layer thickness to accomodate them.
        /// </summary>
        /// <param name="pallet">Pallet that is being packed</param>
        /// <param name="layerBottomY">The bottom Y coordinate of this layer</param>
        /// <param name="layerThickness">The thickness (Y) of this layer</param>
        /// <param name="maxThickness">The max thickness possible for this layer in the pallet</param>
        /// <param name="layerZlimit">The Z limit for this layer. Usually the pallet size but in case of
        /// sub-layer it can be less</param>
        /// <returns></returns>
        long PackLayer(PackedPallet pallet, long layerBottomY, long layerThickness, long maxThickness, long layerZlimit)
        {
            if (layerThickness == 0)
            {
                return layerThickness;
            }
            // Create 2d packing line of the layer. 
            // It grows in a horizontal plane with X being width and Z height of this line. 
            var packLine = new PackLine(pallet.PalletDimensions.X);
            while (!quit)
            {
                var valley = packLine.FindValley();

                // Calculate ideal space and maximum space
                var topZ = valley.NoSegmentsOnSides ? layerZlimit :
                    (valley.NoLeftSegment ? valley.Right.Z : valley.Left.Z);
                var idealBox = new Point3D() { X = valley.Width, Y = layerThickness, Z = topZ - valley.Z };
                var maximumBox = idealBox.WithYZ(maxThickness, layerZlimit - valley.Z);

                // Find the box that fits in the remaining space of the pallet
                var selected = FindBox(pallet.NotPackedBoxes, idealBox, maximumBox);
                if (selected == null || !selected.FitsInLayer)
                {
                    if (selected != null &&
                        (subLayerThickness != 0 || valley.NoSegmentsOnSides))
                    {
                        // We have a box but it is too thick for the current layer
                        if (subLayerThickness == 0)
                        {
                            // sub-layer will be from 0 to valleyZ
                            sublayerZlimit = valley.Z;
                        }
                        // sub-layer thickness 
                        subLayerThickness = subLayerThickness + selected.PackedDimensions.Y - layerThickness;
                        // Now thicken the current layer since we have no more small boxes
                        layerThickness = selected.PackedDimensions.Y;
                    }
                    else if (valley.NoSegmentsOnSides)
                    {
                        // No more boxes for this layer
                        break;
                    }
                    else
                    {
                        valley.FillValley();
                        continue;
                    }
                }

                // Record box location in the pallet and pack the box
                selected.Box.PackingData = new PackingData()
                {
                    PackedDimensions = selected.PackedDimensions,
                    PackedLocation = new Point3D() { X = valley.LeftX, Y = layerBottomY, Z = valley.Z }
                };
                pallet.PackBox(selected.Box);

                // Add the box dimensions to the pack line
                packLine.AddZ(valley, selected.PackedDimensions);

                if (pallet.AllBoxesPacked) break;
            }
            return layerThickness;
        }

        /// <summary>
        /// Find the box that fits in maximumBox and if possible is closest to an idealBox
        /// </summary>
        /// <param name="notPackedBoxes">All available boxes</param>
        /// <param name="idealBox">The ideal box we are looking for</param>
        /// <param name="maximumBox">The absolute max dimensions that could be packed</param>
        /// <returns>The box that could be packed</returns>
        SelectedBox FindBox(List<Box> notPackedBoxes, Point3D idealBox, Point3D maximumBox)
        {
            SelectedBox foundBox = null;
            // Go through all boxes
            foreach (var box in notPackedBoxes)
            {
                // Check all orientations of the box that fit in maximum space
                foreach (var boxDimensions in 
                    box.Dimensions.AllRotations.Where(maximumBox.Contains)) // Order different from original. was XZY, YXZ, YZX, ZXY, ZYX
                {
                    var fitsInLayer = boxDimensions.Y <= idealBox.Y;
                    var delta = idealBox.AbsoluteDiff(boxDimensions);   // Calculate absolute coordinate differences
                    // If this box is a better fit then remember it
                    if (foundBox == null || 
                        foundBox.IsBetterFit(fitsInLayer, delta))
                    {
                        foundBox = new SelectedBox()
                        { Box = box, PackedDimensions = boxDimensions, DeltaFromIdeal = delta, FitsInLayer = fitsInLayer };
                    }
                    // If box is cube then checking one box orientation is enough
                    if (box.Dimensions.IsCube) break;
                }
            }
            return foundBox;
        }
    }
}
