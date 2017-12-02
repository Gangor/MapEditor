using MapEditor.Attributes;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class TextureInfo : INotifyPropertyChanged
    {
        private ushort id = 0;
        private string detail = "";
        private string category = "";
        private string textureName = "";

        [DisplayName("Id")]
        [PropertyGridBrowsable(true)]
        public ushort Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        [DisplayName("Detail")]
        [PropertyGridBrowsable(true)]
        public string Detail
        {
            get { return detail; }
            set
            {
                detail = value;
                OnPropertyChanged("Detail");
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

        [DisplayName("TextureName")]
        [PropertyGridBrowsable(true)]
        public string TextureName
        {
            get { return textureName; }
            set
            {
                textureName = value;
                OnPropertyChanged("TextureName");
            }
        }

        public override string ToString()
        {
            return TextureName;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
