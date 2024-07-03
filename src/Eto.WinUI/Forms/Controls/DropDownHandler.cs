namespace Eto.WinUI.Forms.Controls;

public class DropDownHandler : WinUIControl<muc.ComboBox, DropDown, DropDown.ICallback>, DropDown.IHandler
{
	public bool ShowBorder { get; set; }
	IEnumerable<object> _store;
	public IEnumerable<object> DataStore
	{
		get => _store;
		set
		{
			_store = value;
			var source = _store;
			if (_store is not INotifyCollectionChanged)
				source = new ObservableCollection<object>(_store);
			Control.ItemsSource = source;
		}
	}
	public int SelectedIndex
	{
		get => Control.SelectedIndex;
		set => Control.SelectedIndex = value;
	}
	public IIndirectBinding<string> ItemTextBinding { get; set; }
	public IIndirectBinding<string> ItemKeyBinding { get; set; }

	protected override muc.ComboBox CreateControl() => new();
	protected override void Initialize()
	{
		base.Initialize();
		Control.SelectionChanged += Control_SelectionChanged;
	}

	private void Control_SelectionChanged(object sender, muc.SelectionChangedEventArgs e)
	{
		Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
	}

}
