using MapEditor.Attributes;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class K3DLight : INotifyPropertyChanged
    {
        private KColor diffuse = new KColor();
        private KColor specular = new KColor();
        private KColor ambient = new KColor();
        private float attenuation0 = 0f;
        private float attenuation1 = 0f;
        private float attenuation2 = 0f;

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

        [DisplayName("Attenuation0")]
        [PropertyGridBrowsable(true)]
        public float Attenuation0
        {
            get { return attenuation0; }
            set
            {
                attenuation0 = value;
                OnPropertyChanged("Attenuation0");
            }
        }

        [DisplayName("Attenuation1")]
        [PropertyGridBrowsable(true)]
        public float Attenuation1
        {
            get { return attenuation1; }
            set
            {
                attenuation1 = value;
                OnPropertyChanged("Attenuation1");
            }
        }

        [DisplayName("Attenuation2")]
        [PropertyGridBrowsable(true)]
        public float Attenuation2
        {
            get { return attenuation2; }
            set
            {
                attenuation2 = value;
                OnPropertyChanged("Attenuation2");
            }
        }

        public override string ToString()
        {
            return nameof(K3DLight);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
