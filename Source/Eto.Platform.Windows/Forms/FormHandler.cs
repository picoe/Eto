using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class FormHandler : WindowHandler<SWF.Form, Form>, IForm
	{
		SWF.Form control;
		SWF.Panel main;
		
		public FormHandler()
		{
			control = new SWF.Form();
			this.control.Size = SD.Size.Empty;
			this.control.MinimumSize = SD.Size.Empty;
			this.control.AutoSize = true;
			//this.control.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

			main = new SWF.Panel();
			main.AutoSize = true;
			//this.main.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			main.Dock = SWF.DockStyle.Fill;
			control.Controls.Add(main);
			Control = control;
		}

		public override object ContainerObject
		{
			get { return main; }
		}

		public void Show()
		{
			control.Show();
		}
	}
}
