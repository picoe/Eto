using System;
using Eto.Forms;
using Eto.Drawing;
using System.Text;
using System.Collections.Generic;

namespace Eto.Test
{
	public class MainForm : Form
	{
		TextArea eventLog;
		Panel contentContainer;
		Navigation navigation;

		public TextArea EventLog
		{
			get
			{
				if (eventLog == null)
				{
					eventLog = new TextArea
					{
						Size = new Size (100, 100),
						ReadOnly = true,
						Wrap = false
					};
				}
				return eventLog;
			}
		}

		public MainForm(IEnumerable<Section> topNodes = null)
		{
			this.Title = "Test Application";
			this.Style = "main";
			this.SectionList = new SectionList(topNodes ?? TestSectionList.TopNodes());

#if DESKTOP
			this.Icon = TestIcons.TestIcon;
			this.ClientSize = new Size(900, 650);
#endif
			//this.Opacity = 0.5;

#if DESKTOP
			HandleEvent(MainForm.WindowStateChangedEvent);
#endif
			HandleEvent(MainForm.ClosedEvent, MainForm.ClosingEvent);

			/* Option 1: use actions to generate menu and toolbar (recommended)
			 */
			GenerateMenuToolBarActions();


			/* Option 2: generate menu and toolbar directly
			 *
			GenerateMenu();
			
			GenerateToolBar();
			/*
			 */

			Content = MainContent();
		}

		public SectionList SectionList { get; set; }

		Control MainContent()
		{
			contentContainer = new Panel();

			// set focus when the form is shown
			this.Shown += delegate
			{
				SectionList.Focus();
			};
			SectionList.SelectedItemChanged += (sender, e) => {
				try
				{
					var item = SectionList.SelectedItem;
					Control content = item != null ? item.CreateContent() : null;

					if (navigation != null)
					{
						if (content != null)
							navigation.Push(content, item != null ? item.Text : null);
					}
					else
					{
						contentContainer.Content = content;
					}
				}
				catch (Exception ex)
				{
					Log.Write(this, "Error loading section: {0}", ex.GetBaseException());
					contentContainer.Content = null;
				}

				#if DEBUG
				GC.Collect();
				GC.WaitForPendingFinalizers();
				#endif
			};

			if (Splitter.Supported)
			{
				var splitter = new Splitter
				{
					Position = 200,
					FixedPanel = SplitterFixedPanel.Panel1,
					Panel1 = SectionList,
#if MOBILE
					// for now, don't show log in mobile
					Panel2 = contentContainer
#else
					Panel2 = RightPane ()
#endif
				};
				return splitter;
			}
			else if (Navigation.Supported)
			{
				navigation = new Navigation(SectionList, "Eto.Test");
				return navigation;
			}
			else
				throw new EtoException("Platform must support splitter or navigation");

		}

		Control RightPane()
		{
			var splitter = new Splitter
			{
				Orientation = SplitterOrientation.Vertical,
				Position = 500,
				FixedPanel = SplitterFixedPanel.Panel2
			};

			splitter.Panel1 = contentContainer;
			splitter.Panel2 = this.EventLogSection();

			return splitter;
		}

		Control EventLogSection()
		{
			var layout = new DynamicLayout();
			
			layout.BeginHorizontal();
			layout.Add(EventLog, true);
			
			layout.BeginVertical();
			layout.Add(ClearButton());
			layout.Add(null);
			layout.EndVertical();
			layout.EndHorizontal();
			return layout;
		}

		Control ClearButton()
		{
			var control = new Button
			{
				Text = "Clear"
			};
			control.Click += (sender, e) => {
				EventLog.Text = string.Empty;
			};
			return control;
		}

		void GenerateMenuToolBarActions()
		{
			// use actions to generate menu & toolbar to share logic
			var args = new GenerateActionArgs();

			// generate actions to use in menus and toolbars
			Application.Instance.GetSystemActions(args, true);

			args.Actions.Add(new Actions.About());
			args.Actions.Add(new Actions.Quit());
			args.Actions.Add(new Actions.Close());


			// generate and set the menu
			GenerateMenu(args);

			// generate and set the toolbar
			GenerateToolBar(args);
		}

		void GenerateMenu(GenerateActionArgs args)
		{
			var file = args.Menu.GetSubmenu("&File", 100);
			args.Menu.GetSubmenu("&Edit", 200);
			args.Menu.GetSubmenu("&Window", 900);
			var help = args.Menu.GetSubmenu("&Help", 1000);

			if (Generator.IsMac)
			{
				// have a nice OS X style menu

				var main = args.Menu.GetSubmenu(Application.Instance.Name, 0);
				main.Actions.Add(Actions.About.ActionID, 0);
				main.Actions.Add(Actions.Quit.ActionID, 1000);
			}
			else
			{
				// windows/gtk style window
				file.Actions.Add(Actions.Quit.ActionID);

				help.Actions.Add(Actions.About.ActionID);
			}

#if DESKTOP
			this.Menu = args.Menu.GenerateMenuBar();
#endif
		}

		void GenerateToolBar(GenerateActionArgs args)
		{
			args.ToolBar.Add(Actions.Quit.ActionID);
			args.ToolBar.Add(Actions.About.ActionID);
#if DESKTOP
			// TODO for mobile
			this.ToolBar = args.ToolBar.GenerateToolBar();
#endif
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
		#if DESKTOP
		public override void OnWindowStateChanged(EventArgs e)
		{
			base.OnWindowStateChanged(e);
			Log.Write(this, "StateChanged: {0}", this.WindowState);
		}

		public override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			Log.Write(this, "Closing");
			/*
			 * Note that on OS X, windows usually close, but the application will keep running.  It is
			 * usually better to handle the Application.OnTerminating event instead.
			 * 
			var result = MessageBox.Show (this, "Are you sure you want to close?", MessageBoxButtons.YesNo);
			if (result == DialogResult.No) e.Cancel = true;
			*/
		}
		#endif
		public override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			Log.Write(this, "Closed");
		}
	}
}

