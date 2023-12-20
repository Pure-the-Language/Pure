using CorePackage;
using System.Collections;
using System.Text;

namespace Math
{
    /// <summary>
    /// Main definition of the Matrix type
    /// </summary>
    public sealed class Matrix : ExperimentalLibrary, IEnumerable<Vector1D>, IEnumerable
    {
        #region Library Metadata
        /// <summary>
        /// Status of library
        /// </summary>
        public override StandardLibraryStatus Status => StandardLibraryStatus.Experimental;
        /// <summary>
        /// Name of library
        /// </summary>
        public override string Name => nameof(Matrix);
        /// <summary>
        /// Version of library
        /// </summary>
        public override string Version => "v0.1.0";
        /// <summary>
        /// Description of library
        /// </summary>
        public override string Description => "Provides numerical manipulations of matrix data, not targeted for linear algebra.";
        /// <summary>
        /// Dependancies of library
        /// </summary>
        public override string Dependancies => """
            Pure/Core Vector: For Vector1D type definition.
            """;
        #endregion

        #region Internal Data
        private readonly double[][] _data;
        #endregion

        #region Properties
        /// <summary>
        /// Get raw data
        /// </summary>
        public double[][] Data => _data;
        /// <summary>
        /// Get raw data row
        /// </summary>
        public double[] GetDataRow(int index) => _data[index];
        /// <summary>
        /// Get raw data column
        /// </summary>
        public double[] GetDataColumn(int index) => _data.Select(r => r[index]).ToArray();
        #endregion

        #region Constructor
        public Matrix(double[][] data)
        {
            _data = data;
        }
        public Matrix(int rows, int columns)
        {
            _data = new double[rows][];
            for (int i = 0; i < rows; i++)
                _data[i] = new double[columns];
        }
        public Matrix(string path, bool skipFirstRow = false, bool skipFirstColumn = false, int readRows = 0, int readColumns = 0)
        {
            readRows = readRows == 0 ? int.MaxValue : readRows;
            readColumns = readColumns == 0 ? int.MaxValue : readColumns;

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            _data = File.ReadLines(path)
                .Skip(skipFirstRow ? 1 : 0)
                .Take(readRows)
                .Select(line => line.Split(',').Skip(skipFirstColumn ? 1 : 0).Take(readColumns).Select(double.Parse).ToArray())
                .ToArray();
        }
        #endregion

        #region Reading
        /// <summary>
        /// Read from CSV file
        /// </summary>
        public static Matrix Read(string path, bool skipFirstRow = false, bool skipFirstColumn = false, int readRows = 0, int readColumns = 0)
            => new(path, skipFirstRow, skipFirstColumn, readRows, readColumns);
        #endregion

        #region Writing
        /// <summary>
        /// Print CSV representation as string
        /// </summary>
        public string Print(string? headerRow = null, Func<int, string>? columnRowGenerator = null)
        {
            StringBuilder csvBuilder = new();
            if (headerRow != null)
                csvBuilder.AppendLine(headerRow);
            for (int rowID = 0; rowID < _data.Length; rowID++)
            {
                double[] row = _data[rowID];
                if (columnRowGenerator != null)
                    csvBuilder.Append($"{columnRowGenerator(rowID)},");
                foreach (var col in row)
                    csvBuilder.Append($"{col},");
                csvBuilder.Length--;    // Remove redundant trailing `,`
                csvBuilder.AppendLine();
            }
            return csvBuilder.ToString().TrimEnd();
        }
        #endregion

        #region Operator Overloading
        /// <summary>
        /// Get row as Vector1D
        /// </summary>
        public Vector1D this[int index]
            => new(_data[index]);
        #endregion

        #region Enumeration
        /// <summary>
        /// Enumerate matrix rows
        /// </summary>
        public IEnumerator<Vector1D> GetEnumerator()
            => new MatrixRowEnumerator(_data);
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        #endregion
    }

    public class MatrixRowEnumerator(double[][] data) : IEnumerator<Vector1D>
    {
        private readonly double[][] _data = data;
        private int _row = 0;

        public Vector1D Current => new(_data[_row]);
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool MoveNext()
        {
            _row++;
            if (_row >= _data.Length)
                return true;
            else return false;
        }

        public void Reset()
        {
            _row = 0;
        }
    }
}
