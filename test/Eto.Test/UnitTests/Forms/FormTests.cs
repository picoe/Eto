using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms;

[TestFixture]
public class FormTests : WindowTests<Form>
{
	protected override void Test(Action<Form> test, int timeout = 4000) => Form(test, timeout);
	protected override void ManualTest(string message, Func<Form, Control> test) => ManualForm(message, test);
	protected override void Show(Form window) => window.Show();
	protected override Task ShowAsync(Form window) => window.ShowAsync();

	[Test, ManualTest]
	public void WindowShouldCloseOnLostFocusWithoutHidingParent()
	{
		ManualForm("Click on this window after the child is shown,\nthe form and the main form should not go behind other windows",
		form =>
		{
			var content = new Panel { MinimumSize = new Size(100, 100) };
			form.Shown += (sender, e) =>
			{
				var childForm = new Form
				{
					Title = "Child Form",
					ClientSize = new Size(100, 100),
					Owner = form
				};
				childForm.MouseDown += (s2, e2) => childForm.Close();
				childForm.LostFocus += (s2, e2) => childForm.Close();
				childForm.Show();
			};
			form.Title = "Test Form";
			form.Owner = Application.Instance.MainForm;
			return content;
		}
		);
	}

	// Hm, this seems useful.. should it be added as an extension method somewhere?
	static Task EventAsync<TWidget, TEvent>(TWidget control, Action<TWidget, EventHandler<TEvent>> addHandler, Action<TWidget, EventHandler<TEvent>> removeHandler = null)
		where TWidget : Widget
	{
		var mre = new TaskCompletionSource<bool>();
		void EventTriggered(object sender, TEvent e)
		{
			removeHandler?.Invoke(control, EventTriggered);
			mre.TrySetResult(true);
		}

		addHandler(control, EventTriggered);
		return mre.Task;
	}

	[Test, ManualTest]
	public void MultipleChildWindowsShouldGetFocusWhenClicked() => Async(-1, async () =>
	{
		var form1 = new Form { ClientSize = new Size(200, 200), Location = new Point(300, 300) };
		form1.Owner = Application.Instance.MainForm;
		form1.Title = "Form1";
		form1.Content = new Label
		{
			VerticalAlignment = VerticalAlignment.Center,
			TextAlignment = TextAlignment.Center,
			Text = "Click on Form2, it should then get focus and be on top of this form."
		};
		// var form1ClosedTask = EventTask<EventArgs>(h => form1.Closed += h);
		var form1ClosedTask = EventAsync<Form, EventArgs>(form1, (c, h) => c.Closed += h);

		var form2 = new Form { ClientSize = new Size(200, 200), Location = new Point(400, 400) };
		form2.Owner = Application.Instance.MainForm;
		form2.Title = "Form2";
		form2.Content = new Label
		{
			VerticalAlignment = VerticalAlignment.Center,
			TextAlignment = TextAlignment.Center,
			Text = "Click on Form1, it should then get focus and be on top of this form."
		};
		var form2ClosedTask = EventAsync<Form, EventArgs>(form2, (c, h) => c.Closed += h);

		form1.Show();

		form2.Show();

		// wait till both forms are closed..
		await Task.WhenAll(form1ClosedTask, form2ClosedTask);
	});

	public class SubSubForm : SubForm
	{
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		}
	}

	public class SubForm : Form
	{
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		}
	}

	[Test]
	public void ClosedEventShouldFireOnceWithMultipleSubclasses()
	{
		int closed = 0;
		Form<SubSubForm>(form =>
		{
			form.Content = new Panel { Size = new Size(300, 300) };
			form.Closed += (sender, e) => closed++;
			form.Shown += (sender, e) => form.Close();
		});
		Assert.That(closed, Is.EqualTo(1), "Closed event should only fire once");
	}

	[Test]
	public void AllowFullScreenToggleShouldDefaultToPlatformConvention()
	{
		Invoke(() =>
		{
			var form = new Form();
			Assert.That(form.AllowFullScreenToggle, Is.EqualTo(Platform.Instance.IsMac), "Should only be allowed by default on macOS");
		});
	}

	class KeyForm : Form
	{
		public bool SendKey(Keys key)
		{
			var args = new KeyEventArgs(key, KeyEventType.KeyDown);
			OnKeyDown(args);
			return args.Handled;
		}
	}

	// skips when the window manager never changes the state at all, e.g. under Xvfb in CI which has none
	class StateTracker
	{
		readonly Window window;
		bool changed;

		public StateTracker(Window window)
		{
			this.window = window;
			window.WindowStateChanged += (sender, e) => changed = true;
		}

		public void AssertState(WindowState state)
		{
			if (!changed && window.WindowState != state)
				Assert.Ignore($"The window manager did not change the window to {state}");
			Assert.That(window.WindowState, Is.EqualTo(state), $"Should be {state}");
		}
	}

	[TestCase(WindowState.Normal)]
	[TestCase(WindowState.Maximized)]
	public void F11ShouldToggleFullScreenWhenAllowed(WindowState initialState) => Async(20000, async () =>
	{
		if (Platform.Instance.IsMac)
			Assert.Ignore("macOS uses the system full screen button and shortcut");

		var form = new KeyForm { Content = "F11", ClientSize = new Size(300, 200) };
		var shown = new TaskCompletionSource<bool>();
		var states = new StateTracker(form);
		form.Shown += (sender, e) => shown.TrySetResult(true);

		async Task WaitForState(WindowState state)
		{
			for (int i = 0; i < 50 && form.WindowState != state; i++)
				await Task.Delay(100);
			states.AssertState(state);
		}

		try
		{
			form.Show();
			await shown.Task;
			form.WindowState = initialState;
			await WaitForState(initialState);

			Assert.That(form.SendKey(Keys.F11), Is.False, "F11 should not be handled when not allowed");
			await Task.Delay(200);
			Assert.That(form.WindowState, Is.EqualTo(initialState), "F11 should do nothing when not allowed");

			form.AllowFullScreenToggle = true;
			Assert.That(form.SendKey(Keys.F11 | Keys.Control), Is.False, "Only F11 without modifiers should toggle");
			Assert.That(form.SendKey(Keys.F11), Is.True, "F11 should be handled");
			await WaitForState(WindowState.FullScreen);
			Assert.That(form.SendKey(Keys.F11), Is.True, "F11 should be handled");
			await WaitForState(initialState);

			form.AllowFullScreenToggle = false;
			Assert.That(form.SendKey(Keys.F11), Is.False, "F11 should not be handled after disallowing");
		}
		finally
		{
			form.Close();
		}
	});

	[TestCase(WindowState.Normal)]
	[TestCase(WindowState.Maximized)]
	[TestCase(WindowState.Minimized)]
	public void FullScreenShouldCoverScreenAndRestore(WindowState exitState) => Async(20000, async () =>
	{
		var form = new Form
		{
			Content = "Full screen",
			Location = new Point(100, 100),
			ClientSize = new Size(300, 200)
		};
		var shown = new TaskCompletionSource<bool>();
		var states = new StateTracker(form);
		form.Shown += (sender, e) => shown.TrySetResult(true);
		var stateChanges = new List<WindowState>();
		form.WindowStateChanged += (sender, e) => stateChanges.Add(form.WindowState);

		// some platforms animate the transition, so wait for it to settle
		async Task SetState(WindowState state, Func<bool> settled = null)
		{
			form.WindowState = state;
			for (int i = 0; i < 50 && (form.WindowState != state || settled?.Invoke() == false); i++)
				await Task.Delay(100);
			states.AssertState(state);
		}

		try
		{
			form.Show();
			await shown.Task;
			var normalBounds = form.Bounds;

			// compared against a maximized window as some backends report screen bounds in different units
			await SetState(WindowState.Maximized);
			var maximizedBounds = form.Bounds;
			await SetState(WindowState.Normal, () => form.Bounds == normalBounds);
			stateChanges.Clear();

			// macOS reports the state before the transition finishes, so also wait for the event
			await SetState(WindowState.FullScreen, () => stateChanges.Count > 0 && form.Bounds.Height >= maximizedBounds.Height);
			var fullScreenBounds = form.Bounds;
			// full screen on a Mac with a notch stays below it, so is no taller than maximized when the dock isn't taking space
			if (Platform.Instance.IsMac)
				Assert.That(fullScreenBounds.Height, Is.GreaterThanOrEqualTo(maximizedBounds.Height), "Full screen should cover the dock");
			else
				Assert.That(fullScreenBounds.Height, Is.GreaterThan(maximizedBounds.Height), "Full screen should cover the taskbar/dock");
			Assert.That(fullScreenBounds.Width, Is.GreaterThanOrEqualTo(maximizedBounds.Width - 20), "Full screen should cover the width of the screen");
			Assert.That(form.WindowStyle, Is.EqualTo(WindowStyle.Default), "Style should still report the original value");
			Assert.That(stateChanges, Is.EqualTo(new[] { WindowState.FullScreen }), "Should raise a single state change when entering full screen");

			stateChanges.Clear();
			// a minimized window's bounds are platform specific
			await SetState(exitState, () => stateChanges.Count > 0 && (exitState == WindowState.Minimized || form.Bounds != fullScreenBounds));
			// catch a duplicate event raised once the exit animation finishes
			await Task.Delay(1000);
			if (exitState == WindowState.Normal)
				Assert.That(form.Bounds, Is.EqualTo(normalBounds), "Should restore the original bounds");
			else if (exitState == WindowState.Maximized)
				Assert.That(form.Bounds, Is.EqualTo(maximizedBounds), "Should be maximized with a border");
			Assert.That(form.RestoreBounds, Is.EqualTo(normalBounds), "Restore bounds should be the original bounds");
			Assert.That(stateChanges, Is.EqualTo(new[] { exitState }), "Should raise a single state change when leaving full screen");

			if (exitState == WindowState.Minimized)
			{
				await SetState(WindowState.Normal, () => form.Bounds == normalBounds);
				Assert.That(form.Bounds, Is.EqualTo(normalBounds), "Should restore the original bounds after minimizing");
			}
		}
		finally
		{
			form.Close();
		}
	});

	[Test]
	public void RestoreBoundsShouldBeNormalBoundsWhileFullScreen() => Async(20000, async () =>
	{
		var form = new Form
		{
			Content = "Full screen",
			Location = new Point(100, 100),
			ClientSize = new Size(300, 200)
		};
		var shown = new TaskCompletionSource<bool>();
		var states = new StateTracker(form);
		form.Shown += (sender, e) => shown.TrySetResult(true);
		// apps often save RestoreBounds when the state changes, so it should be correct during the event
		var restoreBoundsInEvents = new List<Rectangle>();
		form.WindowStateChanged += (sender, e) => restoreBoundsInEvents.Add(form.RestoreBounds);

		async Task SetState(WindowState state)
		{
			restoreBoundsInEvents.Clear();
			form.WindowState = state;
			for (int i = 0; i < 50 && (form.WindowState != state || restoreBoundsInEvents.Count == 0); i++)
				await Task.Delay(100);
			await Task.Delay(500);
			states.AssertState(state);
		}

		try
		{
			form.Show();
			await shown.Task;
			// let the window manager finish decorating the window
			await Task.Delay(500);
			var normalBounds = form.Bounds;

			// entering full screen first, as decorations added after showing used to throw it off
			await SetState(WindowState.FullScreen);
			Assert.That(form.RestoreBounds, Is.EqualTo(normalBounds), "Restore bounds should be the normal bounds while full screen");
			Assert.That(restoreBoundsInEvents, Is.All.EqualTo(normalBounds), "Restore bounds should be the normal bounds when entering full screen");

			await SetState(WindowState.Normal);
			Assert.That(restoreBoundsInEvents, Is.All.EqualTo(normalBounds), "Restore bounds should be the normal bounds when leaving full screen");
			Assert.That(form.RestoreBounds, Is.EqualTo(normalBounds), "Restore bounds should be the normal bounds after leaving full screen");
		}
		finally
		{
			form.Close();
		}
	});

	[TestCase(WindowState.Maximized, WindowState.Normal)]
	[TestCase(WindowState.Minimized, WindowState.FullScreen)]
	public void FullScreenShouldGoDirectlyToNewState(WindowState viaState, WindowState finalState) => Async(20000, async () =>
	{
		var form = new Form
		{
			Content = "Full screen",
			Location = new Point(100, 100),
			ClientSize = new Size(300, 200)
		};
		var shown = new TaskCompletionSource<bool>();
		var states = new StateTracker(form);
		form.Shown += (sender, e) => shown.TrySetResult(true);
		var stateChanges = new List<WindowState>();
		form.WindowStateChanged += (sender, e) => stateChanges.Add(form.WindowState);

		async Task SetState(WindowState state)
		{
			stateChanges.Clear();
			form.WindowState = state;
			for (int i = 0; i < 50 && (form.WindowState != state || stateChanges.Count == 0); i++)
				await Task.Delay(100);
			// catch a transient or duplicate event raised once the transition finishes
			await Task.Delay(1000);
			states.AssertState(state);
			Assert.That(stateChanges, Is.EqualTo(new[] { state }), $"Should raise a single state change when going to {state}");
		}

		try
		{
			form.Show();
			await shown.Task;
			var normalBounds = form.Bounds;

			if (viaState == WindowState.Maximized)
			{
				await SetState(WindowState.Maximized);
				await SetState(WindowState.FullScreen);
			}
			else
			{
				await SetState(WindowState.FullScreen);
				await SetState(viaState);
			}
			await SetState(finalState);
			if (finalState == WindowState.Normal)
				Assert.That(form.Bounds, Is.EqualTo(normalBounds), "Should restore the original bounds");
		}
		finally
		{
			form.Close();
		}
	});

	[TestCase(true)]
	[TestCase(false)]
	[ManualTest]
	public void CallingShowTwiceShouldWork(bool showActivated) => Async(-1, async () =>
	{
		var form = new Form();

		form.Content = "Click on this form.  It should hide then show again";
		form.Size = new Size(200, 200);
		form.ShowActivated = showActivated;

		form.Show();

		var tcs = new TaskCompletionSource<bool>(false);

		form.MouseDown += async (sender, e) =>
		{
			form.Visible = false;
			await Task.Delay(1000);
			form.Show();
		};

		form.Closed += (sender, e) => tcs.SetResult(true);

		await tcs.Task;
	});

	[Test]
	[ManualTest]
	public void CallingShowAfterShownShouldNotBringItTopMost() => Async(-1, async () =>
	{
		var form = new Form();

		form.Content = "Click on the main form.  This form should not come back on top.";
		form.Size = new Size(200, 200);

		form.Show();

		var tcs = new TaskCompletionSource<bool>(false);

		form.LostFocus += async (sender, e) =>
		{
			await Task.Delay(1000);
			form.Show();
			await Task.Delay(1000);
			form.Close();
		};

		form.Closed += (sender, e) => tcs.SetResult(true);

		await tcs.Task;
	});
	
	[Test]
	public void LostFocusShouldNotFireAfterDispose() => Async(async () =>
	{
		// A popup closed and thrown away in one go must not get a late LostFocus, its handler can't touch a disposed window.
		var form = new Form { ClientSize = new Size(200, 200) };
		var lostFocusAfterDispose = false;
		form.LostFocus += (sender, e) => lostFocusAfterDispose |= form.IsDisposed;
		form.Show();
		await WaitUntil(() => form.HasFocus);
		Assert.That(form.HasFocus, Is.True, "Form must be active, or closing it won't take focus away");

		form.Close();
		form.Dispose();

		// waiting for something that should not happen, so a fixed delay is the point
		await Task.Delay(250);
		Assert.That(lostFocusAfterDispose, Is.False, "LostFocus should not be raised on a disposed form");
	});

	class MyModel
	{
		public int IntValue { get; set; }
	}
	
	[Test]
	public void HiddenFormShouldNotShowWhenSettingOwner() => Async(async () => {
		var child = new Form();
		var parent = new Form();
		try
		{
			child.Content = "This form should only show briefly";
			child.Size = new Size(200, 200);
			child.Location = new Point(100, 100);
			child.Show();
			child.Visible = false;

			parent.Content = "This form should show";
			parent.Size = new Size(200, 200);
			parent.Location = new Point(400, 400);
			parent.Show();
			await WaitUntil(() => parent.Visible, 250);
			child.Owner = parent;
			// setting the owner must not un-hide the child, so this one stays a fixed delay - there is
			// no settled state to poll for, we're waiting to confirm that nothing happens.
			await Task.Delay(250);
			
			Assert.That(child.Visible, Is.False, "Child should not be visible");
			Assert.That(parent.Visible, Is.True, "Parent should be visible");
			
			// Set visible and test
			child.Visible = true;
			await WaitUntil(() => child.Visible, 250);
			Assert.That(child.Visible, Is.True, "Child should be visible");
			Assert.That(parent.Visible, Is.True, "Parent should be visible");
			
			// Hide again
			child.Visible = false;
			await WaitUntil(() => !child.Visible, 250);

			Assert.That(child.Visible, Is.False, "Child should not be visible");
			Assert.That(parent.Visible, Is.True, "Parent should be visible");
		}
		finally
		{
			// wait for them to actually be gone - a window still on screen when the next test starts
			// stops that test's window from becoming active
			await CloseAsync(child);
			await CloseAsync(parent);
		}
	});

	// Whether an event is attached to the platform is tracked by this marker in the widget's property
	// store. There is no public way to ask, and the whole point of RemoveHandlerEvent is that the
	// answer changes, so the tests below look at it directly.
	static bool IsAttached(Window window, string id) => window.Properties.ContainsKey(id + ".Instance");

	[Test]
	public void PreviewKeyDownShouldDetachWhenLastSubscriberRemoved()
	{
		Invoke(() =>
		{
			var form = new Form();
			try
			{
				void First(object sender, KeyMonitorEventArgs e) { }
				void Second(object sender, KeyMonitorEventArgs e) { }

				Assert.That(IsAttached(form, Window.PreviewKeyDownEvent), Is.False, "Should not be attached before anything subscribes");

				form.PreviewKeyDown += First;
				Assert.That(IsAttached(form, Window.PreviewKeyDownEvent), Is.True, "Should attach for the first subscriber");

				form.PreviewKeyDown += Second;
				form.PreviewKeyDown -= Second;
				Assert.That(IsAttached(form, Window.PreviewKeyDownEvent), Is.True, "Should stay attached while a subscriber remains");

				form.PreviewKeyDown -= First;
				Assert.That(IsAttached(form, Window.PreviewKeyDownEvent), Is.False, "Should detach when the last subscriber goes away");

				// and it should come back, not be stuck detached
				form.PreviewKeyDown += First;
				Assert.That(IsAttached(form, Window.PreviewKeyDownEvent), Is.True, "Should attach again on a later subscription");
				form.PreviewKeyDown -= First;
			}
			finally
			{
				form.Dispose();
			}
		});
	}

	[Test]
	public void PreviewKeyDownAndUpShouldDetachIndependently()
	{
		Invoke(() =>
		{
			var form = new Form();
			try
			{
				void OnKey(object sender, KeyMonitorEventArgs e) { }

				form.PreviewKeyDown += OnKey;
				form.PreviewKeyUp += OnKey;
				Assert.That(IsAttached(form, Window.PreviewKeyUpEvent), Is.True, "Key up should attach");

				// the backends share one registration between the two, so dropping one must not
				// stop the other from being reported
				form.PreviewKeyDown -= OnKey;
				Assert.That(IsAttached(form, Window.PreviewKeyDownEvent), Is.False, "Key down should detach");
				Assert.That(IsAttached(form, Window.PreviewKeyUpEvent), Is.True, "Key up should still be attached");

				form.PreviewKeyUp -= OnKey;
				Assert.That(IsAttached(form, Window.PreviewKeyUpEvent), Is.False, "Key up should detach once it is dropped too");
			}
			finally
			{
				form.Dispose();
			}
		});
	}

	[Test]
	public void PreviewKeyDownShouldBeReleasedWhenClosed()
	{
		// Backends keep their key monitor in process wide state - a thread wide hook on Windows, an
		// AppKit event monitor on macOS - and let go of it in OnUnLoad. Closing has to be enough,
		// since Dispose isn't guaranteed to run, so pin down that closing really does unload.
		Invoke(() =>
		{
			var form = new Form { ClientSize = new Size(100, 100) };
			form.PreviewKeyDown += (sender, e) => { };
			try
			{
				form.Show();
				Assert.That(form.Loaded, Is.True, "Form should be loaded once shown");

				form.Close();
				Assert.That(form.Loaded, Is.False, "Closing should unload the form so the key monitor is released");
			}
			finally
			{
				form.Dispose();
			}
		});
	}
}