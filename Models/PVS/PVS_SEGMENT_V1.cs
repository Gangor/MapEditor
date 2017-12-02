using MapEditor.Attributes;
using MapEditor.Extends;
using MapEditor.Modules;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace MapEditor.Models
{
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

		/// <summary>
		/// Convert to pixel location
		/// </summary>
		/// <returns></returns>
		public PointF[] ToPoint()
		{
			var points = new List<PointF>();
			points.Add(new PointF((SegmentX * 6), (SegmentY * 6)));
			points.Add(new PointF(((SegmentX + 1) * 6), ((SegmentY + 1) * 6)));
			return _2DUtils.UnAdjustPolygonPoint(points, 1f, false, true);
		}

		public override string ToString()
		{
			return nameof(PVS_SEGMENT_V1);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
