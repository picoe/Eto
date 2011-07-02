using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class DialogHandler : WindowHandler<SWF.Form, Dialog>, IDialog
	{
		SWF.Form control;
		SWF.Panel main;
		Button button;

		
		public DialogHandler()
		{
			control = new SWF.Form();
			control.FormBorderStyle = SWF.FormBorderStyle.FixedDialog;
			this.control.Size = SD.Size.Empty;
			this.control.MinimumSize = SD.Size.Empty;
			control.AutoSize = true;
			//control.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			control.ShowInTaskbar = false;
			control.ShowIcon = false;
			control.MaximizeBox = false;
			control.MinimizeBox = false;
			control.StartPosition = SWF.FormStartPosition.CenterParent;
			main = new SWF.Panel();
		
			main.Dock = SWF.DockStyle.Fill;
			main.AutoSize = true;
			//main.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			control.Controls.Add(main);
			control.Load += control_Load;
			this.Control = control;
		}
		
		public Button DefaultButton
		{
			get {
				return button;
			}
			set {
				button = value;
				if (button != null) {
					var b = button.ControlObject as SWF.IButtonControl;
					this.Control.AcceptButton = b;
				}
				else
					this.Control.AcceptButton = null;
			}
		}
		
		
		private void control_Load(Object sender, EventArgs e)
		{
			Widget.OnLoad(e);
		}

		public override object ContainerObject
		{
			get { return main; }
		}

		public override Size Size
		{
			get { return Generator.Convert(control.Size); }
			set { 
				control.Size = Generator.Convert(value);
				control.AutoSize = false;
			}
		}

		public DialogResult ShowDialog(Control parent)
		{
			if (parent != null) control.ShowDialog((SWF.Control)parent.ControlObject);
			else control.ShowDialog();

			return Widget.DialogResult;
		}
	}
}
