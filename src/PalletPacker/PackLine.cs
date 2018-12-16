using System;
using System.Collections.Generic;
using System.Text;

namespace PalletPacker
{
    /// <summary>
    /// Packing line representing the X,Z graph of packed boxes in layer.
    /// Here X is horizontal coordinate and Z is vertical one.
    /// </summary>
    public class PackLine
    {
        /// <summary>
        /// The segment of the packing line.
        /// </summary>
        public class Segment
        {
            /// <summary>
            /// Left and right neighbors of this segment
            /// </summary>
            public Segment Left, Right;

            /// <summary>
            /// The right X coordinate of the segment.
            /// </summary>
            public long RightX;
            /// <summary>
            /// The Z coordinate of the segment.
            /// </summary>
            public long Z;
            /// <summary>
            /// Left x coordinate of the segment.
            /// </summary>
            public long LeftX => NoLeftSegment ? 0 : Left.RightX;
            /// <summary>
            /// Segment width
            /// </summary>
            public long Width => RightX - LeftX;

            /// <summary>
            /// True if segment is the only one.
            /// </summary>
            public bool NoSegmentsOnSides => Left == null && Right == null;
            /// <summary>
            /// True if segment has no left neighbor, i.e. this is the leftmost segment.
            /// </summary>
            public bool NoLeftSegment => Left == null;
            /// <summary>
            /// True if segment has no right neighbor, i.e. this is the rightmost segment.
            /// </summary>
            public bool NoRightSegment => Right == null;

            /// <summary>
            /// Remove segment on the right side and extend current segment.
            /// Keep the same Z value.
            /// </summary>
            protected void RemoveRight()
            {
                RightX = Right.RightX;
                Right = Right?.Right;
                if (Right != null) Right.Left = this;
            }
            /// <summary>
            /// Remove this element from the list.
            /// If segment on the left exists then extend left Z. Otherwise extend right Z.
            /// </summary>
            protected void RemoveSelf()
            {
                if (Left != null)
                {
                    Left.RightX = RightX;
                    Left.Right = Right;
                }
                if (Right != null) Right.Left = Left;
            }

            /// <summary>
            /// Add X,Z box on the left side of the segment.
            /// Assumes box size that is narrower than the width of the segment.
            /// </summary>
            /// <param name="boxSize"></param>
            /// <returns>Newly created segment on the left</returns>
            public Segment InsertLeft(Point3D boxSize)
            {
                var newSegment = new Segment()
                {
                    Left = Left,
                    Right = this
                };
                if (Left != null) Left.Right = newSegment;
                Left = newSegment;

                newSegment.RightX = newSegment.LeftX + boxSize.X;
                newSegment.Z = Z + boxSize.Z;
                return newSegment;
            }

            /// <summary>
            /// Raise the Z of the current segment by specified value.
            /// If Z of the segment becomes equal to left or right neighbor then
            /// eliminate consecutive segments having the same Z value.
            /// </summary>
            /// <param name="z"></param>
            public void AddZ(long z)
            {
                Z += z;
                while (Right != null && Right.Z == Z) RemoveRight();
                if (Left != null && Left.Z == Z) RemoveSelf();
            }

            public void AddLeftZ(Point3D boxSize)
            {
                // assuming that box size is narrower than gap
                if (Left != null && Left.Z == Z + boxSize.Z)
                {
                    Left.RightX += boxSize.X;
                }
                else
                {
                    InsertLeft(boxSize);
                }
            }
            /// <summary>
            /// Raise the Z of the segment to the Z value of the neighbor.
            /// If segment has no neighbors then do nothing.
            /// If segment has two neighbors then use the lower Z value of the two.
            /// </summary>
            public void FillValley()
            {
                if (NoSegmentsOnSides) return;
                if (NoLeftSegment || 
                    Right != null && Left.Z > Right.Z) AddZ(Right.Z - Z);
                else AddZ(Left.Z - Z);
            }
        }

        /// <summary>
        /// The leftmost segment of the packing line.
        /// </summary>
        public Segment FirstSegment;

        /// <summary>
        /// Initialize the class with specified width.
        /// </summary>
        /// <param name="maxWidth"></param>
        public PackLine(long maxWidth)
        {
            FirstSegment = new Segment()
            {
                RightX = maxWidth
            };
        }

        /// <summary>
        /// Find the segment with minimal Z value.
        /// </summary>
        /// <returns></returns>
        public Segment FindValley()
        {
            var valley = FirstSegment;
            for (var segment = FirstSegment; segment.Right != null; segment = segment.Right)
            {
                if (segment.Right.Z < valley.Z) valley = segment.Right;
            }
            return valley;
        }
        /// <summary>
        /// Add X,Z box to the segment of he pack line.
        /// This method assumes dimensions that are no wider than the segment.
        /// If segment is wider than dimensions being added then box is added
        /// to the left side of the segment.
        /// </summary>
        /// <param name="valley"></param>
        /// <param name="dimensions"></param>
        public void AddZ(Segment valley, Point3D dimensions)
        {
            if (dimensions.X == valley.Width) valley.AddZ(dimensions.Z);
            else if (valley.NoLeftSegment) FirstSegment = valley.InsertLeft(dimensions);
            else valley.AddLeftZ(dimensions);
        }
    }
}
