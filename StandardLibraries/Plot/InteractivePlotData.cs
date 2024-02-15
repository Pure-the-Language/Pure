using K4os.Compression.LZ4.Streams;
using System.Text;

namespace Graphing
{
    /// <summary>
    /// Main interoperability data transfer between this library and display backend (frontend)
    /// </summary>
    public class InteractivePlotData
    {
        #region Data
        /// <summary>
        /// Plot type.
        /// </summary>
        public PlotType PlotType { get; set; }
        /// <summary>
        /// Main X-axis numerical data.
        /// </summary>
        public double[] X { get; set; }
        /// <summary>
        /// Main Y-axis values.
        /// </summary>
        public List<double[]> Ys { get; set; }
        /// <summary>
        /// Additional customization options.
        /// </summary>
        public PlotOptions Options { get; set; }
        #endregion

        #region Serialization
        /// <summary>
        /// Load plot data from compressed file.
        /// </summary>
        public static InteractivePlotData LoadData(string filepath)
        {
            using LZ4DecoderStream source = LZ4Stream.Decode(File.OpenRead(filepath));
            using BinaryReader reader = new(source, Encoding.UTF8, false);
            return ReadFromStream(reader);
        }
        /// <summary>
        /// Save plot data as compressed file.
        /// </summary>
        public static void SaveData(InteractivePlotData data, string filepath)
        {
            using LZ4EncoderStream stream = LZ4Stream.Encode(File.Create(filepath));
            using BinaryWriter writer = new(stream, Encoding.UTF8, false);
            WriteToStream(writer, data);
        }
        /// <summary>
        /// Save plot data to stream.
        /// </summary>
        public static void WriteToStream(BinaryWriter writer, InteractivePlotData data)
        {
            // Plot type
            writer.Write((byte)data.PlotType);
            // X
            writer.Write(data.X.Length);
            foreach (var x in data.X)
                writer.Write(x);
            // Y
            writer.Write(data.Ys.Count);
            foreach (var ys in data.Ys)
            {
                writer.Write(ys.Length);
                foreach (var y in ys)
                    writer.Write(y);
            }
            // Options
            writer.Write(data.Options.WindowWidth);
            writer.Write(data.Options.WindowHeight);
            writer.Write(data.Options.Interactive);
            writer.Write(data.Options.OutputImage);
            writer.Write(data.Options.DrawTitle);
            writer.Write(data.Options.DrawAxes);
            writer.Write(data.Options.Title);
            writer.Write(data.Options.XAxis);
            writer.Write(data.Options.YAxis);
            writer.Write(data.Options.Labels.Length);
            foreach (string label in data.Options.Labels)
                writer.Write(label);
            writer.Write(data.Options.HistogramBars);
            writer.Write(data.Options.SignalSampleRate);
        }
        /// <summary>
        /// Read plot data from stream.
        /// </summary>
        public static InteractivePlotData ReadFromStream(BinaryReader reader)
        {
            InteractivePlotData data = new();

            // Plot type
            data.PlotType = (PlotType)reader.ReadByte();
            // X
            data.X = new double[reader.ReadInt32()];
            for (int i = 0; i < data.X.Length; i++)
                data.X[i] = reader.ReadDouble();
            // Y
            int ySeriesLength = reader.ReadInt32();
            data.Ys = new List<double[]>();
            for (int yc = 0; yc < ySeriesLength; yc++)
            {
                double[] y = new double[reader.ReadInt32()];
                for (int i = 0; i < y.Length; i++)
                    y[i] = reader.ReadDouble();
                data.Ys.Add(y);
            }
            // Options
            data.Options = new PlotOptions()
            {
                WindowWidth = reader.ReadInt32(),
                WindowHeight = reader.ReadInt32(),
                Interactive = reader.ReadBoolean(),
                OutputImage = reader.ReadString(),
                DrawTitle = reader.ReadBoolean(),
                DrawAxes = reader.ReadBoolean(),
                Title = reader.ReadString(),
                XAxis = reader.ReadString(),
                YAxis = reader.ReadString(),
                Labels = Enumerable.Range(0, reader.ReadInt32())
                    .Select(i => reader.ReadString())
                    .ToArray(),
                HistogramBars = reader.ReadInt32(),
                SignalSampleRate = reader.ReadInt32(),
            };

            return data;
        }
        #endregion
    }
}
