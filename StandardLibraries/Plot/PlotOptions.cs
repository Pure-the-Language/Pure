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
        LineChart,
        /// <summary>
        /// Evenlly sampled with sample rate
        /// </summary>
        Signal
    }

    /// <summary>
    /// General configurations for plots, certain values are only applicable to specific plots
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
        public bool DrawAxies { get; set; } = false;

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
        /// Applies to Signal type plot
        /// </summary>
        public int SignalSampleRate { get; set; } = 100;
        #endregion
    }
}
