using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MapEditor.Dialog
{
	public partial class MapSeparator : Form
	{
		public MapSeparator()
		{
			InitializeComponent();
		}

		private void textBox1_MouseClick(object sender, MouseEventArgs e)
		{
			var openFile = new OpenFileDialog();
			openFile.Filter = "Image|*.bmp;*.jpg;*.jpeg;*.png";

			if (openFile.ShowDialog() == DialogResult.OK)
			{
				Imagetxt.Text = openFile.FileName;
			}
		}

		private void textBox3_MouseClick(object sender, MouseEventArgs e)
		{
			var openFolder = new FolderBrowserDialog();

			if (openFolder.ShowDialog() == DialogResult.OK)
			{
				Outputtxt.Text = openFolder.SelectedPath;
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			if (!File.Exists(Imagetxt.Text))
			{
				MessageBox.Show("File not exist !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			if (!Directory.Exists(Outputtxt.Text))
			{
				MessageBox.Show("Folder not exist !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			try
			{
				var image = Image.FromFile(Imagetxt.Text);
				int partX;
				int partY;
				var partHeight = 128;
				var partWidth = 128;
				var prefix = string.Empty;

				if (radioButton2.Checked)
				{
					partHeight = 256;
					partWidth = 256;
					prefix = "v256_";
				}

				partX = image.Width / partWidth;
				partY = image.Height / partHeight;

				for (int y = 0; y < partY; y++)
				{
					for (int x = 0; x < partX; x++)
					{
						var filename = $"{prefix}{Nametxt.Text}_{y}_{x}{Encodetxt.Text}.jpg";
						var bitmap = new Bitmap(partWidth, partHeight);
						var graphic = Graphics.FromImage(bitmap);

						graphic.DrawImage(image, new Rectangle(0, 0, partWidth, partHeight), new Rectangle(x * partWidth,y * partHeight, partWidth, partHeight), GraphicsUnit.Pixel);
						graphic.Dispose();

						bitmap.Save(Path.Combine(Outputtxt.Text, filename));
					}
				}

				Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(string.Format("MapManager::Load<Exception> -> {0}", exception), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
