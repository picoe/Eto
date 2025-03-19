using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms;

public abstract class WindowTests<T> : TestBase
  where T : Window, new()
{
	protected abstract void Test(Action<T> test, int timeout = DefaultTimeout);
	protected abstract void ManualTest(string message, Func<T, Control> test);
	protected abstract void Show(T window);
	protected abstract Task ShowAsync(T window);

	[Test]
	[ManualTest]
	public void WindowShouldAutoSize() => Test(window =>
	{
		window.AutoSize = true;
		window.MinimumSize = Size.Empty;

		var bottomPanel = new StackLayout();
		var rightPanel = new StackLayout { Orientation = Orientation.Horizontal };

		var autoSize = new CheckBox { Text = "AutoSize", Checked = window.AutoSize };
		autoSize.CheckedChanged += (sender, e) =>
	  {
		  window.AutoSize = autoSize.Checked == true;
	  };

		var addBottomButton = new Button { Text = "Add bottom control" };
		addBottomButton.Click += (sender, e) =>
	  {
		  bottomPanel.Items.Add(new Panel { Size = new Size(20, 20) });
		  autoSize.Checked = window.AutoSize;
	  };

		var addRightButton = new Button { Text = "Add right control" };
		addRightButton.Click += (sender, e) =>
	  {
		  rightPanel.Items.Add(new Panel { Size = new Size(20, 20) });
		  autoSize.Checked = window.AutoSize;
	  };

		var resetButton = new Button { Text = "Reset" };
		resetButton.Click += (sender, e) =>
	  {
		  window.SuspendLayout();
		  bottomPanel.Items.Clear();
		  rightPanel.Items.Clear();
		  window.ResumeLayout();
		  autoSize.Checked = window.AutoSize;
	  };

		window.SizeChanged += (sender, e) => autoSize.Checked = window.AutoSize;

		var layout = new DynamicLayout();
		layout.BeginHorizontal();
		layout.BeginCentered();
		layout.Add(addRightButton);
		layout.Add(addBottomButton);
		layout.Add(resetButton);
		layout.Add(autoSize);
		layout.EndCentered();
		layout.Add(rightPanel);
		layout.EndHorizontal();
		layout.Add(bottomPanel);

		window.Content = layout;
	}, -1);



	[ManualTest]
	[TestCase(true, true, false)]
	[TestCase(true, false, true)]
	[TestCase(false, true, false)]
	[TestCase(false, false, true)]
	public void WindowShouldHaveCorrectInitialSizeWithWrappedLabel(bool useSize, bool setWidth, bool setHeight) => Async(-1, async () =>
	{
		bool wasClicked = false;

		const string infoText = "Click to change text.\n";
		var label = new Label();
		label.TextColor = Colors.White;
		label.Text = infoText + LoremGenerator.GenerateLines(2, 100);

		T window = new T();
		if (window is Form form)
			form.ShowActivated = false;
		window.AutoSize = true;
		window.BackgroundColor = Colors.Blue;
		window.Resizable = false;
		window.Content = label;

		if (useSize)
		{
			if (setWidth && setHeight)
				window.Size = new Size(150, 150);
			else if (setWidth)
				window.Size = new Size(150, -1);
			else if (setHeight)
				window.Size = new Size(-1, 150);
		}
		else
		{
			if (setWidth && setHeight)
				window.Width = window.Height = 150;
			else if (setWidth)
				window.Width = 150;
			else if (setHeight)
				window.Height = 150;
		}

		label.MouseDown += (sender, e) =>
	  {
		  var rnd = new Random();
		  label.Text = infoText + LoremGenerator.GenerateLines(rnd.Next(1, 10), rnd.Next(200));
		  wasClicked = true;
	  };

		window.Owner = Application.Instance.MainForm;
		await ShowAsync(window);
		Assert.That(wasClicked, Is.True, "#1 You need to click on it to confirm it is resized correctly");
	});

	[Test]
	public void WindowShouldReportInitialSize()
	{
		Size? size = null;
		Shown(form =>
		{
			form.Content = new Panel { Size = new Size(300, 300) };
			form.SizeChanged += (sender, e) => size = form.Size;
		},
		() =>
		{
			Assert.That(size, Is.Not.Null, "#1");
			Assert.That(size.Value.Width >= 300, Is.True, "#2");
			Assert.That(size.Value.Height >= 300, Is.True, "#3");
		});
	}

	[Test]
	public void DefaultFormValuesShouldBeCorrect()
	{
		TestProperties(f => f,
		  f => f.CanFocus,
		  f => f.ShowActivated,
		  f => f.Enabled
		);
	}

	[TestCase(true)]
	[TestCase(false)]
	[ManualTest]
	public void InitialLocationOfWindowShouldBeCorrect(bool withOwner)
	{
		ManualTest("This window should be located at the top left of the screen", form =>
		{
			if (withOwner)
				form.Owner = Application.Instance.MainForm;
			form.Location = new Point(0, 0);

			return new Panel { Size = new Size(200, 200) };
		});
	}

	[TestCase(-1)]
	[TestCase(100)] // fails on Mac and Wpf currently, due to the bottom label being wider than this size...
	[TestCase(250)]
	[ManualTest]
	public void SizeOfFormShouldWorkWithLabels(int width)
	{
		ManualTest("Window should not have large space at\nthe bottom or between labels", form =>
		{
			Label CreateLabel()
			{
				var label = new Label { Text = LoremGenerator.Generate(20) };
				if (width > 0)
					label.Width = width;
				return label;
			}

			var layout = new TableLayout();
			layout.Rows.Add(CreateLabel());
			layout.Rows.Add(CreateLabel());
			layout.Rows.Add(CreateLabel());
			layout.Rows.Add(CreateLabel());

			return layout;
		});
	}

	[Test, ManualTest]
	public void WindowFromPointShouldReturnWindowUnderPoint()
	{
		ManualTest("Move your mouse, it should show the title of the window under the mouse pointer",
		form =>
		{
			var content = new Panel { MinimumSize = new Size(100, 100) };
			var timer = new UITimer { Interval = 0.5 };
			timer.Elapsed += (sender, e) =>
		{
			var window = Window.FromPoint(Mouse.Position);
			content.Content = $"Window: {window?.Title}";
		};
			timer.Start();
			form.Closed += (sender, e) =>
		{
			timer.Stop();
		};
			form.Title = "Test Form";
			return content;
		}
		);
	}

	[TestCase(200, 200, WindowStyle.Default)]
	[TestCase(300, 200, WindowStyle.Default)]
	[TestCase(200, 300, WindowStyle.Default)]
	[TestCase(20, 20, WindowStyle.None)]
	[TestCase(100, 100, WindowStyle.None)]
	[TestCase(200, 200, WindowStyle.None)]
	[TestCase(200, 100, WindowStyle.None)]
	[TestCase(200, 100, WindowStyle.Utility)]
	[TestCase(300, 200, WindowStyle.Utility)]
	[TestCase(200, 300, WindowStyle.Utility)]
	public void GetPreferredSizeShouldWorkInOnLoadComplete(int width, int height, WindowStyle style)
	{
		GetPreferredSizeShouldWorkHelper(width, height, style, true);
	}


	[TestCase(200, 200, WindowStyle.Default)]
	[TestCase(300, 200, WindowStyle.Default)]
	[TestCase(200, 300, WindowStyle.Default)]
	[TestCase(20, 20, WindowStyle.None)]
	[TestCase(100, 100, WindowStyle.None)]
	[TestCase(200, 200, WindowStyle.None)]
	[TestCase(200, 100, WindowStyle.None)]
	[TestCase(200, 100, WindowStyle.Utility)]
	[TestCase(300, 200, WindowStyle.Utility)]
	[TestCase(200, 300, WindowStyle.Utility)]
	public void GetPreferredSizeShouldWorkBeforeShown(int width, int height, WindowStyle style)
	{
		GetPreferredSizeShouldWorkHelper(width, height, style, false);
	}

	void GetPreferredSizeShouldWorkHelper(int width, int height, WindowStyle style, bool inLoadComplete) => Async(async () =>
	{
		var padding = new Padding(10);
		var child = new Panel { Size = new Size(width, height) };
		var parent = new TableLayout
		{
			Rows = {
			" ",
			child
		  }
		};
		var window = new T
		{
			Padding = padding,
			WindowStyle = style,
			// Resizable = false,
			Content = parent
		};
		bool? visibleAfterShown = null;
		SizeF? preferredSize = null;
		Size? shownSize = null;
		Size? loadCompleteSize = null;
		Size? shownChildSize = null;
		Size? loadCompleteChildSize = null;
		if (!inLoadComplete)
			preferredSize = window.GetPreferredSize();

		window.LoadComplete += (sender, e) =>
	  {
		  loadCompleteSize = window.Size;
		  loadCompleteChildSize = child.Size;
		  if (inLoadComplete)
			  preferredSize = window.GetPreferredSize();
	  };
		window.Shown +=
		async
		 (sender, e) =>
	  {
		  shownSize = window.Size;
		  shownChildSize = child.Size;
		  await Task.Delay(100);
		  visibleAfterShown = window.Visible;
		  window.Close();
	  };

		// await Task.Delay(100);

		bool visibleBeforeShown = window.Visible;

		await ShowAsync(window);

		// Ensure content size is what was requested
		Assert.That(shownChildSize, Is.Not.Null, "#1.1 Child Size should be set on Shown");
		Assert.That(shownChildSize.Value, Is.EqualTo(new Size(width, height)), "#1.2 Child Size does not match what was requested");

		Assert.That(loadCompleteChildSize, Is.Not.Null, "#1.1 Child Size should be set on LoadComplete");
		Assert.That(loadCompleteChildSize.Value, Is.EqualTo(new Size(width, height)), "#1.2 Child Size should be set on LoadComplete");

		Assert.That(preferredSize, Is.Not.Null, "#2.1 preferredSize not set (LoadComplete not called)");

		// Preferred size will include window decorations
		Assert.That(preferredSize.Value.Width, Is.GreaterThanOrEqualTo(width + padding.Horizontal), "#2.2 Preferred width is not in range");
		Assert.That(preferredSize.Value.Height, Is.GreaterThanOrEqualTo(height + padding.Vertical), "#2.3 Preferred height is not in range");

		Assert.That(shownSize, Is.Not.Null, "#3.1 Actual size not set (Shown not called)");

		Assert.That(shownSize.Value, Is.EqualTo(Size.Round(preferredSize.Value)), "#3.2 Shown size should match preferred size");

		Assert.That(loadCompleteSize, Is.Not.Null, "#3.3 Size not set in LoadComplete");
		Assert.That(shownSize.Value, Is.EqualTo(loadCompleteSize.Value), "#3.4 Window size should be the same in both LoadComplete and Shown events");

		Assert.That(visibleAfterShown, Is.Not.Null, "#4.1 VisibleWhenShown not set");
		Assert.That(visibleAfterShown, Is.True, "#4.2 Window was not visible when shown");
		Assert.That(visibleBeforeShown, Is.False, "#4.3 Visible should not be true before shown");
	});

	[Test]
	public void ShownShouldBeCalledInCorrectOrder() => Async(async () =>
	{
		bool? parentShown = null;
		bool? childShown = null;
		bool? windowShown = null;
		bool? parentShownBeforeOtherChild = null;
		bool? windowShownBeforeChild = null;

		var child = new Label { Text = "Hello" };

		child.Shown += (sender, e) =>
	  {
		  childShown = true;
		  parentShownBeforeOtherChild = parentShown == true;
	  };

		var parent = new TableLayout { Rows = { child } };
		parent.Shown += (sender, e) =>
	  {
		  parentShown = true;
		  windowShownBeforeChild = windowShown == true;
	  };

		var window = new T();
		window.Content = parent;
		window.Shown += (sender, args) =>
	  {
		  windowShown = true;
		  window.Close();
	  };

		await ShowAsync(window);

		Assert.That(parentShown, Is.Not.Null, "#1.1");
		Assert.That(childShown, Is.Not.Null, "#1.2");
		Assert.That(windowShown, Is.Not.Null, "#1.3");
		Assert.That(parentShownBeforeOtherChild, Is.Not.Null, "#1.4");
		Assert.That(windowShownBeforeChild, Is.Not.Null, "#1.5");

		Assert.That(parentShown, Is.True, "#2.1 - Shown was not triggered for parent control");
		Assert.That(childShown, Is.True, "#2.2 - Shown was not triggered for child control");
		Assert.That(windowShown, Is.True, "#2.3 - Shown was not triggered for Window");

		Assert.That(parentShownBeforeOtherChild, Is.True, "#3.1 - Parent should call Shown before its Child was called");
		Assert.That(windowShownBeforeChild, Is.False, "#3.2 - Window called Shown before Child was called");
	});

	[Test]
	[ManualTest]
	public void WindowShouldNotBeShowingDuringLoadComplete() => Async(-1, async () =>
	{
		bool? loadComplete = null;
		bool? shown = null;

		var window = new T();
		window.Content = "Form should shown entriely\nwithout being blank first.\n\nClick to close.";
		window.Width = 200;
		window.LoadComplete += (sender, e) =>
		{
			loadComplete = window.Visible;
			Thread.Sleep(2000);
		};
		window.MouseDown += (sender, e) => window.Close();
		window.Shown += (sender, e) =>
		{
			shown = window.Visible;
		};

		await ShowAsync(window);

		Assert.That(loadComplete, Is.True, "#1.1 - Window should be visible during LoadComplete");
		Assert.That(shown, Is.True, "#1.2 - Window should be visible during Shown");
	});

	[Test]
	[ManualTest]
	public void HandlingTextInputOnWindowShouldNotPreventShortcuts()
	{
		bool? copyCalled = null;
		bool? keyDownInControl = null;
		bool? keyDownInWindow = null;
		ManualTest(Platform.Instance.IsMac ? "Press CMD+C" : "Press Ctrl+C", form =>
		{
			// var ctl = new Drawable { CanFocus = true, Size = new Size(200, 200), BackgroundColor = Colors.Blue };
			var ctl = new WebView { Size = new Size(200, 200) };
			ctl.KeyDown += (sender, e) =>
			{
				if (e.KeyData == (Application.Instance.CommonModifier | Keys.C))
				{
					keyDownInControl = true;
				}
			};
			ctl.TextInput += (sender, e) =>
			{
				Log.Write(ctl, "TextInput called!");
			};

			form.KeyDown += (sender, e) =>
			{
				if (e.KeyData == (Application.Instance.CommonModifier | Keys.C))
				{
					keyDownInWindow = true;
				}
			};
			form.TextInput += (sender, e) =>
			{
				Log.Write(form, "TextInput called!");
			};

			void Close(object sender, EventArgs e)
			{
				form.Close();
			}

			void Copy(object sender, EventArgs e)
			{
				Log.Write(null, "Copy called!");
				copyCalled = true;
				form.Close();
			}

			ctl.DocumentLoaded += (sender, e) =>
			{
				ctl.Focus();
			};
			ctl.LoadHtml("<html><body><p>Test</p></body></html>");

			var bar = new MenuBar
			{
				IncludeSystemItems = MenuBarSystemItems.Quit,
				Items = {
					new SubMenuItem { Text = "Close", Items = {
							new ButtonMenuItem(Close) { Text = "Close", Shortcut = Application.Instance.CommonModifier | Keys.W }
						}
					},
					new SubMenuItem { Text = "Edit", Items = {
							new ButtonMenuItem(Copy) { Text = "Copy", Shortcut = Application.Instance.CommonModifier | Keys.C }
						}
					}
				}
			};

			form.Menu = bar;


			return ctl;
		});
		Assert.That(copyCalled, Is.True, "#1 - Copy menuItem was not called");
		if (Platform.Instance.IsMac || Platform.Instance.IsGtk)
		{
			// in case this gets "fixed" later to behave like other platforms.
			// Gtk and Mac prioritize menu shortcuts over control key events
			Assert.That(keyDownInControl, Is.Null, "#2 - KeyDown is not called on control on Mac or Gtk with a menu shortcut");
			Assert.That(keyDownInWindow, Is.Null, "#3 - KeyDown is not called on Window on Mac or Gtk with a menu shortcut");
		}
		else
		{
			Assert.That(keyDownInControl, Is.True, "#2 - KeyDown was not called in control");
			Assert.That(keyDownInWindow, Is.True, "#3 - KeyDown was not called in window");
		}
	}
	
	[ManualTest]
	[TestCase(0x00000000)] // transparent
	[TestCase(0x017F7F7F)] // almost transparent, should allow window to move
	[TestCase(0x7F007F00)] // semi-transparent green
	[TestCase(unchecked((int)0xFF7F7F7F))] // non-transparent
	public void TransparentWindowShouldWork(int argb) => Async(-1, async () =>
	{
		var closeButton = new Button { Text = "Close" };
		var window = new T
		{
			Owner = Application.Instance.MainForm,
			WindowStyle = WindowStyle.None,
			BackgroundColor = Color.FromArgb(argb),
			MovableByWindowBackground = true,
			Size = new Size(400, 300),
			Topmost = true,
			Content = new TableLayout
			{
				Rows = {
					new Label {
						BackgroundColor = Colors.Blue,
						Text = "There should be two blue bars with space in between\nClicking in the space should click through the window on Wpf/Mac\nWinForms does not support this.",
						VerticalAlignment = VerticalAlignment.Center,
						TextAlignment = TextAlignment.Center,
					 	TextColor = Colors.White
						},
					null,
					new Panel { 
						BackgroundColor = Colors.Blue, 
						Height = 40,
						Content = TableLayout.AutoSized(closeButton, centered: true)
						}
				}
			}
		};
		closeButton.Click += (sender, e) => window.Close();

		await ShowAsync(window);
	});
}
