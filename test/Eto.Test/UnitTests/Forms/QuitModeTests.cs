using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms;

[TestFixture]
public class QuitModeTests : TestBase
{
	// Terminating is always cancelled so the test host keeps running; we only check when it is asked.
	class QuitTester
	{
		readonly List<Window> windows = new List<Window>();

		public int TerminatingCount { get; private set; }

		public void OnTerminating(object sender, CancelEventArgs e)
		{
			TerminatingCount++;
			e.Cancel = true;
		}

		public T Show<T>(T window, Window owner = null)
			where T : Form
		{
			window.Title = $"QuitModeTests {windows.Count + 1}";
			window.Size = new Size(200, 150);
			window.Location = new Point(100 + windows.Count * 50, 100 + windows.Count * 50);
			window.Owner = owner;
			window.Show();
			windows.Add(window);
			return window;
		}

		public Form Show(Window owner = null) => Show(new Form(), owner);

		public async Task<Dialog> ShowDialogAsync(Window owner = null)
		{
			var dialog = new Dialog { Title = $"QuitModeTests {windows.Count + 1}", Size = new Size(200, 150) };
			windows.Add(dialog);
			_ = dialog.ShowModalAsync(owner);
			await WaitUntil(() => dialog.Visible);
			return dialog;
		}

		// Returns whether closing asked to terminate, in which case the close is cancelled and the window stays open.
		public async Task<bool> CloseAsks(Window window)
		{
			var count = TerminatingCount;
			window.Close();
			await WaitUntil(() => TerminatingCount > count || !IsOpen(window));
			var asked = TerminatingCount > count;
			if (asked)
				Assert.That(IsOpen(window), Is.True, "Cancelling Terminating should cancel the close");
			else
				Assert.That(IsOpen(window), Is.False, "Window should have closed");
			return asked;
		}

		public async Task CloseAllAsync()
		{
			foreach (var window in windows)
				await CloseAsync(window);
		}
	}

	static bool IsOpen(Window window) => Application.Instance.Windows.Contains(window);

	static void QuitTest(QuitMode mode, Func<QuitTester, Task> test) => Async(-1, async () =>
	{
		var app = Application.Instance;
		// other forms (e.g. the test app's own) would keep the application open
		Assume.That(app.Windows.OfType<Form>().Any(r => r.Visible), Is.False, "Another form is open");

		var oldMode = app.QuitMode;
		var oldMainForm = app.MainForm;
		var tester = new QuitTester();
		app.Terminating += tester.OnTerminating;
		app.QuitMode = mode;
		try
		{
			await test(tester);
		}
		finally
		{
			app.QuitMode = QuitMode.Explicit;
			await tester.CloseAllAsync();
			app.Terminating -= tester.OnTerminating;
			app.QuitMode = oldMode;
			if (!ReferenceEquals(app.MainForm, oldMainForm))
				app.MainForm = oldMainForm;
		}
	});

	[Test]
	public void LastWindowClosedShouldQuitWhenClosingLastForm() => QuitTest(QuitMode.LastWindowClosed, async q =>
	{
		var form1 = q.Show();
		var form2 = q.Show();

		Assert.That(await q.CloseAsks(form1), Is.False, "Closing first form");
		Assert.That(await q.CloseAsks(form2), Is.True, "Closing last form");
	});

	[Test]
	public void LastWindowClosedShouldQuitWhenClosingLastFormAfterMainForm() => QuitTest(QuitMode.LastWindowClosed, async q =>
	{
		var mainForm = q.Show();
		Application.Instance.MainForm = mainForm;
		var other = q.Show();

		Assert.That(await q.CloseAsks(mainForm), Is.False, "Closing main form");
		Assert.That(await q.CloseAsks(other), Is.True, "Closing last form");
	});

	[Test]
	public void LastWindowClosedShouldIgnoreOwnedHiddenAndFloatingForms() => QuitTest(QuitMode.LastWindowClosed, async q =>
	{
		var form = q.Show();
		q.Show(owner: form);
		q.Show(new FloatingForm());
		var hidden = q.Show();
		hidden.Visible = false;
		await WaitUntil(() => !hidden.Visible);

		Assert.That(await q.CloseAsks(form), Is.True);
	});

	[Test]
	public void LastWindowClosedShouldCountFormsLeftByClosedOwner() => QuitTest(QuitMode.LastWindowClosed, async q =>
	{
		var owner = q.Show();
		var other = q.Show();
		var owned = q.Show(owner: owner);

		Assert.That(await q.CloseAsks(owner), Is.False, "Closing owner");

		// Wpf and WinForms close owned windows along with their owner, and Mac can hide them
		if (IsOpen(owned) && owned.Visible)
		{
			Assert.That(await q.CloseAsks(other), Is.False, "Closing other form with orphaned form open");
			Assert.That(await q.CloseAsks(owned), Is.True, "Closing orphaned form");
		}
		else
			Assert.That(await q.CloseAsks(other), Is.True, "Closing other form");
	});

	[Test]
	public void LastWindowClosedShouldNotQuitWhileDialogIsOpen() => QuitTest(QuitMode.LastWindowClosed, async q =>
	{
		var form = q.Show();
		var dialog = await q.ShowDialogAsync(form);

		var count = q.TerminatingCount;
		form.Close();
		await WaitUntil(() => !IsOpen(form));

		// Wpf closes the dialog along with its owner, which leaves nothing open
		if (IsOpen(dialog))
		{
			Assert.That(q.TerminatingCount, Is.EqualTo(count), "Closing last form with a dialog open");
			Assert.That(await q.CloseAsks(dialog), Is.True, "Closing the dialog");
		}
		else
			Assert.That(await WaitUntil(() => q.TerminatingCount > count), Is.True, "Closing the form and its dialog");
	});

	[Test]
	public void MainFormClosedShouldQuitWhenClosingMainForm() => QuitTest(QuitMode.MainFormClosed, async q =>
	{
		var mainForm = q.Show();
		Application.Instance.MainForm = mainForm;
		var other = q.Show();

		Assert.That(await q.CloseAsks(other), Is.False, "Closing other form");
		Assert.That(await q.CloseAsks(mainForm), Is.True, "Closing main form");
	});

	[Test]
	public void ExplicitShouldNotQuitWhenClosingForms() => QuitTest(QuitMode.Explicit, async q =>
	{
		var mainForm = q.Show();
		Application.Instance.MainForm = mainForm;

		Assert.That(await q.CloseAsks(mainForm), Is.False);
	});

	[Test]
	public void DefaultOnMacShouldHideMainForm() => QuitTest(QuitMode.Default, async q =>
	{
		Assume.That(Platform.Instance.IsMac, Is.True);
		var mainForm = q.Show();
		Application.Instance.MainForm = mainForm;
		var other = q.Show();

		Assert.That(await q.CloseAsks(other), Is.False, "Closing other form");
		mainForm.Close();
		await Task.Delay(200);
		Assert.That(q.TerminatingCount, Is.EqualTo(0), "Closing main form should not ask to quit");
		Assert.That(mainForm.Visible, Is.False, "Main form should be hidden");
		Assert.That(IsOpen(mainForm), Is.True, "Main form should be kept to show again from the dock");
		Assert.That(Application.Instance.MainForm, Is.SameAs(mainForm));
		mainForm.Show();
		await WaitUntil(() => mainForm.Visible);
	});

	[TestCase(true)]
	[TestCase(false)]
	public void QuitShouldRaiseTerminatingOnce(bool withMainForm) => QuitTest(QuitMode.Explicit, async q =>
	{
		var mainForm = q.Show();
		q.Show();
		if (withMainForm)
			Application.Instance.MainForm = mainForm;

		Application.Instance.Quit();
		await Task.Delay(100);

		Assert.That(q.TerminatingCount, Is.EqualTo(1));
		Assert.That(IsOpen(mainForm), Is.True, "Cancelling Terminating should keep the main form open");
	});
}
