using MapEditor.Attributes;
using MapEditor.Extends;
using MapEditor.Modules;
using System.ComponentModel;

namespace MapEditor.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class KSegment : INotifyPropertyChanged
    {
        private uint version = 0;
        private ushort[] tile = new ushort[3];
        private KVertex[,] hsVector = new KVertex[6, 6];

        [PropertyGridBrowsable(true)]
        [DisplayName("Version")]
        public uint Version
        {
            get { return version; }
            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }

        [PropertyGridBrowsable(false)]
        public ushort[] Tile
        {
            get { return tile; }
            set
            {
                tile = value;
                Texture01Id = value[0];
                Texture02Id = value[1];
                Texture03Id = value[2];
                OnPropertyChanged("Tile");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Texture 1 Id")]
        public ushort Texture01Id
        {
            get { return Tile[0]; }
            set
            {
                Tile[0] = value;
                OnPropertyChanged("Texture01Id");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Texture 1 Filename")]
        [TypeConverter(typeof(TexturePropertyConverter))]
        public string Texture01FileName
        {
            get { return GetTextureName(Tile[0]); }
            set
            {
                Tile[0] = GetTextureId(value);
                OnPropertyChanged("Texture01FileName");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Texture 2 Id")]
        public ushort Texture02Id
        {
            get { return Tile[1]; }
            set
            {
                Tile[1] = value;
                OnPropertyChanged("Texture02Id");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Texture 2 Filename")]
        [TypeConverter(typeof(TexturePropertyConverter))]
        public string Texture02FileName
        {
            get { return GetTextureName(Tile[1]); }
            set
            {
                Tile[1] = GetTextureId(value);
                OnPropertyChanged("Texture02FileName");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Texture 3 Id")]
        public ushort Texture03Id
        {
            get { return Tile[2]; }
            set
            {
                Tile[2] = value;
                OnPropertyChanged("Texture03Id");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Texture 3 Filename")]
        [TypeConverter(typeof(TexturePropertyConverter))]
        public string Texture03FileName
        {
            get { return GetTextureName(Tile[2]); }
            set
            {
                Tile[2] = GetTextureId(value);
                OnPropertyChanged("Texture03FileName");
            }
        }

        [PropertyGridBrowsable(false)]
        public KVertex[,] HsVector
        {
            get { return hsVector; }
            set
            {
                hsVector = value;
                OnPropertyChanged("HsVector");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string GetTextureName(ushort id)
        {
            var texture = CfgManager.Instance.Textures.Find(r => r.Id == id);
            if (texture == null)
            {
                return "Unknown";
            }
            return texture.TextureName;
        }

        private ushort GetTextureId(string filename)
        {
            var texture = CfgManager.Instance.Textures.Find(r => r.TextureName == filename);
            if (texture == null)
            {
                return 0;
            }
            return texture.Id;
        }

        public override string ToString()
        {
            return nameof(KSegment);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
