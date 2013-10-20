using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;
using System.Reflection;

namespace Eto.Test.Dialogs
{
	public class About : Dialog
	{
		public About()
		{
			this.Title = "About Eto Test";
#if DESKTOP
			this.Resizable = true;
#endif

			var layout = new DynamicLayout(new Padding(20, 5), new Size(10, 10));

			layout.AddCentered(new ImageView
			{
				Image = Icon.FromResource ("Eto.Test.TestIcon.ico")
			}, true, true);
			
			layout.Add(new Label
			{
				Text = "Test Application",
				Font = new Font(SystemFont.Bold, 20),
				HorizontalAlign = HorizontalAlign.Center
			});

			var version = Assembly.GetEntryAssembly().GetName().Version;
			layout.Add(new Label
			{
				Text = string.Format("Version {0}", version),
				Font = new Font(SystemFont.Default, 10),
				HorizontalAlign = HorizontalAlign.Center
			});
			
			
			layout.Add(new Label
			{
				Text = "Copyright 2013 by Curtis Wensley aka Eto",
				Font = new Font(SystemFont.Default, 10),
				HorizontalAlign = HorizontalAlign.Center
			});

			layout.AddCentered(CloseButton());

			Content = layout;
		}

		Control CloseButton()
		{
			var button = new Button
			{
				Text = "Close"
			};
			this.DefaultButton = button;
			this.AbortButton = button;
			button.Click += delegate
			{
				Close();
			};
			return button;
		}
	}
}

