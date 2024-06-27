using WinRT.Interop;
using mui = Microsoft.UI.Xaml;

namespace Eto.WinUI.Forms;

public interface IWinUIWindow
{
	mui.Window Control { get; }
}

public class WinUIWindow<TControl, TWidget, TCallback> : WinUIPanel<TControl, TWidget, TCallback>, Window.IHandler, IWinUIWindow//, IInputBindingHost
	where TControl : mui.Window
	where TWidget : Window
	where TCallback : Window.ICallback
{

	public override IntPtr NativeHandle => WindowNative.GetWindowHandle(Control);
	public override mui.FrameworkElement ContainerControl { get; }
	public override mui.FrameworkElement FocusControl { get; }
	public ToolBar ToolBar { get; set; }
	public double Opacity { get; set; }
	public string Title { get; set; }
	public Screen Screen { get; }
	public MenuBar Menu { get; set; }
	public Icon Icon { get; set; }
	public bool Resizable { get; set; }
	public bool Maximizable { get; set; }
	public bool Minimizable { get; set; }
	public bool Closeable { get; set; }
	public bool ShowInTaskbar { get; set; }
	public bool Topmost { get; set; }
	public WindowState WindowState { get; set; }
	public Rectangle RestoreBounds { get; }
	public WindowStyle WindowStyle { get; set; }
	public float LogicalPixelSize { get; }
	public bool MovableByWindowBackground { get; set; }
	public bool AutoSize { get; set; }
	Point Window.IHandler.Location { get; set; }

	protected override TControl CreateControl() => (TControl)new mui.Window();

	public void BringToFront()
	{
		throw new NotImplementedException();
	}

	public void Close() => Control.Close();

	public void SendToBack()
	{
		throw new NotImplementedException();
	}

	public void SetOwner(Window owner)
	{
	}

	public override void SetContainerContent(mui.FrameworkElement content)
	{
		Control.Content = content;
	}

	public override bool Visible
	{
		get => Control.Visible;
		set {  }
		//set => Control..CoreWindow..Visible = value;
	}

	mui.Window IWinUIWindow.Control => Control;

	protected override void Initialize()
	{
		base.Initialize();
		Control.ExtendsContentIntoTitleBar = true;
		//var grid = new mui.Controls.Grid();
		//grid.Children.Add(new mui.Controls.TextBlock { Text = "Hello" });
		//Control.SetTitleBar(grid);

	}

}
