using MapEditor.Attributes;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class K2DVector : INotifyPropertyChanged
    {
        private int x = 0;
        private int y = 0;

        [DisplayName("X")]
        [PropertyGridBrowsable(true)]
        public int X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }

        [DisplayName("Y")]
        [PropertyGridBrowsable(true)]
        public int Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }

        public K2DVector() { }

        public K2DVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return nameof(K2DVector);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
