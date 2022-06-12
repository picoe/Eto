using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Behaviors
{
	[TestFixture]
	public class MouseTests : TestBase
	{
		[Test, ManualTest]
		public void EventsFromParentShouldWorkWhenChildRemoved()
		{
			var mouseUpCalled = false;
			var contentRemoved = false;
			var mouseDown = false;
			var mouseDownInChild = false;
			var mouseMovedEvenAfterRemoved = false;
			ManualForm("Click and drag the blue square,\nit should dissapear and the MouseUp event should be fired",
			form =>
			{
				var top = new Panel { Size = new Size(200, 200) };
				top.MouseDown += (sender, e) =>
				{
					if (e.Buttons == MouseButtons.Primary)
					{
						e.Handled = true;
						mouseDown = true;
					}
				};

				top.MouseMove += (sender, e) =>
				{
					if (mouseDown && mouseDownInChild && e.Buttons == MouseButtons.Primary)
					{
						top.Content = new Panel();
						contentRemoved = true;
					}
				};

				top.MouseUp += (sender, e) =>
				{
					if (contentRemoved)
					{
						e.Handled = true;
						mouseDown = false;
						mouseUpCalled = true;
						Application.Instance.AsyncInvoke(form.Close);
					}
				};

				var child = new Panel { Size = new Size(100, 100), BackgroundColor = Colors.Blue };

				child.MouseDown += (sender, e) =>
				{
					if (e.Buttons == MouseButtons.Primary)
					{
						mouseDownInChild = true;
						// explicitly not setting e.Handled here so it bubbles to the top control
					}
				};
				child.MouseMove += (sender, e) =>
				{
					if (contentRemoved && e.Buttons == MouseButtons.Primary)
					{
						mouseMovedEvenAfterRemoved = true;
					}
				};

				child.MouseUp += (sender, e) =>
				{
					if (mouseDownInChild && e.Buttons == MouseButtons.Primary)
					{
						mouseDownInChild = false;
					}
				};

				top.Content = TableLayout.AutoSized(child, centered: true);

				return top;

			}, allowPass: false);

			Assert.IsTrue(mouseUpCalled, "#1 - MouseUp was not called on top control");
			Assert.IsTrue(contentRemoved, "#2 - Content (blue square) was not removed");
			Assert.IsTrue(mouseDownInChild, "#3 - MouseUp should NOT be called on child even though it was removed");
			Assert.IsFalse(mouseMovedEvenAfterRemoved, "#4 - MouseMoved should NOT be called on child after it was removed");
		}
		
		
	}
	
	
}