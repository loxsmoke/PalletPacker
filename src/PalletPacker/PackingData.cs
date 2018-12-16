using System;
using System.Collections.Generic;
using System.Text;

namespace PalletPacker
{
    /// <summary>
    /// Box packing data.
    /// </summary>
    public class PackingData
    {
        /// <summary>
        /// Packed box dimensions. If orientation of the box did not change during packing
        /// this value is the same as Dimensions field.
        /// </summary>
        public Point3D PackedDimensions;
        /// <summary>
        /// Packed box location in the pallet.
        /// </summary>
        public Point3D PackedLocation;

        /// <summary>
        /// Clone the box.
        /// </summary>
        /// <returns>The clone</returns>
        public PackingData Clone()
        {
            return MemberwiseClone() as PackingData;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"dim={PackedDimensions} at={PackedLocation}";
        }
    }
}
