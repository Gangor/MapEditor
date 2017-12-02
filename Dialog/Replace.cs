using MapEditor.Models;
using MapEditor.Modules;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MapEditor.Dialog
{
	public partial class Replace : Form
	{

		private bool selecting;
		private List<Point> selection;

		private List<string> selectingList = new List<string>();
		private List<string> allList = new List<string>();

		public Replace(List<Point> selection)
		{
			this.selection = selection;
			this.selecting = selection.Any();

			InitializeComponent();
			InitializeTarget();
			InitializeBinding();

			this.groupBox1.Enabled = selecting;
			this.checkBox1.Checked = selecting;
		}

		private void InitializeTarget()
		{
			if (this.selecting)
			{
				foreach (var segment in selection)
				{
					var segmentData = Nfm.Instance.DwTerrainSegment[segment.X, segment.Y];
					if (!selectingList.Any(r => r == segmentData.Texture01FileName))
					{
						selectingList.Add(segmentData.Texture01FileName);
					}
					if (!selectingList.Any(r => r == segmentData.Texture02FileName))
					{
						selectingList.Add(segmentData.Texture02FileName);
					}
					if (!selectingList.Any(r => r == segmentData.Texture03FileName))
					{
						selectingList.Add(segmentData.Texture03FileName);
					}
				}
			}
			foreach (var item in Nfm.Instance.DwTerrainSegment)
			{
				if (!allList.Any(r => r == item.Texture01FileName))
				{
					allList.Add(item.Texture01FileName);
				}
				if (!allList.Any(r => r == item.Texture02FileName))
				{
					allList.Add(item.Texture02FileName);
				}
				if (!allList.Any(r => r == item.Texture03FileName))
				{
					allList.Add(item.Texture03FileName);
				}
			}
		}

		private void InitializeBinding()
		{
			if (selecting)
			{
				this.comboBox1.DataSource = selectingList;
			}
			else
			{
				this.comboBox1.DataSource = allList;
			}
			this.comboBox2.DataSource = CfgManager.Instance.Textures;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.button1.Enabled = (comboBox1.SelectedItem != null);
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.button1.Enabled = (comboBox2.SelectedItem != null);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (this.checkBox1.Checked)
			{
				foreach (var segment in selection)
				{
					var segmentData = Nfm.Instance.DwTerrainSegment[segment.X, segment.Y];

					if (segmentData.Texture01FileName == comboBox1.Text)
					{
						segmentData.Texture01FileName = comboBox2.Text;
					}
					if (segmentData.Texture02FileName == comboBox1.Text)
					{
						segmentData.Texture02FileName = comboBox2.Text;
					}
					if (segmentData.Texture03FileName == comboBox1.Text)
					{
						segmentData.Texture03FileName = comboBox2.Text;
					}
				}
			}
			else
			{
				foreach (var item in Nfm.Instance.DwTerrainSegment)
				{
					if (item.Texture01FileName == comboBox1.Text)
					{
						item.Texture01FileName = comboBox2.Text;
					}
					if (item.Texture02FileName == comboBox1.Text)
					{
						item.Texture02FileName = comboBox2.Text;
					}
					if (item.Texture03FileName == comboBox1.Text)
					{
						item.Texture03FileName = comboBox2.Text;
					}
				}
			}

			this.Close();
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox1.Checked)
			{
				this.comboBox1.DataSource = selectingList;
			}
			else
			{
				this.comboBox1.DataSource = allList;
			}
		}
	}
}
