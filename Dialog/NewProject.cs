using System.Windows.Forms;

namespace MapEditor.Dialog
{
    public partial class NewProject : Form
    {
        public string ProjectName { get; set; }

		/// <summary>
		/// Initialise a new instance of form
		/// </summary>
        public NewProject()
        {
            InitializeComponent();

			textbox.DataBindings.Add(new Binding("Text", this, nameof(ProjectName)));
		}

		/// <summary>
		/// Method used for confirm
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void button1_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
