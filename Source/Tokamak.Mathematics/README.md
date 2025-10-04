# Tokamak Mathematics

Useful functions for working with objects in a pure mathematical way.

Color space math
Converts to/from various color spaces as well as handling Linear/Gamma RGB conversions.

Abstract shape classes and functions:
- Point: 2D Point
- Rect/RectF: 2D Rectangle
- Sphere: 3D sphere with ray casting tests.

Extensions to System.Numerics types.

Various math extensions not found in base System.Math namespace.

### AlmostEquals
Fuzzy test for an ALMOST equals on floating point numbers.
```csharp
if (MathX.AlmostEquals(left, right))
{
    // Do thing when left and right are close to each other in value.
}
```

The AlmostEquals function uses the value found in the `MathX.FUZ` constant.

### Lerp on float/double
```csharp
float pf = MathX.Lerp(0, 1, 0.5f); // Should return 0.5f
double pd = MathX.Lerp(0, 1, 0.5); // Should return 0.5
```

### Wrap functions
You can use the Warp and WrapF functions to wrap a float or double around to some arbitrary maximum value.
```csharp
float angle = MathX.WrapF(v, 359); // Keep a value between 0 and 359
```

### Byte to float conversion
It is often coinvent to work with a byte range from 0 to 255 instead of a floating point range of 0 to 1.

```csharp
byte redByte = MathX.ToByteRange(0.5); // Convert 0.5 to 127
```

## Bézier Curve Functions

### Example
```csharp
// Get a point halfway along a 2D Quadradicurve Bézier curve
Vector2 point = Bezier.QuadSolve(v1, v2, v3, 0.5f);
```