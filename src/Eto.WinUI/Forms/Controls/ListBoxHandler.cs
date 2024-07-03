
namespace Eto.WinUI.Forms.Controls;

public class ListBoxHandler : WinUIControl<muc.ListView, ListBox, ListBox.ICallback>, ListBox.IHandler
{
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
	public ContextMenu ContextMenu { get; set; }

	protected override muc.ListView CreateControl() => new();

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
