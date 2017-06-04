using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Nfw : INotifyPropertyChanged
    {
        private List<Water> waters = new List<Water>();

        [PropertyGridBrowsable(true)]
        [DisplayName("Waters")]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<Water> Waters
        {
            get { return waters; }
            set
            {
                waters = value;
                OnPropertyChanged("Waters");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Nfw(bool instanced)
        {
            if (instanced)
            {
                Instance = this;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Nfw Instance = null;
    }
}
