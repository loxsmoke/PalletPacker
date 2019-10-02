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
        /// Create the point object
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point3D(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Check if all coordinates of other point are less or equal to the coordinates of
        /// this point.
        /// </summary>
        /// <param name="otherDimension"></param>
        /// <returns>True if all dimensions of other point are less or equal. False otherwise</returns>
        public bool ContainsDimension(Point3D otherDimension)
        {
            return otherDimension.X <= X && otherDimension.Y <= Y && otherDimension.Z <= Z;
        }
        /// <summary>
        /// Return the absolute differences of all coordinate values
        /// </summary>
        /// <param name="otherPoint">The point to calculate differences from</param>
        /// <returns></returns>
        public Point3D AbsoluteDiff(Point3D otherPoint)
        {
            return new Point3D(
                Math.Abs(otherPoint.X - X),
                Math.Abs(otherPoint.Y - Y),
                Math.Abs(otherPoint.Z - Z));
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
        public Point3D AsYXZ => new Point3D(Y, X, Z);

        /// <summary>
        /// Return point object with swapped coordinates
        /// </summary>
        public Point3D AsZXY => new Point3D(Z, X, Y);

        /// <summary>
        /// Return point object with swapped coordinates
        /// </summary>
        public Point3D AsXZY => new Point3D(X, Z, Y);

        /// <summary>
        /// Return point object with swapped coordinates
        /// </summary>
        public Point3D AsYZX => new Point3D(Y, Z, X);

        /// <summary>
        /// Return point object with swapped coordinates
        /// </summary>
        public Point3D AsZYX => new Point3D(Z, Y, X);

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

        /// <summary>
        /// Return copy of the object with specified Y and Z coordinates.
        /// </summary>
        /// <param name="otherY"></param>
        /// <param name="otherZ"></param>
        /// <returns></returns>
        public Point3D WithYZ(long otherY, long otherZ)
        {
            return new Point3D() { X = this.X, Y = otherY, Z = otherZ };
        }

        /// <summary>
        /// Subtract value from Y coordinate.
        /// </summary>
        /// <param name="deltaY"></param>
        /// <returns></returns>
        public Point3D SubtractY(long deltaY)
        {
            var clone = ((Point3D)MemberwiseClone());
            clone.Y -= deltaY;
            return clone;
        }

        /// <summary>
        /// String representation of the point.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{X},{Y},{Z}]";
        }
    }
}
