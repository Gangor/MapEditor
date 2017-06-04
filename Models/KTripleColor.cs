using MapEditor.Attributes;
using System.ComponentModel;
using System.Drawing;

namespace MapEditor.Models
{
    public class KTripleColor : INotifyPropertyChanged
    {
        private byte b = 0;
        private byte g = 0;
        private byte r = 0;

        [DisplayName("Color")]
        [PropertyGridBrowsable(true)]
        public Color Color
        {
            get { return Color.FromArgb(r, g, b); }
            set
            {
                b = value.B;
                g = value.G;
                r = value.R;
                OnPropertyChanged("Color");
            }
        }

        public KTripleColor(byte b = 255, byte g = 255, byte r = 255)
        {
            this.b = b;
            this.g = g;
            this.r = r;
        }

        public override string ToString()
        {
            return nameof(KTripleColor);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
