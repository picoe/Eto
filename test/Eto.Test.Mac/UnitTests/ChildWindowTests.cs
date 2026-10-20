using System.Runtime.InteropServices;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	/// <summary>
	/// With "Displays have separate Spaces", the window server hides a child window dragged to a different screen
	/// than its parent, so these need at least two screens to run.
	/// </summary>
	[TestFixture]
	public class ChildWindowTests : TestBase
	{
		const string CoreGraphicsLibrary = "/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices";

		[DllImport(CoreGraphicsLibrary)]
		static extern IntPtr CGEventCreateMouseEvent(IntPtr source, int mouseType, CGPoint mouseCursorPosition, int mouseButton);

		[DllImport(CoreGraphicsLibrary)]
		static extern void CGEventPost(int tap, IntPtr evt);

		[DllImport(CoreGraphicsLibrary)]
		static extern bool AXIsProcessTrusted();

		[DllImport(CoreGraphicsLibrary)]
		static extern void CFRelease(IntPtr obj);

		const int kCGEventLeftMouseDown = 1;
		const int kCGEventLeftMouseUp = 2;
		const int kCGEventMouseMoved = 5;
		const int kCGEventLeftMouseDragged = 6;

		static void PostMouseEvent(int type, PointF location)
		{
			var evt = CGEventCreateMouseEvent(IntPtr.Zero, type, new CGPoint(location.X, location.Y), 0);
			CGEventPost(0, evt);
			CFRelease(evt);
		}

		// posted from a background thread so the UI thread can track the drag
		static Task DragMouse(PointF from, PointF to) => Task.Run(() =>
		{
			PostMouseEvent(kCGEventMouseMoved, from);
			Thread.Sleep(100);
			PostMouseEvent(kCGEventLeftMouseDown, from);
			Thread.Sleep(200);
			const int steps = 30;
			for (int i = 1; i <= steps; i++)
			{
				PostMouseEvent(kCGEventLeftMouseDragged, from + (to - from) * ((float)i / steps));
				Thread.Sleep(20);
			}
			Thread.Sleep(200);
			PostMouseEvent(kCGEventLeftMouseUp, to);
		});

		// AppKit and the window list still report a hidden child window as visible, so hit test it instead
		static bool IsOnScreen(NSWindow window)
		{
			var frame = window.Frame;
			return NSWindow.WindowNumberAtPoint(new CGPoint(frame.GetMidX(), frame.GetMidY()), 0) == window.WindowNumber;
		}

		static void GetScreens(out Rectangle primaryArea, out Rectangle otherArea)
		{
			var primary = Screen.PrimaryScreen;
			var other = Screen.Screens.FirstOrDefault(r => r.Bounds != primary.Bounds);
			if (other == null)
				Assert.Ignore("Requires more than one screen");
			if (!NSScreen.ScreensHaveSeparateSpaces())
				Assert.Ignore("Requires \"Displays have separate Spaces\" to be enabled");
			primaryArea = Rectangle.Round(primary.WorkingArea);
			otherArea = Rectangle.Round(other.WorkingArea);
		}

		static async Task WithOwnedForm(Rectangle area, Func<Form, Form, Task> test)
		{
			var owner = new Form { Title = "Owner", Size = new Size(300, 200) };
			var child = new Form { Title = "Child", Size = new Size(200, 150), Content = "Child" };
			try
			{
				owner.Location = area.Location + 100;
				owner.Show();
				child.Owner = owner;
				child.Location = area.Location + 150;
				child.Show();
				await test(owner, child);
			}
			finally
			{
				await CloseAsync(child);
				await CloseAsync(owner);
			}
		}

		static bool IsChildOf(Form child, Form owner) => Equals(((NSWindow)child.ControlObject).ParentWindow, owner.ControlObject);

		[Test]
		public void ChildWindowShouldDetachWhenMovedToAnotherScreen() => Async(15000, async () =>
		{
			GetScreens(out var primaryArea, out var otherArea);
			await WithOwnedForm(primaryArea, async (owner, child) =>
			{
				await WaitUntil(() => IsChildOf(child, owner), 1000);
				Assert.That(IsChildOf(child, owner), Is.True, "#1 Should be a child window on the owner's screen");

				child.Location = otherArea.Location + 50;
				await WaitUntil(() => !IsChildOf(child, owner), 1000);
				Assert.That(IsChildOf(child, owner), Is.False, "#2.1 Should be detached from the owner on another screen");

				var childLocation = child.Location;
				owner.Location += 20;
				await Task.Delay(100);
				Assert.That(child.Location, Is.EqualTo(childLocation), "#2.2 Should not move with the owner on another screen");

				child.Location = primaryArea.Location + 150;
				await WaitUntil(() => IsChildOf(child, owner), 1000);
				Assert.That(IsChildOf(child, owner), Is.True, "#3.1 Should be a child window again on the owner's screen");

				childLocation = child.Location;
				owner.Location += 20;
				await WaitUntil(() => child.Location != childLocation, 1000);
				Assert.That(child.Location, Is.EqualTo(childLocation + 20), "#3.2 Should move with the owner again");
			});
		});

		/// <summary>
		/// Moving the window in code doesn't trigger the bug, only a real drag does, so this moves the mouse.
		/// </summary>
		[Test]
		public void ChildWindowShouldStayVisibleWhenDraggedToAnotherScreen() => Async(20000, async () =>
		{
			GetScreens(out var primaryArea, out var otherArea);
			if (!AXIsProcessTrusted())
				Assert.Ignore("Requires accessibility access to post mouse events");

			var originalMousePosition = Mouse.Position;
			try
			{
				await WithOwnedForm(primaryArea, async (owner, child) =>
				{
					var childWindow = (NSWindow)child.ControlObject;
					await WaitUntil(() => IsOnScreen(childWindow), 1000);

					var titleBar = (PointF)child.Location + new SizeF(child.Width / 2, 12);
					var otherScreenTitleBar = (PointF)otherArea.Location + new SizeF(300, 200);

					await DragMouse(titleBar, otherScreenTitleBar);
					await WaitUntil(() => otherArea.Contains(child.Location), 1000);
					Assert.That(otherArea.Contains(child.Location), Is.True, "#1.1 Should have been dragged to the other screen");
					// the window server hides it shortly after the mouse is released
					await Task.Delay(500);
					Assert.That(IsOnScreen(childWindow), Is.True, "#1.2 Should be showing on the other screen");

					await DragMouse(otherScreenTitleBar, titleBar);
					await WaitUntil(() => primaryArea.Contains(child.Location), 1000);
					Assert.That(primaryArea.Contains(child.Location), Is.True, "#2.1 Should have been dragged back to the owner's screen");
					Assert.That(IsChildOf(child, owner), Is.True, "#2.2 Should be a child window again on the owner's screen");
					Assert.That(IsOnScreen(childWindow), Is.True, "#2.3 Should be showing on the owner's screen");
				});
			}
			finally
			{
				PostMouseEvent(kCGEventMouseMoved, originalMousePosition);
			}
		});
	}
}
