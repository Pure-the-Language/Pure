namespace Graphing
{
    /// <summary>
    /// Available plot types
    /// </summary>
    public enum PlotType
    {
        /// <summary>
        /// Basic scatter plog
        /// </summary>
        Scatter,
        /// <summary>
        /// Basic line charat
        /// </summary>
        Line,
        /// <summary>
        /// Evenlly sampled with sample rate
        /// </summary>
        Signal,
        /// <summary>
        /// Histogram
        /// </summary>
        Histogram
    }

    /// <summary>
    /// General configurations for plots, certain values are only applicable to specific plots.
    /// Serialized in <see cref="InteractivePlotData"/>
    /// </summary>
    public class PlotOptions
    {
        #region Output Dimension
        public int WindowWidth { get; set; } = 400;
        public int WindowHeight { get; set; } = 300;
        #endregion

        #region Output Behavior
        public bool Interactive { get; set; } = false;
        public string OutputImage { get; set; } = string.Empty; // Remark-cz: Cannot be null because we are serializing this
        #endregion

        #region Plot Customization
        public bool DrawTitle { get; set; } = false;
        public bool DrawAxes { get; set; } = false;

        public string Title { get; set; } = "Untitled";
        public string XAxis { get; set; } = "X Axis";
        public string YAxis { get; set; } = "Y Axis";

        /// <summary>
        /// Series labels
        /// </summary>
        public string[] Labels { get; set; } = Array.Empty<string>();
        #endregion

        #region Plot Type Specific
        /// <summary>
        /// Number of bars for histogram; Input data must have more than this number of elements otherwise the value is not used.
        /// </summary>
        public int HistogramBars { get; set; } = 20;
        /// <summary>
        /// Applies to Signal type plot
        /// </summary>
        public int SignalSampleRate { get; set; } = 100;
        #endregion
    }
}
