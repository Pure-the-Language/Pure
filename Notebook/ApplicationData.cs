using K4os.Compression.LZ4.Streams;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Notebook
{
    public enum CellType
    {
        Markdown,
        CacheOutput,
        CSharp,
        Python
    }
    public class CellBlock : BaseNotifyPropertyChanged
    {
        #region Attributes
        private CellType _CellType;
        private string _Content = string.Empty;
        #endregion

        #region Data Bindings
        public CellType CellType { get => _CellType; set => SetField(ref _CellType, value); }
        public string Content { get => _Content; set => SetField(ref _Content, value); }
        #endregion
    }
    public class ApplicationData : BaseNotifyPropertyChanged
    {
        #region Attributes
        private ObservableCollection<CellBlock> _Cells = new();
        #endregion

        #region Data Bindings
        public ObservableCollection<CellBlock> Cells { get => _Cells; set => SetField(ref _Cells, value); }
        #endregion

        #region Methods
        public static void Save(string filepath, ApplicationData data, bool compressed = true)
        {
            if (compressed)
            {
                using LZ4EncoderStream stream = LZ4Stream.Encode(File.Create(filepath));
                using BinaryWriter writer = new(stream, Encoding.UTF8, false);
                WriteToStream(writer, data);
            }
            else
            {
                using FileStream stream = File.Open(filepath, FileMode.Create);
                using BinaryWriter writer = new(stream, Encoding.UTF8, false);
                WriteToStream(writer, data);
            }
        }
        public static ApplicationData Load(string filepath, bool compressed = true)
        {
            if (compressed)
            {
                using LZ4DecoderStream source = LZ4Stream.Decode(File.OpenRead(filepath));
                using BinaryReader reader = new(source, Encoding.UTF8, false);
                return ReadFromStream(reader);
            }
            else
            {
                using FileStream stream = File.Open(filepath, FileMode.Open);
                using BinaryReader reader = new(stream, Encoding.UTF8, false);
                return ReadFromStream(reader);
            }
        }
        #endregion

        #region Routines
        private static void WriteToStream(BinaryWriter writer, ApplicationData data)
        {
            writer.Write(data.Cells.Count);
            foreach (CellBlock cell in data.Cells)
            {
                writer.Write((byte)cell.CellType);
                writer.Write(cell.Content);
            }
        }
        private static ApplicationData ReadFromStream(BinaryReader reader)
        {
            ApplicationData applicationData = new();

            {
                var cellCount = reader.ReadInt32();
                for (int i = 0; i < cellCount; i++)
                {
                    CellBlock cell = new CellBlock()
                    {
                        CellType = (CellType)reader.ReadByte(),
                        Content = reader.ReadString()
                    };
                    applicationData.Cells.Add(cell);
                }
            }

            return applicationData;
        }
        #endregion
    }
}
