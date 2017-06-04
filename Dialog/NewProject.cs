using System.Windows.Forms;

namespace MapEditor.Dialog
{
    public partial class NewProject : Form
    {
        public string Project { get; set; }

        public NewProject()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            Project = textBox1.Text;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.None;
            Close();
        }
    }
}
