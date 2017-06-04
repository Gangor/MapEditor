using MapEditor.Attributes;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class K3DVector
    {
        private float x = 0;
        private float y = 0;
        private float z = 0;

        [DisplayName("X")]
        [PropertyGridBrowsable(true)]
        public float X
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
        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }

        [DisplayName("Z")]
        [PropertyGridBrowsable(true)]
        public float Z
        {
            get { return z; }
            set
            {
                z = value;
                OnPropertyChanged("Z");
            }
        }

        public K3DVector() : this(0f, 0f, 0f) { }

        public K3DVector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return nameof(K3DVector);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
