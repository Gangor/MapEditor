using MapEditor.Attributes;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class KGrassProp : INotifyPropertyChanged
    {
        private float x;
        private float y;
        private float rotateX;
        private float rotateY;
        private float rotateZ;

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

        [DisplayName("Rotate X")]
        [PropertyGridBrowsable(true)]
        public float RotateX
        {
            get { return rotateX; }
            set
            {
                rotateX = value;
                OnPropertyChanged("RotateX");
            }
        }

        [DisplayName("Rotate Y")]
        [PropertyGridBrowsable(true)]
        public float RotateY
        {
            get { return rotateY; }
            set
            {
                rotateY = value;
                OnPropertyChanged("RotateY");
            }
        }

        [DisplayName("Rotate Z")]
        [PropertyGridBrowsable(true)]
        public float RotateZ
        {
            get { return rotateZ; }
            set
            {
                rotateZ = value;
                OnPropertyChanged("RotateZ");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return nameof(KGrassProp);
        }
    }
}
