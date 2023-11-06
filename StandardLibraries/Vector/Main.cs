namespace Math
{
    /// <summary>
    /// Library entrance
    /// </summary>
    public static class Main
    {
        #region Vector Creation Rouine
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
    }
}
