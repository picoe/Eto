using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface
{
	public class MainForm : Form
	{
		
		public MainForm ()
		{
			this.Text = "Test Application";
			
			this.Icon = new Icon (null, "TestIcon.ico");
			this.Size = new Size (800, 600);
			
			/* Option 1: use actions to generate menu and toolbar (recommended)
			 */
			GenerateMenuToolBarActions ();
			
			
			/* Option 2: generate menu and toolbar directly
			 *
			GenerateMenu();
			
			GenerateToolBar();
			/*
			 */
			 
			
			GenerateContent();
		}
		
		void GenerateContent()
		{
			var splitter = new Splitter();
			
			var contentContainer = new Panel();
			var sectionList = new Controls.SectionList(contentContainer);
			
			splitter.Panel1 = sectionList;
			splitter.Panel2 = contentContainer;

			splitter.Position = 200;
			
			this.AddDockedControl (splitter);
			
			this.Shown += delegate {
				sectionList.Focus ();
			};
		}
		
		void GenerateMenuToolBarActions ()
		{
			// use actions to generate menu & toolbar to share logic
			var args = new GenerateActionArgs ();
			
			// generate actions to use in menus and toolbars
			Application.Instance.GetSystemActions (args);
			
			args.Actions.Add (new Actions.About ());
			args.Actions.Add (new Actions.Quit ());
			args.Actions.Add (new Actions.Close ());
			
			
			// generate and set the menu
			GenerateMenu(args);

			// generate and set the toolbar
			GenerateToolBar(args);
		}
		
		void GenerateMenu(GenerateActionArgs args)
		{
			var file = args.Menu.FindAddSubMenu ("&File", 100);
			var window = args.Menu.FindAddSubMenu ("&Window", 900);
			var help = args.Menu.FindAddSubMenu ("Help", 1000);
			
			if (Generator.ID == "mac") {
				// have a nice OS X style menu
				
				var main = args.Menu.FindAddSubMenu ("Test Application", 0);
				main.Actions.Add (Actions.About.ActionID, 0);
				main.Actions.AddSeparator ();
				main.Actions.Add ("mac_hide", 700);
				main.Actions.Add ("mac_hideothers", 700);
				main.Actions.Add ("mac_showall", 700);
				main.Actions.AddSeparator (900);
				main.Actions.Add (Actions.Quit.ActionID, 1000);
				
				file.Actions.Add (Actions.Close.ActionID, 900);
				
				window.Actions.Add ("mac_performMiniaturize");
				window.Actions.Add ("mac_performZoom");
			} else {
				// windows/gtk style window
				file.Actions.Add (Actions.Quit.ActionID);
				
				help.Actions.Add (Actions.About.ActionID);
			}

			this.Menu = args.Menu.GenerateMenuBar ();
		}
		
		void GenerateToolBar(GenerateActionArgs args)
		{
			args.ToolBar.Add (Actions.Quit.ActionID);
			args.ToolBar.Add (Actions.About.ActionID);
			this.ToolBar = args.ToolBar.GenerateToolBar ();
		}
		
		#region Generate Menu & Toolbar Manually
		
		/*
		void GenerateMenu ()
		{
			var menuBar = new MenuBar ();
			
			var file = new ImageMenuItem{ Text = "&File" };
			menuBar.MenuItems.Add (file);
			
			// close
			var close = new ImageMenuItem { Text = "&Close" };
			close.Click += delegate {
				this.Close ();
			};
			file.MenuItems.Add (close);
			
			
			this.Menu = menuBar;
		}
		
		void GenerateToolBar ()
		{
			var toolBar = new ToolBar ();
			
			// close
			var close = new ToolBarButton{ Text = "Close" };
			close.Click += delegate {
				this.Close ();
			};
			toolBar.Items.Add (close);
		}
		*/
		#endregion
	}
}

