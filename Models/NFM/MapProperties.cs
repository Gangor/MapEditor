using MapEditor.Attributes;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class MapProperties : INotifyPropertyChanged
    {
        private K3DLight primary = new K3DLight();
        private K3DLight secondary = new K3DLight();
        private KColor sky = new KColor();
        private KColor fog = new KColor();
        private float fogNear = 0.1f;
        private float fogFar = 5500.0f;
        private uint skyType;
        private bool showTerrainInGame = true;

        [Category("Lights")]
        [DisplayName("Primary")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DLight Primary
        {
            get { return primary; }
            set
            {
                primary = value;
                OnPropertyChanged("Primary");
            }
        }

        [Category("Lights")]
        [DisplayName("Secondary")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DLight Secondary
        {
            get { return secondary; }
            set
            {
                secondary = value;
                OnPropertyChanged("Secondary");
            }
        }

        [DisplayName("Sky")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KColor Sky
        {
            get { return sky; }
            set
            {
                sky = value;
                OnPropertyChanged("Sky");
            }
        }

        [DisplayName("Fog")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KColor Fog
        {
            get { return fog; }
            set
            {
                fog = value;
                OnPropertyChanged("Fog");
            }
        }

        [DisplayName("Fog near")]
        [PropertyGridBrowsable(true)]
        public float FogNear
        {
            get { return fogNear; }
            set
            {
                fogNear = value;
                OnPropertyChanged("FogNear");
            }
        }

        [DisplayName("Fog far")]
        [PropertyGridBrowsable(true)]
        public float FogFar
        {
            get { return fogFar; }
            set
            {
                fogFar = value;
                OnPropertyChanged("FogFar");
            }
        }

        [DisplayName("Sky type")]
        [PropertyGridBrowsable(true)]
        public uint SkyType
        {
            get { return skyType; }
            set
            {
                skyType = value;
                OnPropertyChanged("SkyType");
            }
        }

        [DisplayName("Show terrain in game")]
        [PropertyGridBrowsable(true)]
        public bool ShowTerrainInGame
        {
            get { return showTerrainInGame; }
            set
            {
                showTerrainInGame = value;
                OnPropertyChanged("ShowTerrainInGame");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return nameof(MapProperties);
        }
    }
}
