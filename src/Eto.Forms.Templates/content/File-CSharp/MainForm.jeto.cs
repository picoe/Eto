#if (UseJeto)
using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Json;

namespace EtoApp._1
{	
	public class MainForm : Form
	{	
		public MainForm()
		{
			JsonReader.Load(this);
		}
#if IsForm

		protected void HandleClickMe(object sender, EventArgs e)
		{
			MessageBox.Show("I was clicked!");
		}

		protected void HandleAbout(object sender, EventArgs e)
		{
			new AboutDialog().ShowDialog(this);
		}

		protected void HandleQuit(object sender, EventArgs e)
		{
			Application.Instance.Quit();
		}
#endif
	}
}
#endif