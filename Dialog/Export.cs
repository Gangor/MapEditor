using DataCore;
using MapEditor.Modules;
using System.IO;
using System.Windows.Forms;
using System;
using System.Threading.Tasks;

namespace MapEditor.Dialog
{
	public partial class Export : Form
	{
		private Core Core;

		public string DataDirectory { get; set; }
		public string ProjectName;

		/// <summary>
		/// Create instance for import from data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="dataDirectory"></param>
		public Export(ref Core core, string dataDirectory)
		{
			InitializeComponent();
			Shown += (sender, e) => folder_FolderOk(DataDirectory);

			this.Core = core;
			this.DataDirectory = dataDirectory;
			
			textBox.DataBindings.Add(new Binding("Text", this, nameof(DataDirectory)));
		}

		/// <summary>
		/// Method used when mouse click from textbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBox_MouseClick(object sender, MouseEventArgs e)
		{
			var folder = new FolderBrowserDialog();
			folder.SelectedPath = DataDirectory;

			if (folder.ShowDialog() == DialogResult.OK)
			{
				folder_FolderOk(folder.SelectedPath);
			}
		}

		/// <summary>
		/// Method used when folder ok
		/// </summary>
		/// <param name="folder"></param>
		private void folder_FolderOk(string folder)
		{
			if (!File.Exists(Path.Combine(folder, "data.000")))
			{
				button1.Enabled = false;
				return;
			}

			DataDirectory = folder;
			button1.Enabled = true;

			Task.Run(() =>
			{
				Core.Load(Path.Combine(DataDirectory, "data.000"));
			});
		}

		/// <summary>
		/// Method used when confirm button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}
	}
}
