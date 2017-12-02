using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class KGrass : INotifyPropertyChanged
    {
        private int segmentId = 0;
        private int grassId = 0;
        private List<KGrassProp> props = new List<KGrassProp>();

        [DisplayName("Segment Id")]
        public int SegmentId
        {
            get { return segmentId; }
            set
            {
                segmentId = value;
                OnPropertyChanged("SegmentId");
            }
        }

        [DisplayName("Grass Id")]
        public int GrassId
        {
            get { return grassId; }
            set
            {
                grassId = value;
                OnPropertyChanged("GrassId");
            }
        }

        [Browsable(false)]
        [DisplayName("Props")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<KGrassProp> Props
        {
            get { return props; }
            set
            {
                props = value;
                OnPropertyChanged("Props");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public KGrass Clone()
        {
            return (KGrass)MemberwiseClone();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return nameof(KGrass);
        }
    }
}
