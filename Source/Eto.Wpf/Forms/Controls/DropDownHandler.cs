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

namespace Eto.Wpf.Forms.Controls
{
	public class DropDownHandler : WpfControl<DropDownHandler.EtoComboBox, DropDown, DropDown.ICallback>, DropDown.IHandler
	{
		IEnumerable<object> store;

		public class EtoComboBox : swc.ComboBox
		{
			int? selected;

			public EtoComboBox()
			{
				Loaded += ComboBoxEx_Loaded;
			}

			public override void OnApplyTemplate()
			{
				base.OnApplyTemplate();

				selected = SelectedIndex;
				SelectedIndex = -1;
			}

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

			protected override sw.Size MeasureOverride(sw.Size constraint)
			{
				var size = base.MeasureOverride(constraint);
				var popup = GetTemplateChild("PART_Popup") as swc.Primitives.Popup;
				if (popup == null)
					return size;
				popup.Child.Measure(Conversions.PositiveInfinitySize); // force generating containers
				if (ItemContainerGenerator.Status != swc.Primitives.GeneratorStatus.ContainersGenerated)
					return size;
				double maxWidth = 0;
				foreach (var item in Items)
				{
					var comboBoxItem = (swc.ComboBoxItem)ItemContainerGenerator.ContainerFromItem(item);
					comboBoxItem.Measure(Conversions.PositiveInfinitySize);
					maxWidth = Math.Max(maxWidth, comboBoxItem.DesiredSize.Width);
				}

				var toggle = GetTemplateChild("toggleButton") as sw.UIElement;
				maxWidth += toggle != null ? toggle.DesiredSize.Width : 20;
				size.Width = Math.Max(maxWidth, size.Width);
				if (!double.IsNaN(constraint.Width))
					size.Width = Math.Min(size.Width, constraint.Width);
				return size;
			}
		}

		public void Create()
		{
			Control = new EtoComboBox();
			Control.SelectionChanged += delegate
			{
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			};
			CreateTemplate();
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }


		public IEnumerable<object> DataStore
		{
			get { return store; }
			set
			{
				store = value;
				Control.ItemsSource = store;
			}
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		public override Color BackgroundColor
		{
			get
			{
				var border = Control.FindChild<swc.Border>();
				return border != null ? border.Background.ToEtoColor() : base.BackgroundColor;
			}
			set
			{
				var border = Control.FindChild<swc.Border>();
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

		void CreateTemplate()
		{
			var template = new sw.DataTemplate();
			template.VisualTree = new WpfTextBindingBlock(() => Widget.TextBinding, setMargin: false);
			Control.ItemTemplate = template;
		}
	}
}
