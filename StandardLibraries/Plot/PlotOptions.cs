namespace Plot
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
        LineChart
    }

    /// <summary>
    /// General configurations for plots, certain values are only applicable to specific plots
    /// </summary>
    public class PlotOptions
    {
        public int WindowWidth { get; set; } = 400;
        public int WindowHeight { get; set; } = 300;

        public bool DrawTitle { get; set; } = false;
        public bool DrawAxies { get; set; } = false;
        public bool SaveImage { get; set; } = false;

        public string Title { get; set; } = "Untitled";
        public string XAxis { get; set; } = "X Axis";
        public string YAxis { get; set; } = "Y Axis";
        public string ImageOutput { get; set; } = "Output.png";
    }
}
