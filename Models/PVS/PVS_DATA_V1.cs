using MapEditor.Attributes;
using System.ComponentModel;

namespace MapEditor.Models
{
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
