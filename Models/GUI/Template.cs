using MapEditor.Attributes;
using MapEditor.Properties;
using System.ComponentModel;
using System.Drawing;

namespace MapEditor.Models
{
    /// <summary>
    /// Template color for element of the map
    /// </summary>
    public class Template : INotifyPropertyChanged
    {
		#region Properties

		private Color collision = Pens.Beige.Color;
        private Color eventArea = Pens.Fuchsia.Color;
        private Color light = Pens.DarkKhaki.Color;
        private Color npcProp = Pens.Gold.Color;
        private Color nfp = Pens.Lime.Color;
        private Color pvsSegment = Pens.LightSalmon.Color;
        private Color pvsProps = Pens.DarkGray.Color;
        private Color prop = Pens.Red.Color;
        private Color respawn = Pens.Turquoise.Color;
        private Color region = Pens.Orange.Color;
        private Color water = Pens.Blue.Color;

        [DisplayName("Collision")]
        [PropertyGridBrowsable(true)]
        public Color Collision
        {
            get { return collision; }
            set
            {
                if (collision == value) return;
                collision = value;
                OnPropertyChanged("Collision");
            }
        }

        [DisplayName("Event Area")]
        [PropertyGridBrowsable(true)]
        public Color EventArea
        {
            get { return eventArea; }
            set
            {
                if (eventArea == value) return;
                eventArea = value;
                OnPropertyChanged("EventArea");
            }
        }

        [DisplayName("Light")]
        [PropertyGridBrowsable(true)]
        public Color Light
        {
            get { return light; }
            set
            {
                if (light == value) return;
                light = value;
                OnPropertyChanged("Light");
            }
        }

        [DisplayName("Npc Prop")]
        [PropertyGridBrowsable(true)]
        public Color NpcProp
        {
            get { return npcProp; }
            set
            {
                if (npcProp == value) return;
                npcProp = value;
                OnPropertyChanged("NpcProp");
            }
        }

        [DisplayName("Nfp")]
        [PropertyGridBrowsable(true)]
        public Color Nfp
        {
            get { return nfp; }
            set
            {
                if (nfp == value) return;
                nfp = value;
                OnPropertyChanged("Nfp");
            }
        }

        [DisplayName("Gate")]
        [PropertyGridBrowsable(true)]
        public Color Prop
        {
            get { return prop; }
            set
            {
                if (prop == value) return;
                prop = value;
                OnPropertyChanged("Prop");
            }
        }

        [DisplayName("Occlusion Segment")]
        [PropertyGridBrowsable(true)]
        public Color PVSSegment
        {
            get { return pvsSegment; }
            set
            {
                if (pvsSegment == value) return;
                pvsSegment = value;
                OnPropertyChanged("PVSSegment");
            }
        }

        [DisplayName("Occlusion props")]
        [PropertyGridBrowsable(true)]
        public Color PVSProp
        {
            get { return pvsProps; }
            set
            {
                if (pvsProps == value) return;
                pvsProps = value;
                OnPropertyChanged("PVSProps");
            }
        }

        [DisplayName("Respawn")]
        [PropertyGridBrowsable(true)]
        public Color Respawn
        {
            get { return respawn; }
            set
            {
                if (respawn == value) return;
                respawn = value;
                OnPropertyChanged("Respawn");
            }
        }

        [DisplayName("Region")]
        [PropertyGridBrowsable(true)]
        public Color Region
        {
            get { return region; }
            set
            {
                if (region == value) return;
                region = value;
                OnPropertyChanged("Region");
            }
        }

        [DisplayName("Water")]
        [PropertyGridBrowsable(true)]
        public Color Water
        {
            get { return water; }
            set
            {
                if (water == value) return;
                water = value;
                OnPropertyChanged("Water");
            }
        }

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		public Template()
		{
			if (Settings.Default.Context == null)
			{
				Settings.Default.Save();
			}

			Collision = Settings.Default.Collision;
			EventArea = Settings.Default.EventArea;
			Light = Settings.Default.Light;
			Nfp = Settings.Default.Nfp;
			NpcProp = Settings.Default.NpcProp;
			Prop = Settings.Default.Prop;
			PVSProp = Settings.Default.PVSProp;
			PVSSegment = Settings.Default.PVSSegment;
			Region = Settings.Default.Region;
			Respawn = Settings.Default.Respawn;
			Water = Settings.Default.Water;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Save()
		{
			Settings.Default.Collision = Collision;
			Settings.Default.EventArea = EventArea;
			Settings.Default.Light = Light;
			Settings.Default.Nfp = Nfp;
			Settings.Default.NpcProp = NpcProp;
			Settings.Default.Prop = Prop;
			Settings.Default.PVSProp = PVSProp;
			Settings.Default.PVSSegment = PVSSegment;
			Settings.Default.Region = Region;
			Settings.Default.Respawn = Respawn;
			Settings.Default.Water = Water;
			Settings.Default.Save();
		}

		public void Update()
		{
			OnPropertyChanged("");
		}
    }
}
