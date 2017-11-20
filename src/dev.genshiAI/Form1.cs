using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dev.genshiAI
{
	public partial class Form1 : Form
	{
		forms.ChefBotForm chefBotForm;
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			chefBotForm = new forms.ChefBotForm();
			chefBotForm.Show();
		}
	}
}
