# Vector

Very comprehensive vector library for handling with 1D data. The goal is short-hand utility of common functions.

## Usage

```C#
Import(Vector)
using Vector = Vector1D;
Vector v = [1, 2, 3, 4, 5];
```

```C#
Import(Vector)
public class Vector: Vector1D {}
Vector v = [1, 2, 3, 4, 5];
```

```C#
Import(Vector)
Vector([1.0, 2.0, 3.0])
```

## TODO

- [x] Implement `Vector a = [1, 2, 3]` syntax (C#12)
    * This syntax is available out-of-box in C#12.
- [x] Implement `Load(filepath)` for plain text rows (optionally skip header row) and CSV (default 0th column)