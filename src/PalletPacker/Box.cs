using System;
using System.Collections.Generic;
using System.Text;

namespace PalletPacker
{
    /// <summary>
    /// Box dimensions.
    /// </summary>
    public class Box
    {
        /// <summary>
        /// Original box dimensions.
        /// </summary>
        public Point3D Dimensions;
        /// <summary>
        /// Box packing data. Location in the pallet and rotated box dimensions.
        /// Null if box is not packed.
        /// </summary>
        public PackingData PackingData;

        /// <summary>
        /// Clone the box.
        /// </summary>
        /// <returns>The clone</returns>
        public Box Clone()
        {
            var clone = MemberwiseClone() as Box;
            clone.PackingData = PackingData?.Clone();
            return clone;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"dim={Dimensions}" + 
                   (PackingData == null ? " not packed" : ($" packed={PackingData}"));
        }
    };
}
