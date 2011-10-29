using System;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class ButtonHandler : WindowsControl<System.Windows.Forms.Button, Button>, IButton
	{
		/*
		private static MethodInfo miClick;

		static ButtonHandler()
		{
			miClick = Generator.GetEventMethod(typeof(Button), "OnClick");
		}
		*/
		
		public override SWF.DockStyle DockStyle {
			get {
				return SWF.DockStyle.Top;
			}
		}
		
		public ButtonHandler()
		{
			Control = new SWF.Button();
			Control.MinimumSize = Generator.Convert(Button.DefaultSize);
			Control.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			Control.AutoSize = true;
			//this.Control.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			//this.Control.
			//this.Control.MinimumSize = new System.Drawing.Size(0, 0);
			//this.Control.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			Control.Click += delegate {
				Widget.OnClick(EventArgs.Empty);
			};
		}
		
		public override Size Size
		{
			get { return Generator.Convert(Control.Size); }
			set { 
				this.Control.Size = Generator.Convert(value);
				this.Control.MinimumSize = Generator.Convert(value);
				//this.Control.AutoSize = false; 
			}
		}

	}
}
