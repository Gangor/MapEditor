using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Nfl : INotifyPropertyChanged
    {
        private K3DVector direction = new K3DVector(0f, 0f, 0f);
        private KColor specular = new KColor();
        private KColor diffuse = new KColor();
        private KColor ambient = new KColor(0, 255, 0, 255);
        private List<StructLights> lights = new List<StructLights>();

        public event PropertyChangedEventHandler PropertyChanged;
        
        [Category("Direct light")]
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
        
        [Category("Direct light")]
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

        [Category("Direct light")]
        [DisplayName("Diffuse")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KColor Diffuse
        {
            get { return diffuse; }
            set
            {
                diffuse = value;
                OnPropertyChanged("DefaultDiffuse");
            }
        }

        [Category("Direct light")]
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

        [DisplayName("Lights")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<StructLights> Lights
        {
            get { return lights; }
            set
            {
                lights = value;
                OnPropertyChanged("Lights");
            }
        }

        public Nfl()
        {
            Instance = this;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Nfl Instance = null;
    }
}
