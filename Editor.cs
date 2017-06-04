using MapEditor.Dialog;
using MapEditor.Extends;
using MapEditor.Events;
using MapEditor.Models;
using MapEditor.Modules;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using MapEditor.Attributes;

namespace MapEditor
{
    public partial class Editor : Form
    {
        public static Editor Instance = null;

        private Template template;
        private XEnv xEnv;

        private string projectName = "";
        private const string projectDefault = "Empty";
        private string projectPath = "";

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
        public Editor()
        {
            Instance = this;
            InitializeComponent();
            InitializeModules();
            InitializeBinding();
            InitializeSubComponent();
        }

        /// <summary>
        /// Initialize modules
        /// </summary>
        public void InitializeModules()
        {
            ///
            xEnv = new XEnv();
            xEnv.LoadFromFile("mapeditor.opt");
            projectPath = xEnv.GetValue("mapeditor.dump_path", Application.StartupPath + "/Resource");
            ///
            MapManager.Init();
            ///
            MapWorker.Init(projectPath);
            MapWorker.Instance.Painting += MapWorker_Painting;
            ///
            template = new Template();
            if (Properties.Settings.Default.Context == null)
            {
                Properties.Settings.Default.Save();
            }

            template.Collision = Properties.Settings.Default.Collision;
            template.EventArea = Properties.Settings.Default.EventArea;
            template.Light = Properties.Settings.Default.Light;
            template.Nfp = Properties.Settings.Default.Nfp;
            template.NpcProp = Properties.Settings.Default.NpcProp;
            template.Prop = Properties.Settings.Default.Prop;
            template.PVSProp = Properties.Settings.Default.PVSProp;
            template.PVSSegment = Properties.Settings.Default.PVSSegment;
            template.Region = Properties.Settings.Default.Region;
            template.Respawn = Properties.Settings.Default.Respawn;
            template.Water = Properties.Settings.Default.Water;
            template.PropertyChanged += Template_PropertyChanged;
            ///
            CollectionEditorExtends.PropertyValueChanged += propertyGrid_PropertyValueChanged;
            CollectionEditorExtends.OkClick += propertyGrid_Updated;
            ///
            DragMove += mapPictureBox_DragMove;
            DrawPoint += mapPictureBox_DrawPoint;
            DrawRectangle += mapPictureBox_DrawRectangle;
            CancelPolygon += mapPictureBox_CancelPolygon;
            DrawPolygon += mapPictureBox_DrawPolygon;
            PreviewDrawRectangle += mapPictureBox_PreviewDrawRectangle;
            PreviewDrawPolygon += mapPictureBox_PreviewDrawPolygon;
        }

        /// <summary>
        /// Initialize binding
        /// </summary>
        public void InitializeBinding()
        {
            mapPictureBox.DataBindings.Add(new Binding("Image", MapManager.Instance, nameof(MapManager.Instance.Picture), false, DataSourceUpdateMode.OnPropertyChanged));
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
            propertyGrid1.BrowsableAttributes = new AttributeCollection(new PropertyGridBrowsableAttribute(true));
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
            treeView1.Nodes[0].ForeColor = Color.White;
            treeView1.Nodes[0].Nodes[collisionIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[eventAreaIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[gateIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[lightIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[nfpIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[npcIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[pvsPropIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[pvsSegmentIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[regionIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[respawnIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Nodes[waterIndex].NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            treeView1.Nodes[0].Expand();
            ///
            setCurrentMode(DrawMapMode.MOVE);
            template.Update();
        }

        #endregion

        /// <summary>
        /// Window control method
        /// </summary>
        #region Window control

        private bool comboBoxDataSourceChanged;
        
        /// <summary>
        /// Method used for about this software
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutMapEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        /// <summary>
        /// Method used for load file list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapWorker.Instance.Watch(projectPath);

            comboBoxDataSourceChanged = true;
            mapToolStripComboBox.ComboBox.DataSource = MapWorker.Instance.Mapping.OrderBy(r => r).ToList();
            mapToolStripComboBox.ComboBox.SelectedIndex = -1;

            loadToolStripMenuItem.Enabled = false;
            updateToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// Method used for update file list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapWorker.Instance.Watch(projectPath);

            comboBoxDataSourceChanged = true;
            mapToolStripComboBox.ComboBox.DataSource = MapWorker.Instance.Mapping.OrderBy(r => r).ToList();
            mapToolStripComboBox.ComboBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Method used for fast load
        /// </summary>
        private void mapToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mapToolStripComboBox.ComboBox.SelectedItem != null && !comboBoxDataSourceChanged)
                OpenMapFile_FileOk((string)mapToolStripComboBox.SelectedItem);

            comboBoxDataSourceChanged = false;
        }

        /// <summary>
        /// Method used when create new
        /// </summary>
        private void nouveauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newProject = new NewProject();
            if (newProject.ShowDialog() == DialogResult.OK)
            {
                NewProject_Ok(newProject.Project);
            }
        }

        /// <summary>
        /// Method used when create new file ok
        /// </summary>
        /// <param name="name"></param>
        private void NewProject_Ok(string name)
        {
            projectName = name;

            ClearLog();
            MapManager.Instance.New();
            MapWorker.Instance.New(projectPath, name);

            Text = string.Format("Map Editor - {0}", projectName);
            //
            propertyGrid1.SelectedObject = null;
            //
            treeView1.SelectedNode = null;
            treeView1.SelectedNode = treeView1.Nodes[0];

        }

        /// <summary>
        /// Method used when open file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ouvrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog();
            openFile.Filter = "Nflavor map|*.nfa;*.nfc;*.nfe;*.nfl;*.nfm;*.nfp;*.nfs;*.nfw;*.pvs;*.qpf|Config file|*.cfg";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                OpenMapFile_FileOk(openFile.SafeFileName.Split(new char[] { '.' })[0]);
            }
        }

        /// <summary>
        /// Method used whend file open ok
        /// </summary>
        /// <param name="filename"></param>
        private void OpenMapFile_FileOk(string filename)
        {
            projectName = filename;

            ClearLog();
            XLog.WriteLine(Levels.Info, "Loading map {0} on path {1}", projectName, projectPath);

            MapManager.Instance.Load(projectPath + @"\jpg\", projectName);
            MapWorker.Instance.Load(projectPath, projectName);

            Text = string.Format("Map Editor - {0}", projectName);
            //
            propertyGrid1.SelectedObject = null;
            //
            treeView1.SelectedNode = null;
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        /// <summary>
        /// Method used when closing map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            projectName = projectDefault;
            
            ClearLog();
            MapManager.Instance.New();
            MapWorker.Instance.Dispose();

            Text = string.Format("Map Editor - {0}", projectName);
            //
            propertyGrid1.SelectedObject = null;
            //
            treeView1.SelectedNode = null;
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        /// <summary>
        /// Copy current coordonnate point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData(
                DataFormats.Text,
                (object)$"Segment point : {segmentToolStripStatusLabel.Text}\n" +
                $"Map point : {coordonateToolStripStatusLabel.Text}\n" +
                $"Game point : {gameToolStripStatusLabel.Text}\n"
            );
        }

        /// <summary>
        /// Method used when save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enregistrerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearLog();
            MapWorker.Instance.Save(projectPath);
        }

        /// <summary>
        /// Method used when save as folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enregistrersousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                MapWorker.Instance.Save(folderDialog.SelectedPath);
            }
        }

        /// <summary>
        /// Method for config color element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void couleursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = template;
        }

        /// <summary>
        /// Method for config dump path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
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
        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Method used when terrain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void terrainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EditorNFM(projectName, MapWorker.Instance.Nfm).ShowDialog();
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
            statusStrip1.Refresh();
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
            PaintingRegion(e.Nfc, e.CollectionChanged);
            PaintingCollision(e.Nfa, e.CollectionChanged);
            PaintingLight(e.Nfl, e.CollectionChanged);
            PaintingEventArea(e.Nfe, e.CollectionChanged);
            PaintingLocation(e.Nfs, e.CollectionChanged);
            PaintingNfp(e.Nfp, e.CollectionChanged);
            PaintingNpcProp(e.Nfs, e.CollectionChanged);
            PaintingWater(e.Nfw, e.CollectionChanged);
            PaintingQuestProp(e.Qpf, e.CollectionChanged);
            //PaintingPSegment(e.Pvs, false, e.CollectionChanged);
            //PaintingPProp(e.Pvs, false, e.CollectionChanged);

            if (treeView1.SelectedNode != previousNode) treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));
            mapPictureBox.Refresh();
        }

        /// <summary>
        /// Draw one collision
        /// </summary>
        /// <param name="polygon"></param>
        private void DrawCollision(Polygon2 polygon)
        {
            var pen = new Pen(template.Collision);
            var positions = polygon.ToPoints();
            var convertPosition = MapManager.Instance.GetPointsRotate180FlipY(positions);
            var index = Nfa.Instance.Polygons.IndexOf(polygon);
            var size = MapManager.Instance.MeasureString($"{index}");

            MapManager.Instance.DrawPolygon(pen, convertPosition);
            MapManager.Instance.DrawString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, 
                (convertPosition.FirstOrDefault().X - (size.Width / 2f)), 
                (convertPosition.FirstOrDefault().Y + (size.Height / 2f)));
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
                var convertPosition = MapManager.Instance.GetPointsRotate180FlipY(positions);
                var index = Nfe.Instance.Events.IndexOf(area);
                var size = MapManager.Instance.MeasureString($"{index}");

                MapManager.Instance.DrawPolygon(pen, convertPosition);
                MapManager.Instance.DrawString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, 
                    (convertPosition.DefaultIfEmpty().Min(p => p.X) - (size.Width / 2f) + 7f), 
                    (convertPosition.DefaultIfEmpty().Min(p => p.Y) + (size.Height / 2f) - 5f));
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
            var convertPosition = MapManager.Instance.GetPointRotate180FlipY(position);
            var index = Nfl.Instance.Lights.IndexOf(light);
            var size = MapManager.Instance.MeasureString($"{index}");

            MapManager.Instance.DrawPoint(pen, convertPosition);
            MapManager.Instance.DrawString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, 
                (convertPosition.X - (size.Width / 2f)), 
                (convertPosition.Y + (size.Height / 2f)));
        }

        /// <summary>
        /// Draw one npc prop
        /// </summary>
        /// <param name="prop"></param>
        private void DrawNpcProp(PropScriptInfo prop)
        {
            var pen = new Pen(template.NpcProp);
            var position = prop.ToPoint();
            var convertPosition = MapManager.Instance.GetPointRotate180FlipY(position);
            var index = Nfs.Instance.Props.IndexOf(prop);
            var size = MapManager.Instance.MeasureString($"{index}");

            MapManager.Instance.DrawPoint(pen, convertPosition);
            MapManager.Instance.DrawString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, 
                (convertPosition.X - (size.Width / 2f)), 
                (convertPosition.Y + (size.Height / 2f)));
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
                var convertPositions = MapManager.Instance.GetPointsRotate180FlipY(positions);
                var index = Nfp.Instance.Records.IndexOf(recorNfp);
                var size = MapManager.Instance.MeasureString($"{index}");

                MapManager.Instance.DrawPolygon(pen, convertPositions);
                MapManager.Instance.DrawString($"{recorNfp.Id}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, 
                    (convertPositions.FirstOrDefault().X - (size.Width / 2f)), 
                    (convertPositions.FirstOrDefault().Y + (size.Height / 2f)));
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
            var convertPosition = MapManager.Instance.GetPointRotate180FlipY(position);
            var index = Qpf.Instance.Props.IndexOf(prop);
            var size = MapManager.Instance.MeasureString($"{index}");

            MapManager.Instance.DrawPoint(pen, convertPosition);
            MapManager.Instance.DrawString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, 
                (convertPosition.X - (size.Width / 2f)), 
                (convertPosition.Y + (size.Height / 2f)));
        }

        /// <summary>
        /// Draw location pvs segment
        /// </summary>
        /// <param name="pvs"></param>
        private void DrawPVSSegment(Pvs pvs, Pen pen)
        {
            var x1 = ((pvs.SegmentLeft * Global.titleLenght * 6) / 7.875f);
            var y1 = ((pvs.SegmentTop * Global.titleLenght * 6) / 7.875f);
            var x2 = ((pvs.SegmentRight * Global.titleLenght * 6) / 7.875f);
            var y2 = ((pvs.SegmentBottom * Global.titleLenght * 6) / 7.875f);
            var point1 = MapManager.Instance.GetPointRotate180FlipY(new PointF(x1, y1));
            var point2 = MapManager.Instance.GetPointRotate180FlipY(new PointF(x2, y2));
            var text = $"m{pvs.MapStartPosX.ToString("000")}_{pvs.MapStartPosY.ToString("000")}";
            var size = MapManager.Instance.MeasureString(text);

            MapManager.Instance.DrawRectangle(pen, new PointF(point1.X, point1.Y), new PointF(point2.X, point2.Y));
            MapManager.Instance.DrawString(text, new Font("Arial", 6, FontStyle.Regular), pen.Brush,
                (point1.X - size.Width / 2f),
                (point1.Y + (size.Height / 2f)));
        }

        /// <summary>
        /// Draw one segment
        /// </summary>
        /// <param name="segment"></param>
        private void DrawPVSSegment(PVS_SEGMENT_V1 segment)
        {
            var x1 = ((segment.SegmentX * Global.titleLenght * 6) / 7.875f);
            var y1 = ((segment.SegmentY * Global.titleLenght * 6) / 7.875f);
            var segmentPoint = MapManager.Instance.GetPointRotate180FlipY(new PointF(x1, y1));
            var index = Pvs.Instance.Segments.IndexOf(segment);
            var size = MapManager.Instance.MeasureString($"{index}");

            MapManager.Instance.DrawPoint(new Pen(template.PVSSegment), segmentPoint);
            MapManager.Instance.DrawString($"{index}", new Font("Arial", 6, FontStyle.Regular), new Pen(template.PVSSegment).Brush, 
                (segmentPoint.X - size.Width / 2), 
                (segmentPoint.Y + size.Height / 2));

            foreach (var includeSegment in segment.IncludeSegments)
            {
                var x = (float)((includeSegment.SegmentX * Global.titleLenght * 6) / 7.875);
                var y = (float)((includeSegment.SegmentY * Global.titleLenght * 6) / 7.875);
                var point = MapManager.Instance.GetPointRotate180FlipY(new PointF(x, y));

                MapManager.Instance.DrawPoint(new Pen(template.PVSSegment), point);
            }
        }

        /// <summary>
        /// Draw one prop
        /// </summary>
        /// <param name="segment"></param>
        private void DrawPVSProp(PVS_PROP_V1 prop)
        {
            var x1 = (float)((prop.SegmentX * Global.titleLenght * 6) / 7.875);
            var y1 = (float)((prop.SegmentY * Global.titleLenght * 6) / 7.875);
            var propPoint = MapManager.Instance.GetPointRotate180FlipY(new PointF(x1, y1));

            MapManager.Instance.DrawPoint(new Pen(template.PVSProp), propPoint);
        }

        /// <summary>
        /// Draw one location
        /// </summary>
        /// <param name="location"></param>
        private void DrawLocation(Location location)
        {
            var pen = new Pen(template.Respawn);
            var positions = location.ToPoints();
            var convertPosition = MapManager.Instance.GetPointsRotate180FlipY(positions);
            var index = Nfs.Instance.Respawns.IndexOf(location);

            MapManager.Instance.DrawRectangle(pen, convertPosition[0], convertPosition[1]);
            MapManager.Instance.DrawString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush,
                (convertPosition.DefaultIfEmpty().Min(r => r.X)),
                (convertPosition.DefaultIfEmpty().Max(r => r.Y)));
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
                var convertPosition = MapManager.Instance.GetPointsRotate180FlipY(positions);
                var convertCenter = MapManager.Instance.GetPointRotate180FlipY(new PointF(locationInfo.X / 7.875f, locationInfo.Y / 7.875f));
                var index = Nfc.Instance.Region.IndexOf(locationInfo);
                var size = MapManager.Instance.MeasureString($"{index}");

                MapManager.Instance.DrawPolygon(pen, convertPosition);
                MapManager.Instance.DrawPoint(pen, convertCenter);
                MapManager.Instance.DrawString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush,
                    (convertCenter.X - (size.Width / 2f)),
                    (convertCenter.Y + (size.Height / 2f)));
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
            var convertPosition = MapManager.Instance.GetPointsRotate180FlipY(positions);
            var index = Nfw.Instance.Waters.IndexOf(water);
            var size = MapManager.Instance.MeasureString($"{index}");

            MapManager.Instance.DrawCrossRectangle(pen, convertPosition[0], convertPosition[1]);
            MapManager.Instance.DrawPoint(pen, convertPosition[2]);
            MapManager.Instance.DrawString($"{index}", new Font("Arial", 6, FontStyle.Regular), pen.Brush, 
                convertPosition[2].X - (size.Width / 2), 
                convertPosition[2].Y + (size.Height / 2));
        }

        /// <summary>
        /// Painting all collision
        /// </summary>
        /// <param name="nfa"></param>
        /// <param name="changed"></param>
        private void PaintingCollision(Nfa nfa, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[collisionIndex].Nodes.Clear();

            foreach (var collision in nfa.Polygons)
            {
                DrawCollision(collision);
                if (changed) treeView1.Nodes[0].Nodes[collisionIndex].Nodes.Add($"{nfa.Polygons.IndexOf(collision)}");
            }
        }

        /// <summary>
        /// Painting all light
        /// </summary>
        /// <param name="nfa"></param>
        /// <param name="changed"></param>
        private void PaintingLight(Nfl nfl, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[lightIndex].Nodes.Clear();

            foreach (var light in nfl.Lights)
            {
                DrawLight(light);
                if (changed) treeView1.Nodes[0].Nodes[lightIndex].Nodes.Add($"{nfl.Lights.IndexOf(light)}");
            }
        }

        /// <summary>
        /// Painting all collision
        /// </summary>
        /// <param name="nfa"></param>
        /// <param name="changed"></param>
        private void PaintingEventArea(Nfe nfe, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[eventAreaIndex].Nodes.Clear();

            foreach (var area in nfe.Events)
            {
                DrawEventArea(area);
                if (changed) treeView1.Nodes[0].Nodes[eventAreaIndex].Nodes.Add($"{nfe.Events.IndexOf(area)}");
            }
        }

        /// <summary>
        /// painting all water
        /// </summary>
        /// <param name="nfs"></param>
        /// <param name="changed"></param>
        private void PaintingWater(Nfw nfw, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[waterIndex].Nodes.Clear();

            foreach (var water in nfw.Waters)
            {
                DrawWater(water);
                if (changed) treeView1.Nodes[0].Nodes[waterIndex].Nodes.Add($"{nfw.Waters.IndexOf(water)}");
            }
        }

        /// <summary>
        /// painting all region location
        /// </summary>
        /// <param name="nfs"></param>
        /// <param name="changed"></param>
        private void PaintingLocation(Nfs nfs, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[respawnIndex].Nodes.Clear();

            foreach (var location in nfs.Respawns)
            {
                DrawLocation(location);
                if (changed) treeView1.Nodes[0].Nodes[respawnIndex].Nodes.Add($"{nfs.Respawns.IndexOf(location)}");
            }
        }

        /// <summary>
        /// Painting all nfp
        /// </summary>
        /// <param name="nfa"></param>
        /// <param name="changed"></param>
        private void PaintingNfp(Nfp nfp, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[nfpIndex].Nodes.Clear();

            foreach (var recorNfp in nfp.Records)
            {
                DrawNfp(recorNfp);
                if (changed) treeView1.Nodes[0].Nodes[nfpIndex].Nodes.Add($"{nfp.Records.IndexOf(recorNfp)}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nfs"></param>
        /// <param name="changed"></param>
        private void PaintingNpcProp(Nfs nfs, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[npcIndex].Nodes.Clear();

            foreach (var prop in nfs.Props)
            {
                DrawNpcProp(prop);
                if (changed) treeView1.Nodes[0].Nodes[npcIndex].Nodes.Add($"{nfs.Props.IndexOf(prop)}");
            }
        }

        /// <summary>
        /// Painting all quest prop
        /// </summary>
        /// <param name="qpf"></param>
        /// <param name="changed"></param>
        private void PaintingQuestProp(Qpf qpf, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[gateIndex].Nodes.Clear();

            foreach (var prop in qpf.Props)
            {
                DrawProp(prop);
                if (changed) treeView1.Nodes[0].Nodes[gateIndex].Nodes.Add($"{qpf.Props.IndexOf(prop)}");
            }
        }

        /// <summary>
        /// painting all region location
        /// </summary>
        /// <param name="nfs"></param>
        /// <param name="changed"></param>
        private void PaintingRegion(Nfc nfc, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[regionIndex].Nodes.Clear();

            foreach (var region in nfc.Region)
            {
                DrawRegion(region);
                if (changed) treeView1.Nodes[0].Nodes[regionIndex].Nodes.Add($"{nfc.Region.IndexOf(region)}");
            }
        }

        /// <summary>
        /// painting segment potencially visible set
        /// </summary>
        /// <param name="nfs"></param>
        /// <param name="changed"></param>
        private void PaintingPSegment(Pvs pvs, bool show, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[pvsSegmentIndex].Nodes.Clear();
            if (show) DrawPVSSegment(pvs, new Pen(template.PVSSegment));

            foreach (var segment in pvs.Segments)
            {
                if (show) DrawPVSSegment(segment);
                if (changed) treeView1.Nodes[0].Nodes[pvsSegmentIndex].Nodes.Add($"{pvs.Segments.IndexOf(segment)}");
            }
        }

        /// <summary>
        /// painting prop potencially visible set
        /// </summary>
        /// <param name="nfs"></param>
        /// <param name="changed"></param>
        private void PaintingPProp(Pvs pvs, bool show, bool changed)
        {
            if (changed) treeView1.Nodes[0].Nodes[pvsPropIndex].Nodes.Clear();
            if (show) DrawPVSSegment(pvs, new Pen(template.PVSProp));

            foreach (var prop in pvs.Props)
            {
                if (show) DrawPVSProp(prop);
                if (changed) treeView1.Nodes[0].Nodes[pvsPropIndex].Nodes.Add($"{pvs.Props.IndexOf(prop)}");
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
            setCurrentElement(DrawElement.NFS01);
        }

        /// <summary>
        /// Component create a new npc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nfs02ModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setCurrentMode(DrawMapMode.POINT);
            setCurrentElement(DrawElement.NFS02);
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
                    break;

                case DrawMapMode.RECTANGLE:
                    mapPictureBox.Cursor = cross;
                    rectangleModeToolStripMenuItem.BackColor = Color.DarkGray;
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
                case DrawElement.NFS01:
                    nfs01ModeToolStripMenuItem.BackColor = Color.DarkGray;
                    break;
                case DrawElement.NFS02:
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
                case DrawElement.NFS01:
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
            
            var pointConvert = MapManager.Instance.GetPointRotate180FlipY(new PointF((First.X / zoom), (First.Y / zoom)));

            switch (currentElement)
            {
                case DrawElement.NFL:
                    MapWorker.Instance.Nfl.Add(new StructLights()
                    {
                        Position = new K3DVector((pointConvert.X * 7.875f), (pointConvert.Y * 7.875f), 0f)
                    });
                    break;
                case DrawElement.NFS02:
                    MapWorker.Instance.Nfs.AddPropScript(new PropScriptInfo()
                    {
                        X = (int)Math.Round(pointConvert.X * 7.875f),
                        Y = (int)Math.Round(pointConvert.Y * 7.875f)
                    });
                    break;
                case DrawElement.QPF:
                    MapWorker.Instance.Qpf.AddLocation(new Prop()
                    {
                        X = (int)Math.Round(pointConvert.X * 7.875f),
                        Y = (int)Math.Round(pointConvert.Y * 7.875f)
                    });
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
            
            var PointB = MapManager.Instance.GetPointRotate180FlipY(new PointF((First.X / zoom), (First.Y / zoom)));
            var PointC = MapManager.Instance.GetPointRotate180FlipY(new PointF((e.X / zoom), (e.Y / zoom)));

            switch (currentElement)
            {
                case DrawElement.NFS01:
                    MapWorker.Instance.Nfs.AddLocation(new Location()
                    {
                        Left = (int)(PointB.X / Global.titleLenght * 7.875f),
                        Top = (int)(PointB.Y / Global.titleLenght * 7.875f),
                        Right = (int)(PointC.X / Global.titleLenght * 7.875f),
                        Bottom = (int)(PointC.Y / Global.titleLenght * 7.875f)
                    });
                    break;

                case DrawElement.NFW:
                    var centerX = (Math.Max(PointB.X, PointC.X) - Math.Min(PointB.X, PointC.X)) / 2 + Math.Min(PointB.X, PointC.X);
                    var centerY = (Math.Max(PointB.Y, PointC.Y) - Math.Min(PointB.Y, PointC.Y)) / 2 + Math.Min(PointB.Y, PointC.Y);
                    var points = new K3DVector[3];
                    
                    MapWorker.Instance.Nfw.AddWater(new Water()
                    {
                        PointA = new K3DVector((PointB.X * 7.875f), (PointB.Y * 7.875f), 0f),
                        PointB = new K3DVector((PointC.X * 7.875f), (PointC.Y * 7.875f), 0f),
                        Center = new K3DVector((centerX * 7.875f), (centerY * 7.875f), 0f)
                    });
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
                var polygon2 = new Polygon2();
                var polygons2 = new List<Polygon2>();
                
                foreach (var point in Polygon)
                {
                    var position = MapManager.Instance.GetPointRotate180FlipY(new PointF((point.X / zoom), (point.Y / zoom)));

                    polygon2.Points.Add(new K2DVector(
                        (int)Math.Round((position.X * 8 * 7.875) / Global.titleLenght),
                        (int)Math.Round((position.Y * 8 * 7.875) / Global.titleLenght)
                    ));
                }

                polygons2.Add(polygon2);

                switch (currentElement)
                {
                    case DrawElement.NFA:
                        MapWorker.Instance.Nfa.AddCollision(polygon2);
                        break;

                    case DrawElement.NFC:

                        var points = new PointF[Polygon.Count];
                        for (int i = 0; i < Polygon.Count; i++)
                        {
                            var position = MapManager.Instance.GetPointRotate180FlipY(new PointF((Polygon[i].X / zoom), (Polygon[i].Y / zoom)));

                            points[i].X = (position.X * 7.875f);
                            points[i].Y = (position.Y * 7.875f);
                        }
                        var center = _2DUtils.GetCenterPolygon(points);

                        MapWorker.Instance.Nfc.AddRegion(new LocationInfo()
                        {
                            Polygons = polygons2,
                            X = center.X,
                            Y = center.Y
                        });
                        break;

                    case DrawElement.NFE:

                        MapWorker.Instance.Nfe.AddEventArea(new EventAreaScript()
                        {
                            Polygons = polygons2
                        });

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
                    mapPictureBox_MouseLeftDown(sender, e);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    mapPictureBox_MouseRightDown(sender, e);
                }
            }
        }

        /// <summary>
        /// Methode used when click left down on the picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapPictureBox_MouseLeftDown(object sender, MouseEventArgs e)
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

        /// <summary>
        /// Methode used when click right down on the picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapPictureBox_MouseRightDown(object sender, MouseEventArgs e)
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

        /// <summary>
        /// Method used whend click up on the picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mapPictureBox_MouseLeftUp(sender, e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                mapPictureBox_MouseRightUp(sender, e);
            }
        }

        /// <summary>
        /// Method used whend click left up on the picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapPictureBox_MouseLeftUp(object sender, MouseEventArgs e)
        {
            switch (currentMode)
            {
                case DrawMapMode.RECTANGLE:
                    DrawRectangle?.Invoke(sender, e);
                    break;
            }

            First = Point.Empty;
        }

        /// <summary>
        /// Method used whend click right up on the picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapPictureBox_MouseRightUp(object sender, MouseEventArgs e)
        {
            First = Point.Empty;
        }

        /// <summary>
        /// Method used when mouse enter or move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var rotate180FlipY = MapManager.Instance.GetPointRotate180FlipY(new PointF((e.X / zoom), (e.Y / zoom)));

            // Coordonate
            Map = new Point((int)Math.Ceiling(rotate180FlipY.X * 7.875), (int)Math.Ceiling(rotate180FlipY.Y * 7.875));
            Game = new Point((Map.X + (MapWorker.Instance.X * 16128)), (Map.Y + (MapWorker.Instance.Y * 16128)));
            Segment = new Point((Map.X / Global.titleLenght / 6), (Map.Y / Global.titleLenght / 6));
            Tile = new Point((Map.X / Global.titleLenght - (6 * Segment.X)), (Map.Y / Global.titleLenght - (6 * Segment.Y)));

            // Segment location
            SegmentF = new PointF((rotate180FlipY.X * 7.875f - (Segment.X * 42 * 6)), (rotate180FlipY.Y * 7.875f - (Segment.Y * 42 * 6)));

            // Number
            SegmentNumber = ((Segment.X) + ((Segment.Y) * 64));
            TileNumber = ((Tile.X) + ((Tile.Y) * 6));

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

            coordonateToolStripStatusLabel.Text = $"{Map.X}, {Map.Y}";
            gameToolStripStatusLabel.Text = $"{Game.X}, {Game.Y}";
            segmentToolStripStatusLabel.Text = $"{Segment.X}, {Segment.Y} ({SegmentNumber})";
            tileToolStripStatusLabel.Text = $"{Tile.X}, {Tile.Y} ({TileNumber})";

            statusStrip1.Refresh();
        }

        /// <summary>
        /// Method used when mouse leave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapPictureBox_MouseLeave(object sender, EventArgs e)
        {
            coordonateToolStripStatusLabel.Text = "";
            gameToolStripStatusLabel.Text = "";
            segmentToolStripStatusLabel.Text = "";
            tileToolStripStatusLabel.Text = "";

            statusStrip1.Refresh();
        }

        #endregion

        /// <summary>
        /// Manage map object
        /// </summary>
        #region TreeView & PropertyGrid control

        private TreeNode previousNode;

        /// <summary>
        /// Method use when template color properties is changed
        /// </summary>
        private void Template_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
            
            Properties.Settings.Default.Collision = template.Collision;
            Properties.Settings.Default.EventArea = template.EventArea;
            Properties.Settings.Default.Light = template.Light;
            Properties.Settings.Default.Nfp = template.Nfp;
            Properties.Settings.Default.NpcProp = template.NpcProp;
            Properties.Settings.Default.Prop = template.Prop;
            Properties.Settings.Default.PVSProp = template.PVSProp;
            Properties.Settings.Default.PVSSegment = template.PVSSegment;
            Properties.Settings.Default.Region = template.Region;
            Properties.Settings.Default.Respawn = template.Respawn;
            Properties.Settings.Default.Water = template.Water;
            Properties.Settings.Default.Save();
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
            
            propertyGrid1.SelectedObject = obj;
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
        /// Method used when property grid value is changed
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
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
        private void propertyGrid1_SelectedObjectsChanged(object sender, EventArgs e)
        {
            var visibility = propertyGrid1.SelectedObject != null;

            propertyGrid1.Visible = visibility;
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
            MapWorker.Instance.Refresh();

            Properties.Settings.Default.Save();
        }

        #endregion

        /// <summary>
        /// Method for context menu strip
        /// </summary>
        #region ContextMenuStrip
        
        /// <summary>
        /// Methos used for cancel polygon
        /// </summary>
        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CancelPolygon?.Invoke(sender, e);
        }

        /// <summary>
        /// Method used for complet polygon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void completedToolStripMenuItem_Click(object sender, EventArgs e)
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
        /// Method used for copy command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, $"/run warp({Game.X}, {Game.Y})");
        }

        /// <summary>
        /// Method used for copy position game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, $"Game position : {Game.X}, {Game.Y}");
        }

        /// <summary>
        /// Method used for copy position map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, $"Map position : {Map.X}, {Map.Y}");
        }

        /// <summary>
        /// Method used for copy position segment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void segmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, $"Segment : {Segment.X}, {Segment.Y} ({SegmentNumber}");
        }

        /// <summary>
        /// Method used for copy position tile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, $"Tile : {Tile.X}, {Tile.Y} {TileNumber}");
        }

        /// <summary>
        /// Method used for remove
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedNode = treeView1.SelectedNode;

            if (selectedNode.Parent.Index == collisionIndex)
            {
                Nfa.Instance.Polygons.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == eventAreaIndex)
            {
                Nfe.Instance.Events.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == lightIndex)
            {
                Nfl.Instance.Lights.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == respawnIndex)
            {
                Nfs.Instance.Respawns.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == nfpIndex)
            {
                Nfp.Instance.Records.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == npcIndex)
            {
                Nfs.Instance.Props.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == pvsSegmentIndex)
            {
                Pvs.Instance.Segments.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == pvsPropIndex)
            {
                Pvs.Instance.Props.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == gateIndex)
            {
                Qpf.Instance.Props.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == waterIndex)
            {
                Nfw.Instance.Waters.RemoveAt(selectedNode.Index);
            }
            else if (selectedNode.Parent.Index == regionIndex)
            {
                Nfc.Instance.Region.RemoveAt(selectedNode.Index);
            }

            MapWorker.Instance.Refresh(true);
            treeView1_AfterSelect(this, new TreeViewEventArgs(selectedNode));
        }

        /// <summary>
        /// Method used for clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
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
        }

        #endregion

        /// <summary>
        /// Method for log
        /// </summary>
        #region Log control

        private Levels currentLevel = Levels.Debug;

        /// <summary>
        /// Event used for collapse/uncollapse log error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (splitContainer4.Panel2Collapsed)
            {
                splitContainer3.SplitterDistance = splitContainer3.Height - 227;
                splitContainer4.Panel2Collapsed = false;
                button1.ForeColor = Color.White;
                LogRtb.ScrollToCaret();
            }
            else
            {
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
                LogRtb.AppendText(txt.Replace(@"\", "/") + "\n");
                LogRtb.ScrollToCaret();
                if (splitContainer4.Panel2Collapsed == true)
                {
                    if (currentLevel < level)
                    {
                        button1.ForeColor = color;
                        currentLevel = level;
                    }
                }
            }
            else
            {
                LogRtb.Invoke(new Action<Levels, string, Color>(InsertLog), level, txt, color);
            }
        }

        /// <summary>
        /// Clear all log
        /// </summary>
        public void ClearLog()
        {
            if (!LogRtb.InvokeRequired)
            {
                LogRtb.Clear();
                LogRtb.ScrollToCaret();
                button1.ForeColor = Color.White;
                currentLevel = Levels.Debug;
            }
            else
            {
                LogRtb.Invoke(new Action(ClearLog));
            }
        }

        #endregion
    }
}
