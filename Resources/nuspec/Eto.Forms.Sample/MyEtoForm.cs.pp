using System;
using Eto.Forms;
using Eto.Drawing;

namespace $rootnamespace$
{
	/// <summary>
	/// Your form
	/// </summary>
	public class MyEtoForm : Form
	{
		public MyEtoForm()
		{
			Title = "My Eto Form";
			ClientSize = new Size(400, 350);

			// DynamicLayout is the typical layout to use
			// but you can also use TableLayout or PixelLayout
			var layout = new DynamicLayout();

			layout.BeginHorizontal();

			layout.Add(null);
			layout.Add(new Label { Text = "Hello World!" });
			layout.Add(null);

			layout.EndHorizontal();

			// scrollable gives you a scrolling region
			Content = new Scrollable { Content = layout };

			GenerateMenuAndToolBar();
		}

		void GenerateMenuAndToolBar()
		{
			// your commands!
			var clickMe = new Command { MenuText = "Click Me!", ShowLabel = true, ToolBarText = "Click Me!" };
			clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

			var quitAction = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
			quitAction.Executed += (sender, e) => Application.Instance.Quit();


			// create menu & get standard menu items (e.g. needed for OS X)
			var menu = new MenuBar();
			Application.Instance.CreateStandardMenu(menu.Items);
			
			// add commands to the menu
			var myMenu = menu.Items.GetSubmenu("&File");
			myMenu.Items.Add(clickMe, 500);
			myMenu.Items.AddSeparator(500);
			myMenu.Items.Add(quitAction, 1000);
			
			menu.Items.Trim();
			this.Menu = menu;
			

			// create toolbar			
			var toolbar = new ToolBar();
			toolbar.Items.Add(clickMe);
			this.ToolBar = toolbar;
		}
	}
}