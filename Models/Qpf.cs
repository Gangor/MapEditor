﻿using MapEditor.Attributes;
using MapEditor.Extends;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Qpf : INotifyPropertyChanged
    {
        private string sign = "nFlavor QuestProp\0";
        private uint version = 3;
        private List<Prop> props = new List<Prop>();

        public event PropertyChangedEventHandler PropertyChanged;

        [ReadOnly(true)]
        [Category("Information")]
        [DisplayName("Signature")]
        [PropertyGridBrowsable(true)]
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
        [Category("Information")]
        [DisplayName("Version")]
        [PropertyGridBrowsable(true)]
        public uint Version
        {
            get { return version; }
            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }

        [DisplayName("Props")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<Prop> Props
        {
            get { return props; }
            set
            {
                props = value;
                OnPropertyChanged("Props");
            }
        }

        public Qpf()
        {
            Instance = this;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Qpf Instance = null;
    }

    public class Prop : INotifyPropertyChanged
    {
        private int questPropID = 0;
        private ushort propNum = 0;
        private float x = 0;
        private float y = 0;
        private float offSet = 0;
        private float rotateX = 0;
        private float rotateY = 0;
        private float rotateZ = 0;
        private float scaleX = 0;
        private float scaleY = 0;
        private float scaleZ = 0;
        private bool lockedHeight = false;
        private float lockHeight = 0;
        private short textureGroupIndex = 0;

        [Category("Gate")]
        [DisplayName("Gate Id")]
        [PropertyGridBrowsable(true)]
        public int QuestPropID
        {
            get { return questPropID; }
            set
            {
                questPropID = value;
                OnPropertyChanged("QuestPropID");
            }
        }

        [Category("Gate")]
        [DisplayName("Gate number")]
        [PropertyGridBrowsable(true)]
        public ushort PropNum
        {
            get { return propNum; }
            set
            {
                propNum = value;
                OnPropertyChanged("PropNum");
            }
        }

        [Category("Coordinate")]
        [DisplayName("X")]
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

        [Category("Coordinate")]
        [DisplayName("Y")]
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

        [Category("Coordinate")]
        [DisplayName("Z")]
        [PropertyGridBrowsable(true)]
        public float OffSet
        {
            get { return offSet; }
            set
            {
                offSet = value;
                OnPropertyChanged("Offset");
            }
        }

        [Category("Rotate")]
        [DisplayName("X")]
        [PropertyGridBrowsable(true)]
        public float RotateX
        {
            get { return rotateX; }
            set
            {
                rotateX = value;
                OnPropertyChanged("RotateX");
            }
        }

        [Category("Rotate")]
        [DisplayName("Y")]
        [PropertyGridBrowsable(true)]
        public float RotateY
        {
            get { return rotateY; }
            set
            {
                rotateY = value;
                OnPropertyChanged("RotateY");
            }
        }

        [Category("Rotate")]
        [DisplayName("Z")]
        [PropertyGridBrowsable(true)]
        public float RotateZ
        {
            get { return rotateZ; }
            set
            {
                rotateZ = value;
                OnPropertyChanged("RotateZ");
            }
        }

        [Category("Scale")]
        [DisplayName("X")]
        [PropertyGridBrowsable(true)]
        public float ScaleX
        {
            get { return scaleX; }
            set
            {
                scaleX = value;
                OnPropertyChanged("ScaleX");
            }
        }

        [Category("Scale")]
        [DisplayName("Y")]
        [PropertyGridBrowsable(true)]
        public float ScaleY
        {
            get { return scaleY; }
            set
            {
                scaleY = value;
                OnPropertyChanged("ScaleY");
            }
        }

        [Category("Scale")]
        [DisplayName("Z")]
        [PropertyGridBrowsable(true)]
        public float ScaleZ
        {
            get { return scaleZ; }
            set
            {
                scaleZ = value;
                OnPropertyChanged("ScaleZ");
            }
        }

        [Category("Locking")]
        [DisplayName("Height locked")]
        [PropertyGridBrowsable(true)]
        public bool LockedHeight
        {
            get { return lockedHeight; }
            set
            {
                lockedHeight = value;
                OnPropertyChanged("LockedHeight");
            }
        }

        [Category("Locking")]
        [DisplayName("Locked")]
        [PropertyGridBrowsable(true)]
        public float LockHeight
        {
            get { return lockHeight; }
            set
            {
                lockHeight = value;
                OnPropertyChanged("LockHeight");
            }
        }

        [Category("Texture")]
        [DisplayName("Group")]
        [PropertyGridBrowsable(true)]
        public short TextureGroupIndex
        {
            get { return textureGroupIndex; }
            set
            {
                textureGroupIndex = value;
                OnPropertyChanged("TextureGroupIndex");
            }
        }
        
        public override string ToString()
        {
            return Properties.Resources.ResourceManager.GetStringFormat("Prop_Name_Text", QuestPropID);
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
}