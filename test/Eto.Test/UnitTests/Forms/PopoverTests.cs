using Eto.Forms.ThemedControls;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms;

[TestFixture]
public class PopoverTests : TestBase
{
	static Platform s_themedPlatform;

	static Popover CreatePopover(bool themed)
	{
		if (!themed)
			return new Popover();
		if (s_themedPlatform == null)
		{
			s_themedPlatform = (Platform)Activator.CreateInstance(Platform.Instance.GetType());
			s_themedPlatform.Add<Popover.IHandler>(() => new ThemedPopoverHandler());
		}
		using (s_themedPlatform.Context)
			return new Popover();
	}

	// Popover raises its events inside its platform's context, so resuming inline would leave the themed platform current
	static Task<EventArgs> WaitPopoverEventAsync(Action<EventHandler<EventArgs>> hookEvent)
	{
		var tcs = new TaskCompletionSource<EventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
		hookEvent((sender, e) => tcs.TrySetResult(e));
		return tcs.Task;
	}

	static Button CreateAnchor(Form form)
	{
		var button = new Button { Text = "Anchor" };
		// Gtk draws its popover inside the window on X11, so leave room for it on every side
		form.Content = new TableLayout { Padding = 200, Rows = { button } };
		return button;
	}

	[TestCase(false)]
	[TestCase(true)]
	public void ContentShouldLoadWhenShownAndUnloadWhenClosedEachTime(bool themed)
	{
		ShownAsync(CreateAnchor, async anchor =>
		{
			using var popover = CreatePopover(themed);
			var content = new Label { Text = "Popover content" };
			popover.Content = content;

			for (int i = 0; i < 2; i++)
			{
				var shown = WaitPopoverEventAsync(h => popover.Shown += h);
				popover.Show(anchor);
				await shown;
				Assert.That(popover.IsOpen, Is.True, "IsOpen after shown");
				Assert.That(content.Loaded, Is.True, "Content should be loaded while shown");
				Assert.That(content.Width, Is.GreaterThan(0), "Content should be given a size");

				var closed = WaitPopoverEventAsync(h => popover.Closed += h);
				popover.Close();
				await closed;
				Assert.That(popover.IsOpen, Is.False, "IsOpen after closed");
				Assert.That(content.Loaded, Is.False, "Content should be unloaded once closed");
			}
		});
	}

	[TestCase(false, PopoverPosition.Top)]
	[TestCase(false, PopoverPosition.Bottom)]
	[TestCase(false, PopoverPosition.Left)]
	[TestCase(false, PopoverPosition.Right)]
	[TestCase(true, PopoverPosition.Top)]
	[TestCase(true, PopoverPosition.Bottom)]
	[TestCase(true, PopoverPosition.Left)]
	[TestCase(true, PopoverPosition.Right)]
	public void ContentShouldBeOnTheRequestedSideOfTheAnchor(bool themed, PopoverPosition position)
	{
		ShownAsync(CreateAnchor, async anchor =>
		{
			using var popover = CreatePopover(themed);
			var content = new Panel { Size = new Size(80, 40) };
			popover.Content = content;

			var shown = WaitPopoverEventAsync(h => popover.Shown += h);
			popover.Show(anchor, position);
			await shown;

			var anchorBounds = anchor.RectangleToScreen(new RectangleF(anchor.Size));
			var contentBounds = content.RectangleToScreen(new RectangleF(content.Size));
			// Gtk lets the content overlap the anchor's edge slightly, so check its center
			var center = contentBounds.Center;
			switch (position)
			{
				case PopoverPosition.Top:
					Assert.That(center.Y, Is.LessThan(anchorBounds.Top));
					break;
				case PopoverPosition.Bottom:
					Assert.That(center.Y, Is.GreaterThan(anchorBounds.Bottom));
					break;
				case PopoverPosition.Left:
					Assert.That(center.X, Is.LessThan(anchorBounds.Left));
					break;
				case PopoverPosition.Right:
					Assert.That(center.X, Is.GreaterThan(anchorBounds.Right));
					break;
			}

			var closed = WaitPopoverEventAsync(h => popover.Closed += h);
			popover.Close();
			await closed;
		});
	}

	class PopoverModel
	{
		public string Text { get; set; }
	}

	[TestCase(false)]
	[TestCase(true)]
	public void ContentShouldInheritDataContext(bool themed)
	{
		ShownAsync(CreateAnchor, async anchor =>
		{
			using var popover = CreatePopover(themed);
			var label = new Label();
			label.TextBinding.BindDataContext((PopoverModel m) => m.Text);
			popover.Content = new Panel { Content = label };
			popover.DataContext = new PopoverModel { Text = "First" };
			Assert.That(label.Text, Is.EqualTo("First"), "Before shown");

			var shown = WaitPopoverEventAsync(h => popover.Shown += h);
			popover.Show(anchor);
			await shown;
			Assert.That(label.Text, Is.EqualTo("First"), "While shown");
			Assert.That(label.FindParent<Popover>(), Is.SameAs(popover), "Popover should be a logical parent of its content");

			var second = new PopoverModel { Text = "Second" };
			popover.DataContext = second;
			Assert.That(label.Text, Is.EqualTo("Second"), "Changed while shown");

			var closed = WaitPopoverEventAsync(h => popover.Closed += h);
			popover.Close();
			await closed;
			Assert.That(label.DataContext, Is.SameAs(second), "After closed");

			popover.Content = null;
			Assert.That(label.DataContext, Is.Null, "After content removed");
		});
	}

	[TestCase(false)]
	[TestCase(true)]
	public void ClosingTheWindowShouldClosePopover(bool themed)
	{
		Async(async () =>
		{
			var form = new Form();
			var anchor = CreateAnchor(form);
			var formShown = WaitEventAsync<EventArgs>(h => form.Shown += h);
			var formClosed = form.ShowAsync();
			await formShown;

			using var popover = CreatePopover(themed);
			popover.Content = new Label { Text = "Popover content" };
			var shown = WaitPopoverEventAsync(h => popover.Shown += h);
			popover.Show(anchor);
			await shown;

			var closed = WaitPopoverEventAsync(h => popover.Closed += h);
			form.Close();
			await formClosed;
			await closed;
			Assert.That(popover.IsOpen, Is.False);
		});
	}

	[Test]
	public void ShowingFromControlNotInAWindowShouldThrow()
	{
		Invoke(() =>
		{
			using var popover = new Popover { Content = new Label { Text = "Popover content" } };
			Assert.Throws<InvalidOperationException>(() => popover.Show(new Button()));
		});
	}
}
