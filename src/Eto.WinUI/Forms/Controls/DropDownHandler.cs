namespace Eto.WinUI.Forms.Controls;


public interface IEtoBindingSource<T>
{
	IIndirectBinding<T> Binding { get; }
}

public class EtoComboBox : muc.ComboBox, IEtoBindingSource<string>
{
	public IWinUIFrameworkElement Handler { get; set; }
	public IIndirectBinding<string> Binding { get; set; }

	protected override wf.Size MeasureOverride(wf.Size availableSize)
	{
		return Handler?.MeasureOverride(availableSize, base.MeasureOverride) ?? base.MeasureOverride(availableSize);
	}
}
public class DropDownHandler : WinUIControl<EtoComboBox, DropDown, DropDown.ICallback>, DropDown.IHandler
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
	private IIndirectBinding<string> _itemTextBinding;
	public IIndirectBinding<string> ItemTextBinding
	{
		get => _itemTextBinding;
		set => _itemTextBinding = Control.Binding = value;
	}

	private IIndirectBinding<string> _itemKeyBinding;
	public IIndirectBinding<string> ItemKeyBinding
	{
		get => _itemKeyBinding;
		set => _itemKeyBinding = value;
	}

	protected override EtoComboBox CreateControl() => new EtoComboBox { Handler = this };
	protected override void Initialize()
	{
		base.Initialize();
		Control.ItemTemplate = mux.Application.Current.Resources["StringItemTemplate"] as mux.DataTemplate;
		Control.SelectionChanged += Control_SelectionChanged;
	}

	private void Control_SelectionChanged(object sender, muc.SelectionChangedEventArgs e)
	{
		Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
	}

}
