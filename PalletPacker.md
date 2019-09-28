# PalletPacker.dll v.1.0.0.0 API documentation

# All types

|   |   |   |
|---|---|---|
| [Box Class](#box-class) | [Packer Class](#packer-class) | [Point3D Class](#point3d-class) |
| [Layer Class](#layer-class) | [PackingData Class](#packingdata-class) | [Segment Class](#segment-class) |
| [PackedPallet Class](#packedpallet-class) | [PackLine Class](#packline-class) | [SelectedBox Class](#selectedbox-class) |
# Box Class

Namespace: PalletPacker

Box dimensions.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **Clone()** | [Box](#box-class) | Clone the box. |
| **ToString()** | string |  |
## Fields

| Name | Type | Summary |
|---|---|---|
| **Dimensions** | [Point3D](#point3d-class) | Original box dimensions. |
| **PackingData** | [PackingData](#packingdata-class) | Box packing data. Location in the pallet and rotated box dimensions.<br>Null if box is not packed. |
# Layer Class

Namespace: PalletPacker

Packing layer.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **ToString()** | string |  |
## Fields

| Name | Type | Summary |
|---|---|---|
| **LayerThickness** | long | Layer thickness. Y dimension of the layer. |
| **MinDifferenceTotal** | long | The total sum of differences of all boxes between this layer thickness and<br>dimension of the box that is closest to the layer thickness. |
# PackedPallet Class

Namespace: PalletPacker

Packed pallet data.

## Properties

| Name | Type | Summary |
|---|---|---|
| **AllBoxesPacked** | bool | True if all boxes were packed |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **PackBox([Box](#box-class) box)** | void | Pack the specified box. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **PalletDimensions** | [Point3D](#point3d-class) | Dimensions of the pallet. This is rotated dimensions of the pallet and it<br>could be different from specified dimensions. |
| **NotPackedBoxes** | List\<[Box](#box-class)\> | The list of boxes that could not be packed. |
| **PackedBoxes** | List\<[Box](#box-class)\> | The list of boxes that were packed. |
| **PackedVolume** | long | Total volume of packed boxes. |
# Packer Class

Namespace: PalletPacker

Based on algorithm from
http://www.dtic.mil/dtic/tr/fulltext/u2/a391201.pdf
Packs a set of boxes into a "bix box" pallet using 2d packing layer by layer.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Quit** | bool | Overridable property that can terminate packing early. For example after some timeout<br>value or number of packing iterations. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **CreateLayers(List\<[Box](#box-class)\> boxList, [Point3D](#point3d-class) pallet)** | List\<[Layer](#layer-class)\> | Generate the list of all possible layers that can fit in the pallet.<br>All box dimensions are used when generating layers. For example if <br>box dimensions are 1,2,3 then three layers could be created and if box<br>dimensions are 1,2,2 then only two layers are possible. |
| **Pack(List\<[Box](#box-class)\> boxList, [Point3D](#point3d-class) palletSize)** | [PackedPallet](#packedpallet-class) | Pack the list of boxes into a pallet. In other word put small boxes into a big one.<br>Function packs pallet in layers and tries all pallet rotations and all possible <br>starting layer sizes.<br>Returned pallet could be null if the list of boxes was empty or packed pallet that may<br>contains all or only some boxes. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **IterationCount** | int |  |
# PackingData Class

Namespace: PalletPacker

Box packing data. 

## Methods

| Name | Returns | Summary |
|---|---|---|
| **Clone()** | [PackingData](#packingdata-class) | Clone the box. |
| **ToString()** | string |  |
## Fields

| Name | Type | Summary |
|---|---|---|
| **PackedDimensions** | [Point3D](#point3d-class) | Packed box dimensions. If orientation of the box did not change during packing<br>this value is the same as box Dimensions field. |
| **PackedLocation** | [Point3D](#point3d-class) | Packed box location in the pallet. |
# PackLine Class

Namespace: PalletPacker

Packing line representing the X,Z graph of packed boxes in layer.
Here X is horizontal coordinate and Z is vertical one.

## Constructors

| Name | Summary |
|---|---|
| **PackLine(long maxWidth)** | Initialize the class with specified width. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **AddZ([Segment](#segment-class) valley, [Point3D](#point3d-class) dimensions)** | void | Add X,Z box to the segment of he pack line.<br>This method assumes dimensions that are no wider than the segment.<br>If segment is wider than dimensions being added then box is added<br>to the left side of the segment. |
| **FindValley()** | [Segment](#segment-class) | Find the segment with minimal Z value. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **FirstSegment** | [Segment](#segment-class) | The leftmost segment of the packing line. |
# Point3D Class

Namespace: PalletPacker

Base class: ValueType

3d dimension or location in 3d space

## Properties

| Name | Type | Summary |
|---|---|---|
| **Volume** | long | Volume calculated from dimensions |
| **IsCube** | bool | True if all coordinates are equal and this is cube  |
| **AsYXZ** | [Point3D](#point3d-class) | Return point object with swapped coordinates |
| **AsZXY** | [Point3D](#point3d-class) | Return point object with swapped coordinates |
| **AsXZY** | [Point3D](#point3d-class) | Return point object with swapped coordinates |
| **AsYZX** | [Point3D](#point3d-class) | Return point object with swapped coordinates |
| **AsZYX** | [Point3D](#point3d-class) | Return point object with swapped coordinates |
| **AllRotations** | IEnumerable\<[Point3D](#point3d-class)\> | All 6 possible combinations of X,Y and Z coordinates |
| **YDimensionVariants** | IEnumerable\<[Point3D](#point3d-class)\> | 3 different Y coordinate variations  |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **AbsoluteDiff([Point3D](#point3d-class) otherPoint)** | [Point3D](#point3d-class) | Return the absolute differences of all coordinate values |
| **AsVariant(int variantIndex)** | [Point3D](#point3d-class) |  |
| **Contains([Point3D](#point3d-class) other)** | bool | Check if all coordinates of other point are less or equal to the coordinates of<br>this point. |
| **MinDiff(long dimension)** | long | Returns minimum absolute difference between X, Y and Z fields and<br>specified value. |
| **SubtractY(long deltaY)** | [Point3D](#point3d-class) |  |
| **ToString()** | string |  |
| **WithX(long otherX)** | [Point3D](#point3d-class) |  |
| **WithY(long otherY)** | [Point3D](#point3d-class) |  |
| **WithYZ(long otherY, long otherZ)** | [Point3D](#point3d-class) |  |
| **WithZ(long otherZ)** | [Point3D](#point3d-class) |  |
## Fields

| Name | Type | Summary |
|---|---|---|
| **X** | long | X coordinate or dimension |
| **Y** | long | Y coordinate or dimension  |
| **Z** | long | Z coordinate or dimension |
# Segment Class

Namespace: PalletPacker

The segment of the packing line.

## Properties

| Name | Type | Summary |
|---|---|---|
| **LeftX** | long | Left x coordinate of the segment. |
| **Width** | long | Segment width |
| **NoSegmentsOnSides** | bool | True if segment is the only one. |
| **NoLeftSegment** | bool | True if segment has no left neighbor, i.e. this is the leftmost segment. |
| **NoRightSegment** | bool | True if segment has no right neighbor, i.e. this is the rightmost segment. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **AddLeftZ([Point3D](#point3d-class) boxSize)** | void |  |
| **AddZ(long z)** | void | Raise the Z of the current segment by specified value.<br>If Z of the segment becomes equal to left or right neighbor then<br>eliminate consecutive segments having the same Z value. |
| **FillValley()** | void | Raise the Z of the segment to the Z value of the neighbor.<br>If segment has no neighbors then do nothing.<br>If segment has two neighbors then use the lower Z value of the two. |
| **InsertLeft([Point3D](#point3d-class) boxSize)** | [Segment](#segment-class) | Add X,Z box on the left side of the segment.<br>Assumes box size that is narrower than the width of the segment. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **Left** | [Segment](#segment-class) | Left and right neighbors of this segment |
| **Right** | [Segment](#segment-class) | Left and right neighbors of this segment |
| **RightX** | long | The right X coordinate of the segment. |
| **Z** | long | The Z coordinate of the segment. |
# SelectedBox Class

Namespace: PalletPacker

The box selected for packing.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **IsBetterFit(bool newFitsInLayer, [Point3D](#point3d-class) newDelta)** | bool | Check if the new box evaluation parameters are better than current ones.   |
## Fields

| Name | Type | Summary |
|---|---|---|
| **Box** | [Box](#box-class) | Found box. |
| **PackedDimensions** | [Point3D](#point3d-class) | Packed box dimensions. If orientation of the box did not change during packing<br>this value is the same as box Dimensions field. |
| **DeltaFromIdeal** | [Point3D](#point3d-class) | Absolute difference between ideal box and currently best box.<br>Parameter used in next best suited box search. Trying to minimize it. |
| **FitsInLayer** | bool | True if current box fits in layer. Boxes that fit in layer are better<br>than not fitting ones. |
