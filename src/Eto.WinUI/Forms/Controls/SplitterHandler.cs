namespace Eto.WinUI.Forms.Controls;

public class SplitterHandler : WinUIControl<muc.SplitView, Splitter, Splitter.ICallback>, Splitter.IHandler
{
	Control _panel1;
	Control _panel2;
	public Orientation Orientation { get; set; }
	public SplitterFixedPanel FixedPanel { get; set; }
	public int Position
	{
		get => (int)Control.OpenPaneLength;
		set => Control.OpenPaneLength = value;
	}

	public double RelativePosition { get; set; }
	public int SplitterWidth { get; set; }
	public Control Panel1
	{
		get => _panel1;
		set
		{
			if (_panel1 == value)
				return;
			_panel1 = value;
			Control.Pane = value.ToNative();
		}
	}
	public Control Panel2
	{
		get => _panel2;
		set
		{
			if (_panel2 == value)
				return;

			_panel2 = value;
			Control.Content = value.ToNative();
		}
	}
	public int Panel1MinimumSize { get; set; }
	public int Panel2MinimumSize { get; set; }
	public Size ClientSize { get; set; }
	public bool RecurseToChildren => true;

	public override IEnumerable<Control> VisualControls => Widget.Controls;

	protected override muc.SplitView CreateControl() => new muc.SplitView
	{
		DisplayMode = muc.SplitViewDisplayMode.Inline,
		IsPaneOpen = true
	};
}
