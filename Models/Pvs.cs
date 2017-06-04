using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Pvs : INotifyPropertyChanged
    {
        private string sign = "RAPPELZ PVS\0\0\0\0\0";
        private ushort version = 1;
        private byte segmentCountPerMap = 0;
        private byte mapStartPosX = 0;
        private byte mapStartPosY = 0;
        private byte segmentLeft = 0;
        private byte segmentTop = 0;
        private byte segmentRight = 0;
        private byte segmentBottom = 0;
        private List<PVS_SEGMENT_V1> segments = new List<PVS_SEGMENT_V1>();
        private List<PVS_PROP_V1> props = new List<PVS_PROP_V1>();

        [ReadOnly(true)]
        [PropertyGridBrowsable(true)]
        [Category("Information")]
        [DisplayName("Signature")]
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
        [PropertyGridBrowsable(true)]
        [DisplayName("Information")]
        public ushort Version
        {
            get { return version; }
            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }
        
        [ReadOnly(true)]
        [PropertyGridBrowsable(true)]
        [Category("Information")]
        [DisplayName("Segment per map")]
        public byte SegmentCountPerMap
        {
            get { return segmentCountPerMap; }
            set
            {
                segmentCountPerMap = value;
                OnPropertyChanged("SegmentCountPerMap");
            }
        }

        [ReadOnly(true)]
        [PropertyGridBrowsable(true)]
        [Category("Position")]
        [DisplayName("X")]
        public byte MapStartPosX
        {
            get { return mapStartPosX; }
            set
            {
                mapStartPosX = value;
                OnPropertyChanged("MapStartPosX");
            }
        }

        [ReadOnly(true)]
        [PropertyGridBrowsable(true)]
        [Category("Position")]
        [DisplayName("Y")]
        public byte MapStartPosY
        {
            get { return mapStartPosY; }
            set
            {
                mapStartPosY = value;
                OnPropertyChanged("MapStartPosY");
            }
        }
        
        [PropertyGridBrowsable(true)]
        [Category("Segment")]
        [DisplayName("Left")]
        public byte SegmentLeft
        {
            get { return segmentLeft; }
            set
            {
                segmentLeft = value;
                OnPropertyChanged("SegmentLeft");
            }
        }

        [PropertyGridBrowsable(true)]
        [Category("Segment")]
        [DisplayName("Top")]
        public byte SegmentTop
        {
            get { return segmentTop; }
            set
            {
                segmentTop = value;
                OnPropertyChanged("SegmentTop");
            }
        }

        [PropertyGridBrowsable(true)]
        [Category("Segment")]
        [DisplayName("Right")]
        public byte SegmentRight
        {
            get { return segmentRight; }
            set
            {
                segmentRight = value;
                OnPropertyChanged("SegmentRight");
            }
        }

        [PropertyGridBrowsable(true)]
        [Category("Segment")]
        [DisplayName("Bottom")]
        public byte SegmentBottom
        {
            get { return segmentBottom; }
            set
            {
                segmentBottom = value;
                OnPropertyChanged("SegmentBottom");
            }
        }

        [PropertyGridBrowsable(true)]
        [Category("Occlusion")]
        [DisplayName("Segments")]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<PVS_SEGMENT_V1> Segments
        {
            get { return segments; }
            set
            {
                segments = value;
                OnPropertyChanged("Segments");
            }
        }

        [PropertyGridBrowsable(true)]
        [Category("Occlusion")]
        [DisplayName("Props")]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<PVS_PROP_V1> Props
        {
            get { return props; }
            set
            {
                props = value;
                OnPropertyChanged("Props");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Pvs()
        {
            Instance = this;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Pvs Instance = null;
    }

    public class PVS_SEGMENT_V1 : INotifyPropertyChanged
    {
        private byte segmentX = 0;
        private byte segmentY = 0;
        private List<SEGMENT_DATA_V1> includeSegments = new List<SEGMENT_DATA_V1>();

        [PropertyGridBrowsable(true)]
        [DisplayName("X")]
        public byte SegmentX
        {
            get { return segmentX; }
            set
            {
                segmentX = value;
                OnPropertyChanged("SegmentX");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Y")]
        public byte SegmentY
        {
            get { return segmentY; }
            set
            {
                segmentY = value;
                OnPropertyChanged("SegmentY");
            }
        }

        [PropertyGridBrowsable(true)]
        [Category("Segments")]
        [DisplayName("Include")]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<SEGMENT_DATA_V1> IncludeSegments
        {
            get { return includeSegments; }
            set
            {
                includeSegments = value;
                OnPropertyChanged("IncludeSegments");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return nameof(PVS_SEGMENT_V1);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SEGMENT_DATA_V1 : INotifyPropertyChanged
    {
        private byte segmentX = 0;
        private byte segmentY = 0;

        [PropertyGridBrowsable(true)]
        [DisplayName("X")]
        public byte SegmentX
        {
            get { return segmentX; }
            set
            {
                segmentX = value;
                OnPropertyChanged("SegmentX");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Y")]
        public byte SegmentY
        {
            get { return segmentY; }
            set
            {
                segmentY = value;
                OnPropertyChanged("SegmentY");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return nameof(SEGMENT_DATA_V1);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PVS_PROP_V1 : INotifyPropertyChanged
    {
        private byte segmentX = 0;
        private byte segmentY = 0;
        private List<PROP_DATA_V1> includeProps = new List<PROP_DATA_V1>();

        [PropertyGridBrowsable(true)]
        [DisplayName("X")]
        public byte SegmentX
        {
            get { return segmentX; }
            set
            {
                segmentX = value;
                OnPropertyChanged("SegmentX");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Y")]
        public byte SegmentY
        {
            get { return segmentY; }
            set
            {
                segmentY = value;
                OnPropertyChanged("SegmentY");
            }
        }

        [PropertyGridBrowsable(true)]
        [Category("Props")]
        [DisplayName("Include")]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<PROP_DATA_V1> IncludeProps
        {
            get { return includeProps; }
            set
            {
                includeProps = value;
                OnPropertyChanged("IncludeProps");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return nameof(PVS_PROP_V1);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PROP_DATA_V1 : INotifyPropertyChanged
    {
        private uint segmentIdx = 0;
        private uint propIdx = 0;

        [PropertyGridBrowsable(true)]
        [DisplayName("Segment Id")]
        public uint SegmentIdx
        {
            get { return segmentIdx; }
            set
            {
                segmentIdx = value;
                OnPropertyChanged("SegmentIdx");
            }
        }

        [PropertyGridBrowsable(true)]
        [DisplayName("Prop Id")]
        public uint PropIdx
        {
            get { return propIdx; }
            set
            {
                propIdx = value;
                OnPropertyChanged("PropIdx");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return nameof(PROP_DATA_V1);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
