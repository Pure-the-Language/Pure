using K4os.Compression.LZ4.Streams;
using System.Text;

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
        Scatter
    }

    /// <summary>
    /// General configurations for plots, certain values are only applicable to specific plots
    /// </summary>
    public struct PlotOptions
    {
        public string Title { get; set; }
        public string XAxis { get; set; }
    }

    /// <summary>
    /// Main interoperability data transfer between this library and display backend (frontend)
    /// </summary>
    public class InteractivePlotData
    {
        #region Data
        public PlotType PlotType { get; set; }
        public double[] X { get; set; }
        public List<double[]> Ys { get; set; }
        public PlotOptions Options { get; set; }
        #endregion

        #region Serialization
        public static InteractivePlotData LoadData(string filepath)
        {
            using LZ4DecoderStream source = LZ4Stream.Decode(File.OpenRead(filepath));
            using BinaryReader reader = new(source, Encoding.UTF8, false);
            return ReadFromStream(reader);
        }
        public static void SaveData(InteractivePlotData data, string filepath)
        {
            using LZ4EncoderStream stream = LZ4Stream.Encode(File.Create(filepath));
            using BinaryWriter writer = new(stream, Encoding.UTF8, false);
            WriteToStream(writer, data);
        }
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
        }
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

            return data;
        }
        #endregion
    }
}
