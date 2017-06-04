using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Nfp : INotifyPropertyChanged
    {
        private List<RecordNfp> records = new List<RecordNfp>();

        [DisplayName("Records")]
        [PropertyGridBrowsable(true)]
        public List<RecordNfp> Records
        {
            get { return records; }
            set
            {
                records = value;
                OnPropertyChanged("Records");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Nfp()
        {
            Instance = this;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Nfp Instance = null;
    }

    public class RecordNfp : INotifyPropertyChanged
    {
        private int id = 0;
        private string description = "";
        private List<Polygon3> polygons = new List<Polygon3>();

        [DisplayName("Id")]
        [PropertyGridBrowsable(true)]
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        [DisplayName("Polygons")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<Polygon3> Polygons
        {
            get { return polygons; }
            set
            {
                polygons = value;
                OnPropertyChanged("Polygons");
            }
        }

        [DisplayName("Description")]
        [PropertyGridBrowsable(true)]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
