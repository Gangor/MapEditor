using MapEditor.Attributes;
using MapEditor.Extends;
using MapEditor.Modules;
using System;
using System.ComponentModel;

namespace MapEditor.Models
{
    public class KProp : INotifyPropertyChanged
    {
        private int segmentId;
        private float x;
        private float y;
        private float z;
        private float rotateX;
        private float rotateY;
        private float rotateZ;
        private float scaleX;
        private float scaleY;
        private float scaleZ;
        private ushort propNum;
        private bool heightLocked;
        private float lockHeight;
        private short textureGroup;

        [DisplayName("Segment Id")]
        public int SegmentId
        {
            get { return segmentId; }
            set
            {
                segmentId = value;
                OnPropertyChanged("SegmentId");
            }
        }

        [DisplayName("X")]
        public float X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }

        [DisplayName("Y")]
        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }

        [DisplayName("Z")]
        public float Z
        {
            get { return z; }
            set
            {
                z = value;
                OnPropertyChanged("Z");
            }
		}

		[Browsable(false)]
		public float RotateX
		{
			get { return rotateX; }
			set
			{
				rotateX = value;
				OnPropertyChanged("RotateX");
			}
		}

		[Browsable(false)]
		public float RotateY
		{
			get { return rotateY; }
			set
			{
				rotateY = value;
				OnPropertyChanged("RotateY");
			}
		}

		[Browsable(false)]
		public float RotateZ
		{
			get { return rotateZ; }
			set
			{
				rotateZ = value;
				OnPropertyChanged("RotateZ");
			}
		}

		[DisplayName("Rotate X")]
        public float mRotateX
        {
            get { return (float)(RotateX * (180.0 / Math.PI)); }
			set
            {
                RotateX = (float)(Math.PI * value / 180.0);
                OnPropertyChanged("RotateX");
            }
        }

        [DisplayName("Rotate Y")]
        public float mRotateY
		{
			get { return (float)(RotateY * (180.0 / Math.PI)); }
			set
			{
				RotateY = (float)(Math.PI * value / 180.0);
				OnPropertyChanged("RotateY");
            }
        }

        [DisplayName("Rotate Z")]
        public float mRotateZ
		{
			get { return (float)(RotateZ * (180.0 / Math.PI)); }
			set
			{
				RotateZ = (float)(Math.PI * value / 180.0);
				OnPropertyChanged("RotateZ");
            }
        }

        [DisplayName("Scale X")]
        public float ScaleX
        {
            get { return scaleX; }
            set
            {
                scaleX = value;
                OnPropertyChanged("ScaleX");
            }
        }

        [DisplayName("Scale Y")]
        public float ScaleY
        {
            get { return scaleY; }
            set
            {
                scaleY = value;
                OnPropertyChanged("ScaleY");
            }
        }

        [DisplayName("Scale Z")]
        public float ScaleZ
        {
            get { return scaleZ; }
            set
            {
                scaleZ = value;
                OnPropertyChanged("ScaleZ");
            }
        }

        [DisplayName("Prop Id")]
        public ushort PropNum
        {
            get { return propNum; }
            set
            {
                propNum = value;
                OnPropertyChanged("PropNum");
            }
        }

        [DisplayName("Filename")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(PropPropertyConverter))]
        public string Filename
        {
            get { return GetPropName(); }
            set
            {
				propNum = GetPropId(value);
                OnPropertyChanged("Filename");
            }
        }

        [DisplayName("Locked height")]
        public bool HeightLocked
        {
            get { return heightLocked; }
            set
            {
                heightLocked = value;
                OnPropertyChanged("HeightLocked");
            }
        }

        [DisplayName("Height")]
        public float LockHeight
        {
            get { return lockHeight; }
            set
            {
                lockHeight = value;
                OnPropertyChanged("LockHeight");
            }
        }

        [DisplayName("Texture group")]
        public short TextureGroup
        {
            get { return textureGroup; }
            set
            {
                textureGroup = value;
                OnPropertyChanged("TextureGroup");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public KProp() { }

        public KProp(string[] cvs)
        {
            SegmentId = Convert.ToInt32(cvs[0]);
            X = Convert.ToSingle(cvs[1]);
            Y = Convert.ToSingle(cvs[2]);
            Z = Convert.ToSingle(cvs[3]);
            mRotateX = Convert.ToSingle(cvs[4]);
            mRotateY = Convert.ToSingle(cvs[5]);
            mRotateZ = Convert.ToSingle(cvs[6]);
            ScaleX = Convert.ToSingle(cvs[7]);
            ScaleY = Convert.ToSingle(cvs[8]);
            ScaleZ = Convert.ToSingle(cvs[9]);
            PropNum = Convert.ToUInt16(cvs[10]);
            Filename = Convert.ToString(cvs[11]);
            HeightLocked = Convert.ToBoolean(cvs[12]);
            LockHeight = Convert.ToSingle(cvs[13]);
            TextureGroup = Convert.ToInt16(cvs[14]);
        }

        public KProp Clone()
        {
            return (KProp)MemberwiseClone();
        }

        private string GetPropName()
        {
            var prop = CfgManager.Instance.Props.Find(r => (ushort)r.Id == PropNum);
            if (prop == null)
            {
                return "Unknown";
            }
            return prop.PropName;
        }

        private ushort GetPropId(string filename)
        {
            var prop = CfgManager.Instance.Props.Find(r => r.PropName == filename);
            if (prop == null)
            {
                return 0;
            }
            return (ushort)prop.Id;
        }

        public override string ToString()
        {
            return nameof(KProp);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
