using MapEditor.Attributes;
using MapEditor.Modules;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace MapEditor.Models
{
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
			return nameof(SEGMENT_DATA_V1);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
