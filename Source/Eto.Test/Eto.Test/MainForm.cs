using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.ComponentModel;

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
			Title = "Test Application";
			Style = "main";
			topNodes = topNodes ?? TestSectionList.TopNodes();
			//SectionList = new SectionListGridView(topNodes);
			//SectionList = new SectionListTreeView(topNodes);
			if (Platform.IsAndroid)
				SectionList = new SectionListGridView(topNodes);
			else
				SectionList = new SectionListTreeGridView(topNodes);

			this.Icon = TestIcons.TestIcon;

			if (Platform.IsDesktop)
				ClientSize = new Size(900, 650);
			//Opacity = 0.5;

			// Commenting the next line on iOS displays just the toolbar. Otherwise it is hidden for some reason.
			Content = MainContent();

			CreateMenuToolBar();
		}

		public SectionList SectionList { get; set; }

		Control MainContent()
		{
			contentContainer = new Panel();

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

			if (Splitter.IsSupported)
			{
				var splitter = new Splitter
				{
					Position = 200,
					FixedPanel = SplitterFixedPanel.Panel1,
					Panel1 = SectionList.Control,
					// for now, don't show log in mobile
					Panel2 = Platform.IsMobile ? contentContainer : RightPane()
				};

				return splitter;
			}
			if (Navigation.IsSupported)
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

		void CreateMenuToolBar()
		{
			var about = new Actions.About();
			var quit = new Actions.Quit();

			if (Platform.Supports<MenuBar>())
			{
				// create standard system menu (e.g. for OS X)
				var menu = MenuBar.CreateStandardMenu();

				// add our own items to the menu

				var file = menu.Items.GetSubmenu("&File", 100);
				menu.Items.GetSubmenu("&Edit", 200);
				menu.Items.GetSubmenu("&Window", 900);
				var help = menu.Items.GetSubmenu("&Help", 1000);

				if (Platform.IsMac)
				{
					// have a nice OS X style menu
					var main = menu.Items.GetSubmenu(Application.Instance.Name, 0);
					main.Items.Add(about, 0);
					main.Items.Add(quit, 1000);
				}
				else
				{
					// windows/gtk style window
					file.Items.Add(quit);
					help.Items.Add(about);
				}

				Menu = menu;
			}

			if (Platform.Supports<ToolBar>())
			{
				// create and set the toolbar
				var toolBar = new ToolBar();
				toolBar.Items.Add(quit);
				toolBar.Items.Add(about);

				ToolBar = toolBar;
			}
		}

		public override void OnWindowStateChanged(EventArgs e)
		{
			base.OnWindowStateChanged(e);
			Log.Write(this, "StateChanged: {0}", WindowState);
		}

		public override void OnClosing(CancelEventArgs e)
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

		public override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			Log.Write(this, "Closed");
		}
	}
}

