namespace Eto.WinUI.Forms;

public class ApplicationHandler : WidgetHandler<mux.Application, Application, Application.ICallback>, Application.IHandler
{
	mud.DispatcherQueue _dispatcher;
	Thread _mainThread;

	public bool QuitIsSupported => true;
	public Keys CommonModifier => Keys.Control;
	public Keys AlternateModifier => Keys.Alt;
	public string BadgeLabel { get; set; }
	public bool IsActive { get; }

	public ApplicationHandler()
	{
		Control = mux.Application.Current;
	}

	public void AsyncInvoke(Action action)
	{
		_dispatcher.TryEnqueue(new mud.DispatcherQueueHandler(action));
	}

	public void Attach(object context)
	{
		Control = context as mux.Application;
	}

	protected override void Initialize()
	{
		base.Initialize();
		_dispatcher = mud.DispatcherQueue.GetForCurrentThread();
		_mainThread = Thread.CurrentThread;
	}

	public void Invoke(Action action)
	{

		if (_dispatcher == null || Thread.CurrentThread == _mainThread)
			action();
		else
		{
			var mre = new ManualResetEvent(false);
			_dispatcher.TryEnqueue(() =>
			{
				action();
				mre.Set();
			});
			mre.WaitOne();
		}
	}

	public void OnMainFormChanged()
	{
		//mux.Application.Current..MainWindow = Widget.MainForm.ToNative();
	}

	public void Open(string url)
	{
		Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
	}

	public void Quit()
	{
		Control.Exit();
	}

	public void Restart()
	{
	}

	public void Run()
	{
		Callback.OnInitialized(Widget, EventArgs.Empty);
	}

	public void RunIteration()
	{
		//var frame = new DispatcherFrame();
		//Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
		//Dispatcher.PushFrame(frame);
		//WpfFrameworkElementHelper.ShouldCaptureMouse = false;
	}
}
