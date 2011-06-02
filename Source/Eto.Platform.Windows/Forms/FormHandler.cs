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
			control.AutoSize = true;

			main = new SWF.Panel();
			main.AutoSize = true;
			main.Dock = SWF.DockStyle.Fill;
			control.Controls.Add(main);
			control.Closed += control_Closed;
			Control = control;
		}

		public override object ContainerObject
		{
			get { return main; }
		}

		public override Size Size
		{
			get	{ return Generator.Convert(control.Size); }
			set { control.Size = Generator.Convert(value); }
		}

		public void Show()
		{
			control.Show();
		}

		private void control_Closed(object sender, EventArgs e)
		{
			Widget.OnClosed(EventArgs.Empty);
		}
	}
}
