using MapEditor.Attributes;
using MapEditor.Modules;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class K3DPosition
    {
        private float x = 0;
        private float y = 0;
        private float z = 0;
		
        [PropertyGridBrowsable(false)]
        public float X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }
		
        [PropertyGridBrowsable(false)]
        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
		}

		[DisplayName("X")]
		[PropertyGridBrowsable(true)]
		public float mX
		{
			get { return (MapWorker.Instance.X * 16128) + x; }
			set
			{
				var part = MapWorker.Instance.X * 16128;
				var partmax = MapWorker.Instance.X * 16128 + 16128;

				if (value < part || value > partmax)
				{
					x = 0;
					return;
				}

				x = (value - part);
				OnPropertyChanged("X");
			}
		}

		[DisplayName("Y")]
		[PropertyGridBrowsable(true)]
		public float mY
		{
			get { return (MapWorker.Instance.Y * 16128) + y; }
			set
			{
				var part = MapWorker.Instance.Y * 16128;
				var partmax = MapWorker.Instance.Y * 16128 + 16128;

				if (value < part || value > partmax)
				{
					y = 0;
					return;
				}

				y = (value - part);
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

        public K3DPosition() : this(0f, 0f, 0f) { }

        public K3DPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return nameof(K3DPosition);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
