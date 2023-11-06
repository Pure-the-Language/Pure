using System.Collections;

namespace Math
{
    /// <remarks>
    /// Design: Syntax elegancy is more important than performance.
    /// This provides base container and operations.
    /// Data type is immutable.
    /// </remarks>
    public partial class Vector1D : IList<double>
    {
        #region Properties
        private double[] Values { get; set; }
        #endregion

        #region Construction
        /// <summary>
        /// Construct empty.
        /// </summary>
        public Vector1D() 
            => Values = Array.Empty<double>();
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector1D(IEnumerable<double> values)
            => Values = values.ToArray();
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector1D(IEnumerable<int> values)
            => Values = values.Select(v => (double)v).ToArray();
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector1D(IEnumerable<bool> values)
            => Values = values.Select(v => v ? 1.0 : 0.0).ToArray();
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector1D(IEnumerable<float> values)
            => Values = values.Select(v => (double)v).ToArray();
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector1D(IEnumerable<string> values)
            => Values = values.Select(v => double.Parse(v)).ToArray();
        /// <summary>
        /// Construct from param arguments.
        /// </summary>
        public Vector1D(params double[] values)
            => Values = values;
        /// <summary>
        /// Construct from string, either comma delimited or space delimited.
        /// </summary>
        public Vector1D(string values)
            => Values = values.Contains(',')
                ? values.Split(',', StringSplitOptions.TrimEntries).Select(double.Parse).ToArray()
                : values.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToArray();
        #endregion

        #region Query
        /// <summary>
        /// Get length of vector
        /// </summary>
        public int Length => Values.Length;
        /// <summary>
        /// Another name for length
        /// </summary>
        public int Norm() => Length;
        /// <summary>
        /// Get string representation of size.
        /// </summary>
        public string Size => $"Vector: {Values.Length} elements";
        /// <summary>
        /// ToString override
        /// </summary>
        public override string ToString() => $"[{string.Join(", ", Values)}]";
        #endregion

        #region Operator Operations
        /// <summary>
        /// Identity (no copy is made)
        /// </summary>
        public static Vector1D operator +(Vector1D a) => a;
        /// <summary>
        /// Gets negative
        /// </summary>
        public static Vector1D operator -(Vector1D a) => new(a.Values.Select(v => -v));
        /// <summary>
        /// Adds two vectors
        /// </summary>
        public static Vector1D operator +(Vector1D a, Vector1D b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector1D(a.Zip(b, (a, b) => a + b));
        }
        /// <summary>
        /// Adds a scalar to every element
        /// </summary>
        public static Vector1D operator +(Vector1D a, double v)
        {
            return new Vector1D(a.Select(a => a + v));
        }
        /// <summary>
        /// Subtract two vectors
        /// </summary>
        public static Vector1D operator -(Vector1D a, Vector1D b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector1D(a.Zip(b, (a, b) => a - b));
        }
        /// <summary>
        /// Subtract a scalar to every element
        /// </summary>
        public static Vector1D operator -(Vector1D a, double v)
        {
            return new Vector1D(a.Select(a => a - v));
        }
        /// <summary>
        /// Multiply element-wise
        /// </summary>
        public static Vector1D operator *(Vector1D a, Vector1D b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector1D(a.Zip(b, (a, b) => a * b));
        }
        /// <summary>
        /// Multiply every element by a scalar
        /// </summary>
        public static Vector1D operator *(Vector1D a, double v)
        {
            return new Vector1D(a.Select(a => a * v));
        }
        /// <summary>
        /// Divide element-wise
        /// </summary>
        public static Vector1D operator /(Vector1D a, Vector1D b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector1D(a.Zip(b, (a, b) => b == 0 ? throw new DivideByZeroException() : a / b));
        }
        /// <summary>
        /// Multiply every element by a scalar
        /// </summary>
        public static Vector1D operator /(Vector1D a, double v)
        {
            if (v == 0)
                throw new DivideByZeroException();
            return new Vector1D(a.Select(a => a / v));
        }
        /// <summary>
        /// Exponent element-wise
        /// </summary>
        public static Vector1D operator ^(Vector1D a, double v)
        {
            return new Vector1D(a.Select(a => System.Math.Pow(a, v)));
        }
        /// <summary>
        /// Append an element
        /// </summary>
        public static Vector1D operator |(Vector1D a, double v)
        {
            return new Vector1D(a.Append(v));
        }
        /// <summary>
        /// Append an entire vector
        /// </summary>
        public static Vector1D operator |(Vector1D a, Vector1D b)
        {
            return new Vector1D(a.Concat(b));
        }
        #endregion

        #region Basic Numerical Operations
        /// <summary>
        /// Apply element-wise arbitrary function
        /// </summary>
        public Vector1D Apply(Func<double, double> fun)
            => new(Values.Select(fun));
        /// <summary>
        /// Compute cos element-wise
        /// </summary>
        public Vector1D Cos()
            => new(Values.Select(System.Math.Cos));
        /// <summary>
        /// Compute cosh element-wise
        /// </summary>
        public Vector1D Cosh()
            => new(Values.Select(System.Math.Cosh));
        /// <summary>
        /// Compute sin element-wise
        /// </summary>
        public Vector1D Sin()
            => new(Values.Select(System.Math.Sin));
        /// <summary>
        /// Compute sinh element-wise
        /// </summary>
        public Vector1D Sinh()
            => new(Values.Select(System.Math.Sinh));
        /// <summary>
        /// Compute pow element-wise
        /// </summary>
        public Vector1D Pow(double expo)
            => new(Values.Select(v => System.Math.Pow(v, expo)));
        /// <summary>
        /// Compute sqrt element-wise
        /// </summary>
        public Vector1D Sqrt()
            => new(Values.Select(System.Math.Sqrt));
        #endregion

        #region Construction
        /// <summary>
        /// Make a copy
        /// </summary>
        public Vector1D Copy()
            => new (this);
        #endregion

        #region IList Interface
        public double this[int index] { get => Values[index]; set => Values[index] = value; }
        public int Count => Values.Length;
        public bool IsReadOnly { get; set; }

        public void Add(double item)
        {
            if (IsReadOnly) { throw new InvalidOperationException(); }

            var old = Values;
            Values = new double[Count + 1];
            for (int i = 0; i < old.Length; i++)
                Values[i] = old[i];
            Values[old.Length] = item;
        }
        public void Clear()
        {
            Values = new double[Count];
        }
        public bool Contains(double item)
        {
            return Values.Contains(item);
        }
        public void CopyTo(double[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < Values.Length; i++)
                array[i] = Values[i];
        }
        public IEnumerator<double> GetEnumerator()
        {
            foreach (var value in Values)
                yield return value;
        }
        public int IndexOf(double item)
        {
            return Array.IndexOf(Values, item);
        }
        public void Insert(int index, double item)
        {
            Values = Values.Take(index)
                .Concat(new double[] { item })
                .Concat(Values.Skip(index + 1))
                .ToArray();
        }
        public bool Remove(double item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }
        public void RemoveAt(int index)
        {
            Values = Values.Take(index).Concat(Values.Skip(index + 1)).ToArray();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
        #endregion
    }
}
