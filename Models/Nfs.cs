using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Nfs : INotifyPropertyChanged
    {
        private string sign = "nFlavor Script\0\0";
        private int version = 2;
        
        private List<Location> respawns = new List<Location>();
        private List<PropScriptInfo> props = new List<PropScriptInfo>();

        [ReadOnly(true)]
        [PropertyGridBrowsable(true)]
        [DisplayName("Signature")]
        [Category("Information")]
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
        [DisplayName("Version")]
        [Category("Information")]
        public int Version
        {
            get { return version; }
            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }

        [DisplayName("Respawns")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<Location> Respawns
        {
            get { return respawns; }
            set
            {
                respawns = value;
                OnPropertyChanged("Respawns");
            }
        }

        [DisplayName("NPC Props")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<PropScriptInfo> Props
        {
            get { return props; }
            set
            {
                props = value;
                OnPropertyChanged("Props");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Nfs()
        {
            Instance = this;
        }

        public int GetScriptCount()
        {
            int i = 0;
            foreach (var location in Respawns)
            {
                if (location.Scripts.Count != 0) i++;
            }
            return i;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Nfs Instance = null;
    }

    public class Location : INotifyPropertyChanged
    {
        private int left = 0;
        private int top = 0;
        private int right = 0;
        private int bottom = 0;
        private string description = "";
        private List<Function> script = new  List<Function>();

        [DisplayName("Left")]
        [PropertyGridBrowsable(true)]
        public int Left
        {
            get { return left; }
            set
            {
                left = value;
                OnPropertyChanged("Left");
            }
        }

        [DisplayName("Top")]
        [PropertyGridBrowsable(true)]
        public int Top
        {
            get { return top; }
            set
            {
                top = value;
                OnPropertyChanged("Top");
            }
        }

        [DisplayName("Right")]
        [PropertyGridBrowsable(true)]
        public int Right
        {
            get { return right; }
            set
            {
                right = value;
                OnPropertyChanged("Right");
            }
        }

        [DisplayName("Bottom")]
        [PropertyGridBrowsable(true)]
        public int Bottom
        {
            get { return bottom; }
            set
            {
                bottom = value;
                OnPropertyChanged("Bottom");
            }
        }

        [DisplayName("Description")]
        [PropertyGridBrowsable(true)]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        [DisplayName("Scripts")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<Function> Scripts
        {
            get { return script; }
            set
            {
                script = value;
                OnPropertyChanged("Script");
            }
        }
        
        public override string ToString()
        {
            return string.Format("Region : {0}", Description);
        }
        
        /// <summary>
        /// Convert to pixel location
        /// </summary>
        /// <returns></returns>
        public PointF[] ToPoints()
        {
            var points = new PointF[2];
            points[0].X = (Left * Global.titleLenght / 7.875f);
            points[0].Y = (Top * Global.titleLenght / 7.875f);
            points[1].X = (Right * Global.titleLenght / 7.875f);
            points[1].Y = (Bottom * Global.titleLenght / 7.875f);
            return points;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PropScriptInfo : INotifyPropertyChanged
    {
        private int propId = 0;
        private float x = 0;
        private float y = 0;
        private short modelId = 0;
        private List<Function> records = new List<Function>();

        [DisplayName("PropId")]
        [PropertyGridBrowsable(true)]
        public int PropId
        {
            get { return propId; }
            set
            {
                propId = value;
                OnPropertyChanged("PropId");
            }
        }

        [DisplayName("X")]
        [Category("Coordonate")]
        [PropertyGridBrowsable(true)]
        public float X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }

        [DisplayName("Y")]
        [Category("Coordonate")]
        [PropertyGridBrowsable(true)]
        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }

        [DisplayName("Model Id")]
        [PropertyGridBrowsable(true)]
        public short ModelId
        {
            get { return modelId; }
            set
            {
                modelId = value;
                OnPropertyChanged("ModelId");
            }
        }

        [DisplayName("Scripts")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<Function> Scripts
        {
            get { return records; }
            set
            {
                records = value;
                OnPropertyChanged("Records");
            }
        }
        
        public override string ToString()
        {
            return string.Format("NPC : {0}", PropId);
        }

        /// <summary>
        /// Convert to pixel location
        /// </summary>
        /// <returns></returns>
        public PointF ToPoint()
        {
            var point = new PointF();
            point.X = (X / 7.875f);
            point.Y = (Y / 7.875f);
            return point;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Function : INotifyPropertyChanged
    {
        public string functionString = "";

        [DisplayName("Function string")]
        [PropertyGridBrowsable(true)]
        public string FunctionString
        {
            get { return functionString; }
            set
            {
                functionString = value;
                OnPropertyChanged("FunctionString");
            }
        }
        
        public override string ToString()
        {
            return string.Format("Function : {0}", FunctionString);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
