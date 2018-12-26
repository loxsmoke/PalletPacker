using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PalletPacker
{
    /// <summary>
    /// 3d dimension or location in 3d space
    /// </summary>
    public struct Point3D
    {
        /// <summary>
        /// X coordinate or dimension
        /// </summary>
        public long X;
        /// <summary>
        /// Y coordinate or dimension 
        /// </summary>
        public long Y;
        /// <summary>
        /// Z coordinate or dimension
        /// </summary>
        public long Z;
        /// <summary>
        /// Volume calculated from dimensions
        /// </summary>
        public long Volume => X * Y * Z;
        /// <summary>
        /// True if all coordinates are equal and this is cube 
        /// </summary>
        public bool IsCube => X == Y && Y == Z;
        /// <summary>
        /// Check if all coordinates of other point are less or equal to the coordinates of
        /// this point.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if all dimensions of other are less or equal. False otherwise</returns>
        public bool Contains(Point3D other)
        {
            return other.X <= X && other.Y <= Y && other.Z <= Z;
        }
        /// <summary>
        /// Return the absolute differences of all coordinate values
        /// </summary>
        /// <param name="otherPoint">The point to calculate differences from</param>
        /// <returns></returns>
        public Point3D AbsoluteDiff(Point3D otherPoint)
        {
            return new Point3D()
            {
                X = Math.Abs(otherPoint.X - X),
                Y = Math.Abs(otherPoint.Y - Y),
                Z = Math.Abs(otherPoint.Z - Z)
            };
        }
        /// <summary>
        /// Returns minimum absolute difference between X, Y and Z fields and
        /// specified value.
        /// </summary>
        /// <param name="dimension">Value used for difference calculations</param>
        /// <returns></returns>
        public long MinDiff(long dimension)
        {
            return Math.Min(
                Math.Abs(dimension - X),
                Math.Min(
                    Math.Abs(dimension - Y),
                    Math.Abs(dimension - Z)));
        }

        /// <summary>
        /// Return point object with swapped coordinates
        /// </summary>
        public Point3D AsYXZ => new Point3D()
        {
            X = this.Y,
            Y = this.X,
            Z = this.Z
        };
        /// <summary>
        /// Return point object with swapped coordinates
        /// </summary>
        public Point3D AsZXY => new Point3D()
        {
            X = this.Z,
            Y = this.X,
            Z = this.Y
        };
        /// <summary>
        /// Return point object with swapped coordinates
        /// </summary>
        public Point3D AsXZY => new Point3D()
        {
            X = this.X,
            Y = this.Z,
            Z = this.Y
        };
        /// <summary>
        /// Return point object with swapped coordinates
        /// </summary>
        public Point3D AsYZX => new Point3D()
        {
            X = this.Y,
            Y = this.Z,
            Z = this.X
        };
        /// <summary>
        /// Return point object with swapped coordinates
        /// </summary>
        public Point3D AsZYX => new Point3D()
        {
            X = this.Z,
            Y = this.Y,
            Z = this.X
        };

        /// <summary>
        /// All 6 possible combinations of X,Y and Z coordinates
        /// </summary>
        public IEnumerable<Point3D> AllRotations
        {
            get
            {
                yield return this;
                yield return this.AsZYX;
                yield return this.AsZXY;
                yield return this.AsYXZ;
                yield return this.AsXZY;
                yield return this.AsYZX;
            }
        }

        /// <summary>
        /// 3 different Y coordinate variations 
        /// </summary>
        public IEnumerable<Point3D> YDimensionVariants
        {
            get
            {
                yield return this.AsYXZ;
                yield return this;
                yield return this.AsXZY;
            }
        }


        public Point3D AsVariant(int variantIndex)
        {
            switch (variantIndex)
            {
                case 0:
                    return this;
                case 1:
                    return this.AsZYX;
                case 2:
                    return this.AsZXY;
                case 3:
                    return this.AsYXZ;
                case 4:
                    return this.AsXZY;
                case 5:
                    return this.AsYZX;
                default:
                    throw new ArgumentException(nameof(variantIndex));
            }
        }

        public Point3D WithX(long otherX)
        {
            return new Point3D() { X = otherX, Y = this.Y, Z = this.Z };
        }
        public Point3D WithY(long otherY)
        {
            return new Point3D() { X = this.X, Y = otherY, Z = this.Z };
        }
        public Point3D WithZ(long otherZ)
        {
            return new Point3D() { X = this.X, Y = this.Y, Z = otherZ };
        }
        public Point3D WithYZ(long otherY, long otherZ)
        {
            return new Point3D() { X = this.X, Y = otherY, Z = otherZ };
        }
        public Point3D SubtractY(long deltaY)
        {
            var clone = ((Point3D)MemberwiseClone());
            clone.Y -= deltaY;
            return clone;
        }
        public override string ToString()
        {
            return $"[{X},{Y},{Z}]";
        }
    }
}
