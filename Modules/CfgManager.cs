using DataCore;
using MapEditor.Attributes;
using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

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
		public void Open(string folder)
		{
			OpenProp(folder);
			OpenTexture(folder);
		}

		/// <summary>
		/// Load prop info cfg
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filename"></param>
		private void OpenProp(string path, string filename = "terrainpropinfo.cfg")
		{
			XLog.Write(Levels.Info, $"Loading {filename}...\t");

			var fullname = Path.Combine(path, filename);

			if (!File.Exists(fullname))
			{
				XLog.WriteLine(Levels.Warning, "Introuvable");
				return;
			}
            
			LoadProp(File.ReadAllBytes(fullname));
		}

		/// <summary>
		/// Load texture info cfg
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filename"></param>
		public void OpenTexture(string path, string filename = "terraintextureinfo.cfg")
		{
			XLog.Write(Levels.Info, $"Loading {filename}...\t");

			var fullname = Path.Combine(path, filename);

			if (!File.Exists(fullname))
			{
				XLog.WriteLine(Levels.Warning, "Introuvable");
				return;
			}

			LoadTexture(File.ReadAllBytes(fullname));
		}

		/// <summary>
		/// Import  prop by data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="path"></param>
		/// <param name="filename"></param>
		private void ImportProp(Core core, string filename = "terrainpropinfo.cfg")
		{
			XLog.Write(Levels.Info, $"Loading {filename}...\t");

			if (!core.GetEntryExists(filename))
			{
				XLog.WriteLine(Levels.Warning, "Introuvable");
				return;
			}

			LoadProp(core.GetFileBytes(filename));
		}

		/// <summary>
		/// Import texture by data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="path"></param>
		/// <param name="filename"></param>
		private void ImportTexture(Core core, string filename = "terraintextureinfo.cfg")
		{
			XLog.Write(Levels.Info, $"Loading {filename}...\t");

			if (!core.GetEntryExists(filename))
			{
				XLog.WriteLine(Levels.Warning, "Introuvable");
				return;
			}

			LoadTexture(core.GetFileBytes(filename));
		}

		/// <summary>
		/// Load existing prop
		/// </summary>
		/// <param name="buffer"></param>
		public void LoadProp(byte[] buffer)
		{
			Props = new List<PropInfo>();
			try
			{
				using (StreamReader b = new StreamReader(new MemoryStream(buffer)))
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

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Props = new List<PropInfo>();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "CfgManager::Prop::Load<Exception> -> {0}", exception);
			}
		}

		/// <summary>
		/// Load existing collision
		/// </summary>
		/// <param name="buffer"></param>
		public void LoadTexture(byte[] buffer)
		{
			Textures = new List<TextureInfo>();
			try
			{
				using (StreamReader b = new StreamReader(new MemoryStream(buffer)))
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

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Textures = new List<TextureInfo>();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "CfgManager::Texture::Load<Exception> -> {0}", exception);
			}
		}

		[ReadOnly(true)]
		public override string ToString()
		{
			return "Config Game File";
		}

		public static CfgManager Instance = null;
	}
}
