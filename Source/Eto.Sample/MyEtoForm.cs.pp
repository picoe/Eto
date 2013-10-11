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
			var args = new GenerateActionArgs();
			// get system actions (e.g. needed for OS X)
			Application.Instance.GetSystemActions(args, true);

			// your actions!
			var clickMe = new ButtonAction { Text = "Click Me!", ID = "clickme", ShowLabel = true, ToolBarText = "Click Me!" };
			clickMe.Activated += (sender, e) => MessageBox.Show(this, "I was clicked!");

			var quitAction = new ButtonAction { Text = "Quit", ID = "quit", Accelerator = Application.Instance.CommonModifier | Key.Q };
			quitAction.Activated += (sender, e) => Application.Instance.Quit();

			// add actions to the menu & toolbar definitions
			var myMenu = args.Menu.FindAddSubMenu("&File");
			myMenu.Actions.Add(clickMe);
			myMenu.Actions.AddSeparator();
			myMenu.Actions.Add(quitAction, 1000); // show last

			args.ToolBar.Add(clickMe);

			// generate menu & toolbar for this form
			this.Menu = args.Menu.GenerateMenuBar();
			this.ToolBar = args.ToolBar.GenerateToolBar();
		}
	}
}