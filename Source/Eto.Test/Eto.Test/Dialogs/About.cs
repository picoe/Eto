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
			this.Resizable = true;

			var layout = new DynamicLayout { Padding = new Padding(20, 5), Spacing = new Size(10, 10) };

			layout.AddCentered(new ImageView
			{
				Image = TestIcons.TestIcon
			}, padding: null, xscale: true, yscale: true);
			
			layout.Add(new Label
			{
				Text = "Test Application",
				Font = new Font(SystemFont.Bold, 20),
				TextAlignment = TextAlignment.Center
			});

			#if PCL
			var version = GetType().GetTypeInfo().Assembly.GetName().Version;
			#else
			var version = Assembly.GetEntryAssembly().GetName().Version;
			#endif
			layout.Add(new Label
			{
				Text = string.Format("Version {0}", version),
				Font = new Font(SystemFont.Default, 10),
				TextAlignment = TextAlignment.Center
			});

			layout.Add(new Label
			{
				Text = "Copyright 2013 by Curtis Wensley aka Eto",
				Font = new Font(SystemFont.Default, 10),
				TextAlignment = TextAlignment.Center
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
			DefaultButton = button;
			AbortButton = button;
			button.Click += delegate
			{
				Close();
			};
			return button;
		}
	}
}

