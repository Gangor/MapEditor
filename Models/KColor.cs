using MapEditor.Attributes;
using System.ComponentModel;
using System.Drawing;

namespace MapEditor.Models
{
	public class KColor : INotifyPropertyChanged
	{
		private byte b = 0;
		private byte g = 0;
		private byte r = 0;
		private byte a = 0;

		[PropertyGridBrowsable(true)]
		[DisplayName("Alpha")]
		public byte A
		{
			get { return a; }
			set
			{
				a = value;
				OnPropertyChanged("A");
			}
		}

		[DisplayName("Color")]
		[PropertyGridBrowsable(true)]
		public Color Color
		{
			get { return Color.FromArgb(a, r, g, b); }
			set
			{
				b = value.B;
				g = value.G;
				r = value.R;
				a = value.A;
				OnPropertyChanged("Color");
			}
		}

		public KColor(byte b = 255, byte g = 255, byte r = 255, byte a = 255)
		{
			this.b = b;
			this.g = g;
			this.r = r;
			this.a = a;
		}

		public override string ToString()
		{
			return nameof(KColor);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
