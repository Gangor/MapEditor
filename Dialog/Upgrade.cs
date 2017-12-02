using System.IO;
using System.Windows.Forms;

namespace MapEditor.Dialog
{
	public partial class Upgrade : Form
	{
		/// <summary>
		/// Initialize and show a new instance of form
		/// </summary>
		public Upgrade()
		{
			InitializeComponent();

			if (File.Exists("upgrade.log"))
			{
				richTextBox1.Text = File.ReadAllText("upgrade.log");
				ShowDialog();
			}
		}
	}
}
