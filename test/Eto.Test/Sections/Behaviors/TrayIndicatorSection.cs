using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", typeof(TrayIndicator))]
    public class TrayIndicatorSection : Panel
    {
		TrayIndicator tray;

		public TrayIndicatorSection()
        {
			tray = new TrayIndicator();
			tray.Image = TestIcons.TestIcon;
			tray.Title = "Eto Test App";

			var menu = new ContextMenu();
			menu.Items.Add(new Commands.About());
			menu.Items.Add(new Commands.Quit());
			tray.Menu = menu;

			tray.Activated += (o, e) => MessageBox.Show("Hello World!!!");

			tray.Show();

			Content = TableLayout.AutoSized("Tray should now be visible");
        }

		protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			tray.Hide();
		}
	}
}
