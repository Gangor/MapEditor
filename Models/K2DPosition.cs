using MapEditor.Attributes;
using MapEditor.Modules;
using System;
using System.ComponentModel;

namespace MapEditor.Models
{
	public class K2DPosition : INotifyPropertyChanged
	{
		private int x = 0;
		private int y = 0;
		
		[PropertyGridBrowsable(false)]
		public int X
		{
			get { return x; }
			set
			{
				x = value ;
				OnPropertyChanged("X");
			}
		}
		
		[PropertyGridBrowsable(false)]
		public int Y
		{
			get { return y; }
			set
			{
				y = value;
				OnPropertyChanged("Y");
			}
		}

		[DisplayName("X")]
		[PropertyGridBrowsable(true)]
		public int mX
		{
			get { return (MapWorker.Instance.X * 16128) + (x * Global.tileLenght / 8); }
			set
			{
				var part = MapWorker.Instance.X * 16128;
				var partmax = MapWorker.Instance.X * 16128 + 16128;

				if (value < part || value > partmax)
				{
					x = 0;
					return;
				}
				
				x = (int)Math.Round((decimal)(value - part) * 8 / Global.tileLenght);
				OnPropertyChanged("X");
			}
		}

		[DisplayName("Y")]
		[PropertyGridBrowsable(true)]
		public int mY
		{
			get { return (MapWorker.Instance.Y * 16128) + (y * Global.tileLenght / 8); }
			set
			{
				var part = MapWorker.Instance.Y * 16128;
				var partmax = MapWorker.Instance.Y * 16128 + 16128;

				if (value < part || value > partmax)
				{
					y = 0;
					return;
				}

				y = (int)Math.Round((decimal)(value - part) * 8 / Global.tileLenght);
				OnPropertyChanged("Y");
			}
		}

		public K2DPosition() { }

		public K2DPosition(int x, int y)
		{
			X = x;
			Y = y;
		}

		public override string ToString()
		{
			return nameof(K2DPosition);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
