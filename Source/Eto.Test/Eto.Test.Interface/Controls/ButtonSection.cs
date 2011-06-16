using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Interface.Controls
{
	public class ButtonSection : Panel
	{
		public ButtonSection ()
		{
			var layout = new TableLayout(this, 1, 10);
			
			//layout.SetColumnScale(0);
			
			layout.Add (ScalingControl(new Button{ Text = "Click Me" }), 0, 0);
			layout.Add (ScalingControl(new Button{ Text = "This is a long(er) button title" }), 0, 1);
			layout.Add (ScalingControl(new Button{ Text = "Button with Color", BackgroundColor = Color.Green }), 0, 2);
		}

		Control ScalingControl(Button button)
		{
			button.Click += delegate {
				MessageBox.Show (this, string.Format ("You clicked '{0}'", button.Text));
			};
			return ScalingControl((Control)button);
		}
		
		Control ScalingControl(Control control)
		{
			var layout = new TableLayout(new Panel{ }, 3, 1);
			layout.Padding = Padding.Empty;
			layout.SetColumnScale(0);
			layout.SetColumnScale(2);
			layout.Add (control, 1, 0);
			return layout.Container;
		}
	}
}

