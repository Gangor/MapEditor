using DataCore;
using MapEditor.Attributes;
using MapEditor.Dialog;
using MapEditor.Events;
using MapEditor.Extends;
using MapEditor.Models;
using MapEditor.Modules;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
	public partial class Editor : Form
	{
		public static Editor Instance = null;

		private Template template;
		private XEnv xEnv;

		private const string projectDefault = "Empty";
		private string projectName = "";
		private string projectPath = "";
		private string dataDirectory = "";

		private int collisionIndex = 0;
		private int eventAreaIndex = 0;
		private int lightIndex = 0;
		private int nfpIndex = 0;
		private int npcIndex = 0;
		private int gateIndex = 0;
		private int pvsPropIndex = 0;
		private int pvsSegmentIndex = 0;
		private int regionIndex = 0;
		private int respawnIndex = 0;
		private int waterIndex = 0;

		/// <summary>
		/// Initialize method
		/// </summary>
		#region Constructor

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Editor(bool upgrading = false)
		{
			Instance = this;
			InitializeComponent();
			InitializeModules();
			InitializeSubComponent();
			Shown += (sender, e) =>
			{
				if (upgrading) { new Upgrade(); }
				UpdateMapping();
			};

			Text = $"Map Editor - {projectDefault}";
			setCurrentMode(DrawMapMode.MOVE);
			template.Update();

			mapPictureBox.DataBindings.Add(new Binding("Image", MapManager.Instance, nameof(MapManager.Instance.Picture), false, DataSourceUpdateMode.OnPropertyChanged));
		}

		/// <summary>
		/// Initialize modules
		/// </summary>
		public void InitializeModules()
		{
			//
			DragMove += mapPictureBox_DragMove;
			DrawPoint += mapPictureBox_DrawPoint;
			DrawRectangle += mapPictureBox_DrawRectangle;
			CancelPolygon += mapPictureBox_CancelPolygon;
			DrawPolygon += mapPictureBox_DrawPolygon;
			PreviewDrawRectangle += mapPictureBox_PreviewDrawRectangle;
			PreviewDrawPolygon += mapPictureBox_PreviewDrawPolygon;
			//
			// Collection
			//
			CollectionEditorExtends.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
			CollectionEditorExtends.OkClick += propertyGrid_Updated;
			//
			// Map manager
			//
			MapManager.Init();
			//
			// Map worker
			//
			MapWorker.Init(projectPath);
			MapWorker.Instance.Painting += MapWorker_Painting;
			MapWorker.Instance.Reset += MapWorker_Reset;
			//
			// Template
			//
			template = new Template();
			template.PropertyChanged += template_PropertyChanged;
			//
			// XEnv
			//
			xEnv = new XEnv();
			xEnv.LoadFromFile("mapeditor.opt");
			dataDirectory = xEnv.GetValue("datacore.path", Application.StartupPath + @"\Resource");
			projectPath = xEnv.GetValue("mapeditor.dump_path", Application.StartupPath + @"\Resource");
		}

		/// <summary>
		/// Initialize sub component
		/// </summary>
		public void InitializeSubComponent()
		{
			//
			// menuStrip
			//
			menuStrip1.Renderer = new BrowserMenuRenderer();
			menuStrip2.Renderer = new BrowserMenuRenderer();
			menuStrip3.Renderer = new BrowserMenuRenderer();
			//
			// contextMenuStrip
			//
			contextMenuStrip1.Renderer = new BrowserMenuRenderer();
			//
			// propertyGrid
			//
			propertyGrid.BrowsableAttributes = new AttributeCollection(new PropertyGridBrowsableAttribute(true));
			//
			// treeView1
			//
			treeView1.Nodes.Add("All");
			collisionIndex = treeView1.Nodes[0].Nodes.Add("Collisions").Index;
			eventAreaIndex = treeView1.Nodes[0].Nodes.Add("Events area").Index;
			gateIndex = treeView1.Nodes[0].Nodes.Add("Gates").Index;
			lightIndex = treeView1.Nodes[0].Nodes.Add("Lights").Index;
			nfpIndex = treeView1.Nodes[0].Nodes.Add("Nfp").Index;
			npcIndex = treeView1.Nodes[0].Nodes.Add("Npc props").Index;
			pvsPropIndex = treeView1.Nodes[0].Nodes.Add("Occlusion props").Index;
			pvsSegmentIndex = treeView1.Nodes[0].Nodes.Add("Occlusion segments").Index;
			regionIndex = treeView1.Nodes[0].Nodes.Add("Regions").Index;
			respawnIndex = treeView1.Nodes[0].Nodes.Add("Respawns").Index;
			waterIndex = treeView1.Nodes[0].Nodes.Add("Waters").Index;
			treeView1.Nodes[0].Expand();
		}

		#endregion

		/// <summary>
		/// Window control method
		/// </summary>
		#region Window control

		private bool mappingChanged;

		/// <summary>
		/// Change load or save state
		/// </summary>
		/// <param name="value"></param>
		private void ActionProgressChanged(bool value)
		{
			if (!InvokeRequired)
			{
				openMenuItem.Enabled = !value;
				mapComboBox.Enabled = !value;
				saveAsMenuItem.Enabled = !value;
				saveAsMenuItem.Enabled = !value;
				importMenuItem.Enabled = !value;
				//exportMenuItem.Enabled = !value;
			}
			else
			{
				Invoke(new Action<bool>(ActionProgressChanged), value);
			}
		}

		/// <summary>
		/// Method used for about this software
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void About_Click(object sender, EventArgs e)
		{
			new About().ShowDialog();
		}

		/// <summary>
		/// Method used when closing map
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Close_Click(object sender, EventArgs e)
		{
			Text = $"Map Editor - {projectDefault}";
			projectName = projectDefault;

			Task.Run(() => MapWorker.Instance.Dispose());
		}

		/// <summary>
		/// Method for config color element
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Color_Click(object sender, EventArgs e)
		{
			propertyGrid.SelectedObject = template;
		}

		/// <summary>
		/// Method for export file in the data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Export_Click(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// Method for import file by data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Import_Click(object sender, EventArgs e)
		{
			try
			{
				var core = new Core();
				var import = new Import(ref core, dataDirectory);
				if (import.ShowDialog() == DialogResult.OK)
				{
					Text = $"Map Editor - {import.ProjectName}";
					dataDirectory = import.DataDirectory;
					projectName = import.ProjectName;

					ActionProgressChanged(true);

					Task.Run(() =>
					{
						var resolve = MapWorker.ResolveEncode(projectName);
						
						MapWorker.Instance.Import(core, resolve.Item1, resolve.Item2);
						MapManager.Instance.Load(core, import.DataDirectory, resolve.Item1, resolve.Item2, true);
						MapWorker.Instance.Refresh();

						ActionProgressChanged(false);
					});
				}
			}
			catch (Exception exception)
			{
				XLog.WriteLine(Levels.Error, "Editor::Import_Click<Exception> -> {0}", exception);
			}
		}

		/// <summary>
		/// Method used for fast load
		/// </summary>
		private void Mappings_SelectedChanged(object sender, EventArgs e)
		{
			if (mapComboBox.SelectedItem != null && !mappingChanged)
			{
				OpenMapFile_FileOk(projectPath, (string)mapComboBox.SelectedItem);
			}

			mappingChanged = false;
		}

		/// <summary>
		/// Method used when create new
		/// </summary>
		private void New_Click(object sender, EventArgs e)
		{
			var newProject = new NewProject();
			if (newProject.ShowDialog() == DialogResult.OK)
			{
				Text = $"Map Editor - {newProject.ProjectName}";
				projectName = newProject.ProjectName;
				ClearLog();

				ActionProgressChanged(true);

				Task.Run(() =>
				{
					MapManager.Instance.New();
					MapWorker.Instance.New(projectPath, projectName);

					ActionProgressChanged(false);
				});
			}
		}

		/// <summary>
		/// Method used when open file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Open_Click(object sender, EventArgs e)
		{
			var openFile = new OpenFileDialog();
			openFile.Filter = "Nflavor map|*.nfa;*.nfc;*.nfe;*.nfl;*.nfm;*.nfp;*.nfs;*.nfw;*.pvs;*.qpf";

			if (openFile.ShowDialog() == DialogResult.OK)
			{
				var filename = Path.GetFileNameWithoutExtension(openFile.FileName);
				var path = Path.GetDirectoryName(openFile.FileName);
				var dir = new DirectoryInfo(path);

				OpenMapFile_FileOk(dir.Parent.FullName, filename);
			}
		}

		/// <summary>
		/// Method used whend file open ok
		/// </summary>
		/// <param name="filename"></param>
		private void OpenMapFile_FileOk(string path, string filename)
		{
			Text = $"Map Editor - {filename}";
			projectName = filename;

			ActionProgressChanged(true);

			Task.Run(() =>
			{
				var resolve = MapWorker.ResolveEncode(filename);
				
				MapWorker.Instance.Load(path, resolve.Item1, resolve.Item2);
				MapManager.Instance.Load(path + @"\jpg\", resolve.Item1, resolve.Item2);
				MapWorker.Instance.Refresh();

				ActionProgressChanged(false);
			});
		}

		/// <summary>
		/// Method for config dump path
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Options_Click(object sender, EventArgs e)
		{
			var option = new Options(projectPath);
			if (option.ShowDialog() == DialogResult.OK)
			{
				projectPath = option.RootPath;
			}
		}

		/// <summary>
		/// Method used when quit application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Quit_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Method used when report bug
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Report_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.elitepvpers.com/forum/rappelz-private-server/4384171-release-map-editor.html");
		}

		/// <summary>
		/// Method used when save
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Save_Click(object sender, EventArgs e) { Save_FileOk(projectPath); }

		/// <summary>
		/// Method used when save as folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveAs_Click(object sender, EventArgs e)
		{
			var folderDialog = new FolderBrowserDialog();
			if (folderDialog.ShowDialog() == DialogResult.OK)
			{
				Save_FileOk(folderDialog.SelectedPath);
			}
		}

		private void Save_FileOk(string path)
		{
			ActionProgressChanged(true);
			MapWorker.Instance.Save(path);
			ActionProgressChanged(false);
		}

		/// <summary>
		/// Method used when terrain
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Terrain_Click(object sender, EventArgs e)
		{
			NFM.ShowDialog(projectName);
		}

		/// <summary>
		/// Method used for update file list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Update_Click(object sender, EventArgs e)
		{
			UpdateMapping();
		}

		/// <summary>
		/// Load all file on project path directory
		/// </summary>
		private void UpdateMapping()
		{
			var data = MapWorker.Watch(projectPath);
			mappingChanged = true;
			//
			mapComboBox.ComboBox.DataSource = data;
			mapComboBox.ComboBox.SelectedIndex = -1;
		}

		#endregion

		/// <summary>
		/// Map control method
		/// </summary>
		#region Zoom control

		const float zoomMin = 0.25f;
		const float zoomMax = 3f;
		const float zoomNormal = 1f;
		const float zoomIncrement = 0.25f;

		private float zoom = 0.25f;

		/// <summary>
		/// Method for enable or disable zoom
		/// </summary>
		/// <param name="state"></param>
		private void zoomChangeState(bool state)
		{
			zoomLessToolStripMenuItem.Enabled = state;
			zoomPlustoolStripMenuItem.Enabled = state;
			zoomRestoreToolStripMenuItem.Enabled = state;
		}

		/// <summary>
		/// Method for zoom plus
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void zoomMoreToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (zoom <= zoomMax)
			{
				zoom += zoomIncrement;
				zoomApply();
			}
		}

		/// <summary>
		/// Method for zoom less
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void zoomLessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (zoom != zoomMin)
			{
				zoom -= zoomIncrement;
				zoomApply();
			}
		}

		/// <summary>
		/// Method for restore zoom
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void zoomRestoreToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (zoom != zoomNormal)
			{
				zoom = zoomNormal;
				zoomApply();
			}
		}

		/// <summary>
		/// Change zoom of the map
		/// </summary>
		private void zoomApply()
		{
			mapPictureBox.Width = (int)Math.Round(MapManager.Width * zoom);
			mapPictureBox.Height = (int)Math.Round(MapManager.Height * zoom);

			zoomLessToolStripMenuItem.BackColor = zoom < zoomNormal ? Color.DarkGray : Color.Transparent;
			zoomPlustoolStripMenuItem.BackColor = zoom > zoomNormal ? Color.DarkGray : Color.Transparent;
			zoomLessToolStripMenuItem.Enabled = zoom != zoomMin ? true : false;
			zoomPlustoolStripMenuItem.Enabled = zoom != zoomMax ? true : false;
			zoomRestoreToolStripMenuItem.Enabled = zoom != zoomNormal ? true : false;

			zoomToolStripStatusLabel.Text = $"x{zoom}";
			statusStrip.Refresh();
		}

		#endregion

		/// <summary>
		/// Painting method
		/// </summary>
		#region Painting

		/// <summary>
		/// Method used when painting
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapWorker_Painting(object sender, PaintingArgs e)
		{
			if (!InvokeRequired)
			{
				PaintingRegion(e.Nfc, e.CollectionChanged);
				PaintingCollision(e.Nfa, e.CollectionChanged);
				PaintingLight(e.Nfl, e.CollectionChanged);
				PaintingEventArea(e.Nfe, e.CollectionChanged);
				PaintingLocation(e.Nfs, e.CollectionChanged);
				PaintingNfp(e.Nfp, e.CollectionChanged);
				PaintingNpcProp(e.Nfs, e.CollectionChanged);
				PaintingWater(e.Nfw, e.CollectionChanged);
				PaintingQuestProp(e.Qpf, e.CollectionChanged);
				PaintingPSegment(e.Pvs, false, false);
				PaintingPProp(e.Pvs, false, false);

				if (treeView1.SelectedNode != previousNode)
				{
					treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));
				}

				mapPictureBox.Refresh();
			}
			else
			{
				mapPictureBox.Invoke(new Action<object, PaintingArgs>(MapWorker_Painting), sender, e);
			}
		}

		/// <summary>
		/// Method used when reset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapWorker_Reset(object sender, EventArgs e)
		{
			if (!InvokeRequired)
			{
				ClearLog();

				MapManager.Instance.New();

				propertyGrid.SelectedObject = null;
				treeView1.SelectedNode = null;
			}
			else
			{
				Invoke(new Action<object, EventArgs>(MapWorker_Reset), sender, e);
			}
		}

		/// <summary>
		/// Draw one collision
		/// </summary>
		/// <param name="polygon"></param>
		private void DrawCollision(Polygon2 polygon)
		{
			var pen = new Pen(template.Collision);
			var positions = polygon.ToPoints();
			var index = Nfa.Instance.Polygons.IndexOf(polygon);

			MapManager.Instance.DrawPolygon(pen, positions);
			MapManager.Instance.DrawCenterString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, positions.FirstOrDefault().X, positions.FirstOrDefault().Y);
		}

		/// <summary>
		/// Draw one region info
		/// </summary>
		/// <param name="location"></param>
		private void DrawEventArea(EventAreaScript area)
		{
			var pen = new Pen(template.EventArea);

			for (int i = 0; i < area.Polygons.Count; i++)
			{
				var positions = area.Polygons[i].ToPoints();
				var index = Nfe.Instance.Events.IndexOf(area);

				MapManager.Instance.DrawPolygon(pen, positions);
				MapManager.Instance.DrawCenterString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, (positions.DefaultIfEmpty().Min(p => p.X) + 7f), (positions.DefaultIfEmpty().Min(p => p.Y) - 5f));
			}
		}

		/// <summary>
		/// Draw one light
		/// </summary>
		/// <param name="prop"></param>
		private void DrawLight(StructLights light)
		{
			var pen = new Pen(template.Light);
			var position = light.ToPoint();
			var index = Nfl.Instance.Lights.IndexOf(light);

			MapManager.Instance.DrawPoint(pen, position);
			MapManager.Instance.DrawCenterString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, position.X, position.Y);
		}

		/// <summary>
		/// Draw one npc prop
		/// </summary>
		/// <param name="prop"></param>
		private void DrawNpcProp(PropScriptInfo prop)
		{
			var pen = new Pen(template.NpcProp);
			var position = prop.ToPoint();
			var index = Nfs.Instance.Props.IndexOf(prop);

			MapManager.Instance.DrawPoint(pen, position);
			MapManager.Instance.DrawCenterString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, position.X, position.Y);
		}

		/// <summary>
		/// Draw one npc prop
		/// </summary>
		/// <param name="prop"></param>
		private void DrawNfp(RecordNfp recorNfp)
		{
			var pen = new Pen(template.Nfp);
			foreach (var polygon in recorNfp.Polygons)
			{
				var positions = polygon.ToPoints();
				var index = Nfp.Instance.Records.IndexOf(recorNfp);

				MapManager.Instance.DrawPolygon(pen, positions);
				MapManager.Instance.DrawCenterString($"{recorNfp.Id}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, positions.FirstOrDefault().X, positions.FirstOrDefault().Y);
			}
		}

		/// <summary>
		/// Draw one prop
		/// </summary>
		/// <param name="prop"></param>
		private void DrawProp(Prop prop)
		{
			var pen = new Pen(template.Prop);
			var position = prop.ToPoint();
			var index = Qpf.Instance.Props.IndexOf(prop);

			MapManager.Instance.DrawPoint(pen, position);
			MapManager.Instance.DrawCenterString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, position.X, position.Y);
		}

		/// <summary>
		/// Draw location pvs segment
		/// </summary>
		/// <param name="pvs"></param>
		private void DrawPVS(Pvs pvs, Pen pen)
		{
			var position = pvs.ToPoints();
			var text = $"m{pvs.MapStartPosX.ToString("000")}_{pvs.MapStartPosY.ToString("000")}";

			MapManager.Instance.DrawRectangle(pen, position[0], position[1]);
			MapManager.Instance.DrawString(text, new Font("Arial", 6, FontStyle.Regular), pen.Brush, position[0].X + 5, position[0].Y - 10);
		}

		/// <summary>
		/// Draw one segment
		/// </summary>
		/// <param name="segment"></param>
		private void DrawPVSSegment(PVS_SEGMENT_V1 segment)
		{
			var position = segment.ToPoint();
			MapManager.Instance.DrawRectangle(new Pen(template.PVSSegment), position[0], position[1]);

			foreach (var item in segment.IncludeSegments)
			{
				var position2 = item.ToPoint();
				MapManager.Instance.DrawRectangle(new Pen(template.PVSSegment), position2[0], position2[1]);
			}
		}

		/// <summary>
		/// Draw one prop
		/// </summary>
		/// <param name="prop"></param>
		private void DrawPVSProp(PVS_PROP_V1 prop)
		{
			var position = prop.ToPoint();
			MapManager.Instance.DrawRectangle(new Pen(template.PVSProp), position[0], position[1]);
		}

		/// <summary>
		/// Draw one location
		/// </summary>
		/// <param name="location"></param>
		private void DrawLocation(Location location)
		{
			var pen = new Pen(template.Respawn);
			var positions = location.ToPoints();
			var index = Nfs.Instance.Respawns.IndexOf(location);

			MapManager.Instance.DrawRectangle(pen, positions[0], positions[1]);
			MapManager.Instance.DrawCenterString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, (positions.DefaultIfEmpty().Min(r => r.X)), (positions.DefaultIfEmpty().Max(r => r.Y)));
		}

		/// <summary>
		/// Draw one region info
		/// </summary>
		/// <param name="location"></param>
		private void DrawRegion(LocationInfo locationInfo)
		{
			var pen = new Pen(template.Region);

			for (int i = 0; i < locationInfo.Polygons.Count; i++)
			{
				var positions = locationInfo.Polygons[i].ToPoints();
				var center = locationInfo.ToPoint();
				var index = Nfc.Instance.Region.IndexOf(locationInfo);

				MapManager.Instance.DrawPolygon(pen, positions);
				MapManager.Instance.DrawPoint(pen, center);
				MapManager.Instance.DrawCenterString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, center.X, center.Y);
			}
		}

		/// <summary>
		/// Draw one region info
		/// </summary>
		/// <param name="location"></param>
		private void DrawWater(Water water)
		{
			var pen = new Pen(template.Water);
			var positions = water.ToPoints();
			var index = Nfw.Instance.Waters.IndexOf(water);

			MapManager.Instance.DrawCrossRectangle(pen, positions[0], positions[1]);
			MapManager.Instance.DrawPoint(pen, positions[2]);
			MapManager.Instance.DrawCenterString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, positions[2].X, positions[2].Y);
		}

		/// <summary>
		/// Painting all collision
		/// </summary>
		/// <param name="nfa"></param>
		/// <param name="changed"></param>
		private void PaintingCollision(Nfa nfa, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[collisionIndex].Nodes.Clear();

				foreach (var collision in nfa.Polygons)
				{
					DrawCollision(collision);
					if (changed) treeView1.Nodes[0].Nodes[collisionIndex].Nodes.Add($"{nfa.Polygons.IndexOf(collision)}");
				}
			}
			else
			{
				Invoke(new Action<Nfa, bool>(PaintingCollision), nfa, changed);
			}
		}

		/// <summary>
		/// Painting all light
		/// </summary>
		/// <param name="nfa"></param>
		/// <param name="changed"></param>
		private void PaintingLight(Nfl nfl, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[lightIndex].Nodes.Clear();

				foreach (var light in nfl.Lights)
				{
					DrawLight(light);
					if (changed) treeView1.Nodes[0].Nodes[lightIndex].Nodes.Add($"{nfl.Lights.IndexOf(light)}");
				}
			}
			else
			{
				Invoke(new Action<Nfl, bool>(PaintingLight), nfl, changed);
			}
		}

		/// <summary>
		/// Painting all collision
		/// </summary>
		/// <param name="nfa"></param>
		/// <param name="changed"></param>
		private void PaintingEventArea(Nfe nfe, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[eventAreaIndex].Nodes.Clear();

				foreach (var area in nfe.Events)
				{
					DrawEventArea(area);
					if (changed) treeView1.Nodes[0].Nodes[eventAreaIndex].Nodes.Add($"{nfe.Events.IndexOf(area)}");
				}
			}
			else
			{
				Invoke(new Action<Nfe, bool>(PaintingEventArea), nfe, changed);
			}
		}

		/// <summary>
		/// painting all water
		/// </summary>
		/// <param name="nfs"></param>
		/// <param name="changed"></param>
		private void PaintingWater(Nfw nfw, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[waterIndex].Nodes.Clear();

				foreach (var water in nfw.Waters)
				{
					DrawWater(water);
					if (changed) treeView1.Nodes[0].Nodes[waterIndex].Nodes.Add($"{nfw.Waters.IndexOf(water)}");
				}
			}
			else
			{
				Invoke(new Action<Nfw, bool>(PaintingWater), nfw, changed);
			}
		}

		/// <summary>
		/// painting all region location
		/// </summary>
		/// <param name="nfs"></param>
		/// <param name="changed"></param>
		private void PaintingLocation(Nfs nfs, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[respawnIndex].Nodes.Clear();

				foreach (var location in nfs.Respawns)
				{
					DrawLocation(location);
					if (changed) treeView1.Nodes[0].Nodes[respawnIndex].Nodes.Add($"{nfs.Respawns.IndexOf(location)}");
				}
			}
			else
			{
				Invoke(new Action<Nfs, bool>(PaintingLocation), nfs, changed);
			}
		}

		/// <summary>
		/// Painting all nfp
		/// </summary>
		/// <param name="nfa"></param>
		/// <param name="changed"></param>
		private void PaintingNfp(Nfp nfp, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[nfpIndex].Nodes.Clear();

				foreach (var recorNfp in nfp.Records)
				{
					DrawNfp(recorNfp);
					if (changed) treeView1.Nodes[0].Nodes[nfpIndex].Nodes.Add($"{nfp.Records.IndexOf(recorNfp)}");
				}
			}
			else
			{
				Invoke(new Action<Nfp, bool>(PaintingNfp), nfp, changed);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nfs"></param>
		/// <param name="changed"></param>
		private void PaintingNpcProp(Nfs nfs, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[npcIndex].Nodes.Clear();

				foreach (var prop in nfs.Props)
				{
					DrawNpcProp(prop);
					if (changed) treeView1.Nodes[0].Nodes[npcIndex].Nodes.Add($"{nfs.Props.IndexOf(prop)}");
				}
			}
			else
			{
				Invoke(new Action<Nfs, bool>(PaintingNpcProp), nfs, changed);
			}
		}

		/// <summary>
		/// Painting all quest prop
		/// </summary>
		/// <param name="qpf"></param>
		/// <param name="changed"></param>
		private void PaintingQuestProp(Qpf qpf, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[gateIndex].Nodes.Clear();

				foreach (var prop in qpf.Props)
				{
					DrawProp(prop);
					if (changed) treeView1.Nodes[0].Nodes[gateIndex].Nodes.Add($"{qpf.Props.IndexOf(prop)}");
				}
			}
			else
			{
				Invoke(new Action<Qpf, bool>(PaintingQuestProp), qpf, changed);
			}
		}

		/// <summary>
		/// painting all region location
		/// </summary>
		/// <param name="nfs"></param>
		/// <param name="changed"></param>
		private void PaintingRegion(Nfc nfc, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[regionIndex].Nodes.Clear();

				foreach (var region in nfc.Region)
				{
					DrawRegion(region);
					if (changed) treeView1.Nodes[0].Nodes[regionIndex].Nodes.Add($"{nfc.Region.IndexOf(region)}");
				}
			}
			else
			{
				Invoke(new Action<Nfc, bool>(PaintingRegion), nfc, changed);
			}
		}

		/// <summary>
		/// painting segment potencially visible set
		/// </summary>
		/// <param name="nfs"></param>
		/// <param name="changed"></param>
		private void PaintingPSegment(Pvs pvs, bool show, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[pvsSegmentIndex].Nodes.Clear();
				if (show) DrawPVS(pvs, new Pen(template.PVSSegment));

				foreach (var segment in pvs.Segments)
				{
					if (show) DrawPVSSegment(segment);
					if (changed) treeView1.Nodes[0].Nodes[pvsSegmentIndex].Nodes.Add($"{pvs.Segments.IndexOf(segment)}");
				}
			}
			else
			{
				Invoke(new Action<Pvs, bool, bool>(PaintingPSegment), pvs, show, changed);
			}
		}

		/// <summary>
		/// painting prop potencially visible set
		/// </summary>
		/// <param name="nfs"></param>
		/// <param name="changed"></param>
		private void PaintingPProp(Pvs pvs, bool show, bool changed)
		{
			if (!InvokeRequired)
			{
				if (changed) treeView1.Nodes[0].Nodes[pvsPropIndex].Nodes.Clear();
				if (show) DrawPVS(pvs, new Pen(template.PVSProp));

				foreach (var prop in pvs.Props)
				{
					if (show) DrawPVSProp(prop);
					if (changed) treeView1.Nodes[0].Nodes[pvsPropIndex].Nodes.Add($"{pvs.Props.IndexOf(prop)}");
				}
			}
			else
			{
				Invoke(new Action<Pvs, bool, bool>(PaintingPProp), pvs, show, changed);
			}
		}

		#endregion

		/// <summary>
		/// Draw new object
		/// </summary>
		#region Draw Mode

		private static Cursor move = CursorExtends.SizeAll;
		private static Cursor pen = CursorExtends.Pencil;
		private static Cursor cross = CursorExtends.Cross;
		private static Cursor pointer = CursorExtends.LeftPtr;

		private DrawMapMode currentMode = DrawMapMode.MOVE;
		private DrawElement currentElement = DrawElement.NONE;

		/// <summary>
		/// Component hand for move image with mouse
		/// </summary>
		private void moveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(DrawMapMode.MOVE);
		}

		/// <summary>
		/// Component create a new collision
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nfaModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(DrawMapMode.POLYGON);
			setCurrentElement(DrawElement.NFA);
		}

		/// <summary>
		/// Component create a new region
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nfcModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(DrawMapMode.POLYGON);
			setCurrentElement(DrawElement.NFC);
		}

		/// <summary>
		/// Component create a new event area
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nfeModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(DrawMapMode.POLYGON);
			setCurrentElement(DrawElement.NFE);
		}

		/// <summary>
		/// Component create a new light
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nflModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(DrawMapMode.POINT);
			setCurrentElement(DrawElement.NFL);
		}

		/// <summary>
		/// Component create a new location
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nfs01ModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(DrawMapMode.RECTANGLE);
			setCurrentElement(DrawElement.NFS_SCRIPT);
		}

		/// <summary>
		/// Component create a new npc
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nfs02ModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(DrawMapMode.POINT);
			setCurrentElement(DrawElement.NFS_NPC);
		}

		/// <summary>
		/// Component create a new water
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nfwModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(DrawMapMode.RECTANGLE);
			setCurrentElement(DrawElement.NFW);
		}

		/// <summary>
		/// Component create a new prop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void qpfModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(DrawMapMode.POINT);
			setCurrentElement(DrawElement.QPF);
		}

		/// <summary>
		/// Change the current mode
		/// </summary>
		/// <param name="mode"></param>
		private void setCurrentMode(DrawMapMode mode)
		{
			First = Point.Empty;

			moveModeToolStripMenuItem.BackColor = Color.Transparent;
			pointModeToolStripMenuItem.BackColor = Color.Transparent;
			polygonModeToolStripMenuItem.BackColor = Color.Transparent;
			rectangleModeToolStripMenuItem.BackColor = Color.Transparent;

			zoomChangeState(true);

			switch (mode)
			{
				case DrawMapMode.MOVE:
					mapPictureBox.Cursor = move;
					moveModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;

				case DrawMapMode.POINT:
					mapPictureBox.Cursor = pointer;
					pointModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;

				case DrawMapMode.POLYGON:
					mapPictureBox.Cursor = pen;
					polygonModeToolStripMenuItem.BackColor = Color.DarkGray;
					zoomChangeState(false);
					break;

				case DrawMapMode.RECTANGLE:
					mapPictureBox.Cursor = cross;
					rectangleModeToolStripMenuItem.BackColor = Color.DarkGray;
					zoomChangeState(false);
					break;
			}

			currentMode = mode;
			setCurrentElement(DrawElement.NONE);
		}

		/// <summary>
		/// Change the current mode
		/// </summary>
		/// <param name="mode"></param>
		private void setCurrentElement(DrawElement element)
		{
			First = Point.Empty;

			nfaModeToolStripMenuItem.BackColor = Color.Transparent;
			nfcModeToolStripMenuItem.BackColor = Color.Transparent;
			nfeModeToolStripMenuItem.BackColor = Color.Transparent;
			nflModeToolStripMenuItem.BackColor = Color.Transparent;
			nfs01ModeToolStripMenuItem.BackColor = Color.Transparent;
			nfs02ModeToolStripMenuItem.BackColor = Color.Transparent;
			nfwModeToolStripMenuItem.BackColor = Color.Transparent;
			qpfModeToolStripMenuItem.BackColor = Color.Transparent;

			switch (element)
			{
				case DrawElement.NFA:
					nfaModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;
				case DrawElement.NFC:
					nfcModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;
				case DrawElement.NFE:
					nfeModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;
				case DrawElement.NFL:
					nflModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;
				case DrawElement.NFS_SCRIPT:
					nfs01ModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;
				case DrawElement.NFS_NPC:
					nfs02ModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;
				case DrawElement.NFW:
					nfwModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;
				case DrawElement.QPF:
					qpfModeToolStripMenuItem.BackColor = Color.DarkGray;
					break;
			}

			currentElement = element;
		}

		#endregion

		/// <summary>
		/// Method using mode
		/// </summary>
		#region Draw Element

		private event EventHandler<MouseEventArgs> DragMove;
		private event EventHandler<MouseEventArgs> DrawPoint;
		private event EventHandler<MouseEventArgs> DrawRectangle;
		private event EventHandler<EventArgs> CancelPolygon;
		private event EventHandler<EventArgs> DrawPolygon;
		private event EventHandler<MouseEventArgs> PreviewDrawRectangle;
		private event EventHandler<MouseEventArgs> PreviewDrawPolygon;

		/// <summary>
		/// Move picture by mouse
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_DragMove(object sender, MouseEventArgs e)
		{
			if (First == Point.Empty) return;

			int newX = mapPictureBox.Location.X + (e.Location.X - First.X);
			int newY = mapPictureBox.Location.Y + (e.Location.Y - First.Y);

			flowLayoutPanel1.AutoScrollPosition = new Point(flowLayoutPanel1.AutoScrollOffset.X - newX, flowLayoutPanel1.AutoScrollOffset.Y - newY);
			mapPictureBox.Refresh();
		}

		/// <summary>
		/// Make preview for the rectangle
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_PreviewDrawRectangle(object sender, MouseEventArgs e)
		{
			if (First == Point.Empty) return;

			Pen pen = null;

			switch (currentElement)
			{
				case DrawElement.NFS_SCRIPT:
					pen = new Pen(template.Respawn);
					break;
				case DrawElement.NFW:
					pen = new Pen(template.Water);
					break;
			}

			var pointA = new Point(e.X, First.Y);
			var pointB = First;
			var pointC = e.Location;
			var pointD = new Point(First.X, e.Y);
			var graphic = mapPictureBox.CreateGraphics();

			mapPictureBox.Refresh();
			graphic.DrawLine(pen, pointA, pointB);
			graphic.DrawLine(pen, pointB, pointD);
			graphic.DrawLine(pen, pointD, pointC);
			graphic.DrawLine(pen, pointC, pointA);
		}

		/// <summary>
		/// Make preview for the polygon
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_PreviewDrawPolygon(object sender, MouseEventArgs e)
		{
			if (Polygon.Count == 0) return;

			Pen pen = null;

			switch (currentElement)
			{
				case DrawElement.NFA:
					pen = new Pen(template.Collision);
					break;
				case DrawElement.NFC:
					pen = new Pen(template.Region);
					break;
				case DrawElement.NFE:
					pen = new Pen(template.EventArea);
					break;
			}

			var array = Polygon.ToArray();
			var points = array.Concat(new PointF[] { e.Location });

			mapPictureBox.Refresh();
			mapPictureBox.CreateGraphics().DrawPolygon(pen, points.ToArray());
		}

		/// <summary>
		/// Make the make
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_DrawPoint(object sender, MouseEventArgs e)
		{
			if (First == Point.Empty) return;
			
			switch (currentElement)
			{
				case DrawElement.NFL:
					MapWorker.Instance.Nfl.Add(_2DUtils.AdjustPoint(First, zoom, false, false));
					break;
				case DrawElement.NFS_NPC:
					MapWorker.Instance.Nfs.Add(_2DUtils.AdjustPoint(First, zoom, false, false));
					break;
				case DrawElement.QPF:
					MapWorker.Instance.Qpf.Add(_2DUtils.AdjustPoint(First, zoom, false, false));
					break;
			}

			First = Point.Empty;
		}

		/// <summary>
		/// Make the rectangle
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_DrawRectangle(object sender, MouseEventArgs e)
		{
			if (First == Point.Empty || First == e.Location) return;

			switch (currentElement)
			{
				case DrawElement.NFS_SCRIPT:
					MapWorker.Instance.Nfs.Add(
						_2DUtils.AdjustPoint(First, zoom, false, true),
						_2DUtils.AdjustPoint(e.Location, zoom, false, true));
					break;

				case DrawElement.NFW:
					MapWorker.Instance.Nfw.Add(
						_2DUtils.AdjustPoint(First, zoom, false, false),
						_2DUtils.AdjustPoint(e.Location, zoom, false, false));
					break;
			}
		}

		/// <summary>
		/// Cancel the current polygon
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_CancelPolygon(object sender, EventArgs e)
		{
			Polygon.Clear();
			mapPictureBox.Refresh();
		}

		/// <summary>
		/// Make the polygon
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_DrawPolygon(object sender, EventArgs e)
		{
			if (Polygon.Count > 2)
			{
				switch (currentElement)
				{
					case DrawElement.NFA:
						MapWorker.Instance.Nfa.Add(_2DUtils.AdjustPolygonPoint(Polygon, zoom, true, true));
						break;

					case DrawElement.NFC:
						var normal = _2DUtils.AdjustPolygonPoint(Polygon, zoom, false, false);
						var center = _2DUtils.GetCenterPolygon(normal);
						MapWorker.Instance.Nfc.Add(_2DUtils.AdjustPolygonPoint(Polygon, zoom, true, true), center);
						break;

					case DrawElement.NFE:
						MapWorker.Instance.Nfe.Add(_2DUtils.AdjustPolygonPoint(Polygon, zoom, true, true));
						break;
				}
			}

			Polygon.Clear();
		}

		#endregion

		/// <summary>
		/// Method for map picture
		/// </summary>
		#region Picture

		/// <summary>
		/// Stock the first point
		/// </summary>
		private Point First = Point.Empty;
		private Point Game = Point.Empty;
		private Point Map = Point.Empty;
		private PointF SegmentF = Point.Empty;
		private Point Segment = Point.Empty;
		private Point Tile = Point.Empty;

		private int SegmentNumber = 0;
		private int TileNumber = 0;

		/// <summary>
		/// Stock point of the polygon
		/// </summary>
		private List<PointF> Polygon = new List<PointF>();

		/// <summary>
		/// Methode used when click down on the picture
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (currentMode == DrawMapMode.MOVE || currentElement != DrawElement.NONE)
			{
				if (e.Button == MouseButtons.Left)
				{
					First = e.Location;

					switch (currentMode)
					{
						case DrawMapMode.POINT:
							DrawPoint?.Invoke(sender, e);
							break;
						case DrawMapMode.POLYGON:
							Polygon.Add(e.Location);
							break;
					}
				}
				else if (e.Button == MouseButtons.Right)
				{
					cancelToolStripMenuItem.Visible = false;
					completedToolStripMenuItem.Visible = false;

					switch (currentMode)
					{
						case DrawMapMode.POLYGON:
							cancelToolStripMenuItem.Visible = true;
							completedToolStripMenuItem.Visible = true;
							break;
					}
				}
			}
		}

		/// <summary>
		/// Method used whend click up on the picture
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				switch (currentMode)
				{
					case DrawMapMode.RECTANGLE:
						DrawRectangle?.Invoke(sender, e);
						break;
				}
			}

			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) { First = Point.Empty; }
		}

		/// <summary>
		/// Method used when mouse enter or move
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			Map = _2DUtils.AdjustPoint(e.Location, zoom, false, false);

			//
			// Game point
			//
			var mapX = Map.X + MapWorker.Instance.X * 16128;
			var mapY = Map.Y + MapWorker.Instance.Y * 16128;

			Game = new Point(mapX, mapY);

			//
			// Segment point
			//
			var segmentX = Math.Min(Map.X / Global.tileLenght / 6, 63);
			var segmentY = Math.Min(Map.Y / Global.tileLenght / 6, 63);

			Segment = new Point(segmentX, segmentY);
			SegmentNumber = Segment.X + Segment.Y * 64;

			//
			// Tile point
			//
			var tileX = Math.Min(Map.X / Global.tileLenght - 6 * Segment.X, 5);
			var tileY = Math.Min(Map.Y / Global.tileLenght - 6 * Segment.Y, 5);

			Tile = new Point(tileX, tileY);
			TileNumber = Tile.X + Tile.Y * 6;

			//
			// Segment location
			//
			var segmentFX = Map.X - Segment.X * 42 * 6;
			var segmentFY = Map.Y - Segment.Y * 42 * 6;

			SegmentF = new PointF(segmentFX, segmentFY);

			switch (currentMode)
			{
				case DrawMapMode.MOVE:
					DragMove?.Invoke(sender, e);
					break;
				case DrawMapMode.POLYGON:
					PreviewDrawPolygon?.Invoke(sender, e);
					break;
				case DrawMapMode.RECTANGLE:
					PreviewDrawRectangle?.Invoke(sender, e);
					break;
			}

			lbMapCoordinate.Text = $"{Map.X}, {Map.Y}";
			lbGameCoordinate.Text = $"{Game.X}, {Game.Y}";
			lbSegmentCoordinate.Text = $"{Segment.X}, {Segment.Y} ({SegmentNumber})";
			lbTileCoodinate.Text = $"{Tile.X}, {Tile.Y} ({TileNumber})";

			statusStrip.Refresh();
		}

		/// <summary>
		/// Method used when mouse leave
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapPictureBox_MouseLeave(object sender, EventArgs e)
		{
			lbMapCoordinate.Text = "";
			lbGameCoordinate.Text = "";
			lbSegmentCoordinate.Text = "";
			lbTileCoodinate.Text = "";

			statusStrip.Refresh();
		}

		#endregion

		/// <summary>
		/// Manage map object
		/// </summary>
		#region TreeView & PropertyGrid control

		private TreeNode previousNode;

		/// <summary>
		/// Method used when property grid value is changed
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				MapManager.Instance.Restore();

				if (treeView1.SelectedNode.Level == 1)
				{
					SelectedMapTypeChanged(treeView1.SelectedNode);
				}
				else if (treeView1.SelectedNode.Level == 2)
				{
					SelectedMapItemChanged(treeView1.SelectedNode);
				}
				else
				{
					MapWorker.Instance.Refresh(false);
				}

				mapPictureBox.Refresh();
			}
		}

		/// <summary>
		/// Method used when property objet is change.
		/// Disable visibility when no objet associate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void propertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
		{
			var visibility = propertyGrid.SelectedObject != null;

			propertyGrid.Visible = visibility;
			labelProperty.Visible = visibility;
		}

		/// <summary>
		/// Method used when collection editor as Add, Remove, PropertyChanger
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void propertyGrid_Updated(object sender, EventArgs e)
		{
			MapManager.Instance.Restore();
			MapWorker.Instance.Refresh(true);
			
			template.Save();

			treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));
		}

		/// <summary>
		/// Method use when select node map item object
		/// </summary>
		/// <param name="e"></param>
		private dynamic SelectedMapItemChanged(TreeNode e)
		{
			dynamic obj = null;

			if (e.Parent.Index == collisionIndex)
			{
				obj = Nfa.Instance.Polygons[e.Index];
				DrawCollision(obj);
			}
			else if (e.Parent.Index == eventAreaIndex)
			{
				obj = Nfe.Instance.Events[e.Index];
				DrawEventArea(obj);
			}
			else if (e.Parent.Index == lightIndex)
			{
				obj = Nfl.Instance.Lights[e.Index];
				DrawLight(obj);
			}
			else if (e.Parent.Index == respawnIndex)
			{
				obj = Nfs.Instance.Respawns[e.Index];
				DrawLocation(obj);
			}
			else if (e.Parent.Index == nfpIndex)
			{
				obj = Nfp.Instance.Records[e.Index];
				DrawNfp(obj);
			}
			else if (e.Parent.Index == npcIndex)
			{
				obj = Nfs.Instance.Props[e.Index];
				DrawNpcProp(obj);
			}
			else if (e.Parent.Index == pvsSegmentIndex)
			{
				obj = Pvs.Instance.Segments[e.Index];
				DrawPVSSegment(obj);
			}
			else if (e.Parent.Index == pvsPropIndex)
			{
				obj = Pvs.Instance.Props[e.Index];
				DrawPVSProp(obj);
			}
			else if (e.Parent.Index == gateIndex)
			{
				obj = Qpf.Instance.Props[e.Index];
				DrawProp(obj);
			}
			else if (e.Parent.Index == waterIndex)
			{
				obj = Nfw.Instance.Waters[e.Index];
				DrawWater(obj);
			}
			else if (e.Parent.Index == regionIndex)
			{
				obj = Nfc.Instance.Region[e.Index];
				DrawRegion(obj);
			}

			return obj;
		}

		/// <summary>
		/// Method use when select node map type object
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		private dynamic SelectedMapTypeChanged(TreeNode e)
		{
			dynamic obj = null;

			if (e.Index == collisionIndex)
			{
				obj = Nfa.Instance;
				PaintingCollision(obj, false);
			}
			else if (e.Index == eventAreaIndex)
			{
				obj = Nfe.Instance;
				PaintingEventArea(obj, false);
			}
			else if (e.Index == lightIndex)
			{
				obj = Nfl.Instance;
				PaintingLight(obj, false);
			}
			else if (e.Index == respawnIndex)
			{
				obj = Nfs.Instance;
				PaintingLocation(obj, false);
			}
			else if (e.Index == nfpIndex)
			{
				obj = Nfp.Instance;
				PaintingNfp(obj, false);
			}
			else if (e.Index == npcIndex)
			{
				obj = Nfs.Instance;
				PaintingNpcProp(obj, false);
			}
			else if (e.Index == gateIndex)
			{
				obj = Qpf.Instance;
				PaintingQuestProp(obj, false);
			}
			else if (e.Index == pvsSegmentIndex)
			{
				obj = Pvs.Instance;
				PaintingPSegment(obj, true, false);
			}
			else if (e.Index == pvsPropIndex)
			{
				obj = Pvs.Instance;
				PaintingPProp(obj, true, false);
			}
			else if (e.Index == waterIndex)
			{
				obj = Nfw.Instance;
				PaintingWater(obj, false);
			}
			else if (e.Index == regionIndex)
			{
				obj = Nfc.Instance;
				PaintingRegion(obj, false);
			}

			return obj;
		}

		/// <summary>
		/// Method use when template color properties is changed
		/// </summary>
		private void template_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			treeView1.Nodes[0].Nodes[collisionIndex].ForeColor = template.Collision;
			treeView1.Nodes[0].Nodes[waterIndex].ForeColor = template.Water;
			treeView1.Nodes[0].Nodes[lightIndex].ForeColor = template.Light;
			treeView1.Nodes[0].Nodes[respawnIndex].ForeColor = template.Respawn;
			treeView1.Nodes[0].Nodes[eventAreaIndex].ForeColor = template.EventArea;
			treeView1.Nodes[0].Nodes[nfpIndex].ForeColor = template.Nfp;
			treeView1.Nodes[0].Nodes[npcIndex].ForeColor = template.NpcProp;
			treeView1.Nodes[0].Nodes[pvsSegmentIndex].ForeColor = template.PVSSegment;
			treeView1.Nodes[0].Nodes[pvsPropIndex].ForeColor = template.PVSProp;
			treeView1.Nodes[0].Nodes[gateIndex].ForeColor = template.Prop;
			treeView1.Nodes[0].Nodes[regionIndex].ForeColor = template.Region;

			template.Save();
		}

		/// <summary>
		/// Method use when selected node item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null) return;

			dynamic obj = null;
			clearToolStripMenuItem.Enabled = false;
			removeToolStripMenuItem.Enabled = false;

			previousNode = e.Node;
			MapManager.Instance.Restore();

			if (e.Node.Level == 1)
			{
				obj = SelectedMapTypeChanged(e.Node);
				clearToolStripMenuItem.Enabled = true;
			}
			else if (e.Node.Level == 2)
			{
				obj = SelectedMapItemChanged(e.Node);
				removeToolStripMenuItem.Enabled = true;
			}
			else
			{
				obj = MapWorker.Instance;
				MapWorker.Instance.Refresh(false);
			}
            
			propertyGrid.SelectedObject = obj;
			mapPictureBox.Refresh();
		}

		/// <summary>
		/// Method use when collapse node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Level == 0) e.Cancel = true;
		}

		/// <summary>
		/// Method use when node mouse click right
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				treeView1.SelectedNode = e.Node;
			}
		}

		#endregion

		/// <summary>
		/// Method for context menu strip
		/// </summary>
		#region ContextMenuStrip

		/// <summary>
		/// Methos used for cancel polygon
		/// </summary>
		private void CancelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CancelPolygon?.Invoke(sender, e);
		}

		/// <summary>
		/// Method used for clear
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selectedNode = treeView1.SelectedNode;

			if (selectedNode.Index == collisionIndex)
			{
				Nfa.Instance.Polygons.Clear();
			}
			else if (selectedNode.Index == eventAreaIndex)
			{
				Nfe.Instance.Events.Clear();
			}
			else if (selectedNode.Index == lightIndex)
			{
				Nfl.Instance.Lights.Clear();
			}
			else if (selectedNode.Index == respawnIndex)
			{
				Nfs.Instance.Respawns.Clear();
			}
			else if (selectedNode.Index == nfpIndex)
			{
				Nfp.Instance.Records.Clear();
			}
			else if (selectedNode.Index == npcIndex)
			{
				Nfs.Instance.Props.Clear();
			}
			else if (selectedNode.Index == pvsSegmentIndex)
			{
				Pvs.Instance.Segments.Clear();
			}
			else if (selectedNode.Index == pvsPropIndex)
			{
				Pvs.Instance.Props.Clear();
			}
			else if (selectedNode.Index == gateIndex)
			{
				Qpf.Instance.Props.Clear();
			}
			else if (selectedNode.Index == waterIndex)
			{
				Nfw.Instance.Waters.Clear();
			}
			else if (selectedNode.Index == regionIndex)
			{
				Nfc.Instance.Region.Clear();
			}

			MapWorker.Instance.Refresh(true);
			treeView1_AfterSelect(this, new TreeViewEventArgs(selectedNode));
		}

		/// <summary>
		/// Method used for copy command
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CommandToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.Clear();
			Clipboard.SetData(DataFormats.Text, $"/run warp({Game.X}, {Game.Y})");
		}

		/// <summary>
		/// Method used for complet polygon
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CompletedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (currentMode)
			{
				case DrawMapMode.POLYGON:
					DrawPolygon?.Invoke(sender, e);
					break;
				default:
					First = Point.Empty;
					break;
			}
		}

		/// <summary>
		/// Method used for copy position game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.Clear();
			Clipboard.SetData(DataFormats.Text, $"Game position : {Game.X}, {Game.Y}");
		}

		/// <summary>
		/// Method used for copy position map
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.Clear();
			Clipboard.SetData(DataFormats.Text, $"Map position : {Map.X}, {Map.Y}");
		}

		/// <summary>
		/// Method used for remove
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selectedNode = treeView1.SelectedNode;

			if (selectedNode.Parent.Index == collisionIndex)
			{
				MapWorker.Instance.Nfa.Remove(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == eventAreaIndex)
			{
				MapWorker.Instance.Nfe.Remove(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == lightIndex)
			{
				MapWorker.Instance.Nfl.Remove(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == respawnIndex)
			{
				MapWorker.Instance.Nfs.RemoveR(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == nfpIndex)
			{
				MapWorker.Instance.Nfp.Remove(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == npcIndex)
			{
				MapWorker.Instance.Nfs.RemoveP(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == pvsSegmentIndex)
			{
				MapWorker.Instance.Pvs.RemoveS(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == pvsPropIndex)
			{
				MapWorker.Instance.Pvs.RemoveP(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == gateIndex)
			{
				MapWorker.Instance.Qpf.Remove(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == waterIndex)
			{
				MapWorker.Instance.Nfw.Remove(selectedNode.Index);
			}
			else if (selectedNode.Parent.Index == regionIndex)
			{
				MapWorker.Instance.Nfc.Remove(selectedNode.Index);
			}

			MapWorker.Instance.Refresh(true);
			treeView1_AfterSelect(this, new TreeViewEventArgs(selectedNode));
		}

		/// <summary>
		/// Method used for copy position segment
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SegmentToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.Clear();
			Clipboard.SetData(DataFormats.Text, $"Segment : {Segment.X}, {Segment.Y} ({SegmentNumber}");
		}

		/// <summary>
		/// Method used for copy position tile
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.Clear();
			Clipboard.SetData(DataFormats.Text, $"Tile : {Tile.X}, {Tile.Y} {TileNumber}");
		}

		#endregion

		/// <summary>
		/// Method for log
		/// </summary>
		#region Console
		
		private Levels currentLevel = Levels.Debug;

		private int errorCount = 0;
		private int oldHeightConsole = 227;
		private int warningCount = 0;

		/// <summary>
		/// Event used for collapse/uncollapse log error
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Click(object sender, EventArgs e)
		{
			if (splitContainer4.Panel2Collapsed)
			{
				button1.ForeColor = Color.White;
				button1.FlatAppearance.BorderColor = Color.FromArgb(255, 45, 45, 48);
				splitContainer3.IsSplitterFixed = false;
				splitContainer3.SplitterDistance = splitContainer3.Height - oldHeightConsole;
				splitContainer4.Panel2Collapsed = false;
				LogRtb.ScrollToCaret();
			}
			else
			{
				oldHeightConsole = splitContainer4.Height;
				splitContainer3.IsSplitterFixed = true;
				splitContainer3.SplitterDistance = splitContainer3.Height - 27;
				splitContainer4.Panel2Collapsed = true;
			}

			splitContainer4.SplitterDistance = 27;
			splitContainer4.Refresh();
		}

		/// <summary>
		/// Insert log in the console
		/// </summary>
		/// <param name="txt"></param>
		/// <param name="color"></param>
		public void InsertLog(Levels level, string txt, Color color)
		{
			if (!LogRtb.InvokeRequired)
			{
				LogRtb.SelectionColor = color;
				LogRtb.AppendText(txt.Replace(@"\", "/"));
				LogRtb.ScrollToCaret();

				switch (level)
				{
					case Levels.Error:
						errorCount++;
						break;
					case Levels.Warning:
						warningCount++;
						break;
				}

				if (splitContainer4.Panel2Collapsed == true)
				{
					if (currentLevel < level)
					{
						button1.ForeColor = color;
						button1.FlatAppearance.BorderColor = color;
						currentLevel = level;
					}
				}

				RefreshLog();
			}
			else
			{
				LogRtb.Invoke(new Action<Levels, string, Color>(InsertLog), level, txt, color);
			}
		}

		/// <summary>
		/// Clear all log
		/// </summary>
		private void ClearLog()
		{
			if (!LogRtb.InvokeRequired)
			{
				button1.ForeColor = Color.White;

				errorCount = 0;
				warningCount = 0;
				currentLevel = Levels.Debug;

				LogRtb.Clear();
				LogRtb.ScrollToCaret();
				RefreshLog();
			}
			else
			{
				LogRtb.Invoke(new Action(ClearLog));
			}
		}

		/// <summary>
		/// Refresh state log 
		/// </summary>
		private void RefreshLog()
		{
			lbError.Text = $"{errorCount} Error(s)";
			lbWarning.Text = $"{warningCount} Warning(s)";
		}

		#endregion
	}
}
