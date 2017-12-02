using System.Windows.Forms;

namespace MapEditor.Dialog
{
	public partial class Options : Form
	{
		public string RootPath { get; set; }

		/// <summary>
		/// Initialize a new instance of form
		/// </summary>
		/// <param name="path"></param>
		public Options(string path)
		{
			InitializeComponent();

			this.RootPath = path;

			textBox2.DataBindings.Add(new Binding("Text", this, nameof(RootPath), false, DataSourceUpdateMode.OnPropertyChanged));
		}

		/// <summary>
		/// Method used when textbox mouse click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBox2_MouseClick(object sender, MouseEventArgs e)
		{
			var openFolder = new FolderBrowserDialog();
			if (openFolder.ShowDialog() == DialogResult.OK)
			{
				RootPath = openFolder.SelectedPath;
				DialogResult = DialogResult.OK;
			}
		}
	}
}
