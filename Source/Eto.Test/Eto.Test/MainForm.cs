using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Eto.Forms;
using Eto.Drawing;

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
						Size = new Size(100, 100),
						ReadOnly = true,
						Wrap = false
					};
				}
				return eventLog;
			}
		}

		public MainForm(IEnumerable<Section> topNodes = null)
		{
			Title = string.Format("Test Application [{0}]", Platform.ID);
			Style = "main";
			MinimumSize = new Size(400, 400);
			topNodes = topNodes ?? TestSections.Get(TestApplication.DefaultTestAssemblies());
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

			Content = MainContent();

			CreateMenuBar();
		}

		public SectionList SectionList { get; set; }

		Control MainContent()
		{
			Splitter splitter = null;

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
				splitter = new Splitter
				{
					Position = 200,
					FixedPanel = SplitterFixedPanel.Panel1,
					Panel1 = SectionList.Control,
					// for now, don't show log in mobile
					Panel2 = Platform.IsMobile ? contentContainer : RightPane()
				};
			}

			if (DockView.IsSupported)
			{
				DockView dockView = new DockView
				{
					Content = splitter,
					Items =
					{
						new DockViewItem
						{
							Content = new ToolBarView { Content = this.CreateToolBarTop(), Dock = DockPosition.Top },
							Dock = DockPosition.Top,
							Order = 1,
							Position = new Eto.Drawing.Point(0,0)
						},
						new DockViewItem
						{
							Content = new ToolBarView { Content = this.CreateToolBarRight(), Dock = DockPosition.Bottom },
							Dock = DockPosition.Right
						},
						new DockViewItem
						{
							Content = new TextBox { PlaceholderText ="Search", Text = "Search for" },
							Dock = DockPosition.None,
							Order = 2,
							Position = new Eto.Drawing.Point(200,0)
						}
					}
				};
				
				return dockView;
			}

			if (ToolBarView.IsSupported)
			{
				this.ToolBar = new ToolBarView { Content = this.CreateToolBarTop(), Dock = DockPosition.Top };
			}

			if (Splitter.IsSupported)
			{
				return splitter;
			}

			if (Navigation.IsSupported)
			{
				navigation = new Navigation(SectionList.Control, "Eto.Test");

				return navigation;
			}
			
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Platform must support splitter or navigation"));
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
			var layout = new DynamicLayout { Size = new Size(100, 120), Spacing = Size.Empty };
			
			layout.BeginHorizontal();
			layout.Add(EventLog, true);
			
			layout.BeginVertical(new Padding(5, 0));
			layout.Add(ClearButton());
			layout.Add(MemoryButton());
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

		Control MemoryButton()
		{
			var control = new Button
			{
				Text = "Memory"
			};
			control.Click += (sender, e) =>
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Log.Write(null, "Memory: {0}", GC.GetTotalMemory(true));
			};
			return control;
		}

		void CreateMenuBar()
		{
			var about = new Commands.About();
			var quit = new Commands.Quit();

			if (Platform.Supports<MenuBar>())
			{
				Menu = new MenuBar
				{
					Items =
					{
						// custom top-level menu items
						new ButtonMenuItem { Text = "&File", Items = { new Command { MenuText = "File Command" } } },
						new ButtonMenuItem { Text = "&Edit", Items = { new Command { MenuText = "Edit Command" } } },
						new ButtonMenuItem { Text = "&View", Items = { new Command { MenuText = "View Command" } } },
						new ButtonMenuItem { Text = "&Window", Order = 1000, Items = { new Command { MenuText = "Window Command" } } },
					},
					ApplicationItems =
					{
						// custom menu items for the application menu (Application on OS X, File on others)
						new Command { MenuText = "Application command" },
						new ButtonMenuItem { Text = "Application menu item" }
					},
					HelpItems =
					{
						new Command { MenuText = "Help Command" }
					},
					QuitItem = quit,
					AboutItem = about
				};
			}
		}

		ToolBar CreateToolBarRight()
		{
			ToolBar toolBar = new ToolBar();

			if (Platform.Supports<ToolBar>())
			{
				toolBar.Items.Add(new Commands.About());
				toolBar.Items.Add(new Commands.Quit());
			}

			return toolBar;
		}

		ToolBar CreateToolBarTop()
		{
			ToolBar toolBar = new ToolBar();

			if (Platform.Supports<ToolBar>())
			{
				if (Platform.Supports<ButtonToolItem>())
				{
					toolBar.Items.Add(new ButtonToolItem { Text = "Button1", Image = TestIcons.TestImage });
					toolBar.Items.Add(new CheckToolItem { Text = "Button2", Image = TestIcons.TestImage });
					toolBar.Items.Add(new CheckToolItem { Text = "Button3", Image = TestIcons.TestImage });
					toolBar.Items.Add(new SeparatorToolItem { Type = SeparatorToolItemType.Divider });
				}
				if (Platform.Supports<CheckToolItem>())
				{
					toolBar.Items.Add(new CheckToolItem { Text = "Check1", Image = TestIcons.TestImage });
					toolBar.Items.Add(new SeparatorToolItem { Type = SeparatorToolItemType.Divider });
					toolBar.Items.Add(new CheckToolItem { Text = "Check2", Image = TestIcons.TestImage });
				}
				if (Platform.Supports<RadioToolItem>())
				{
					toolBar.Items.Add(new RadioToolItem { Text = "Radio1", Image = TestIcons.TestIcon, Checked = true });
					toolBar.Items.Add(new SeparatorToolItem { Type = SeparatorToolItemType.FlexibleSpace });
					toolBar.Items.Add(new RadioToolItem { Text = "Radio2", Image = TestIcons.TestImage });
				}
				if (Platform.Supports<ButtonToolItem>())
				{
					toolBar.Items.Add(new ButtonToolItem { Text = "Button4", Image = TestIcons.TestImage });
				}
			}

			return toolBar;
		}

		protected override void OnWindowStateChanged(EventArgs e)
		{
			base.OnWindowStateChanged(e);
			Log.Write(this, "StateChanged: {0}", WindowState);
		}

		protected override void OnClosing(CancelEventArgs e)
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

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			Log.Write(this, "Closed");
		}
	}
}

