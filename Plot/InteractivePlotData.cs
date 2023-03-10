using K4os.Compression.LZ4.Streams;
using System.Text;

namespace Plot
{
    public enum PlotType
    {
        Scatter
    }
    public class InteractivePlotData
    {
        #region Data
        public PlotType PlotType { get; set; }
        public double[] X { get; set; }
        public List<double[]> Ys { get; set; }
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
            writer.Write((byte)data.PlotType);
            writer.Write(data.X.Length);
            foreach (var x in data.X)
                writer.Write(x);
            writer.Write(data.Ys.Count);
            foreach (var ys in data.Ys)
            {
                writer.Write(ys.Length);
                foreach (var y in ys)
                    writer.Write(y);
            }
        }
        public static InteractivePlotData ReadFromStream(BinaryReader reader)
        {
            InteractivePlotData data = new ();

            data.PlotType = (PlotType)reader.ReadByte();
            data.X = new double[reader.ReadInt32()];
            for (int i = 0; i < data.X.Length; i++)
                data.X[i] = reader.ReadDouble();
            int ySeriesLength = reader.ReadInt32();
            data.Ys = new List<double[]>();
            for (int yc = 0; yc < ySeriesLength; yc++)
            {
                double[] y = new double[reader.ReadInt32()];
                for (int i = 0; i < y.Length; i++)
                    y[i] = reader.ReadDouble();
                data.Ys.Add(y);
            }

            return data;
        }
        #endregion
    }
}
