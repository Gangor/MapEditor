using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Nfe : INotifyPropertyChanged
    {
        private List<EventAreaScript> @event = new List<EventAreaScript>();

        [DisplayName("Events")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<EventAreaScript> Events
        {
            get { return @event; }
            set
            {
                @event = value;
                OnPropertyChanged("Events");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Nfe(bool instanced)
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

        public static Nfe Instance = null;
    }
}
