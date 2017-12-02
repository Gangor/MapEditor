using MapEditor.Attributes;
using MapEditor.Extends;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class Nfm : INotifyPropertyChanged
    {
        private string sign = "nFlavor Map\0\0\0\0\0";
        private int version = 22;
        private int tileCountPerSegment = 6;
        private int segmentCountPerMap = 64;
        private float tileLenght = 42.0f;
        private MapProperties mapProperties = new MapProperties();
        private KSegment[,] dwTerrainSegment = new KSegment[64,64];
        private SortableBindingList<KProp> dwProps = new SortableBindingList<KProp>();
        private SortableBindingList<KGrass> dwGrass = new SortableBindingList<KGrass>();
        private SortableBindingList<Polygon2> dwVectorAttr = new SortableBindingList<Polygon2>();
        private SortableBindingList<Water> dwWater = new SortableBindingList<Water>();
        private SortableBindingList<SpeedGrassColony> dwGrassColony = new SortableBindingList<SpeedGrassColony>();
        private SortableBindingList<EventAreaScript> dwEventArea = new SortableBindingList<EventAreaScript>();
        
        [ReadOnly(true)]
        [DisplayName("Signature")]
        [PropertyGridBrowsable(true)]
        public string Sign
        {
            get { return sign; }
            set
            {
                sign = value;
                OnPropertyChanged("Sign");
            }
        }

        [ReadOnly(true)]
        [DisplayName("Version")]
        [PropertyGridBrowsable(true)]
        public int Version
        {
            get { return version; }
            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }

        [ReadOnly(true)]
        [DisplayName("Tile per segment")]
        [PropertyGridBrowsable(true)]
        public int TileCountPerSegment
        {
            get { return tileCountPerSegment; }
            set
            {
                tileCountPerSegment = value;
                OnPropertyChanged("TileCountPerSegment");
            }
        }
        
        [ReadOnly(true)]
        [DisplayName("Segment per map")]
        [PropertyGridBrowsable(true)]
        public int SegmentCountPerMap
        {
            get { return segmentCountPerMap; }
            set
            {
                segmentCountPerMap = value;
                OnPropertyChanged("SegmentCountPerMap");
            }
        }
        
        [ReadOnly(true)]
        [DisplayName("Tile lenght")]
        [PropertyGridBrowsable(true)]
        public float TileLenght
        {
            get { return tileLenght; }
            set
            {
                tileLenght = value;
                OnPropertyChanged("TileLenght");
            }
        }
        
        public MapProperties MapProperties
        {
            get { return mapProperties; }
            set
            {
                mapProperties = value;
                OnPropertyChanged("MapProperties");
            }
        }
        
        public KSegment[,] DwTerrainSegment
        {
            get { return dwTerrainSegment; }
            set
            {
                dwTerrainSegment = value;
                OnPropertyChanged("DwTerrainSegment");
            }
        }
        
        public SortableBindingList<KProp> DwProps
        {
            get { return dwProps; }
            set
            {
                dwProps = value;
                OnPropertyChanged("DwProps");
            }
        }
        
        public SortableBindingList<KGrass> DwGrass
        {
            get { return dwGrass; }
            set
            {
                dwGrass = value;
                OnPropertyChanged("DwGrass");
            }
        }
        
        public SortableBindingList<Polygon2> DwVectorAttr
        {
            get { return dwVectorAttr; }
            set
            {
                dwVectorAttr = value;
                OnPropertyChanged("DwVectorAttr");
            }
        }
        
        public SortableBindingList<Water> DwWater
        {
            get { return dwWater; }
            set
            {
                dwWater = value;
                OnPropertyChanged("DwWater");
            }
        }
        
        public SortableBindingList<SpeedGrassColony> DwGrassColony
        {
            get { return dwGrassColony; }
            set
            {
                dwGrassColony = value;
                OnPropertyChanged("DwGrassColony");
            }
        }
        
        public SortableBindingList<EventAreaScript> DwEventArea
        {
            get { return dwEventArea; }
            set
            {
                dwEventArea = value;
                OnPropertyChanged("DwEventArea");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Nfm()
        {
            Instance = this;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Nfm Instance = null;
    }
}
