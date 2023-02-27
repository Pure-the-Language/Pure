using K4os.Compression.LZ4.Streams;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Righteous
{
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        #region Data Binding
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<TType>(ref TType field, TType value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TType>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }

    public class ApplicationData : BaseNotifyPropertyChanged
    {
        #region Fields
        private string _Name = string.Empty;
        private string _Description = string.Empty;
        private ObservableCollection<DataModel> _Steps = new();
        #endregion

        #region Data Binding
        public string Name { get => _Name; set => SetField(ref _Name, value); }
        public string Description { get => _Description; set => SetField(ref _Description, value); }
        public ObservableCollection<DataModel> Steps { get => _Steps; set => SetField(ref _Steps, value); }
        #endregion

        #region Methods
        internal void Reorder(DataModel model, int order)
        {
            if (!_Steps.Contains(model))
                throw new ApplicationException("Step doesn't exist.");
            _Steps.Remove(model);
            _Steps.Insert(order, model);
            for (int i = 0; i < _Steps.Count; i++)
            {
                DataModel item = _Steps[i];
                item.ID = i;
            }
        }
        #endregion
    }

    public static class ApplicationDataSerializer
    {
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
            writer.Write(data.Name);
            writer.Write(data.Description);

            writer.Write(data.Steps.Count);
            foreach (DataModel model in data.Steps)
            {
                writer.Write(model.ID);
                writer.Write(model.Name);
                writer.Write(model.Description);
                writer.Write(model.Scripts);
                writer.Write(model.Location.X);
                writer.Write(model.Location.Y);
            }
        }
        private static ApplicationData ReadFromStream(BinaryReader reader)
        {
            ApplicationData applicationData = new()
            {
                Name = reader.ReadString(),
                Description = reader.ReadString()
            };

            {
                var stepsCount = reader.ReadInt32();
                for (int i = 0; i < stepsCount; i++)
                {
                    DataModel model = new(i)
                    {
                        ID = reader.ReadInt32(),
                        Name = reader.ReadString(),
                        Description = reader.ReadString(),
                        Scripts = reader.ReadString(),
                        Location = new System.Windows.Vector(reader.ReadDouble(), reader.ReadDouble())
                    };

                    applicationData.Steps.Add(model);
                }
            }

            return applicationData;
        }
        #endregion
    }
}
