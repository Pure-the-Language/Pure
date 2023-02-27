using System.Windows;

namespace Righteous
{
    public class DataModel: BaseNotifyPropertyChanged
    {
        #region Constructor
        public DataModel(int id)
        {
            ID = id;
        }
        #endregion

        #region Properties
        private int _Id;
        private string _Name;
        private string _Description;
        private string _Scripts;
        private Vector _Location;
        #endregion

        #region Data Binding Properties
        public int ID { get => _Id; set => SetField(ref _Id, value); }
        public string Name { get => _Name; set => SetField(ref _Name, value); }
        public string Description { get => _Description; set => SetField(ref _Description, value); }
        public string Scripts { get => _Scripts; set => SetField(ref _Scripts, value); }
        public Vector Location { get => _Location; set => SetField(ref _Location, value); }
        #endregion
    }
}
