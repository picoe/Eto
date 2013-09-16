using System;
using Eto.Forms;
using Eto.Drawing;

namespace Tutorial2
{
	public class MyAction : ButtonAction
	{
		public const string ActionID = "my_action";

		public MyAction()
		{
			this.ID = ActionID;
			this.MenuText = "C&lick Me";
			this.ToolBarText = "Click Me";
			this.TooltipText = "This shows a dialog for no reason";
			//this.Icon = Icon.FromResource ("MyResourceName.ico");
			this.Accelerator = Application.Instance.CommonModifier | Key.M;  // control+M or cmd+M
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			
			MessageBox.Show(Application.Instance.MainForm, "You clicked me!", "Tutorial 2", MessageBoxButtons.OK);
		}
	}

	public class MyForm : Form
	{
		public MyForm()
		{
			this.ClientSize = new Size(600, 400);
			this.Title = "Menus and Toolbars";

			GenerateActions();
		}

		void GenerateActions()
		{
			var actions = new GenerateActionArgs(this);

			Application.Instance.GetSystemActions(actions, true);

			var myAction = new MyAction();
				
			// add action to file sub-menu
			var file = actions.Menu.FindAddSubMenu("&File");
			file.Actions.Add(myAction);

			// add action to toolbar
			actions.ToolBar.Add(myAction);

			// generate menu & toolbar
			this.Menu = actions.Menu.GenerateMenuBar();
			this.ToolBar = actions.ToolBar.GenerateToolBar();
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
