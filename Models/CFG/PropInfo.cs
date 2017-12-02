using MapEditor.Attributes;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class PropInfo : INotifyPropertyChanged
    {
        private uint id = 0;
        private string category = "";
        private string propName = "";
        private string lightMapType = "";
        private string renderType = "";
        private string visibleRatio = "";

        [DisplayName("Id")]
        [PropertyGridBrowsable(true)]
        public uint Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        [DisplayName("Category")]
        [PropertyGridBrowsable(true)]
        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                OnPropertyChanged("Category");
            }
        }

        [DisplayName("Prop name")]
        [PropertyGridBrowsable(true)]
        public string PropName
        {
            get { return propName; }
            set
            {
                propName = value;
                OnPropertyChanged("PropName");
            }
        }

        [DisplayName("Lightmap type")]
        [PropertyGridBrowsable(true)]
        public string LightMapType
        {
            get { return lightMapType; }
            set
            {
                lightMapType = value;
                OnPropertyChanged("LightMapType");
            }
        }

        [DisplayName("Render type")]
        [PropertyGridBrowsable(true)]
        public string RenderType
        {
            get { return renderType; }
            set
            {
                renderType = value;
                OnPropertyChanged("RenderType");
            }
        }

        [DisplayName("Visible ratio")]
        [PropertyGridBrowsable(true)]
        public string VisibleRatio
        {
            get { return visibleRatio; }
            set
            {
                visibleRatio = value;
                OnPropertyChanged("VisibleRatio");
            }
        }

        public override string ToString()
        {
            return PropName;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
