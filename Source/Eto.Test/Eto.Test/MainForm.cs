using System;
using Eto.Forms;
using Eto.Drawing;
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
#if DESKTOP
			this.SectionList = new SectionListGridView(topNodes ?? TestSectionList.TopNodes());
#else
			this.SectionList = new SectionListTreeView(topNodes ?? TestSectionList.TopNodes());
#endif

#if DESKTOP
			this.Icon = TestIcons.TestIcon;
			this.ClientSize = new Size(900, 650);
#endif
			//this.Opacity = 0.5;

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

			Splitter splitter = null;

			// set focus when the form is shown
			Shown += delegate
			{
				SectionList.Focus();
			};
			SectionList.SelectedItemChanged += (sender, e) =>
			{
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
#if MOBILE
						// For now, avoid nested controls in mobile.
						splitter.Panel2 = content;
#else
						contentContainer.Content = content;
#endif
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

			if (Splitter.IsSupported())
			{
				splitter = new Splitter
				{
					Position = 200,
					FixedPanel = SplitterFixedPanel.Panel1,
					Panel1 = SectionList.Control,
#if MOBILE
					// for now, don't show log in mobile
					Panel2 = contentContainer
#else
					Panel2 = RightPane()
#endif
				};
				return splitter;
			}
			if (Navigation.IsSupported())
			{
				navigation = new Navigation(SectionList.Control, "Eto.Test");
				return navigation;
			}
			throw new EtoException("Platform must support splitter or navigation");

		}

		Control RightPane()
		{
			return new Splitter
			{
				Orientation = SplitterOrientation.Vertical,
				FixedPanel = SplitterFixedPanel.Panel2,
				Panel1 = contentContainer,
				Panel2 = EventLogSection()
			};
		}

		Control EventLogSection()
		{
			var layout = new DynamicLayout { Size = new Size(100, 120) };
			
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
			control.Click += (sender, e) => EventLog.Text = string.Empty;
			return control;
		}

		void GenerateMenuToolBarActions()
		{
			// use actions to generate menu & toolbar to share logic
			var actions = new List<BaseAction>();
			var menu = new MenuBar();
			var toolBar = new ToolBar();

			// generate actions to use in menus and toolbars
			Application.Instance.GetSystemActions(actions, menu, toolBar, true);
			var about = new Actions.About();
			var quit = new Actions.Quit();
			var close = new Actions.Close();

			// generate and set the menu
			var file = menu.GetSubmenu("&File", 100);
			menu.GetSubmenu("&Edit", 200);
			menu.GetSubmenu("&Window", 900);
			var help = menu.GetSubmenu("&Help", 1000);

			if (Generator.IsMac)
			{
				// have a nice OS X style menu

				var main = menu.GetSubmenu(Application.Instance.Name, 0);
				main.MenuItems.Add(about.CreateMenuItem()); // TODO: Order = , 0;
				main.MenuItems.Add(quit.CreateMenuItem()); // TODO: Order = , 1000);
			}
			else
			{
				// windows/gtk style window
				file.MenuItems.Add(quit.CreateMenuItem());
				help.MenuItems.Add(about.CreateMenuItem());
			}

#if DESKTOP
			this.Menu = menu;
#endif

			// generate and set the toolbar
			toolBar.Items.Add(quit.CreateToolBarItem());
			toolBar.Items.Add(about.CreateToolBarItem());

			// TODO for mobile
			this.ToolBar = toolBar;
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
			Log.Write(this, "StateChanged: {0}", WindowState);
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

