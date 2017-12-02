using MapEditor.Attributes;
using System.ComponentModel;

namespace MapEditor.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class KVertex : INotifyPropertyChanged
    {
        private float height = 0.0f;
        private uint[] fillBits = new uint[2];
        private long attribute = 0;
        private KTripleColor color = new KTripleColor();

        [DisplayName("Height")]
        [Category("Height")]
        [PropertyGridBrowsable(true)]
        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }
        
        [PropertyGridBrowsable(false)]
        public uint[] FillBits
        {
            get { return fillBits; }
            set
            {
                fillBits = value;
                OnPropertyChanged("FillBits");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Fill 01")]
        public uint FillBit01
        {
            get { return FillBits[0]; }
            set
            {
                FillBits[0] = value;
                OnPropertyChanged("FillBit01");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Fill 02")]
        public uint FillBit02
        {
            get { return FillBits[1]; }
            set
            {
                FillBits[1] = value;
                OnPropertyChanged("FillBit02");
            }
        }

        [DisplayName("Attribute")]
        [PropertyGridBrowsable(true)]
        public long Attribute
        {
            get { return attribute; }
            set
            {
                attribute = value;
                OnPropertyChanged("Attribute");
            }
        }

        [DisplayName("Color")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KTripleColor Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return nameof(KVertex);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
