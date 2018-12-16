using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PalletPacker;
using Xunit;

namespace PalletPackerTests
{
    public class PackLineTests
    {
        [Fact]
        public void Constructor()
        {
            var pl = new PackLine(100);
            Assert.NotNull(pl.FirstSegment);
            Assert.Equal(0, pl.FirstSegment.Z);
            Assert.Equal(100, pl.FirstSegment.RightX);
        }

        // Parameter: list of space separated Z, X values
        PackLine Construct(string packLineData)
        {
            var values = packLineData.Split(' ').Select(x => int.Parse(x.Trim())).ToList();
            var packLine = new PackLine(1);
            packLine.FirstSegment = null;
            PackLine.Segment lastSegment = null; 
            for (int i = 0; i < values.Count; i += 2)
            {
                var segment = new PackLine.Segment() { Z = values[i], RightX = values[i+1]};
                if (packLine.FirstSegment == null)
                {
                    packLine.FirstSegment = segment;
                }
                else
                {
                    lastSegment.Right = segment;
                    segment.Left = lastSegment;
                }
                lastSegment = segment;
            }
            return packLine;
        }

        [Theory]
        [InlineData("10 10", 10, 10)]
        [InlineData("10 10 20 20 30 30", 10, 10)]
        [InlineData("50 50 10 60", 10, 60)]
        [InlineData("50 10 10 20 20 30", 10, 20)]
        public void Valley(string packLineData, int foundZ, int foundRightX)
        {
            var packLine = Construct(packLineData);
            var valley = packLine.FindValley();
            Assert.NotNull(valley);
            Assert.Equal(foundZ, valley.Z);
            Assert.Equal(foundRightX, valley.RightX);
        }

        int SegmentCount(PackLine line)
        {
            var count = 0;
            var segment = line.FirstSegment;
            while (segment != null)
            {
                count++;
                segment = segment.Right;
            }

            return count;
        }

        [Theory]
        // Z RightX Z RightX ...
        [InlineData("10 10", 20, 10, "30 10")]
        [InlineData("10 10", 20, 5, "30 5 10 10")]
        [InlineData("10 10 5 20 10 30", 5, 10, "10 30")]
        [InlineData("10 10 5 20 10 30", 5, 5, "10 15 5 20 10 30")]
        [InlineData("10 10 5 20", 5, 5, "10 15 5 20")]
        [InlineData("0 160", 120, 120, "120 120 0 160")]
        [InlineData("120 120 0 160", 30, 30, "120 120 30 150 0 160")]
        [InlineData("120 120 30 150 0 160", 30, 10, "120 120 30 160")]
        public void AddZ(string packLineData, int dimZ, int dimX, string expectedPackLineData)
        {
            // Setup
            var packLine = Construct(packLineData);
            // Action
            var valley = packLine.FindValley();
            packLine.AddZ(valley, new Point3D() { X = dimX, Y = 1, Z = dimZ});
            // Result check
            var expectedPackLine = Construct(expectedPackLineData);
            Assert.Equal(SegmentCount(expectedPackLine), SegmentCount(packLine));
            var segment = packLine.FirstSegment;
            var expectedSegment = expectedPackLine.FirstSegment;
            while (segment != null && expectedSegment != null)
            {
                Assert.Equal(expectedSegment.RightX, segment.RightX);
                Assert.Equal(expectedSegment.Z, segment.Z);
                segment = segment.Right;
                expectedSegment = expectedSegment.Right;
            }
        }

    }
}
