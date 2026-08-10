using swd = System.Windows.Data;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoListBox : swc.ListBox, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		public IIndirectBinding<string> TextBinding
		{
			get { return (IIndirectBinding<string>)GetValue(TextBindingProperty); }
			set { SetValue(TextBindingProperty, value); }
		}

		public static readonly sw.DependencyProperty TextBindingProperty =
			sw.DependencyProperty.Register(
				nameof(TextBinding),
				typeof(IIndirectBinding<string>),
				typeof(EtoListBox),
				new sw.FrameworkPropertyMetadata(null));

		public IIndirectBinding<Image> ImageBinding
		{
			get { return (IIndirectBinding<Image>)GetValue(ImageBindingProperty); }
			set { SetValue(ImageBindingProperty, value); }
		}

		public static readonly sw.DependencyProperty ImageBindingProperty =
			sw.DependencyProperty.Register(
				nameof(ImageBinding),
				typeof(IIndirectBinding<Image>),
				typeof(EtoListBox),
				new sw.FrameworkPropertyMetadata(null));

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class ListBoxHandler : WpfControl<EtoListBox, ListBox, ListBox.ICallback>, ListBox.IHandler
	{
		IEnumerable<object> store;

		protected override sw.Size DefaultSize => new sw.Size(100, 120);

		public ListBoxHandler()
		{
			Control = new EtoListBox
			{
				HorizontalAlignment = sw.HorizontalAlignment.Stretch,
				Handler = this
			};

			Control.SelectionChanged += delegate
			{
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			};
			Control.MouseDoubleClick += delegate
			{
				if (SelectedIndex >= 0)
					Callback.OnActivated(Widget, EventArgs.Empty);
			};
			Control.KeyDown += (sender, e) =>
			{
				if (e.Key == sw.Input.Key.Return)
				{
					if (SelectedIndex >= 0)
					{
						Callback.OnActivated(Widget, EventArgs.Empty);
						e.Handled = true;
					}
				}
			};
		}

		public override void Focus()
		{
			if (Control.IsLoaded)
			{
				var item = Control.ItemContainerGenerator.ContainerFromIndex(Math.Max(0, SelectedIndex)) as sw.FrameworkElement;
				if (item != null)
					item.Focus();
				else
					Control.Focus();
			}
			else
			{
				Control.Loaded += Control_Loaded;
			}
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			Focus();
			Control.Loaded -= Control_Loaded;
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public IEnumerable<object> DataStore
		{
			get { return store; }
			set
			{
				store = value;
				var source = store as IEnumerable<object>;
				if (source != null && !(source is INotifyCollectionChanged))
					source = new ObservableCollection<object>(source);
				Control.ItemsSource = source;
			}
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set
			{
				Control.SelectedIndex = value;
				if (value >= 0 && store != null)
				{
					var item = store.AsEnumerable().Skip(value).FirstOrDefault();
					Control.ScrollIntoView(item);
				}
			}
		}

		public IIndirectBinding<string> ItemTextBinding
		{
			get => Control.TextBinding;
			set
			{
				Control.TextBinding = value;
				Control.InvalidateVisual();
			}
		}
		public IIndirectBinding<string> ItemKeyBinding { get; set; }

		static readonly object Border_Key = new object();
		public BorderType Border
		{
			get { return Widget.Properties.Get(Border_Key, BorderType.Bezel); }
			set { if (Widget.Properties.TrySet(Border_Key, value)) Control.SetEtoBorderType(value); }
		}

		public IIndirectBinding<Image> ItemImageBinding
		{
			get => Control.ImageBinding;
			set
			{
				Control.ImageBinding = value;
				Control.InvalidateVisual();
			}
		}

	}
	
	public class IndirectBindingConverter<T> : swd.IMultiValueConverter
	{		
		public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			// values[0] = item, values[1] = binding
			if (values.Length < 2)
				return null;
			
			var binding = values[1] as IIndirectBinding<T>;
			if (binding == null)
				return null;
			
			return binding.GetValue(values[0]);
		}

		public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
	
	
	/// <summary>
	/// A multi-value converter for ListBox item text in XAML templates.
	/// Receives the item value and the TextBinding property from EtoListBox.
	/// </summary>
	public class ListBoxItemTextConverter : IndirectBindingConverter<string>
	{
	}

	/// <summary>
	/// A multi-value converter for ListBox item images in XAML templates.
	/// Receives the item value and the ImageBinding property from EtoListBox.
	/// </summary>
	public class ListBoxItemImageConverter : IndirectBindingConverter<Image>
	{
		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var img = base.Convert(values, targetType, parameter, culture) as Image;
			return img?.ToWpf();
		}
	}
	
}
