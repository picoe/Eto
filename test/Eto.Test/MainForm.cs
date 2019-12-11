using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Eto.Forms;
using Eto.Drawing;
using System.Threading.Tasks;
using System.Linq;

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

		string initialSection; // set to initial section to select for easier debugging

		public MainForm(IEnumerable<Section> topNodes = null)
		{
			Title = string.Format("Test Application [{0}, {1} {2}, {3}]",
				Platform.ID,
				EtoEnvironment.Is64BitProcess ? "64bit" : "32bit",
				EtoEnvironment.Platform.IsMono ? "Mono" : ".NET",
				EtoEnvironment.Platform.IsWindows ? EtoEnvironment.Platform.IsWinRT
				? "WinRT" : "Windows" : EtoEnvironment.Platform.IsMac
				? "Mac" : EtoEnvironment.Platform.IsLinux
				? "Linux" : EtoEnvironment.Platform.IsUnix
				? "Unix" : "Unknown");
			Style = "main";
			MinimumSize = new Size(400, 400);
			topNodes = topNodes ?? TestSections.Get(TestApplication.DefaultTestAssemblies());

			var nodes = topNodes.ToList();
			if (Platform.IsAndroid)
				SectionList = new SectionListGridView(nodes);
			else
				SectionList = new SectionListTreeGridView(nodes);

			SectionList.SelectedItemChanged += (sender, e) =>
			{
				Control content = null;
				var item = SectionList.SelectedItem;

				try
				{
					content = item?.CreateContent();
				}
				catch (Exception ex)
				{
					Log.Write(this, "Error loading section: {0}", ex.GetBaseException());
					contentContainer.Content = null;
				}
				finally
				{
					if (navigation != null)
					{
						if (content != null)
							navigation.Push(content, item?.Text);
					}
					else
					{
						contentContainer.Content = content;
					}
				}

#if DEBUG
				GC.Collect();
				GC.WaitForPendingFinalizers();
#endif
			};


			this.Icon = TestIcons.TestIcon;

			if (Platform.IsDesktop)
				ClientSize = new Size(900, 650);
			//Opacity = 0.5;

			Content = MainContent();

			CreateMenuToolBar();

			if (initialSection != null)
			{
				SectionList.SelectedItem = nodes.SelectMany(r => r).OfType<ISection>().FirstOrDefault(r => r.Text == initialSection);
			}

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
			if (Splitter.IsSupported)
			{
				var splitter = new Splitter
				{
					Position = 200,
					FixedPanel = SplitterFixedPanel.Panel1,
					Panel1 = SectionList.Control,
					Panel1MinimumSize = 150,
					Panel2MinimumSize = 300,
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
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Platform must support splitter or navigation"));

		}

		Control RightPane()
		{
			return new Splitter
			{
				Orientation = Orientation.Vertical,
				FixedPanel = SplitterFixedPanel.Panel2,
				Panel1 = contentContainer,
				Panel2 = EventLogSection()
			};
		}

		Control EventLogSection()
		{
			var layout = new DynamicLayout { Size = new Size(100, 120), DefaultSpacing = new Size(5, 5) };

			layout.BeginHorizontal();
			layout.Add(EventLog, true);

			layout.BeginVertical(new Padding(0, 5, 5, 0));
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
				Log.Write(null, $"GC Memory: {GC.GetTotalMemory(true)}");
				using (var process = Process.GetCurrentProcess())
					Log.Write (null, $"Process Total {process.PrivateMemorySize64}");
			};
			return control;
		}

		void CreateMenuToolBar()
		{
			var about = new Commands.About();
			var quit = new Commands.Quit();

			if (Platform.Supports<MenuBar>())
			{
				var fileCommand = new Command { MenuText = "File Command", Shortcut = Application.Instance.CommonModifier | Keys.F };
				fileCommand.Executed += (sender, e) => Log.Write(sender, "Executed");
				var editCommand = new Command { MenuText = "Edit Command", Shortcut = Keys.Shift | Keys.E };
				editCommand.Executed += (sender, e) => Log.Write(sender, "Executed");
				var viewCommand = new Command { MenuText = "View Command", Shortcut = Keys.Control | Keys.Shift | Keys.V };
				viewCommand.Executed += (sender, e) => Log.Write(sender, "Executed");
				var windowCommand = new Command { MenuText = "Window Command" };
				windowCommand.Executed += (sender, e) => Log.Write(sender, "Executed");

				var crashCommand = new Command { MenuText = "Test Exception" };
				crashCommand.Executed += (sender, e) =>
				{
					throw new InvalidOperationException("This is the exception message");
				};

				var subMenu = new ButtonMenuItem { Text = "Sub Menu" };
				subMenu.Items.Add(new ButtonMenuItem { Text = "Item 1" });
				subMenu.Items.Add(new ButtonMenuItem { Text = "Item 2" });
				subMenu.Items.Add(new ButtonMenuItem { Text = "Item 3" });

				var file = new ButtonMenuItem { Text = "&File", Items = { fileCommand, crashCommand } };
				var edit = new ButtonMenuItem { Text = "&Edit", Items = { editCommand, subMenu } };
                var view = new ButtonMenuItem { Text = "&View", Items = { viewCommand } };
				var window = new ButtonMenuItem { Text = "&Window", Order = 1000, Items = { windowCommand } };

				if (Platform.Supports<CheckMenuItem>())
				{
					edit.Items.AddSeparator();

					var checkMenuItem1 = new CheckMenuItem { Text = "Check Menu Item", Shortcut = Keys.Shift | Keys.K };
					checkMenuItem1.Click += (sender, e) => Log.Write(sender, "Click, {0}, Checked: {1}", checkMenuItem1.Text, checkMenuItem1.Checked);
					checkMenuItem1.CheckedChanged += (sender, e) => Log.Write(sender, "CheckedChanged, {0}: {1}", checkMenuItem1.Text, checkMenuItem1.Checked);
					edit.Items.Add(checkMenuItem1);

					var checkMenuItem2 = new CheckMenuItem { Text = "Initially Checked Menu Item", Checked = true };
					checkMenuItem2.Click += (sender, e) => Log.Write(sender, "Click, {0}, Checked: {1}", checkMenuItem2.Text, checkMenuItem2.Checked);
					checkMenuItem2.CheckedChanged += (sender, e) => Log.Write(sender, "CheckedChanged, {0}: {1}", checkMenuItem2.Text, checkMenuItem2.Checked);
					edit.Items.Add(checkMenuItem2);

					var checkMenuItem3 = new CheckCommand { MenuText = "Check Command", Shortcut = Keys.Shift | Keys.K };
					checkMenuItem3.Executed += (sender, e) => Log.Write(sender, "Executed, {0}, Checked: {1}", checkMenuItem3.MenuText, checkMenuItem3.Checked);
					checkMenuItem3.CheckedChanged += (sender, e) => Log.Write(sender, "CheckedChanged, {0}: {1}", checkMenuItem3.MenuText, checkMenuItem3.Checked);
					edit.Items.Add(checkMenuItem3);

					checkMenuItem1.Click += (sender, e) => checkMenuItem3.Checked = !checkMenuItem3.Checked;

				}

				if (Platform.Supports<RadioMenuItem>())
				{
					edit.Items.AddSeparator();

					RadioMenuItem controller = null;
					for (int i = 0; i < 5; i++)
					{
						var radio = new RadioMenuItem(controller) { Text = "Radio Menu Item " + (i + 1) };
						if (controller == null)
						{
							radio.Checked = true; // check the first item initially
							controller = radio;
						}
						radio.Click += (sender, e) => Log.Write(radio, "Click, {0}, Checked: {1}", radio.Text, radio.Checked);
						radio.CheckedChanged += (sender, e) => Log.Write(radio, "CheckedChanged, {0}: {1}", radio.Text, radio.Checked);
						edit.Items.Add(radio);
					}

					edit.Items.AddSeparator();

					RadioCommand commandController = null;
					for (int i = 0; i < 2; i++)
					{
						var radio = new RadioCommand { MenuText = "Radio Command " + (i + 1), Controller = commandController };
						if (commandController == null)
						{
							radio.Checked = true; // check the first item initially
							commandController = radio;
						}
						radio.Executed += (sender, e) => Log.Write(radio, "Executed, {0}, Checked: {1}", radio.MenuText, radio.Checked);
						radio.CheckedChanged += (sender, e) => Log.Write(radio, "CheckedChanged, {0}: {1}", radio.MenuText, radio.Checked);
						edit.Items.Add(radio);
					}
				}

				edit.Items.AddSeparator();
				var hiddenItem = new ButtonMenuItem { Text = "This button should not be visible!", Visible = false };
				var toggleHiddenItem = new ButtonMenuItem { Text = "Toggle Hidden Item" };
				toggleHiddenItem.Click += (sender, e) => hiddenItem.Visible = !hiddenItem.Visible;
				edit.Items.Add(hiddenItem);
				edit.Items.Add(toggleHiddenItem);


				Menu = new MenuBar
				{
					Items =
					{
						// custom top-level menu items
						file, edit, view, window
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

			if (Platform.Supports<ToolBar>())
			{
				// create and set the toolbar
				ToolBar = new ToolBar();

				ToolBar.Items.Add(about);
				if (Platform.Supports<CheckToolItem>())
				{
					ToolBar.Items.Add(new SeparatorToolItem { Type = SeparatorToolItemType.Divider });
					ToolBar.Items.Add(new CheckToolItem { Text = "Check", Image = TestIcons.TestImage });
				}
				ToolBar.Items.Add(new SeparatorToolItem { Type = SeparatorToolItemType.Space });
				ButtonToolItem clickButton = new ButtonToolItem { Text = "Click Me", Image = TestIcons.Logo };
				ToolBar.Items.Add(clickButton);
				if (Platform.Supports<RadioToolItem>())
				{
					ToolBar.Items.Add(new SeparatorToolItem { Type = SeparatorToolItemType.FlexibleSpace });
					ToolBar.Items.Add(new RadioToolItem { Text = "Radio1", Image = TestIcons.Logo, Checked = true });
					ToolBar.Items.Add(new RadioToolItem { Text = "Radio2", Image = TestIcons.TestImage });
					ToolBar.Items.Add(new RadioToolItem { Text = "Radio3 (Disabled)", Image = TestIcons.TestImage, Enabled = false });
				}

				// add an invisible button and separator and allow them to be toggled.
				var invisibleButton = new ButtonToolItem { Text = "Invisible", Visible = false };
				var sep = new SeparatorToolItem { Type = SeparatorToolItemType.Divider, Visible = false };
				ToolBar.Items.Add(sep);
				ToolBar.Items.Add(invisibleButton);
				clickButton.Click += (sender, e) =>
				{
					invisibleButton.Visible = !invisibleButton.Visible;
					sep.Visible = invisibleButton.Visible;
				};
			}
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

		protected override void OnLogicalPixelSizeChanged(EventArgs e)
		{
			base.OnLogicalPixelSizeChanged(e);
			Log.Write(this, $"LogicalPixelSizeChanged: {LogicalPixelSize}");
		}
	}
}

