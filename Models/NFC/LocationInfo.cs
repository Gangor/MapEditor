using MapEditor.Attributes;
using MapEditor.Extends;
using MapEditor.Modules;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class LocationInfo : INotifyPropertyChanged
    {
        private int priority = 1;
        private float x = 0;
        private float y = 0;
        private float z = 0;
        private float radius = 0;
        private string description = "";
        private string scripts = "";
        private List<Polygon2> polygons = new List<Polygon2>();

        [PropertyGridBrowsable(true)]
        [DisplayName("Priority")]
        [Description("The higher the number, the higher the priority")]
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                OnPropertyChanged("Priority");
            }
		}

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
		[Category("Coordonnate")]
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
		[Category("Coordonnate")]
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
        [Category("Coordonnate")]
        [PropertyGridBrowsable(true)]
        public float Z
        {
            get { return z; }
            set
            {
                z = value;
                OnPropertyChanged("Y");
            }
        }

        [DisplayName("Radius")]
        [Category("Coordonnate")]
        [PropertyGridBrowsable(true)]
        public float Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                OnPropertyChanged("Radius");
            }
        }

        [DisplayName("Description")]
        [PropertyGridBrowsable(true)]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        [DisplayName("Scripts")]
        [PropertyGridBrowsable(true)]
        public string Scripts
        {
            get { return scripts; }
            set
            {
                scripts = value;
                OnPropertyChanged("Scripts");
            }
        }

        [DisplayName("Polygons")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<Polygon2> Polygons
        {
            get { return polygons; }
            set
            {
                polygons = value;
                OnPropertyChanged("Polygons");
            }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Convert to pixel location
		/// </summary>
		/// <returns></returns>
		public PointF ToPoint()
		{
			return _2DUtils.UnAdjustPoint(new PointF(X, Y), 1f, false, false);
		}

		public override string ToString()
        {
            return nameof(LocationInfo);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
