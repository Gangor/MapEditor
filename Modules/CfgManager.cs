using MapEditor.Attributes;
using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace MapEditor.Modules
{
    public class CfgManager
    {
        [ReadOnly(true)]
        [DisplayName("Props")]
        [PropertyGridBrowsable(true)]
        public List<PropInfo> Props { get; set; } = new List<PropInfo>();

        [ReadOnly(true)]
        [DisplayName("Textures")]
        [PropertyGridBrowsable(true)]
        public List<TextureInfo> Textures { get; set; } = new List<TextureInfo>();

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public CfgManager()
        {
            Instance = this;
        }

        /// <summary>
        /// Load all cfg
        /// </summary>
        /// <param name="folder"></param>
        public void Load(string folder)
        {
            this.LoadProp(folder);
            this.LoadTexture(folder);
        }

        /// <summary>
        /// Load prop info cfg
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filename"></param>
        private void LoadProp(string folder, string filename = "terrainpropinfo.cfg")
        {
            Props = new List<PropInfo>();

            if (!File.Exists(Path.Combine(folder, filename)))
            {
                XLog.WriteLine(Levels.Error, $"Missing file {filename}.");
                return;
            }
            
            XLog.WriteLine(Levels.Info, $"Loading {filename}...");
            try
            {
                using (StreamReader b = new StreamReader(Path.Combine(folder, filename), Encoding.Default))
                {
                    string line;

                    string category = "";
                    string renderType = "";
                    string lightMapType = "";
                    string visibleRatio = "";

                    while ((line = b.ReadLine()) != null)
                    {
                        line.Trim();

                        var properties = line.Split(new char[] { '=' }, 2);
                        if (!line.StartsWith(";") && properties.Length == 2)
                        {
                            if (properties[0] == "CATEGORY") category = properties[1].ToString();
                            else if (properties[0] == "RENDERTYPE") renderType = properties[1];
                            else if (properties[0] == "LIGHTMAPTYPE") lightMapType = properties[1];
                            else if (properties[0] == "VISIBLE_RATIO") visibleRatio = properties[1];
                            else if (properties[0] == "PROPNAME")
                            {
                                var values = properties[1].Split(new char[] { ',' }, 2);
                                if (values.Length == 2)
                                {
                                    var prop = new PropInfo();
                                    prop.Id = uint.Parse(values[0]);
                                    prop.Category = category;
                                    prop.PropName = values[1];
                                    prop.LightMapType = lightMapType;
                                    prop.VisibleRatio = visibleRatio;
                                    prop.RenderType = renderType;
                                    Props.Add(prop);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Props = new List<PropInfo>();
                XLog.WriteLine(Levels.Error, "CfgManager::Prop::Load<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Load texture info cfg
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filename"></param>
        private void LoadTexture(string folder, string filename = "terraintextureinfo.cfg")
        {
            Textures = new List<TextureInfo>();

            if (!File.Exists(Path.Combine(folder, filename)))
            {
                XLog.WriteLine(Levels.Error, $"Missing file {filename}.");
                return;
            }

            XLog.WriteLine(Levels.Info, $"Loading {filename}...");
            try
            {
                using (StreamReader b = new StreamReader(Path.Combine(folder, filename), Encoding.Default))
                {
                    string line;

                    string category = "";
                    string details = "";

                    while ((line = b.ReadLine()) != null)
                    {
                        line.Trim();

                        var properties = line.Split(new char[] { '=' }, 2);
                        if (!line.StartsWith(";") && properties.Length == 2)
                        {
                            if (properties[0] == "CATEGORY") category = properties[1].ToString();
                            else if (properties[0] == "DETAIL") details = properties[1];
                            else if (properties[0] == "TEXTURE")
                            {
                                var values = properties[1].Split(new char[] { ',' }, 2);
                                if (values.Length == 2)
                                {
                                    var texture = new TextureInfo();
                                    texture.Id = ushort.Parse(values[0]);
                                    texture.Detail = details;
                                    texture.Category = category;
                                    texture.TextureName = values[1];
                                    Textures.Add(texture);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Textures = new List<TextureInfo>();
                XLog.WriteLine(Levels.Error, "CfgManager::Texture::Load<Exception> -> {0}", exception);
            }
        }

        public override string ToString()
        {
            return "Config Game File";
        }

        public static CfgManager Instance = null;
    }
}
