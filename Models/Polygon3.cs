using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Polygon3 : INotifyPropertyChanged
    {
        private List<K3DVector> points = new List<K3DVector>();

        [DisplayName("Points")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<K3DVector> Points
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
            return string.Format("Polygons x{0}", Points.Count);
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
                list[i] = new PointF();
                list[i].X = (Points[i].X / 7.875f);
                list[i].Y = (Points[i].Y / 7.875f);
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
