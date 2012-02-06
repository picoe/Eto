using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;
using Eto.Misc;
using System.Reflection;

namespace Eto.Test.Dialogs
{
	public class About : Dialog
	{
		public About ()
		{
			this.Text = "About Eto Test";
			this.Resizable = true;

			var layout = new DynamicLayout (this);

			layout.AddCentered(new ImageView{
				Image = new Icon(null, "Eto.Test.TestIcon.ico"),
				Size = new Size(128, 128)
			}, true, true);
			
			layout.Add (new Label{
				Text = "Test Application",
				Font = new Font(FontFamily.Sans, 16, FontStyle.Bold),
				HorizontalAlign = HorizontalAlign.Center
			});

			var version = Assembly.GetEntryAssembly ().GetName ().Version;
			layout.Add (new Label {
				Text = string.Format("Version {0}", version),
				HorizontalAlign = HorizontalAlign.Center
			});
			
			
			layout.Add (new Label{
				Text = "Copyright 2011 by Curtis Wensley aka Eto",
				HorizontalAlign = HorizontalAlign.Center
			});

			layout.AddCentered (CloseButton ());
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

