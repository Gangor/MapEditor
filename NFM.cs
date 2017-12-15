using MapEditor.Extends;
using MapEditor.Attributes;
using MapEditor.Modules;
using System.ComponentModel;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using MapEditor.Models;
using System.Linq;
using System.Data;
using System.Drawing;
using MapEditor.Dialog;

namespace MapEditor
{
	public partial class NFM : Form
	{
		private NfmManager manager;

		private Bitmap minimap = new Bitmap(236, 205);
		private Bitmap segmentMask = new Bitmap(2048, 2048);
		private Bitmap tileMask = new Bitmap(2048, 2048);
        
		private Bitmap cache;
		private Bitmap map;

		private Graphics graphic;

		#region Constructor

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		/// <param name="manager"></param>
		private NFM(string title)
		{
			manager = MapWorker.Instance.Nfm;
			cache = (Bitmap)MapManager.Instance.Map.Clone();
			map = (Bitmap)MapManager.Instance.Map.Clone();
			graphic = Graphics.FromImage(map);

			InitializeComponent();
			InitializeSubComponent();

			Text = $"nFlavor Map - {title}";
			tabControl_SelectedIndexChanged(this, EventArgs.Empty);

			mapPictureBox.DataBindings.Add(new Binding("Image", map, ""));
			minimapPictureBox.DataBindings.Add(new Binding("Image", minimap, ""));
		}

		/// <summary>
		/// Initialize sub component
		/// </summary>
		private void InitializeSubComponent()
		{
			//
			// contextMenuStrip
			//
			contextMenuStrip1.Renderer = new BrowserMenuRenderer();
			//
			// Menu
			//
			menuStrip1.Renderer = new BrowserMenuRenderer();
			menuStrip2.Renderer = new BrowserMenuRenderer();
			menuStrip3.Renderer = new BrowserMenuRenderer();
			//
			// Datagrid
			//
			PropGrid.DataSource = manager.DwProps;
			grassGrid.DataSource = manager.DwGrass;
			speedGrassGrid.DataSource = manager.DwGrassColony;
			//
			// Map
			//
			mapPictureBox.Cursor = CursorExtends.SizeAll;
			//
			// Minimap
			//
			minimapPictureBox.Cursor = CursorExtends.LeftPtrWatch;
			PointerChanged += MinimapPicture_PointerChanged;
			PointerRemoved += MinimapPicture_PointerRemoved;
			//
			// Property
			//
			propertyGrid1.BrowsableAttributes = new AttributeCollection(new PropertyGridBrowsableAttribute(true));
		}

		/// <summary>
		/// 
		/// </summary>
		public static void ShowDialog(string title)
		{
			var dialog = new NFM(title);
			dialog.ShowDialog();
		}

		#endregion

		/// <summary>
		/// Datagrid control
		/// </summary>
		#region Datagrid

		/// <summary>
		/// Event used when databinding complete for sorting column
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Datagrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			//Add this as an event on DataBindingComplete
			DataGridView dataGridView = sender as DataGridView;

			foreach (DataGridViewColumn column in dataGridView.Columns)
			{
				column.SortMode = DataGridViewColumnSortMode.Automatic;

			}
		}

		/// <summary>
		/// Method used when ctrl+v for past data
		/// </summary>
		private void PropGrid_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control == true && e.KeyCode == Keys.V)
			{
				if (PropGrid.CurrentCell.RowIndex != PropGrid.NewRowIndex) return;

				if (Clipboard.ContainsText())
				{
					var lines = Clipboard.GetText().Split('\n');
					var props = new KProp[lines.Count()];
					var lastRow = PropGrid.NewRowIndex;

					manager.DwProps.RemoveAt(lastRow);

					for (int i = 0; i < props.Count(); i++)
					{
						var column = lines[i].Replace("\r", "").Split('\t');

						try
						{ 
							manager.DwProps.Add(new KProp(column));
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
					}
				}
			}
		}

		/// <summary>
		/// Select prop on data grid
		/// </summary>
		/// <param name="prop"></param>
		private void Datagrid_SearchByText(DataGridView datagrid, string value)
		{
			if (datagrid == null) return;

			var currentRow = (datagrid.CurrentCell == null) ? 0 : datagrid.CurrentCell.RowIndex;
			var currentColumn = (datagrid.CurrentCell == null) ? 0 : datagrid.CurrentCell.ColumnIndex;
			var ColumnIndex = toolStripComboBox1.SelectedIndex;

			if (ColumnIndex == currentColumn) currentRow++;

			datagrid.ClearSelection();

			for (int i = currentRow; i < datagrid.Rows.Count - 1; i++)
			{
				var val = datagrid.Rows[i].Cells[ColumnIndex].Value.ToString();
				if (val.IndexOf(value, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
				{
					datagrid.CurrentCell = datagrid[ColumnIndex, i];
					datagrid.Rows[i].Cells[ColumnIndex].Selected = true;
					datagrid.FirstDisplayedScrollingRowIndex = i;
					datagrid.FirstDisplayedScrollingColumnIndex = ColumnIndex;
					datagrid.Update();
					return;
				}
			}

			MessageBox.Show("No resultat found.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <summary>
		/// Select prop on data grid
		/// </summary>
		/// <param name="prop"></param>
		private void Datagrid_SearchByItem(DataGridView datagrid, object obj)
		{
			if (datagrid == null) return;

			datagrid.ClearSelection();

			for (int i = 0; i < datagrid.Rows.Count - 1; i++)
			{
				if (datagrid.Rows[i].DataBoundItem == obj)
				{
					datagrid.CurrentCell = datagrid[0, i];
					datagrid.Rows[i].Cells[0].Selected = true;
					datagrid.FirstDisplayedScrollingRowIndex = i;
					return;
				}
			}

		}

		/// <summary>
		/// Select prop on data grid
		/// </summary>
		/// <param name="prop"></param>
		private void Datagrid_SearchLastItem(DataGridView datagrid, object obj)
		{
			if (datagrid == null) return;

			datagrid.ClearSelection();

			var index = -1;

			for (int i = 0; i < datagrid.Rows.Count - 1; i++)
			{
				if (datagrid.Rows[i].DataBoundItem == obj) index = i;
			}

			datagrid.CurrentCell = datagrid[0, index];
			datagrid.Rows[index].Cells[0].Selected = true;
			datagrid.FirstDisplayedScrollingRowIndex = index;
		}

		/// <summary>
		/// Method used when cell context menu need
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PropGrid_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
		{
			PropGrid.ClearSelection();

			if (e.RowIndex != -1)
			{
				PropGrid.CurrentCell = PropGrid[0, e.RowIndex];
				PropGrid.Rows[e.RowIndex].Cells[0].Selected = true;
				e.ContextMenuStrip = contextMenuStrip2;
			}
		}

		/// <summary>
		/// Method used when cell context menu need
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void grassGrid_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
		{
			grassGrid.ClearSelection();

			if (e.RowIndex != -1)
			{
				grassGrid.CurrentCell = grassGrid[0, e.RowIndex];
				grassGrid.Rows[e.RowIndex].Cells[0].Selected = true;
				e.ContextMenuStrip = contextMenuStrip2;
			}
		}

		#endregion

		/// <summary>
		/// Map control method
		/// </summary>
		#region Minimap zoom control

		private float minimapZoom = 1f;

		/// <summary>
		/// Method for zoom plus
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void minimapZoomLessButton_Click(object sender, EventArgs e)
		{
			if (minimapZoom != zoomMin)
			{
				minimapZoom -= zoomIncrement;
				zoomMinimapApply();
			}
		}

		/// <summary>
		/// Method for zoom less
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void minimapZoomPlusButton_Click(object sender, EventArgs e)
		{
			if (minimapZoom <= zoomMax)
			{
				minimapZoom += zoomIncrement;
				zoomMinimapApply();
			}
		}

		/// <summary>
		/// Change zoom of the map
		/// </summary>
		private void zoomMinimapApply()
		{
			minimapZoomLessButton.BackColor = minimapZoom < zoomNormal ? Color.DarkGray : Color.Transparent;
			minimapZoomPlusButton.BackColor = minimapZoom > zoomNormal ? Color.DarkGray : Color.Transparent;
			minimapZoomLessButton.Enabled = minimapZoom != zoomMin ? true : false;
			minimapZoomPlusButton.Enabled = minimapZoom != zoomMax ? true : false;

			MinimapPicture_PointerChanged(this, new EventArgs());
		}

		#endregion

		/// <summary>
		/// Map control method
		/// </summary>
		#region Terrain main zoom control

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
		private void zoomPlusToolStripMenuItem_Click(object sender, EventArgs e)
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
			zoomPlusToolStripMenuItem.BackColor = zoom > zoomNormal ? Color.DarkGray : Color.Transparent;
			zoomLessToolStripMenuItem.Enabled = zoom != zoomMin ? true : false;
			zoomPlusToolStripMenuItem.Enabled = zoom != zoomMax ? true : false;
			zoomRestoreToolStripMenuItem.Enabled = zoom != zoomNormal ? true : false;

			zoomToolStripStatusLabel.Text = $"x{zoom}";
			statusStrip.Refresh();
		}

		#endregion

		/// <summary>
		/// Tabcontrol method
		/// </summary>
		#region TabControl & Selection

		/// <summary>
		/// Draw item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();

			TabPage CurrentTab = tabControl.TabPages[e.Index];
			Rectangle ItemRect = tabControl.GetTabRect(e.Index);
			SolidBrush FillBrush = new SolidBrush(Color.FromArgb(45, 45, 48));
			SolidBrush TextBrush = new SolidBrush(Color.White);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;

			//If we are currently painting the Selected TabItem we'll
			//change the brush colors and inflate the rectangle.
			if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
			{
				FillBrush.Color = Color.FromArgb(28, 28, 28);
				TextBrush.Color = Color.White;
				ItemRect.Inflate(2, 2);
			}

			//Set up rotation for left and right aligned tabs
			if (tabControl.Alignment == TabAlignment.Left || tabControl.Alignment == TabAlignment.Right)
			{
				float RotateAngle = 90;
				if (tabControl.Alignment == TabAlignment.Left)
					RotateAngle = 270;
				PointF cp = new PointF(ItemRect.Left + (ItemRect.Width / 2), ItemRect.Top + (ItemRect.Height / 2));
				e.Graphics.TranslateTransform(cp.X, cp.Y);
				e.Graphics.RotateTransform(RotateAngle);
				ItemRect = new Rectangle(-(ItemRect.Height / 2), -(ItemRect.Width / 2), ItemRect.Height, ItemRect.Width);
			}

			//Next we'll paint the TabItem with our Fill Brush
			e.Graphics.FillRectangle(FillBrush, ItemRect);

			//Now draw the text.
			e.Graphics.DrawString(CurrentTab.Text, e.Font, TextBrush, (RectangleF)ItemRect, sf);

			//Reset any Graphics rotation
			e.Graphics.ResetTransform();

			//Finally, we should Dispose of our brushes.
			FillBrush.Dispose();
			TextBrush.Dispose();

		}

		/// <summary>
		/// Tab page index changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Context menu
			grassToolStripMenuItem1.Visible = true;
			propToolStripMenuItem.Visible = true;

			// PropertyGrid
			propertyGrid1.SelectedObject = null;

			// Minimap
			splitContainer2.Panel1Collapsed = true;

			// Search menu
			toolStripTextBox1.Text = "";
			menuStrip2.Visible = true;

			// Terrain menu
			menuStrip1.Visible = zoomToolStripStatusLabel.Visible = toolStripStatusLabel3.Visible = menuStrip3.Visible = false;

			switch (tabControl.SelectedTab.Name)
			{
				case nameof(grassTab):
					toolStripComboBox1.ComboBox.DataSource = grassGrid.Columns.Cast<DataGridViewColumn>().Select(r => r.HeaderText).ToArray();
					grassGrid_CurrentCellChanged(sender, e);
					splitContainer2.Panel1Collapsed = false;
					propToolStripMenuItem.Visible = false;
					break;

				case nameof(propTab):
					toolStripComboBox1.ComboBox.DataSource = PropGrid.Columns.Cast<DataGridViewColumn>().Select(r => r.HeaderText).ToArray();
					PropGrid_CurrentCellChanged(sender, e);
					splitContainer2.Panel1Collapsed = false;
					grassToolStripMenuItem1.Visible = false;
					break;

				case nameof(terrainTab):
					toolStripComboBox1.ComboBox.DataSource = null;
					propertyGrid1.SelectedObject = manager.MapProperties;
					menuStrip1.Visible = zoomToolStripStatusLabel.Visible = toolStripStatusLabel3.Visible = menuStrip3.Visible = true;
					menuStrip2.Visible = false;
					break;

				case nameof(speedGrassTab):
					toolStripComboBox1.ComboBox.DataSource = speedGrassGrid.Columns.Cast<DataGridViewColumn>().Select(r => r.HeaderText).ToArray();
					speedGrassGrid_CurrentCellChanged(sender, e);
					break;
			}
		}


		/// <summary>
		/// Select speed grass colony
		/// </summary>
		#region Speed Grass Colony

		/// <summary>
		/// Current cell changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void speedGrassGrid_CurrentCellChanged(object sender, System.EventArgs e)
		{
			if (speedGrassGrid.CurrentCell == null) return;

			propertyGrid1.SelectedObject = (speedGrassGrid.SelectedCells.Count == 1) ? 
				speedGrassGrid.Rows[speedGrassGrid.CurrentCell.RowIndex].DataBoundItem
				: null;
		}

		#endregion

		/// <summary>
		/// Select prop
		/// </summary>
		#region Prop

		/// <summary>
		/// Current cell changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PropGrid_CurrentCellChanged(object sender, EventArgs e)
		{
			try
			{
				var prop = (KProp)PropGrid.Rows[PropGrid.CurrentCell.RowIndex].DataBoundItem;
				segmentId = prop.SegmentId;
				position = new PointF[] { new PointF(prop.X, prop.Y) };

				propertyGrid1.SelectedObject = prop;
				PointerChanged?.Invoke(this, new EventArgs());
			}
			catch (Exception)
			{
				PointerRemoved?.Invoke(this, new EventArgs());
			}
		}

		#endregion

		/// <summary>
		/// Select grass
		/// </summary>
		#region grass

		/// <summary>
		/// Current cell changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void grassGrid_CurrentCellChanged(object sender, System.EventArgs e)
		{
			try
			{
				var grass = (KGrass)grassGrid.Rows[grassGrid.CurrentCell.RowIndex].DataBoundItem;
				segmentId = grass.SegmentId;

				position = new PointF[grass.Props.Count];
				for (int i = 0; i < grass.Props.Count; i++)
				{
					position[i] = new PointF(grass.Props[i].X, grass.Props[i].Y);
				}

				propertyGrid1.SelectedObject = grass;
				PointerChanged?.Invoke(this, new EventArgs());
			}
			catch (Exception)
			{
				PointerRemoved?.Invoke(this, new EventArgs());
			}
		}

		#endregion

		/// <summary>
		/// Select segment
		/// </summary>
		#region Segment

		/// <summary>
		/// Events used when selected segment
		/// </summary>
		/// <param name="segmentX"></param>
		/// <param name="segmentY"></param>
		private void mapPicturedBox_SelectedSegment(int index, System.Drawing.Point segment)
		{
			var nX1 = (segment.X) * Global.tileLenght * 6 / 7.875f;
			var nY1 = (segment.Y) * Global.tileLenght * 6 / 7.875f;
			var nX2 = (segment.X + 1) * Global.tileLenght * 6 / 7.875f;
			var nY2 = (segment.Y + 1) * Global.tileLenght * 6 / 7.875f;

			var pointC = _2DUtils.GetPointRotate180FlipY(new PointF(nX1, nY1));
			var pointB = _2DUtils.GetPointRotate180FlipY(new PointF(nX2, nY2));
			var pointA = new PointF(pointC.X, pointB.Y);
			var pointD = new PointF(pointB.X, pointC.Y);

			graphic.DrawLine(Pens.Red, pointA, pointB);
			graphic.DrawLine(Pens.Red, pointB, pointD);
			graphic.DrawLine(Pens.Red, pointD, pointC);
			graphic.DrawLine(Pens.Red, pointC, pointA);

			graphic.DrawString($"{index}", 
				new Font("Arial", 8, FontStyle.Regular), Brushes.Red, 
				RectangleF.FromLTRB(pointA.X, pointA.Y, pointD.X, pointD.Y), 
				new StringFormat() {
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			});
		}

		#endregion

		/// <summary>
		/// Select tile
		/// </summary>
		#region Tile

		/// <summary>
		/// Events used when selected tile
		/// </summary>
		/// <param name="segmentX"></param>
		/// <param name="segmentY"></param>
		/// <param name="tileX"></param>
		/// <param name="tileY"></param>
		private void mapPicturedBox_SelectedTile(int index , System.Drawing.Point segment, System.Drawing.Point tile)
		{
			var nX1 = ((segment.X * Global.tileLenght * 6 / 7.875f) + ((tile.X * Global.tileLenght / 7.875f)));
			var nY1 = ((segment.Y * Global.tileLenght * 6 / 7.875f) + ((tile.Y * Global.tileLenght / 7.875f)));
			var nX2 = ((segment.X * Global.tileLenght * 6 / 7.875f) + ((tile.X + 1) * Global.tileLenght / 7.875f));
			var nY2 = ((segment.Y * Global.tileLenght * 6 / 7.875f) + ((tile.Y + 1) * Global.tileLenght / 7.875f));

			var pointC = _2DUtils.GetPointRotate180FlipY(new PointF(nX1, nY1));
			var pointB = _2DUtils.GetPointRotate180FlipY(new PointF(nX2, nY2));
			var pointA = new PointF(pointC.X, pointB.Y);
			var pointD = new PointF(pointB.X, pointC.Y);

			graphic.DrawLine(Pens.Red, pointA, pointB);
			graphic.DrawLine(Pens.Red, pointB, pointD);
			graphic.DrawLine(Pens.Red, pointD, pointC);
			graphic.DrawLine(Pens.Red, pointC, pointA);
		}

		#endregion

		#endregion

		/// <summary>
		/// Picture manager method
		/// </summary>
		#region Map picture
        
		/// <summary>
		/// Stock the points
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
		/// Stock the selection of objet
		/// </summary>
		private List<Point> Segments = new List<System.Drawing.Point>();
		private List<Tile> Tiles = new List<Tile>();

		/// <summary>
		/// Event used when mouse down
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapPictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			MapPictureBox_MouseMove(sender, e);

			if (e.Button == MouseButtons.Left)
			{
				switch (CurrentMode)
				{
					case Mode.MOVE:
						First = e.Location;
						break;

					case Mode.SEGMENT:

						if (Segment.X > 63 || Segment.Y > 63) return;

						RestoreSegment();

						#region Control

						if ((ModifierKeys & Keys.Control) != Keys.Control && (ModifierKeys & Keys.Shift) != Keys.Shift)
						{
							Segments.Clear();
						}

						#endregion

						#region Shift

						if ((ModifierKeys & Keys.Shift) == Keys.Shift)
						{
							if (Segments.Any())
							{
								var last = Segments.Last();
								var lessSegment = last.Y <= Segment.Y && last.X < Segment.X ? last : Segment;
								var largeSegment = last.Y >= Segment.Y && last.X > Segment.X ? last : Segment;

								// Cursor coordonate
								var cursorX = lessSegment.X;
								var cursorY = lessSegment.Y;

								// Max coordonate
								var maxX = largeSegment.X;
								var maxY = largeSegment.Y;

								// increment/decrement coordonate
								if (last.Y <= Segment.Y && last.X < Segment.X) { cursorX++; }
								else if (last.Y >= Segment.Y && last.X > Segment.X) { maxX--; }

								for (var y = cursorY; y < 64; y++)
								{
									cursorY = y;

									for (var x = cursorX; x < 64; x++)
									{
										cursorX = x;

										var info = new Point(x, y);
										if (!Segments.Any(r => r == info))
											Segments.Add(info);
										else
											Segments.Remove(info);

										if (cursorX == maxX && cursorY == maxY) break;
									}

									if (cursorX == maxX && cursorY == maxY) break;
									if (cursorX == 63) cursorX = 0;
								}
							}
						}
						else
						{
							if (!Segments.Any(r => r == Segment))
								Segments.Add(Segment);
							else
								Segments.Remove(Segment);
						}

						#endregion

						var kSegment = new KSegment[Segments.Count];
						for (int i = 0; i < Segments.Count; i++)
						{
							kSegment[i] = manager.DwTerrainSegment[Segments[i].X, Segments[i].Y];
							mapPicturedBox_SelectedSegment(i, Segments[i]);
						}
                        
						propertyGrid1.SelectedObject = kSegment;

						break;

					case Mode.TILE:

						if (Segment.X > 63 || Segment.Y > 63) return;
						if (Tile.X > 5 || Tile.Y > 5) return;

						RestoreTile();

						#region Control

						if ((ModifierKeys & Keys.Control) != Keys.Control && (ModifierKeys & Keys.Shift) != Keys.Shift)
						{
							Tiles.Clear();
						}

						#endregion

						#region Shift

						if ((ModifierKeys & Keys.Shift) == Keys.Shift)
						{
							if (Tiles.Any())
							{
								var lastSegment = Tiles.Last().segment;
								var lastTile = Tiles.Last().tile;
								//
								var segmentNumber = (lastSegment.Y * 64) + lastSegment.X;
								var tileNumber = (lastTile.Y * 6) + lastTile.X;
								//
								var lessSegment = segmentNumber < SegmentNumber || (segmentNumber == SegmentNumber && tileNumber < TileNumber) ? lastSegment : Segment;
								var largeSegment = segmentNumber > SegmentNumber || (segmentNumber == SegmentNumber && tileNumber > TileNumber) ? lastSegment : Segment;
								var lessTile = segmentNumber < SegmentNumber || (segmentNumber == SegmentNumber && tileNumber < TileNumber) ? lastTile : Tile;
								var largeTile = segmentNumber > SegmentNumber || (segmentNumber == SegmentNumber && tileNumber > TileNumber) ? lastTile : Tile;

								// Cursor coordonate
								var cursorX = (lessSegment.X * 6) + lessTile.X;
								var cursorY = (lessSegment.Y * 6) + lessTile.Y;

								// Max tile coordonate
								var maxX = (largeSegment.X * 6) + largeTile.X;
								var maxY = (largeSegment.Y * 6) + largeTile.Y;

								// increment/decrement coordonate
								if (segmentNumber < SegmentNumber || (segmentNumber == SegmentNumber && tileNumber < TileNumber)) { cursorX++; }
								else if (segmentNumber > SegmentNumber || (segmentNumber == SegmentNumber && tileNumber > TileNumber)) { maxX--; }

								for (int y = cursorY; y < 384; y++)
								{
									cursorY = y;

									for (int x = cursorX; x < 384; x++)
									{
										cursorX = x;

										var info = new Tile()
										{
											segment = new Point(cursorX / 6, cursorY / 6),
											tile = new Point(cursorX % 6, cursorY % 6)
										};

										var tile = Tiles.SingleOrDefault(r => r.segment == info.segment && r.tile == info.tile);
										if (tile == null)
											Tiles.Add(info);
										else
											Tiles.Remove(tile);

										if (cursorX == maxX && cursorY == maxY) break;
									}
									
									if (cursorX == maxX && cursorY == maxY) break;
									if (cursorX == 383) cursorX = 0;
								}
							}
						}
						else
						{
							var info = new Tile() { segment = Segment, tile = Tile };
							var tile = Tiles.SingleOrDefault(r => r.segment == Segment && r.tile == Tile);
							if (tile == null)
								Tiles.Add(info);
							else
								Tiles.Remove(tile);
						}

						#endregion

						var kVectex = new KVertex[Tiles.Count];
						for (int i = 0; i < Tiles.Count; i++)
						{
							kVectex[i] = manager.DwTerrainSegment[Tiles[i].segment.X, Tiles[i].segment.Y].HsVector[Tiles[i].tile.X, Tiles[i].tile.Y];
							mapPicturedBox_SelectedTile(i, Tiles[i].segment, Tiles[i].tile);
						}

						propertyGrid1.SelectedObject = kVectex;

						break;
				}
			}

			mapPictureBox.Refresh();
		}

		/// <summary>
		/// Event used when mouse move & enter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapPictureBox_MouseMove(object sender, MouseEventArgs e)
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

			if (CurrentMode == Mode.MOVE && !First.IsEmpty)
			{
				int newX = mapPictureBox.Location.X + (e.Location.X - First.X);
				int newY = mapPictureBox.Location.Y + (e.Location.Y - First.Y);

				flowLayoutPanel1.AutoScrollPosition = new Point(flowLayoutPanel1.AutoScrollOffset.X - newX, flowLayoutPanel1.AutoScrollOffset.Y - newY);
				mapPictureBox.Refresh();
			}

			lbMapCoordinate.Text = $"{Map.X}, {Map.Y}";
			lbGameCoordinate.Text = $"{Game.X}, {Game.Y}";
			lbSegmentCoordinate.Text = $"{Segment.X}, {Segment.Y} ({SegmentNumber})";
			lbTileCoodinate.Text = $"{Tile.X}, {Tile.Y} ({TileNumber})";

			statusStrip.Refresh();
		}

		/// <summary>
		/// Event used when mouse up
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapPictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			First = Point.Empty;
		}

		/// <summary>
		/// Event used when mouse leave picture
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapPictureBox_MouseLeave(object sender, EventArgs e)
		{
			lbMapCoordinate.Text = "";
			lbGameCoordinate.Text = "";
			lbSegmentCoordinate.Text = "";
			lbTileCoodinate.Text = "";
		}

		#endregion

		/// <summary>
		/// Mode manage method
		/// </summary>
		#region Mode picture

		private static Cursor move = CursorExtends.SizeAll;
		private static Cursor pen = CursorExtends.Pencil;

		private bool initialized = false;
		private Mode CurrentMode = 0;

		/// <summary>
		/// Event used for move mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void moveModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(Mode.MOVE);
		}

		/// <summary>
		/// Event used for segment mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void segmentModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(Mode.SEGMENT);
		}

		/// <summary>
		/// Event used for tile mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tileModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurrentMode(Mode.TILE);
		}

		/// <summary>
		/// Change the current mode
		/// </summary>
		/// <param name="mode"></param>
		private void setCurrentMode(Mode mode)
		{
			First = System.Drawing.Point.Empty;

			if (!initialized)
			{
				InitializeSegment();
				InitializeTile();

				initialized = true;
			}

			Segments.Clear();
			Tiles.Clear();

			moveModeToolStripMenuItem.BackColor = Color.Transparent;
			segmentModeToolStripMenuItem.BackColor = Color.Transparent;
			tileModeToolStripMenuItem.BackColor = Color.Transparent;

			switch (mode)
			{
				case Mode.MOVE:
					mapPictureBox.Cursor = move;
					moveModeToolStripMenuItem.BackColor = Color.DarkGray;
					propertyGrid1.SelectedObject = manager.MapProperties;
					break;

				case Mode.SEGMENT:
					mapPictureBox.Cursor = pen;
					segmentModeToolStripMenuItem.BackColor = Color.DarkGray;
					propertyGrid1.SelectedObject = null;
					RestoreSegment();
					break;

				case Mode.TILE:
					mapPictureBox.Cursor = pen;
					tileModeToolStripMenuItem.BackColor = Color.DarkGray;
					propertyGrid1.SelectedObject = null;
					RestoreTile();
					break;
			}

			CurrentMode = mode;
			tabControl.SelectedTab = terrainTab;
			mapPictureBox.Refresh();
		}
        
		/// <summary>
		/// Initialize segment
		/// </summary>
		private void InitializeSegment()
		{
			segmentMask = new Bitmap(2048, 2048);

			using (Graphics g = Graphics.FromImage(segmentMask))
			{
				g.Clear(Color.Transparent);
                
				for (int segmentY = 0; segmentY < 64; segmentY++)
					for (int segmentX = 0; segmentX < 64; segmentX++)
					{
						var x1 = ((segmentX / 7.875f * Global.tileLenght * 6));
						var y1 = ((segmentY / 7.875f * Global.tileLenght * 6));
						//
						var x2 = (((segmentX + 1) / 7.875f * Global.tileLenght * 6));
						var y2 = (((segmentY + 1) / 7.875f * Global.tileLenght * 6));
						//
						var pointa = _2DUtils.GetPointRotate180FlipY(new PointF(x1, y1));
						var pointc = _2DUtils.GetPointRotate180FlipY(new PointF(x2, y2));

						g.DrawLine(Pens.White, pointa, new PointF(pointa.X, pointc.Y));
						g.DrawLine(Pens.White, new PointF(pointa.X, pointc.Y), pointc);
						g.DrawLine(Pens.White, pointc, new PointF(pointc.X, pointa.Y));
						g.DrawLine(Pens.White, new PointF(pointa.X, pointc.Y), pointa);
                        
					}
			}
		}

		/// <summary>
		/// Initialize segment
		/// </summary>
		private void InitializeTile()
		{
			tileMask = new Bitmap(2048, 2048);

			using (Graphics g = Graphics.FromImage(tileMask))
			{
				g.Clear(Color.Transparent);

				var tile = 0;
				for (int titleY = 0; titleY < 384; titleY++)
					for (int titleX = 0; titleX < 384; titleX++)
					{
						var x1 = (int)Math.Round(((titleX * Global.tileLenght) / 7.875f));
						var y1 = (int)Math.Round(((titleY * Global.tileLenght) / 7.875f));
						//
						var x2 = (int)Math.Round((((titleX + 1) * Global.tileLenght) / 7.875f));
						var y2 = (int)Math.Round((((titleY + 1) * Global.tileLenght) / 7.875f));
						//
						var pointa = _2DUtils.GetPointRotate180FlipY(new PointF(x1, y1));
						var pointc = _2DUtils.GetPointRotate180FlipY(new PointF(x2, y2));

						g.DrawLine(Pens.White, pointa, new PointF(pointa.X, pointc.Y));
						g.DrawLine(Pens.White, new PointF(pointa.X, pointc.Y), pointc);
						g.DrawLine(Pens.White, pointc, new PointF(pointc.X, pointa.Y));
						g.DrawLine(Pens.White, new PointF(pointa.X, pointc.Y), pointa);

						tile++;
					}
			}
		}

		/// <summary>
		/// Restore segment mask
		/// </summary>
		private void RestoreSegment()
		{
			graphic.DrawImage(cache, 0, 0, cache.Width, cache.Height);
			graphic.DrawImage(segmentMask, 0, 0, segmentMask.Width, segmentMask.Height);
		}

		/// <summary>
		/// Restore tile mask
		/// </summary>
		private void RestoreTile()
		{
			graphic.DrawImage(cache, 0, 0, cache.Width, cache.Height);
			graphic.DrawImage(tileMask, 0, 0, tileMask.Width, tileMask.Height);
		}

		#endregion

		/// <summary>
		/// Method for context menu strip
		/// </summary>
		#region ContextMenuStrip

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
		/// Method used for adding a grass
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void grassToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (tabControl.SelectedTab == grassTab)
			{
				if (grassGrid.CurrentCell != null && grassGrid.SelectedCells.Count == 1)
					goto ADDING;
				else
					goto CREATE;
			}

			CREATE:
			{
				var grass = new KGrass();
				grass.SegmentId = SegmentNumber;
				grass.Props.Add(new KGrassProp { X = SegmentF.X, Y = SegmentF.Y });

				manager.DwGrass.Add(grass);
				manager.DwProps.RaiseListChangedEvents = true;

				Datagrid_SearchByItem(grassGrid, grass);
				tabControl.SelectedTab = grassTab;
				return;
			}

			ADDING:
			{
				var grass = (KGrass)grassGrid.Rows[grassGrid.CurrentCell.RowIndex].DataBoundItem;

				if (grass.SegmentId != SegmentNumber) goto CREATE;

				var kProp = new KGrassProp();
				kProp.X = SegmentF.X;
				kProp.Y = SegmentF.Y;
				grass.Props.Add(kProp);

				grassGrid_CurrentCellChanged(this, e);
				return;
			}
		}

		/// <summary>
		/// Method used for adding a prop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void propToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var prop = new KProp();
			prop.SegmentId = SegmentNumber;
			prop.X = SegmentF.X;
			prop.Y = SegmentF.Y;

			manager.DwProps.Add(prop);
			manager.DwProps.RaiseListChangedEvents = true;

			tabControl.SelectedTab = propTab;
			Datagrid_SearchByItem(PropGrid, prop);
		}

		/// <summary>
		/// Method used for duplicated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch(tabControl.SelectedTab.Name)
			{
				case nameof(propTab):
					if (PropGrid.CurrentCell != null)
					{
						var prop = (KProp)PropGrid.Rows[PropGrid.CurrentCell.RowIndex].DataBoundItem;
						if (prop != null)
						{
							manager.DwProps.Add(prop.Clone());
							manager.DwProps.RaiseListChangedEvents = true;

							Datagrid_SearchLastItem(PropGrid, prop);
						}
					}
					break;

				case nameof(grassTab):
					if (grassGrid.CurrentCell != null && grassGrid.CurrentCell.RowIndex != -1)
					{
						var grass = (KGrass)grassGrid.Rows[grassGrid.CurrentCell.RowIndex].DataBoundItem;
						if (grass != null)
						{
							manager.DwGrass.Add(grass.Clone());
							manager.DwGrass.RaiseListChangedEvents = true;

							Datagrid_SearchLastItem(grassGrid, grass);
						}
					}
					break;
			}
		}

		#endregion

		/// <summary>
		///  Method for toolstrip
		/// </summary>
		#region MenuToolstrip
			
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Replace_Click(object sender, EventArgs e)
		{
			var dialog = new Replace(Segments);
			dialog.ShowDialog();

			propertyGrid1.Refresh();
		}

		#endregion

		/// <summary>
		/// Method for search 
		/// </summary>
		#region Search control

		/// <summary>
		/// Search when key enter down
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				ToolStripMenuItem1_Click(sender, new EventArgs());

				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		/// <summary>
		/// Search when buttton confirmed 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (toolStripTextBox1.Text == string.Empty) return;

			switch (tabControl.SelectedTab.Name)
			{
				case nameof(grassTab):
					Datagrid_SearchByText(grassGrid, toolStripTextBox1.Text);
					break;

				case nameof(propTab):
					Datagrid_SearchByText(PropGrid, toolStripTextBox1.Text);
					break;

				case nameof(speedGrassGrid):
					Datagrid_SearchByText(speedGrassGrid, toolStripTextBox1.Text);
					break;
			}
		}

		#endregion

		/// <summary>
		///  Method for minimap
		/// </summary>
		#region Minimap

		private int segmentId = 0;
        
		private PointF[] position = null;
		private PointF normal = PointF.Empty;
		private PointF angleLT = PointF.Empty;

		public event EventHandler<EventArgs> PointerChanged;
		public event EventHandler<EventArgs> PointerRemoved;

		/// <summary>
		/// Event used when mouse move
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MinimapPictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (angleLT != PointF.Empty)
			{
				Map = _2DUtils.AdjustPoint(e.Location, zoom, false, false);

				//
				// Game point
				//
				var mapX = Map.X + MapWorker.Instance.X * 16128;
				var mapY = Map.Y + MapWorker.Instance.Y * 16128;

				Game = new System.Drawing.Point(mapX, mapY);

				//
				// Segment point
				//
				var segmentX = Math.Min(Map.X / Global.tileLenght / 6, 63);
				var segmentY = Math.Min(Map.Y / Global.tileLenght / 6, 63);

				Segment = new System.Drawing.Point(segmentX, segmentY);
				SegmentNumber = Segment.X + Segment.Y * 64;

				//
				// Tile point
				//
				var tileX = Math.Min(Map.X / Global.tileLenght - 6 * Segment.X, 5);
				var tileY = Math.Min(Map.Y / Global.tileLenght - 6 * Segment.Y, 5);

				Tile = new System.Drawing.Point(tileX, tileY);
				TileNumber = Tile.X + Tile.Y * 6;

				//
				// Segment location
				//
				var segmentFX = Map.X - Segment.X * 42 * 6;
				var segmentFY = Map.Y - Segment.Y * 42 * 6;

				SegmentF = new PointF(segmentFX, segmentFY);

				lbMapCoordinate.Text = $"{Map.X}, {Map.Y}";
				lbGameCoordinate.Text = $"{Game.X}, {Game.Y}";
				lbSegmentCoordinate.Text = $"{Segment.X}, {Segment.Y} ({SegmentNumber})";
				lbTileCoodinate.Text = $"{Tile.X}, {Tile.Y} ({TileNumber})";

				statusStrip.Refresh();
			}
		}

		/// <summary>
		/// Event used when pointer changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void MinimapPicture_PointerChanged(object sender, EventArgs e)
		{
			if (position == null) return;

			var segmentX = segmentId % 64;
			var segmentY = segmentId / 64;

			using (var graphic = Graphics.FromImage(minimap))
			{
				var Height = minimapPictureBox.Height / minimapZoom;
				var Width = minimapPictureBox.Width / minimapZoom;
				var Max = new PointF(cache.Width - Width, cache.Height - Height);

				normal = _2DUtils.GetPointRotate180FlipY(new PointF(((segmentX * Global.tileLenght * 6 + position.FirstOrDefault().X) / 7.875f), ((segmentY * Global.tileLenght * 6 + position.FirstOrDefault().Y) / 7.875f)));

				var X = normal.X - (Width / 2);
				var Y = normal.Y - (Height / 2);

				if (X < 0) X = 0;
				if (Y < 0) Y = 0;
				if (X > Max.X) X = Max.X;
				if (Y > Max.Y) Y = Max.Y;

				angleLT = new PointF(X, Y);

				graphic.DrawImage(cache, new RectangleF(0, 0, minimap.Width, minimap.Height), new RectangleF(X, Y, Width, Height), GraphicsUnit.Pixel);
                    
				for (int i = 0; i < position.Length; i++)
				{
					var point = _2DUtils.GetPointRotate180FlipY(new PointF(((segmentX * Global.tileLenght * 6 + position[i].X) / 7.875f), ((segmentY * Global.tileLenght * 6 + position[i].Y) / 7.875f)));
					var pointX = (point.X - X) * minimapZoom;
					var pointY = (point.Y - Y) * minimapZoom;

					if (pointX < 0) pointX = 0;
					if (pointY < 0) pointY = 0;
					
					graphic.DrawLine(Pens.Red, pointX - .5f, pointY - .5f, pointX + .5f, pointY + .5f);

					if (position.Count() == 1)
					{
						graphic.DrawRectangles(Pens.Red, new RectangleF[] { new RectangleF((pointX - 2), (pointY - 2), 4, 4) });
						graphic.DrawLine(Pens.Red, new PointF(pointX - 10, pointY), new PointF(pointX + 10, pointY));
						graphic.DrawLine(Pens.Red, new PointF(pointX, pointY - 10), new PointF(pointX, pointY + 10));
					}
				}
			}

			minimapZoomPlusButton.Enabled = minimapZoomLessButton.Enabled = true;
			minimapPictureBox.ContextMenuStrip = contextMenuStrip1;
			minimapPictureBox.Refresh();
		}

		/// <summary>
		/// Event used when pointer removed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void MinimapPicture_PointerRemoved(object sender, EventArgs e)
		{
			angleLT = PointF.Empty;
			segmentId = 0;
			position = null;

			using (var graphic = Graphics.FromImage(minimap))
			{
				graphic.DrawImage(cache, new Rectangle(0, 0, minimap.Width, minimap.Height), new Rectangle(0, 0, cache.Width, cache.Height), GraphicsUnit.Pixel);
			}

			minimapZoomPlusButton.Enabled = minimapZoomLessButton.Enabled = false;
			minimapPictureBox.ContextMenuStrip = null;
			minimapPictureBox.Refresh();
		}

		#endregion

		public enum Mode
		{
			MOVE,
			SEGMENT,
			TILE
		}
	}
}
