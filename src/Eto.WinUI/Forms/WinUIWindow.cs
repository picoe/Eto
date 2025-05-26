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
	readonly muc.Grid _rootGrid = new();
	readonly muc.TextBlock _title = new();

	public override IntPtr NativeHandle => WindowNative.GetWindowHandle(Control);
	public override mui.FrameworkElement ContainerControl => _rootGrid;
	public ToolBar ToolBar { get; set; }
	public double Opacity { get; set; }
	public string Title
	{
		get => _title.Text;
		set => _title.Text = value;	
	}
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

	public override Color BackgroundColor
	{
		get => _rootGrid.Background.ToEtoColor();
		set => _rootGrid.Background = value.ToWinUIBrush();
	}

	Point Window.IHandler.Location
	{
		get => Control.AppWindow.Position.ToEto();
		set => Control.AppWindow.Move(value.ToWinUIPointInt32());
	}

	protected override TControl CreateControl() => (TControl)new mui.Window();

	public void BringToFront() => Control.AppWindow.MoveInZOrderAtTop();

	public void Close() => Control.Close();

	public void SendToBack() => Control.AppWindow.MoveInZOrderAtBottom();

	public void SetOwner(Window owner)
	{
	}

	public override void SetContainerContent(mui.FrameworkElement content)
	{
		muc.Grid.SetRow(content, 1);
		_rootGrid.Children.Add(content);
		//Control.Content = content;
	}

	public override bool Visible
	{
		get => Control.Visible;
		set
		{
			if (value)
				Control.AppWindow.Show();
			else
				Control.AppWindow.Hide();
		}
	}

	mui.Window IWinUIWindow.Control => Control;

	protected override void Initialize()
	{
		base.Initialize();

		Control.ExtendsContentIntoTitleBar = true;

		_rootGrid.Background = mux.Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as muxm.Brush;

		// Define rows: one for toolbar, one for content
		_rootGrid.RowDefinitions.Add(new muc.RowDefinition { Height = mux.GridLength.Auto });
		_rootGrid.RowDefinitions.Add(new muc.RowDefinition { Height = new mux.GridLength(1, mux.GridUnitType.Star) });

		// Create CommandBar
		var toolbar = new muc.CommandBar
		{
			
		};

		// add title to the toolbar
		_title.FontSize = 14;
		_title.Margin = new mui.Thickness(8);

		toolbar.Content = _title;


		muc.Grid.SetRow(toolbar, 0);
		_rootGrid.Children.Add(toolbar);


		Control.Content = _rootGrid;

	}

}
