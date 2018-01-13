using System;
using Eto.Forms;
using Eto.Drawing;
using Eto;

namespace Tutorial2
{
	/// <summary>
	/// Custom command
	/// </summary>
	/// <remarks>
	/// You can create your own command subclasses, or create instances of Command directly.
	/// Commands can be used for either the menu or toolbar.
	/// Otherwise, you can use MenuItem or ToolItem classes directly.
	/// </remarks>
	public class MyCommand : Command
	{
		public MyCommand()
		{
			MenuText = "C&lick Me, Command";
			ToolBarText = "Click Me";
			ToolTip = "This shows a dialog for no reason";
			//Image = Icon.FromResource ("MyResourceName.ico");
			//Image = Bitmap.FromResource ("MyResourceName.png");
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

			// create menu
			Menu = new MenuBar
			{
				Items =
				{
					new ButtonMenuItem
					{ 
						Text = "&File",
						Items =
						{ 
							// you can add commands or menu items
							new MyCommand(),
							new ButtonMenuItem { Text = "Click Me, MenuItem" }
						}
					} 
				},
				// quit item (goes in Application menu on OS X, File menu for others)
				QuitItem = new Command((sender, e) => Application.Instance.Quit())
				{ 
					MenuText = "Quit", 
					Shortcut = Application.Instance.CommonModifier | Keys.Q
				},
				// about command (goes in Application menu on OS X, Help menu for others)
				AboutItem = new Command((sender, e) => new Dialog { Content = new Label { Text = "About my app..." }, ClientSize = new Size(200, 200) }.ShowModal(this))
				{ 
					MenuText = "About my app"
				}
			};

			// create toolbar
			ToolBar = new ToolBar
			{
				Items =
				{ 
					new MyCommand(),
					new SeparatorToolItem(),
					new ButtonToolItem { Text = "Click Me, ToolItem" }
				}
			};
		}
	}

	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application().Run(new MyForm());
		}
	}
}
