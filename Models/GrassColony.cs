using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class SpeedGrassColony : INotifyPropertyChanged
    {
        private int grassId = 0;
        private float density = 5.0f;
        private float distribution = 16.0f;
        private float size = 12.0f;
        private float heightP = 0.3f;
        private float heightM = 0.1f;
        private KColor color = new KColor();
        private float colorRatio = 0.6f;
        private float colorTone = 1.0f;
        private float chroma = 0.3f;
        private float brightness = 0.5f;
        private float combinationRatio = 50.0f;
        private float windReaction = 0.2f;
        private string filename = "";
        private List<Polygon2> polygon = new List<Polygon2>();

        [DisplayName("Grass Id")]
        public int GrassId
        {
            get { return grassId; }
            set
            {
                grassId = value;
                OnPropertyChanged("GrassId");
            }
        }

        [DisplayName("Density")]
        public float Density
        {
            get { return density; }
            set
            {
                density = value;
                OnPropertyChanged("Density");
            }
        }

        [DisplayName("Distribution")]
        public float Distribution
        {
            get { return distribution; }
            set
            {
                distribution = value;
                OnPropertyChanged("Distribution");
            }
        }

        [DisplayName("Size")]
        public float Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }

        [DisplayName("Height P")]
        public float HeightP
        {
            get { return heightP; }
            set
            {
                heightP = value;
                OnPropertyChanged("HeightP");
            }
        }

        [DisplayName("Height M")]
        public float HeightM
        {
            get { return heightM; }
            set
            {
                heightM = value;
                OnPropertyChanged("HeightM");
            }
        }

        [DisplayName("Color")]
        [Browsable(false)]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KColor Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }
        }

        [DisplayName("ColorRatio")]
        public float ColorRatio
        {
            get { return colorRatio; }
            set
            {
                colorRatio = value;
                OnPropertyChanged("ColorRatio");
            }
        }

        [DisplayName("ColorTone")]
        public float ColorTone
        {
            get { return colorTone; }
            set
            {
                colorTone = value;
                OnPropertyChanged("ColorTone");
            }
        }

        [DisplayName("Chroma")]
        public float Chroma
        {
            get { return chroma; }
            set
            {
                chroma = value;
                OnPropertyChanged("Chroma");
            }
        }

        [DisplayName("Brightness")]
        public float Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                OnPropertyChanged("Brightness");
            }
        }

        [DisplayName("CombinationRatio")]
        public float CombinationRatio
        {
            get { return combinationRatio; }
            set
            {
                combinationRatio = value;
                OnPropertyChanged("CombinationRatio");
            }
        }

        [DisplayName("WindReaction")]
        public float WindReaction
        {
            get { return windReaction; }
            set
            {
                windReaction = value;
                OnPropertyChanged("WindReaction");
            }
        }

        [DisplayName("Filename")]
        public string Filename
        {
            get { return filename; }
            set
            {
                filename = value;
                OnPropertyChanged("Filename");
            }
        }

        [Browsable(false)]
        [DisplayName("Polygons")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<Polygon2> Polygons
        {
            get { return polygon; }
            set
            {
                polygon = value;
                OnPropertyChanged("Polygons");
            }
        }

        public override string ToString()
        {
            return nameof(SpeedGrassColony);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    };
}
