using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eto.Forms;

namespace EmbedEtoInWinForms
{
	public partial class Form1 : System.Windows.Forms.Form
	{
		public Form1()
		{
			InitializeComponent();

			// Get native control for the panel
			// passing true so that we can embed, otherwise we just get a reference to the control
			var nativeView = new MyEtoPanel().ToNative(true);
			// set where we want it, size, dock attributes, etc.
			nativeView.Location = new Point(100, 100);
			//nativeView.Dock = DockStyle.Fill;

			Controls.Add(nativeView);
		}
	}
}
