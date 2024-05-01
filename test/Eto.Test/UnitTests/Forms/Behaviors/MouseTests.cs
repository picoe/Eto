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

		[Test, ManualTest]
		public void MouseDownHandledInPanelWithChildShouldReleaseMouseCapture() => ManualForm("Click once on both blue squares,\nthey should both turn green.", form =>
		{
			StackLayout CreateClickableBox()
			{
				var child = new Panel
				{
					BackgroundColor = Colors.Blue,
					Size = new Size(40, 40)
				};

				var parent = new Panel();
				parent.Content = child;

				parent.MouseDown += (sender, e) =>
				{
					child.BackgroundColor = child.BackgroundColor == Colors.Blue ? Colors.Green : Colors.Blue;
					e.Handled = true;
				};

				return new StackLayout(parent);
			}

			var section1 = CreateClickableBox();

			var section2 = CreateClickableBox();

			return new TableLayout
			{
				Rows = { section1, section2, null },
				Size = new Size(200, 200),
				Spacing = new Size(4, 4),
				Padding = 8
			};
		});

		[Test, ManualTest]
		public void MouseShouldBeCapturedByOtherControl()
		{
			bool wasCaptured = false;
			bool isMouseCaptured = false;
			bool parent1Entered = false;
			bool parent2Entered = false;
			bool parent2ShouldBeEntered = false;
			bool parent1ShouldNotBeEntered = false;
			bool panel1ShouldNeverBeCaptured = false;
			bool panel2ShouldNeverHaveMouseMoveWithoutCapturing = false;
			var points = new List<PointF>();
			ManualForm("Click and drag on panel 1, events should go to panel 2.\nThen click and drag on panel 2, it should remain green", form =>
			{
				var panel2 = new Drawable
				{
					CanFocus = true,
					Size = new Size(200, 200),
					BackgroundColor = Colors.Green,
					Content = "Panel 2"
				};

				var panel1 = new Drawable
				{
					CanFocus = true,
					Size = new Size(200, 200),
					BackgroundColor = Colors.Blue,
					Content = "Panel 1"
				};
				panel1.MouseEnter += (sender, e) => Debug.WriteLine("Panel1.MouseEnter");
				panel1.MouseLeave += (sender, e) => Debug.WriteLine("Panel1.MouseLeave");
				panel1.MouseUp += (sender, e) => Debug.WriteLine("Panel1.MouseUp");
				panel1.MouseMove += (sender, e) =>
				{
					if (panel1.IsMouseCaptured)
					{
						panel1.BackgroundColor = Colors.Red;
						panel1ShouldNeverBeCaptured = true;
						form.Close();
					}
					Debug.WriteLine("Panel1.MouseMove");
				};
				panel1.MouseDown += (sender, e) =>
				{
					Debug.WriteLine("Panel1.MouseDown");
					wasCaptured = panel2.CaptureMouse();
					isMouseCaptured = panel2.IsMouseCaptured;
					if (!wasCaptured || !isMouseCaptured)
						form.Close();
					e.Handled = true;
					points = new List<PointF>();
				};
				panel1.KeyDown += (sender, e) =>
				{
					Debug.WriteLine("Panel1.KeyDown");
					if (panel2.IsMouseCaptured)
					{
						panel2.ReleaseMouseCapture();
					}
					else
					{
						wasCaptured = panel2.CaptureMouse();
						isMouseCaptured = panel2.IsMouseCaptured;
						if (!wasCaptured || !isMouseCaptured)
							form.Close();
						points = new List<PointF>();
					}
					e.Handled = true;
				};

				panel2.MouseEnter += (sender, e) => Debug.WriteLine("Panel2.MouseEnter");
				panel2.MouseLeave += (sender, e) => Debug.WriteLine("Panel2.MouseLeave");
				panel2.MouseDown += (sender, e) =>
				{
					Debug.WriteLine("Panel2.MouseDown");
					// wasCaptured = panel2.CaptureMouse();
					// isMouseCaptured = panel2.IsMouseCaptured;
					points = new List<PointF>
					{
						e.Location
					};
					panel2.Invalidate();
					// if (!wasCaptured || !isMouseCaptured)
					// 	form.Close();
					e.Handled = true; // should capture the mouse
				};
				panel2.MouseUp += (sender, e) =>
				{
					Debug.WriteLine("Panel2.MouseUp");
					panel2.ReleaseMouseCapture();
				};
				panel2.MouseMove += (sender, e) =>
				{
					Debug.WriteLine("Panel2.MouseMove");
					if (panel2.IsMouseCaptured)
					{
						// note: WinForms does not bubble enter/leave events currently
						if (!parent2Entered && !Platform.Instance.IsWinForms)
						{
							form.Close();
							return;
						}
						parent2ShouldBeEntered = true;
						if (parent1Entered && !Platform.Instance.IsWinForms)
						{
							form.Close();
							return;
						}
						parent1ShouldNotBeEntered = true;
						points.Add(e.Location);
						panel2.Invalidate();
					}
					else if (e.Buttons != MouseButtons.None)
					{
						panel2.BackgroundColor = Colors.Red;
						panel2ShouldNeverHaveMouseMoveWithoutCapturing = true;
						form.Close();
					}
				};
				panel2.Paint += (sender, e) =>
				{
					if (points.Count > 1)
						e.Graphics.DrawLines(Colors.White, points);
				};


				var parent1 = new Panel { Content = panel1 };
				parent1.MouseEnter += (sender, e) =>
				{
					parent1Entered = true;
					Debug.WriteLine($"parent1.MouseEnter");
				};
				parent1.MouseLeave += (sender, e) =>
				{
					parent1Entered = false;
					Debug.WriteLine($"parent1.MouseLeave");
				};

				var parent2 = new Panel { Content = panel2 };
				parent2.MouseEnter += (sender, e) =>
				{
					parent2Entered = true;
					Debug.WriteLine($"parent2.MouseEnter");
				};
				parent2.MouseLeave += (sender, e) =>
				{
					parent2Entered = false;
					Debug.WriteLine($"parent2.MouseLeave");
				};

				var layout = new TableLayout(
					new TableRow(null, parent1, null, parent2, null)
				);

				form.Shown += (sender, e) => panel1.Focus();
				return layout;
			});
			Assert.IsTrue(wasCaptured, "#1 - Mouse was not able to be captured");
			Assert.IsTrue(isMouseCaptured, "#2 - Control.IsMouseCaptured should be true after CaptureMouse() is successful");
			Assert.IsTrue(parent2ShouldBeEntered, "#3 - Parent2 should be entered when mouse is captured on Panel2");
			Assert.IsTrue(parent1ShouldNotBeEntered, "#4 - Parent1 should not be entered when mouse is captured on Panel2");
			Assert.IsFalse(panel1ShouldNeverBeCaptured, "#5 - Panel2 should never be captured");
			Assert.IsFalse(panel2ShouldNeverHaveMouseMoveWithoutCapturing, "#6 - Panel2 should not have a MouseMove with a button down without being captured");
		}
	}
}