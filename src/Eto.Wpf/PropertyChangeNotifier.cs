using System;
using sw = System.Windows;
using swd = System.Windows.Data;

namespace Eto.Wpf
{
	/// <summary>
	/// Used to listen to property changes without creating a static strong reference with
	/// DependencyPropertyDescriptor.AddValueChanged, which prevents controls from being garbage collected.
	/// </summary>
	/// <remarks>
	/// Modified from https://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/
	/// </remarks>
	class PropertyChangeNotifier : sw.DependencyObject, IDisposable
	{
		WeakReference _propertySource;
		sw.PropertyPath _property;
		public event EventHandler ValueChanged;

		public static PropertyChangeNotifier Register(sw.DependencyProperty property, EventHandler handler, sw.DependencyObject propertySource = null)
		{
			var notifier = new PropertyChangeNotifier(property);
			if (propertySource != null)
				notifier.PropertySource = propertySource;
			notifier.ValueChanged += handler;
			return notifier;
		}

		public PropertyChangeNotifier(sw.DependencyProperty property)
		: this(new sw.PropertyPath(property))
		{
		}

		public PropertyChangeNotifier(sw.PropertyPath property)
		{
			_property = property ?? throw new ArgumentNullException(nameof(property));
		}

		public sw.DependencyObject PropertySource
		{
			get => _propertySource?.Target as sw.DependencyObject;
			set
			{
				Clear();

				if (value != null)
				{
					_propertySource = new WeakReference(value);
					swd.Binding binding = new swd.Binding();
					binding.Path = _property;
					binding.Mode = swd.BindingMode.OneWay;
					binding.Source = value;
					swd.BindingOperations.SetBinding(this, ValueProperty, binding);
				}
			}
		}
		public static readonly sw.DependencyProperty ValueProperty = sw.DependencyProperty.Register(
			"Value", typeof(object), typeof(PropertyChangeNotifier), 
			new sw.FrameworkPropertyMetadata(null, new sw.PropertyChangedCallback(OnPropertyChanged)));

		private static void OnPropertyChanged(sw.DependencyObject d, sw.DependencyPropertyChangedEventArgs e)
		{
			var notifier = (PropertyChangeNotifier)d;
			notifier.ValueChanged?.Invoke(notifier, EventArgs.Empty);
		}

		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		void Clear()
		{
			if (_propertySource != null)
			{
				swd.BindingOperations.ClearBinding(this, ValueProperty);
				_propertySource = null;
			}
		}

		public void Dispose() => Clear();
	}
}
