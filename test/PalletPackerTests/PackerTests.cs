using System;
using System.Collections.Generic;
using System.Text;
using PalletPacker;
using Xunit;

namespace PalletPackerTests
{
    public class PackerTests
    {
        [Fact]
        public void CreateLayers()
        {
            var pallet = new Point3D() { X = 104, Y = 96, Z = 84 };
            var boxList = new List<Box>();
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 70, Y = 104, Z = 24 } });
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 70, Y = 104, Z = 24 } });
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 70, Y = 104, Z = 24 } });
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 70, Y = 104, Z = 24 } });

            boxList.Add(new Box() { Dimensions = new Point3D() { X = 14, Y = 104, Z = 48 } });
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 14, Y = 104, Z = 48 } });

            boxList.Add(new Box() { Dimensions = new Point3D() { X = 40, Y = 52, Z = 36 } });
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 40, Y = 52, Z = 36 } });
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 40, Y = 52, Z = 36 } });

            var layers = Packer.CreateLayers(boxList, pallet);

            Assert.True(layers.Count == 7);
            Assert.True(layers[0].LayerY == 24 && layers[0].MinDifferenceTotal == 56);
            Assert.True(layers[1].LayerY == 36 && layers[1].MinDifferenceTotal == 72);
            Assert.True(layers[2].LayerY == 40 && layers[2].MinDifferenceTotal == 80);
            Assert.True(layers[3].LayerY == 52 && layers[3].MinDifferenceTotal == 80);
            Assert.True(layers[4].LayerY == 70 && layers[4].MinDifferenceTotal == 98);
            Assert.True(layers[5].LayerY == 48 && layers[5].MinDifferenceTotal == 100);
            Assert.True(layers[6].LayerY == 14 && layers[6].MinDifferenceTotal == 106);
        }

        [Fact]
        public void Pack()
        {
            var pallet = new Point3D() {X = 16 *1000, Y = 16 * 1000, Z = 16 * 1000};
            var boxList = new List<Box>();
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 12 * 1000, Y = 12 * 1000, Z = 1000 / 2 } });
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 19 * 1000, Y = 11 * 1000 + 1000 / 4, Z = 3 * 1000 } });
            boxList.Add(new Box() { Dimensions = new Point3D() { X = 3 * 1000, Y = 3 * 1000, Z = 1000 / 2 } });
            var packer = new Packer();
            var packed = packer.Pack(boxList, pallet);

        }
    }
}
