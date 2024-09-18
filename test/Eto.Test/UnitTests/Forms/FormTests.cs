using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class FormTests : WindowTests<Form>
	{
		protected override void Test(Action<Form> test, int timeout = 4000) => Form(test, timeout);
		protected override void ManualTest(string message, Func<Form, Control> test) => ManualForm(message, test);
		protected override void Show(Form window) => window.Show();
		protected override Task ShowAsync(Form window)
		{
			var tcs = new TaskCompletionSource<bool>();
			window.Closed += (sender, e) => tcs.TrySetResult(true);
			window.Show();
			return tcs.Task;
		}
		
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
		public void MultipleChildWindowsShouldGetFocusWhenClicked() => Async(async () =>
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
			Assert.AreEqual(1, closed, "Closed event should only fire once");
		}
		
	}
}

