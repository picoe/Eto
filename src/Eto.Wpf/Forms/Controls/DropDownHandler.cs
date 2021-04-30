using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swa = System.Windows.Automation;
using swm = System.Windows.Media;
using Eto.Forms;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using System.Globalization;
using Eto.Wpf.Drawing;
using System.Linq;
using System.Collections.Specialized;

namespace Eto.Wpf.Forms.Controls
{
	public class DropDownHandler : DropDownHandler<EtoComboBox, DropDown, DropDown.ICallback>
	{
		internal static readonly object AllowVirtualization_Key = new object();
		internal static readonly object VirtualizationThreshold_Key = new object();
	}

	public class EtoComboBox : swc.ComboBox, IEtoWpfControl
	{
		int? selected;

		public EtoComboBox()
		{
			Loaded += ComboBoxEx_Loaded;
		}

		public bool IsVirtualizing
		{
			get => swc.VirtualizingPanel.GetIsVirtualizing(this);
			set => swc.VirtualizingPanel.SetIsVirtualizing(this, value);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (!IsLoaded)
			{
				var lastSelected = SelectedIndex;
				if (lastSelected != -1)
				{
					selected = lastSelected;
					SelectedIndex = -1;
				}
			}
		}

		internal void SetSelected(int? index) => selected = index;

		public swc.Primitives.Popup Popup => GetTemplateChild("PART_Popup") as swc.Primitives.Popup;

		public swc.ScrollViewer ContentHost
		{
			get
			{
				var tb = TextBox;
				if (tb == null)
					return null;
				return tb.Template.FindName("PART_ContentHost", tb) as swc.ScrollViewer;
			}
		}

		public swc.TextBox TextBox => GetTemplateChild("PART_EditableTextBox") as swc.TextBox;

		public IWpfFrameworkElement Handler { get; set; }

		protected override void OnSelectionChanged(swc.SelectionChangedEventArgs e)
		{
			if (selected == null)
				base.OnSelectionChanged(e);
		}

		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
			if (IsLoaded)
			{
				InvalidateMeasure();
			}
		}

		void ComboBoxEx_Loaded(object sender, sw.RoutedEventArgs e)
		{
			if (selected == null) return;
			SelectedIndex = selected.Value;
			selected = null;
		}

		sw.Size FindMaxSize(sw.Size constraint)
		{
			var size = base.MeasureOverride(constraint);
			var popup = GetTemplateChild("PART_Popup") as swc.Primitives.Popup;
			if (popup == null)
				return size;

			popup.Child.Measure(WpfConversions.PositiveInfinitySize); // force generating containers

			if (ItemContainerGenerator.Status != swc.Primitives.GeneratorStatus.ContainersGenerated)
				return size;

			double maxWidth = 0;
			for (int i = 0; i < Items.Count; i++)
			{
				var item = Items[i];
				var comboBoxItem = (swc.ComboBoxItem)ItemContainerGenerator.ContainerFromItem(item);
				if (comboBoxItem == null)
					continue;
				comboBoxItem.Measure(WpfConversions.PositiveInfinitySize);
				maxWidth = Math.Max(maxWidth, comboBoxItem.DesiredSize.Width);
			}

			var toggle = GetTemplateChild("toggleButton") as sw.UIElement;
			maxWidth += toggle?.DesiredSize.Width ?? 20;
			size.Width = Math.Max(maxWidth, size.Width);
			if (!double.IsNaN(constraint.Width))
				size.Width = Math.Min(size.Width, constraint.Width);

			return size;
		}

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, FindMaxSize) ?? FindMaxSize(constraint);
		}
	}


	public class DropDownHandler<TControl, TWidget, TCallback> : WpfControl<TControl, TWidget, TCallback>, DropDown.IHandler
		where TControl: EtoComboBox
		where TWidget: DropDown
		where TCallback: DropDown.ICallback
	{
		IEnumerable<object> store;

		public DropDownHandler()
		{
			Control = (TControl)new EtoComboBox();
			Control.Handler = this;
			Control.SelectionChanged += (sender, e) => Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
		}

		protected override void Initialize()
		{
			base.Initialize();
			CreateTemplate();
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		/// <summary>
		/// Gets or sets a valuie indication virtualation will be automatically enabled/disabled based on <see cref="VirtualizationThreshold" />.
		/// </summary>
		public bool AllowVirtualization
		{
			get => Widget.Properties.Get<bool>(DropDownHandler.AllowVirtualization_Key, true);
			set
			{
				if (Widget.Properties.TrySet(DropDownHandler.AllowVirtualization_Key, value, true))
					SetVirtualization();
			}
		}

		/// <summary>
		/// Gets or sets the minimum number of items before virtualization is enabled.
		/// </summary>
		/// <value>Threshold of items required when virtualization is enabled</value>
		public int VirtualizationThreshold
		{
			get => Widget.Properties.Get<int>(DropDownHandler.VirtualizationThreshold_Key, 200);
			set
			{
				if (Widget.Properties.TrySet(DropDownHandler.VirtualizationThreshold_Key, value, 200))
				{
					SetVirtualization();
				}
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return store; }
			set
			{
				if (store is INotifyCollectionChanged notifyChanged)
				{
					notifyChanged.CollectionChanged -= Store_CollectionChanged;
				}

				var oldSelectedIndex = SelectedIndex;
				store = value;

				if (store is INotifyCollectionChanged notifyChanged2)
				{
					notifyChanged2.CollectionChanged += Store_CollectionChanged;
				}

				SetVirtualization();

				Control.ItemsSource = store;
				// WPF only triggers a slection change when the item instance has changed
				if (oldSelectedIndex != SelectedIndex && SelectedIndex >= 0)
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			}
		}

		private void Store_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (AllowVirtualization)
				SetVirtualization();	
		}

		private void SetVirtualization()
		{
			// Use virtualization for large lists
			Control.IsVirtualizing = AllowVirtualization && store?.Count() >= VirtualizationThreshold;
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set 
			{
				Control.SelectedIndex = value;
				if (!Control.IsLoaded)
					Control.SetSelected(value);
			}
		}

		protected virtual swc.Border BorderControl => Control.FindChild<swc.Border>();

		public override Color BackgroundColor
		{
			get
			{
				return BorderControl?.Background.ToEtoColor() ?? base.BackgroundColor;
			}
			set
			{
				var border = BorderControl;
				if (border != null)
				{
					border.Background = value.ToWpfBrush(border.Background);
				}
			}
		}

		public override Color TextColor
		{
			get
			{
				var block = Control.FindChild<swc.TextBlock>();
				return block != null ? block.Foreground.ToEtoColor() : base.TextColor;
			}
			set
			{
				var block = Control.FindChild<swc.TextBlock>();
				if (block != null)
					block.Foreground = value.ToWpfBrush();
				else
					base.TextColor = value;
			}
		}

		IIndirectBinding<string> _itemTextBinding;
		public IIndirectBinding<string> ItemTextBinding
		{
			get => _itemTextBinding;
			set
			{
				_itemTextBinding = value;
				if (Widget.Loaded)
					CreateTemplate();
			}
		}

		public IIndirectBinding<string> ItemKeyBinding { get; set; }

		void CreateTemplate()
		{
			var textBlock = new WpfImageTextBindingBlock(() => Widget.ItemTextBinding, () => Widget.ItemImageBinding, false);
			if (IsEventHandled(DropDown.FormatItemEvent))
			{
				var fontConverter = new WpfActionValueConverter(ConvertFontFamily);
				textBlock.SetBinding(swc.TextBlock.FontFamilyProperty, new swd.Binding { Converter = fontConverter });
				textBlock.SetBinding(swc.TextBlock.FontStretchProperty, new swd.Binding { Converter = fontConverter });
				textBlock.SetBinding(swc.TextBlock.FontWeightProperty, new swd.Binding { Converter = fontConverter });
				textBlock.SetBinding(swc.TextBlock.FontStyleProperty, new swd.Binding { Converter = fontConverter });
				textBlock.SetBinding(swc.TextBlock.FontSizeProperty, new swd.Binding { Converter = fontConverter });
			}

			Control.ItemTemplate = new sw.DataTemplate { VisualTree = textBlock };
		}

		private object ConvertFontFamily(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var args = new DropDownFormatEventArgs(value, 0, Font);
			Callback.OnFormatItem(Widget, args);
			if (args.Font is Font font && font.Handler is FontHandler fontHandler)
			{
				if (typeof(swm.FontFamily).IsAssignableFrom(targetType))
					return fontHandler.WpfFamily;
				else if (typeof(sw.FontStretch).IsAssignableFrom(targetType))
					return fontHandler.WpfFontStretch;
				else if (typeof(sw.FontWeight).IsAssignableFrom(targetType))
					return fontHandler.WpfFontWeight;
				else if (typeof(sw.FontStyle).IsAssignableFrom(targetType))
					return fontHandler.WpfFontStyle;
				else if (typeof(double).IsAssignableFrom(targetType))
					return fontHandler.WpfSize;
				else if (typeof(swm.Typeface).IsAssignableFrom(targetType))
					return fontHandler.WpfTypeface;
			}
			return null;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case DropDown.DropDownOpeningEvent:
					Control.DropDownOpened += (sender, e) => Callback.OnDropDownOpening(Widget, EventArgs.Empty);
					break;
				case DropDown.DropDownClosedEvent:
					Control.DropDownClosed += (sender, e) => Callback.OnDropDownClosed(Widget, EventArgs.Empty);
					break;
				case DropDown.FormatItemEvent:
					CreateTemplate();
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
		public void AttachEvent(object widget, object id)
		{
			
			//if (id == )
		}
	}
}
