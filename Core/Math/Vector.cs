using System.Collections;

namespace Core.Math
{
    /// <remarks>
    /// Design: Syntax elegancy is more important than performance
    /// </remarks>
    public class Vector : IList<double>
    {
        #region Properties
        private double[] Values { get; set; }
        #endregion

        #region Construction
        public Vector() { Values = Array.Empty<double>(); }
        public Vector(IEnumerable<double> values)
        {
            Values = values.ToArray();
        }
        public Vector(params double[] values)
        {
            Values = values;
        }
        public int Length => Values.Length;
        public string Size => $"Vector: {Values.Length} elements";
        public override string ToString() => $"[{string.Join(", ", Values)}]";
        #endregion

        #region Operator Operations
        public static Vector operator +(Vector a) => a;
        public static Vector operator -(Vector a) => new Vector(a.Values.Select(v => -v));
        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector(a.Zip(b, (a, b) => a + b));
        }
        public static Vector operator +(Vector a, double v)
        {
            return new Vector(a.Select(a => a + v));
        }
        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector(a.Zip(b, (a, b) => a - b));
        }
        public static Vector operator -(Vector a, double v)
        {
            return new Vector(a.Select(a => a - v));
        }
        public static Vector operator *(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector(a.Zip(b, (a, b) => a * b));
        }
        public static Vector operator *(Vector a, double v)
        {
            return new Vector(a.Select(a => a * v));
        }
        public static Vector operator /(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector(a.Zip(b, (a, b) => b == 0 ? throw new DivideByZeroException() : a / b));
        }
        public static Vector operator /(Vector a, double v)
        {
            if (v == 0)
                throw new DivideByZeroException();
            return new Vector(a.Select(a => a / v));
        }
        public static Vector operator ^(Vector a, double v)
        {
            return new Vector(a.Select(a => System.Math.Pow(a, v)));
        }
        #endregion

        #region Numerical Operations
        public Vector Cos()
            => new(Values.Select(v => System.Math.Cos(v)));
        public Vector Cosh()
            => new(Values.Select(v => System.Math.Cosh(v)));
        public Vector Sin()
            => new(Values.Select(v => System.Math.Sin(v)));
        public Vector Sinh()
            => new(Values.Select(v => System.Math.Sinh(v)));
        public Vector Pow(double expo)
            => new(Values.Select(v => System.Math.Pow(v, expo)));
        public Vector Sqrt()
            => new(Values.Select(v => System.Math.Sqrt(v)));
        #endregion

        #region Statistics
        public double Min => Values.Min();
        public double Mean => Values.Average();
        public double Max => Values.Max();
        public double Sum => Values.Sum();
        public double Variance
        {
            get
            {
                double variance = 0.0;
                if (Values.Count() > 1)
                {
                    double avg = Values.Average();
                    variance += Values.Sum(value => System.Math.Pow(value - avg, 2.0));
                }
                // For population, use n-1, for sample, use n
                return variance / Values.Length;
            }
        }
        public double STD => System.Math.Sqrt(Variance);
        public double PopulationVariance
        {
            get
            {
                double variance = 0.0;
                if (Values.Count() > 1)
                {
                    double avg = Values.Average();
                    variance += Values.Sum(value => System.Math.Pow(value - avg, 2.0));
                }
                // For population, use n-1, for sample, use n
                return variance / (Values.Length - 1);
            }
        }
        public double PopulationSTD => System.Math.Sqrt(Variance);

        public double Correlation(Vector other)
        {
            double covariance = Covariance(other);
            double std1 = PopulationSTD;   // Always use n-1 for population
            double std2 = other.PopulationSTD;
            return covariance / (std1 * std2);
        }
        public double Covariance(Vector other)
        {
            if (Values.Length != other.Values.Length)
                throw new ArgumentException("Vector size doesn't match");

            double variance = 0.0;
            if (Values.Count() > 1)
            {
                double avg1 = Values.Average();
                double avg2 = Values.Average();
                for (int i = 0; i < Values.Length; i++)
                    variance += (Values[i] - avg1) * (other.Values[i] - avg2);
            }
            return variance / (Values.Length - 1); // Always use n-1 for population
        }
        #endregion

        #region Construction
        public Vector Copy()
        { 
            return new Vector(this); 
        }
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
