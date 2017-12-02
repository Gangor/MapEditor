using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace MapEditor.Models
{
	public class EventAreaScript : INotifyPropertyChanged
	{
		private int areaId = 0;
		private List<Polygon2> polygons = new List<Polygon2>();

		[PropertyGridBrowsable(true)]
		[DisplayName("Area Id")]
		[Description("Trigger an input and output script from the area")]
		public int AreaId
		{
			get { return areaId; }
			set
			{
				areaId = value;
				OnPropertyChanged("AreaId");
			}
		}

		[DisplayName("Polygons")]
		[PropertyGridBrowsable(true)]
		[Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
		public List<Polygon2> Polygons
		{
			get { return polygons; }
			set
			{
				polygons = value;
				OnPropertyChanged("Polygons");
			}
		}

		public override string ToString()
		{
			return string.Format("Event : {0}", AreaId);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
