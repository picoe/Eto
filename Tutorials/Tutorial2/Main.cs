using System;
using Eto.Forms;
using Eto.Drawing;

namespace Tutorial2
{
	public class MyCommand : Command
	{
		public const string ActionID = "my_action";

		public MyCommand()
		{
			this.ID = ActionID;
			this.MenuText = "C&lick Me";
			this.ToolBarText = "Click Me";
			this.ToolTip = "This shows a dialog for no reason";
			//this.Icon = Icon.FromResource ("MyResourceName.ico");
			this.Shortcut = Application.Instance.CommonModifier | Key.M;  // control+M or cmd+M
		}

		public override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);

			MessageBox.Show(Application.Instance.MainForm, "You clicked me!", "Tutorial 2", MessageBoxButtons.OK);
		}
	}

	public class MyForm : Form
	{
		public MyForm()
		{
			this.ClientSize = new Size(600, 400);
			this.Title = "Menus and Toolbars";

			Menu = CreateMenu();
			ToolBar = CreateToolBar();
		}

		MenuBar CreateMenu()
		{
			var menu = new MenuBar();
			// add standard menu items (e.g. for OS X)
			Application.Instance.CreateStandardMenu(menu.Items);

			// add command to file sub-menu
			var file = menu.Items.GetSubmenu("&File");
			file.Items.Add(new MyCommand());

			return menu;
		}

		ToolBar CreateToolBar()
		{
			var toolbar = new ToolBar();

			toolbar.Items.Add(new MyCommand());

			return toolbar;
		}
	}

	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var app = new Application();
			
			app.Initialized += delegate
			{
				app.MainForm = new MyForm();
				app.MainForm.Show();
			};
			app.Run(args);
		}
	}
}
