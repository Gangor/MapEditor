using MapEditor.Attributes;
using MapEditor.Extends;
using System.ComponentModel;
using System.Drawing;

namespace MapEditor.Models
{
    public class Water : INotifyPropertyChanged
    {
        private K3DVector pointA = new K3DVector();
        private K3DVector pointB = new K3DVector();
        private K3DVector center = new K3DVector();
        private int useReflect = 0;
        private int waterId = 0;

        [Browsable(false)]
        [DisplayName("Angle 1")]
        [Category("Coordonnate")]
        [Description("Different point of water location")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DVector PointA
        {
            get { return pointA; }
            set
            {
                pointA = value;
                OnPropertyChanged("PointA");
            }
        }

        [Browsable(false)]
        [DisplayName("Angle 2")]
        [Category("Coordonnate")]
        [Description("Different point of water location")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DVector PointB
        {
            get { return pointB; }
            set
            {
                pointB = value;
                OnPropertyChanged("PointB");
            }
        }

        [Browsable(false)]
        [DisplayName("Center")]
        [Category("Coordonnate")]
        [Description("Center point of water location")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DVector Center
        {
            get { return center; }
            set
            {
                center = value;
                OnPropertyChanged("Center");
            }
        }

        [DisplayName("Use reflection")]
        [Category("Aspect")]
        [Description("Enables or disables water reflection")]
        [PropertyGridBrowsable(true)]
        public int UseReflect
        {
            get { return useReflect; }
            set
            {
                useReflect = value;
                OnPropertyChanged("UseReflect");
            }
        }

        [DisplayName("Water Id")]
        [Category("Aspect")]
        [Description("Represents the texture or color")]
        [PropertyGridBrowsable(true)]
        public int WaterId
        {
            get { return waterId; }
            set
            {
                waterId = value;
                OnPropertyChanged("WaterId");
            }
        }

        [DisplayName("Color")]
        [Category("Aspect")]
        [Description("Setting coloring water")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Color Color
        {
            get { return Color.FromArgb(WaterId); }
            set
            {
                waterId = value.ToArgb();
                OnPropertyChanged("WaterId");
            }
        }
        
        public override string ToString()
        {
            return Properties.Resources.ResourceManager.GetStringFormat("Water_Name_Text", waterId);
        }

        /// <summary>
        /// Convert to pixel location
        /// </summary>
        /// <returns></returns>
        public PointF[] ToPoints()
        {
            var points = new PointF[3];
            points[0] = new PointF((PointA.X / 7.875f), (PointA.Y / 7.875f));
            points[1] = new PointF((PointB.X / 7.875f), (PointB.Y / 7.875f));
            points[2] = new PointF((Center.X / 7.875f), (Center.Y / 7.875f));
            return points;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
