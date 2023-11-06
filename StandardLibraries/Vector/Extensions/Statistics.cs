namespace Math
{
    public partial class Vector1D : IList<double>
    {
        #region Statistical Measures
        /// <summary>
        /// Get min.
        /// </summary>
        public double Min => Values.Min();
        /// <summary>
        /// Get mean.
        /// </summary>
        public double Mean => Values.Average();
        /// <summary>
        /// Get average (same as mean).
        /// </summary>
        public double Average => Values.Average();
        /// <summary>
        /// Get max.
        /// </summary>
        public double Max => Values.Max();
        /// <summary>
        /// Get sum.
        /// </summary>
        public double Sum => Values.Sum();
        /// <summary>
        /// Get variance.
        /// </summary>
        public double Variance
        {
            get
            {
                double variance = 0.0;
                if (Values.Length > 1)
                {
                    double avg = Values.Average();
                    variance += Values.Sum(value => System.Math.Pow(value - avg, 2.0));
                }
                // For population, use n-1, for sample, use n
                return variance / Values.Length;
            }
        }
        /// <summary>
        /// Get std.
        /// </summary>
        public double STD
            => System.Math.Sqrt(Variance);
        /// <summary>
        /// Get population variance.
        /// </summary>
        public double PopulationVariance
        {
            get
            {
                double variance = 0.0;
                if (Values.Length > 1)
                {
                    double avg = Values.Average();
                    variance += Values.Sum(value => System.Math.Pow(value - avg, 2.0));
                }
                // For population, use n-1, for sample, use n
                return variance / (Values.Length - 1);
            }
        }
        /// <summary>
        /// Get population std.
        /// </summary>
        public double PopulationSTD
            => System.Math.Sqrt(Variance);
        #endregion

        #region Statistics Methods
        /// <summary>
        /// Compute correlation
        /// </summary>
        public double Correlation(Vector1D other)
        {
            double covariance = Covariance(other);
            double std1 = PopulationSTD;   // Always use n-1 for population
            double std2 = other.PopulationSTD;
            return covariance / (std1 * std2);
        }
        /// <summary>
        /// Compute covariance
        /// </summary>
        public double Covariance(Vector1D other)
        {
            if (Values.Length != other.Values.Length)
                throw new ArgumentException("Vector size doesn't match");

            double variance = 0.0;
            if (Values.Length > 1)
            {
                double avg1 = Values.Average();
                double avg2 = Values.Average();
                for (int i = 0; i < Values.Length; i++)
                    variance += (Values[i] - avg1) * (other.Values[i] - avg2);
            }
            return variance / (Values.Length - 1); // Always use n-1 for population
        }
        #endregion
    }
}
