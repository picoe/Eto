#if (UseJeto)
using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Json;

namespace EtoApp
{	
	public class MainForm : Form
	{	
		public MainForm()
		{
			JsonReader.Load(this);
		}

		protected void HandleClickMe(object sender, EventArgs e)
		{
			MessageBox.Show("I was clicked!");
		}

		protected void HandleQuit(object sender, EventArgs e)
		{
			Application.Instance.Quit();
		}
	}
}
#endif