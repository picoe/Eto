namespace Eto.Test.Mac.UnitTests;

/// <summary>
/// Synthesizes a click on a control, including controls that run a mouse tracking loop of their own and so
/// can't be exercised by sending a mouse down on its own.
/// </summary>
static class ClickEvents
{
	/// <summary>
	/// Clicks the center of <paramref name="control"/> and returns once it has been handled.  The mouse up is
	/// queued rather than sent, so that a control tracking the mouse until it is released finds it there like it
	/// would for a real click.
	/// </summary>
	public static void Click(Control control)
	{
		var view = control.ControlObject as NSView ?? throw new ArgumentException("Control is not backed by an NSView", nameof(control));
		var window = view.Window ?? throw new ArgumentException("Control is not in a window", nameof(control));

		var location = view.ConvertPointToView(new CGPoint(view.Bounds.GetMidX(), view.Bounds.GetMidY()), null);
		var app = NSApplication.SharedApplication;

		// a control that doesn't track the mouse leaves the mouse up in the queue, where it would otherwise be
		// picked up by the next click
		Flush(app);

		app.PostEvent(CreateEvent(NSEventType.LeftMouseUp, location, window), false);
		app.SendEvent(CreateEvent(NSEventType.LeftMouseDown, location, window));

		Flush(app);
	}

	const NSEventMask MouseEvents = NSEventMask.LeftMouseDown | NSEventMask.LeftMouseUp | NSEventMask.LeftMouseDragged;

	static void Flush(NSApplication app)
	{
		while (app.NextEvent(MouseEvents, NSDate.Now, NSRunLoopMode.Default, true) != null)
		{
		}
	}

	static NSEvent CreateEvent(NSEventType type, CGPoint location, NSWindow window) =>
		NSEvent.MouseEvent(type, location, 0, NSDate.Now.SecondsSinceReferenceDate, window.WindowNumber, null, 0, 1, 1);
}
