using System;
using Eto.Forms;
using Eto.Drawing;
using Eto;

namespace Tutorial2
{
	public class MyCommand : Command
	{
		public MyCommand()
		{
			MenuText = "C&lick Me";
			ToolBarText = "Click Me";
			ToolTip = "This shows a dialog for no reason";
			//Icon = Icon.FromResource ("MyResourceName.ico");
			Shortcut = Application.Instance.CommonModifier | Keys.M;  // control+M or cmd+M
		}

		protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);

			MessageBox.Show(Application.Instance.MainForm, "You clicked me!", "Tutorial 2", MessageBoxButtons.OK);
		}
	}

	public class MyForm : Form
	{
		public MyForm()
		{
			ClientSize = new Size(600, 400);
			Title = "Menus and Toolbars";

			Menu = CreateMenu();
			ToolBar = CreateToolBar();
		}

		MenuBar CreateMenu()
		{
			var menu = MenuBar.CreateStandardMenu();

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
