using System.Windows.Forms;

namespace MapEditor.Dialog
{
    public partial class Options : Form
    {
        public string RootPath { get; set; }

        public Options(string currentPath)
        {
            InitializeComponent();
            textBox2.Text = currentPath;
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            var openFolder = new FolderBrowserDialog();
            if (openFolder.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFolder.SelectedPath;
                RootPath = openFolder.SelectedPath;
                DialogResult = DialogResult.OK;
            }
        }
    }
}
