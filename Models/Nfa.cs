using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Nfa : INotifyPropertyChanged
    {
        private List<Polygon2> polygons = new List<Polygon2>();

        [DisplayName("Polygons")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<Polygon2> Polygons
        {
            get { return polygons; }
            set
            {
                polygons = value;
                OnPropertyChanged("Polygons");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Nfa()
        {
            Instance = this;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Nfa Instance = null;
    }
}
