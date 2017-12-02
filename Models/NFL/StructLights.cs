using MapEditor.Attributes;
using MapEditor.Enum;
using MapEditor.Modules;
using System.ComponentModel;
using System.Drawing;

namespace MapEditor.Models
{
    public class StructLights : INotifyPropertyChanged
    {
        private K3DPosition position = new K3DPosition(0f, 0f, 0f);
        private float height = 0;
        private K3DVector direction = new K3DVector(0f, 0f, 0f);
        private KColor specular = new KColor(0, 0, 0, 0);
        private KColor diffuse = new KColor(0, 0, 0, 0);
        private KColor ambient = new KColor(0, 0, 0, 0);
        private LightsType lightType = LightsType.LIGHT_OMNI;

        [DisplayName("Position")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DPosition Position
        {
            get { return position; }
            set
            {
                position = value;
                OnPropertyChanged("Position");
            }
        }

        [DisplayName("Height")]
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

        [DisplayName("Direction")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DVector Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                OnPropertyChanged("Direction");
            }
        }

        [Category("Color")]
        [DisplayName("Specular")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KColor Specular
        {
            get { return specular; }
            set
            {
                specular = value;
                OnPropertyChanged("Specular");
            }
        }

        [Category("Color")]
        [DisplayName("Diffuse")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KColor Diffuse
        {
            get { return diffuse; }
            set
            {
                diffuse = value;
                OnPropertyChanged("Diffuse");
            }
        }

        [Category("Color")]
        [DisplayName("Ambient")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KColor Ambient
        {
            get { return ambient; }
            set
            {
                ambient = value;
                OnPropertyChanged("Ambient");
            }
        }

        [Category("Type")]
        [DisplayName("Light type")]
        [PropertyGridBrowsable(true)]
        public LightsType LightType
        {
            get { return lightType; }
            set
            {
                lightType = value;
                OnPropertyChanged("LightType");
            }
        }

        public override string ToString()
        {
            return nameof(StructLights);
        }

        /// <summary>
        /// Convert to pixel location
        /// </summary>
        /// <returns></returns>
        public PointF ToPoint()
        {
			return _2DUtils.UnAdjustPoint(new PointF(Position.X, Position.Y), 1f, false, false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
