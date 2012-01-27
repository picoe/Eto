using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;
using Eto.Misc;
using System.Reflection;

namespace Eto.Test.Interface.Dialogs
{
	public class About : Dialog
	{
		public About ()
		{
			this.Text = "About Eto Test";
			this.ClientSize = new Size (300, 280);
			this.Resizable = true;
			
			var layout = new TableLayout (this, 1, 5);
			layout.Padding = new Padding (0, 10);
			
			layout.Add (new ImageView{
				Image = new Icon(null, "Eto.Test.Interface.TestIcon.ico"),
				Size = new Size(128, 128)
			}, 0, 0, true, true);
			
			layout.Add (new Label{
				Text = "Test Application",
				Size = new Size(240, 24),
				Font = new Font(FontFamily.Sans, 16, FontStyle.Bold),
				HorizontalAlign = HorizontalAlign.Center
			}, 0, 1);

			var version = Assembly.GetEntryAssembly ().GetName ().Version;
			layout.Add (new Label{
				Text = string.Format("Version {0}", version),
				HorizontalAlign = HorizontalAlign.Center
			}, 0, 2);
			
			
			layout.Add (new Label{
				Text = "Copyright 2011 by Curtis Wensley aka Eto",
				HorizontalAlign = HorizontalAlign.Center
			}, 0, 3);
			
			layout.Add (Buttons (), 0, 4);
		}
		
		Control Buttons ()
		{
			var buttons = new TableLayout (new Panel{
				Size = new Size(90, 26)
			}, 3, 1);
			
			buttons.Padding = Padding.Empty;
			buttons.SetColumnScale (0);
			buttons.SetColumnScale (2);
				
			buttons.Add (CloseButton(), 1, 0);
			
			return buttons.Container;
		}
		
		Control CloseButton()
		{
			var button = new Button{ 
				Text = "Close",
				Size = new Size(90, 26)
			};
			this.DefaultButton = button;
			this.AbortButton = button;
			button.Click += delegate {
				Close ();
			};
			return button;
		}
		
	}
}

