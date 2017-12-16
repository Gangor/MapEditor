using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor.Dialog
{
	public partial class FillBit : Form
	{
		public int Bit01 { get; set; }
		public int Bit02 { get; set; }

		public FillBit(int bit01, int bit02)
		{
			InitializeComponent();

			this.Bit01 = bit01;
			this.Bit02 = bit02;
			
		}


	}
}
