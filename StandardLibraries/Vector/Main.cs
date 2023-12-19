namespace Math
{
    /// <summary>
    /// Library entrance
    /// </summary>
    public static class Main
    {
        #region Vector Creation Routine
        /// <summary>
        /// Create empty vector
        /// </summary>
        public static Vector1D Vector()
            => new();
        /// <summary>
        /// Create vector from enumerable
        /// </summary>
        public static Vector1D Vector(IEnumerable<double> values)
             => new(values);
        /// <summary>
        /// Create vector from enumerable
        /// </summary>
        public static Vector1D Vector(IEnumerable<int> values)
             => new(values);
        /// <summary>
        /// Create vector from enumerable
        /// </summary>
        public static Vector1D Vector(IEnumerable<bool> values)
             => new(values);
        /// <summary>
        /// Create vector from enumerable
        /// </summary>
        public static Vector1D Vector(IEnumerable<float> values)
             => new(values);
        /// <summary>
        /// Create vector from enumerable
        /// </summary>
        public static Vector1D Vector(IEnumerable<string> values)
             => new(values);
        /// <summary>
        /// Create vector from variable length arguments
        /// </summary>
        public static Vector1D Vector(params double[] values)
            => new(values);
        /// <summary>
        /// Create vector from string
        /// </summary>
        public static Vector1D Vector(string values)
            => new(values);
        #endregion

        #region Data Loading
        /// <summary>
        /// Load values from CSV or plain text file
        /// </summary>
        public static Vector1D Load(string path, bool containsHeaderRow, int takeNthCSVColumn = 0)
        {
            if (!File.Exists(path))
                Console.WriteLine($"File {path} doesn't exist.");

            return new Vector1D(File.ReadLines(path)
                .Skip(containsHeaderRow ? 1 : 0)
                .Select(line => line.Split(',')[takeNthCSVColumn])
                .Select(double.Parse));
        }
        #endregion
    }
}
