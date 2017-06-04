using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Polygon2 : INotifyPropertyChanged
    {
        private int count = 0;
        private List<K2DVector> points = new List<K2DVector>();

        [DisplayName("Count")]
        public int Count
        {
            get { return Points.Count; }
            set
            {
                count = value;
            }
        }

        [DisplayName("Points")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<K2DVector> Points
        {
            get { return points; }
            set
            {
                points = value;
                OnPropertyChanged("Points");
            }
        }

        public override string ToString()
        {
            return Properties.Resources.ResourceManager.GetStringFormat("Polygon2_Name_Text", Points.Count);
        }

        /// <summary>
        /// Convert to pixel location
        /// </summary>
        /// <returns></returns>
        public PointF[] ToPoints()
        {
            var list = new PointF[Points.Count];

            for (int i = 0; i < Points.Count; i++)
            {
                var pts = new PointF();
                pts.X = ((Points[i].X * Global.titleLenght / 8) / 7.875f);
                pts.Y = ((Points[i].Y * Global.titleLenght / 8) / 7.875f);
                list[i] = pts;
            }
            return list;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
