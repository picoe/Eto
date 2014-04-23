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
			//this.SectionList = new SectionListGridView(topNodes ?? TestSectionList.TopNodes());
			//this.SectionList = new SectionListTreeView(topNodes ?? TestSectionList.TopNodes());
#if ANDROID
			this.SectionList = new SectionListGridView(topNodes ?? TestSectionList.TopNodes());
#else
			this.SectionList = new SectionListTreeGridView(topNodes ?? TestSectionList.TopNodes());
#endif

#if DESKTOP
			this.Icon = TestIcons.TestIcon();
			this.ClientSize = new Size(900, 650);
#endif
			//this.Opacity = 0.5;

			// Commenting the next line on iOS displays just the toolbar. Otherwise it is hidden for some reason.
			Content = MainContent();

			GenerateMenuToolBar();
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

			if (Splitter.IsSupported())
			{
				var splitter = new Splitter
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

		void GenerateMenuToolBar()
		{
			var about = new Actions.About();
			var quit = new Actions.Quit();

			if (Generator.Supports<IMenuBar>())
			{
				var menu = new MenuBar();
				// create standard system menu (e.g. for OS X)
				Application.Instance.CreateStandardMenu(menu.Items);

				// add our own items to the menu

				var file = menu.Items.GetSubmenu("&File", 100);
				menu.Items.GetSubmenu("&Edit", 200);
				menu.Items.GetSubmenu("&Window", 900);
				var help = menu.Items.GetSubmenu("&Help", 1000);

				if (Generator.IsMac)
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

				// optional, removes empty submenus and duplicate separators
				menu.Items.Trim();

				Menu = menu;
			}

			// generate and set the toolbar
			var toolBar = new ToolBar();
			toolBar.Items.Add(quit);
			toolBar.Items.Add(new ButtonToolItem(about));

			ToolBar = toolBar;
		}

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

