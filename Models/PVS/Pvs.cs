using MapEditor.Attributes;
using MapEditor.Extends;
using MapEditor.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

		/// <summary>
		/// Convert to pixel location
		/// </summary>
		/// <returns></returns>
		public PointF[] ToPoints()
		{
			var points = new List<PointF>();
			points.Add(new PointF((SegmentLeft * 6), (SegmentTop * 6)));
			points.Add(new PointF(Math.Min((SegmentRight + 1), 64) * 6, Math.Min((SegmentBottom + 1), 64) * 6));
			return _2DUtils.UnAdjustPolygonPoint(points, 1f, false, true);
		}

		protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Pvs Instance = null;
    }
}
