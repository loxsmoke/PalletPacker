using System;
using System.Collections.Generic;
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
            Assert.Equal(contains, pt.Contains(pt2));
        }

        public void AbsoluteDiff()
        {
        }
        public void MinDiff()
        {
        }
    }
}
