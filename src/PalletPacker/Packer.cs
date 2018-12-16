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
    /// </summary>

    public class Packer
    {

        public class PackedPallet
        {
            public Point3D PalletDimensions;
            public List<Box> NotPackedBoxes;
            public List<Box> PackedBoxes;
            public long PackedVolume;
        }
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
        /// X,Z layer packing line
        /// </summary>
        private PackLine packLine;
        /// <summary>
        /// The list of all possible packing layers created from each possible box dimension
        /// </summary>
        private List<Layer> layers;
        private PackedPallet packedPallet, bestPackedPallet;
        private Point3D palletOriginal, pallet;
        private bool quit, allPacked;
        public int iterations;
        private long layerInLayer, layerInLayerZlimit;

        public PackedPallet Pack(List<Box> boxList, Point3D palletSize)
        {
            quit = false;
            allPacked = false;
            iterations = 0;
            palletOriginal = palletSize;
            bestPackedPallet = null;
            foreach (var palletVariant in palletOriginal.AllVariants)
            {
                pallet = palletVariant;
                layers = CreateLayers(boxList, pallet);
                packedPallet = new PackedPallet()
                {
                    PalletDimensions = palletVariant,
                    NotPackedBoxes = boxList.Select(box => box.Clone()).ToList(),
                    PackedBoxes = new List<Box>()
                };
                foreach (var layer in layers)
                {
                    if (quit) break;
                    ++iterations;
                    PackPallet(layer);
                    if (quit) break;
                    if (bestPackedPallet == null ||
                        packedPallet.PackedVolume > bestPackedPallet.PackedVolume)
                    {
                        bestPackedPallet = packedPallet;
                    }
                    if (allPacked) break;
                }

                if (allPacked) break;
                if (palletOriginal.IsCube) break;
            }
            return bestPackedPallet;
        }

        public static List<Layer> CreateLayers(List<Box> boxList, Point3D pallet)
        {
            var layers = new List<Layer>();
            foreach (var box in boxList)
            {
                foreach (var checkDim in box.Dimensions.YDimensionVariants)
                {
                    if (checkDim.Y > pallet.Y ||
                        ((checkDim.X > pallet.X ||
                          checkDim.Z > pallet.Z) &&
                         (checkDim.Z > pallet.X ||
                          checkDim.X > pallet.Z))) continue;
                    if (layers.Count > 0 &&
                        layers.Any(layer => layer.LayerY == checkDim.Y)) continue;

                    layers.Add(new Layer()
                    {
                        LayerY = checkDim.Y,
                        MinDifferenceTotal = CalculateMinDiffTotal(boxList, box, checkDim.Y)
                    });
                }
            }
            return layers.OrderBy(layer => layer.MinDifferenceTotal).ToList();
        }

        void PackPallet(Layer startLayer)
        {
            var layerY = 0L;
            var layerThickness = startLayer.LayerY;
            do
            {
                layerInLayer = 0;
                var oldLayerThickness = layerThickness;
                layerThickness = PackLayer(layerY, layerThickness, pallet.Y - layerY, pallet.Z);
                if (allPacked) break;
                if (layerInLayer != 0 && !quit)
                {
                    PackLayer(layerY + oldLayerThickness,
                        layerInLayer,
                        layerThickness - oldLayerThickness,
                        layerInLayerZlimit);
                    if (allPacked) break;
                }
                layerY += layerThickness;
                layerThickness = FindLayer(pallet.Y - layerY);
                if (layerThickness == 0) break;
            } while (!quit);
        }

        long FindLayer(long maxThickness)
        {
            var layerThickness = 0L;
            var minDiffTotal = long.MaxValue;
            foreach (var box in packedPallet.NotPackedBoxes)
            {
                foreach (var checkDim in box.Dimensions.YDimensionVariants)
                {
                    if (checkDim.Y <= maxThickness &&
                        (checkDim.X <= pallet.X && checkDim.Z <= pallet.Z ||
                         checkDim.Z < pallet.X && checkDim.X <= pallet.Z))
                    {
                        var diffTotal = CalculateMinDiffTotal(packedPallet.NotPackedBoxes, box, checkDim.Y);
                        if (diffTotal < minDiffTotal)
                        {
                            minDiffTotal = diffTotal;
                            layerThickness = checkDim.Y;
                        }
                    }
                }
            }
            return layerThickness > maxThickness ? 0 : layerThickness;
        }

        static long CalculateMinDiffTotal(List<Box> boxList, Box excludeBox, long layerThickness)
        {
            if (boxList.Count <= 1) return 0;
            return boxList
                .Where(box => box != excludeBox)
                .Sum(box => box.Dimensions.MinDiff(layerThickness));
        }

        long PackLayer(long layerY, long layerThickness, long maxThickness, long layerZlimit)
        {
            if (layerThickness == 0)
            {
                return layerThickness;
            }
            packLine = new PackLine(pallet.X);
            while (!quit)
            {
                //if (kbhit()) if (toupper(getch())=='Q'){
                //    quit = 1;
                //    Console.WriteLine("\n\nWait p1ease...\n");
                //}
                var valley = packLine.FindValley();

                // Calculate ideal space and maximum space
                var topZ = valley.NoSegmentsOnSides ? layerZlimit :
                    (valley.NoLeftSegment ? valley.Right.Z : valley.Left.Z);
                var idealBox = new Point3D() { X = valley.Width, Y = layerThickness, Z = topZ - valley.Z };
                var maximumBox = idealBox.WithYZ(maxThickness, layerZlimit - valley.Z);

                // Find box and pack it
                var selected = FindBox(idealBox, maximumBox);
                if (selected == null || !selected.FitsInLayer)
                {
                    if (selected != null &&
                        (layerInLayer != 0 || valley.NoSegmentsOnSides))
                    {
                        if (layerInLayer == 0)
                        {
                            layerInLayerZlimit = valley.Z; // layer in layer bus nuo 0 iki valleyZ
                        }
                        // extra layer thickness 
                        layerInLayer = layerInLayer + selected.PackedDimensions.Y - layerThickness;
                        // naujas layer storis = dezes Y storesne nei layer
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

                PackBox(selected.Box,
                    new Point3D() { X = valley.LeftX, Y = layerY, Z = valley.Z },
                    selected.PackedDimensions);

                packLine.AddZ(valley, selected.PackedDimensions);

                if (allPacked) break;
            }
            return layerThickness;
        }

        SelectedBox FindBox(Point3D idealBox, Point3D maximumBox)
        {
            SelectedBox foundBox = null;
            // Go through all boxes
            foreach (var box in packedPallet.NotPackedBoxes)
            {
                // Check all orientations of the box that fit in maximum space
                foreach (var boxDimensions in 
                    box.Dimensions.AllVariants.Where(maximumBox.Contains)) // Order different from original. was XZY, YXZ, YZX, ZXY, ZYX
                {
                    var fitsInLayer = boxDimensions.Y <= idealBox.Y;    // Check if box fits in layer
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

        void PackBox(Box box, Point3D location, Point3D packedDimensions)
        {
            packedPallet.PackedBoxes.Add(box);
            packedPallet.NotPackedBoxes.Remove(box);
            packedPallet.PackedVolume += box.Dimensions.Volume;

            box.PackingData = new PackingData()
            {
                PackedDimensions = packedDimensions, 
                PackedLocation = location
            };

            if (packedPallet.NotPackedBoxes.Count == 0 ||
                packedPallet.PackedVolume == packedPallet.PalletDimensions.Volume)
            {
                allPacked = true;
            }
        }
    }
}
