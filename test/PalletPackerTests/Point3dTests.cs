using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PalletPacker;
using Xunit;

namespace PalletPackerTests
{
    public class Point3dTests
    {
        [Fact]
        public void Volume()
        {
            var pt = new Point3D() { X = 10, Y = 20, Z = 30};
            Assert.Equal(6000, pt.Volume);
        }

        [Theory]
        [InlineData(10, 20, 30, false)]
        [InlineData(20, 20, 20, true)]
        public void IsCube(int x, int y, int z, bool isCube)
        {
            var pt = new Point3D() { X = x, Y = y, Z = z };
            Assert.Equal(isCube, pt.IsCube);
        }

        [Theory]
        [InlineData(10, 10, 10, true, 10, 10, 10)]
        [InlineData(10, 10, 10, true, 6, 6, 6)]
        [InlineData(10, 10, 10, false, 16, 16, 16)]
        [InlineData(10, 10, 10, false, 10, 10, 11)]
        public void Contains(int x, int y, int z, bool contains, int x2, int y2, int z2)
        {
            var pt = new Point3D() { X = x, Y = y, Z = z };
            var pt2 = new Point3D() { X = x2, Y = y2, Z = z2 };
            Assert.Equal(contains, pt.ContainsDimension(pt2));
        }

        [Fact]
        public void AllRotations()
        {
            var pt = new Point3D() { X = 1, Y = 2, Z = 3 };
            var allRotations = pt.AllRotations.ToList();

            Assert.Contains(new Point3D(1, 2, 3), allRotations);
            Assert.Contains(new Point3D(1, 3, 2), allRotations);
            Assert.Contains(new Point3D(3, 1, 2), allRotations);
            Assert.Contains(new Point3D(3, 2, 1), allRotations);
            Assert.Contains(new Point3D(3, 1, 2), allRotations);
            Assert.Contains(new Point3D(3, 2, 1), allRotations);
        }

        [Fact]
        public void AsOOO()
        {
            var pt = new Point3D(1, 2, 3);

            Assert.Equal(new Point3D(2, 1, 3), pt.AsYXZ);
            Assert.Equal(new Point3D(3, 1, 2), pt.AsZXY);
            Assert.Equal(new Point3D(1, 3, 2), pt.AsXZY);
            Assert.Equal(new Point3D(2, 3, 1), pt.AsYZX);
            Assert.Equal(new Point3D(3, 2, 1), pt.AsZYX);
        }

        [Fact]
        public void YDimensionVariants()
        {
            var Y = 222;
            var allYdimensionVariants = new Point3D(1, Y, 3).YDimensionVariants.ToList();

            Assert.Contains(new Point3D(1, Y, 3), allYdimensionVariants);
            Assert.Contains(new Point3D(Y, 1, 3), allYdimensionVariants);
            Assert.Contains(new Point3D(1, 3, Y), allYdimensionVariants);
        }

        [Theory]
        [InlineData(10, 10, 10, 10, 10, 10, 0, 0, 0)]
        [InlineData(10, 10, 10, 0, 0, 0, 10, 10, 10)]
        [InlineData(0, 0, 0, 10, 10, 10, 10, 10, 10)]
        [InlineData(10, 20, 30, 1, 2, 3, 9, 18, 27)]
        public void AbsoluteDiff(int x, int y, int z, int otherX, int otherY, int otherZ, int deltaX, int deltaY, int deltaZ)
        {
            var calculated = new Point3D(x,y,z).AbsoluteDiff(new Point3D(otherX, otherY, otherZ));
            Assert.Equal(deltaX, calculated.X);
            Assert.Equal(deltaY, calculated.Y);
            Assert.Equal(deltaZ, calculated.Z);
        }

        [Theory]
        [InlineData(10, 10, 10, 0, 10)]
        [InlineData(10, 20, 30, 31, 1)]
        [InlineData(10, 20, 30, 21, 1)]
        [InlineData(10, 20, 30, 11, 1)]
        [InlineData(10, 20, 30, 29, 1)]
        [InlineData(10, 20, 30, 19, 1)]
        [InlineData(10, 20, 30, 9, 1)]
        public void MinDiff(int x, int y, int z, int dimension, int expectedDiff)
        {
            var calculated = new Point3D(x, y, z).MinDiff(dimension);
            Assert.Equal(expectedDiff, calculated);
        }
    }
}
