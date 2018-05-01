using MapEditor.Attributes;
using System;
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



		[Browsable(false)]
		public float RotateX
		{
			get { return rotateX; }
			set
			{
				rotateX = value;
				OnPropertyChanged("RotateX");
			}
		}

		[Browsable(false)]
		public float RotateY
		{
			get { return rotateY; }
			set
			{
				rotateY = value;
				OnPropertyChanged("RotateY");
			}
		}

		[Browsable(false)]
		public float RotateZ
		{
			get { return rotateZ; }
			set
			{
				rotateZ = value;
				OnPropertyChanged("RotateZ");
			}
		}

		[DisplayName("Rotate X")]
		public float mRotateX
		{
			get { return (float)(RotateX * (180.0 / Math.PI)); }
			set
			{
				RotateX = (float)(Math.PI * value / 180.0);
				OnPropertyChanged("RotateX");
			}
		}

		[DisplayName("Rotate Y")]
		public float mRotateY
		{
			get { return (float)(RotateY * (180.0 / Math.PI)); }
			set
			{
				RotateY = (float)(Math.PI * value / 180.0);
				OnPropertyChanged("RotateY");
			}
		}

		[DisplayName("Rotate Z")]
		public float mRotateZ
		{
			get { return (float)(RotateZ * (180.0 / Math.PI)); }
			set
			{
				RotateZ = (float)(Math.PI * value / 180.0);
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
