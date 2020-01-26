# PalletPacker.dll v.0.2.0.0 API documentation

Created by 
[mddox](https://github.com/loxsmoke/mddox) on 1/26/2020

# All types

|   |   |   |
|---|---|---|
| [Packer Class](#packer-class) | [PackedPallet Class](#packedpallet-class) | [SelectedBox Class](#selectedbox-class) |
| [Box Class](#box-class) | [PackingData Class](#packingdata-class) |   |
| [Layer Class](#layer-class) | [Point3D Class](#point3d-class) |   |
# Packer Class

Namespace: PalletPacker

Based on algorithm from
http://www.dtic.mil/dtic/tr/fulltext/u2/a391201.pdf
Packs a set of boxes into a "big box" pallet using 2d packing layer by layer.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **CreateLayers(List\<[Box](#box-class)\> boxList, [Point3D](#point3d-class) pallet)** | List\<[Layer](#layer-class)\> | Generate the list of all possible layers that can fit in the pallet.<br>All box dimensions are used when generating layers. For example if <br>box dimensions are 1,2,3 then three layers could be created and if box<br>dimensions are 1,2,2 then only two layers are possible. |
| **Pack(List\<[Box](#box-class)\> boxList, [Point3D](#point3d-class) palletSize)** | [PackedPallet](#packedpallet-class) | Pack the list of boxes into a pallet. In other words put small boxes into a big one.<br>Function packs pallet in layers and tries all pallet rotations and all possible <br>starting layer sizes.<br>Returned pallet could be null if the list of boxes was empty or packed pallet that may<br>contains all or only some boxes.<br>Packing is done in the loop each time incrementing IterationCount field.<br>Packing stops when either all boxes fit in the pallet, packing times out or the number of packing iterations<br>is above the specified limit. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **TimeoutMilliseconds** | int? | Packing timeout milliseconds. Null for no timeout. |
| **PackingIterationLimit** | int? | Packing iteration limit. Null for no iteration limit. |
| **IterationCount** | int | The number of pallete packing attempts. Value is initialized to 0 and then increased by one before <br>each packing attempt. |
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
| **ToString()** | string | String representation of the object. |
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
## Constructors

| Name | Summary |
|---|---|
| **Point3D(long x, long y, long z)** | Create the point object |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **AbsoluteDiff([Point3D](#point3d-class) otherPoint)** | [Point3D](#point3d-class) | Return the absolute differences of all coordinate values |
| **ContainsDimension([Point3D](#point3d-class) otherDimension)** | bool | Check if all coordinates of other point are less or equal to the coordinates of<br>this point. |
| **MinDiff(long dimension)** | long | Returns minimum absolute difference between X, Y and Z fields and<br>specified value. |
| **SubtractY(long deltaY)** | [Point3D](#point3d-class) | Subtract value from Y coordinate. |
| **ToString()** | string | String representation of the point. |
| **WithYZ(long otherY, long otherZ)** | [Point3D](#point3d-class) | Return copy of the object with specified Y and Z coordinates. |
## Fields

| Name | Type | Summary |
|---|---|---|
| **X** | long | X coordinate or dimension |
| **Y** | long | Y coordinate or dimension  |
| **Z** | long | Z coordinate or dimension |
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
