using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using swd = Windows.UI.Xaml.Data;
using swa = Windows.UI.Xaml.Automation;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using System.Collections;

namespace Eto.Platform.Xaml.Forms.Controls
{
	/// <summary>
	/// Combobox handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ComboBoxHandler : WpfControl<ComboBoxHandler.EtoComboBox, ComboBox>, IComboBox
	{
		IListStore store;

		public class EtoComboBox : swc.ComboBox
		{
			int? _selected;

			public EtoComboBox()
			{
				Loaded += ComboBoxEx_Loaded;
			}

			protected override void OnApplyTemplate()
			{
				base.OnApplyTemplate();

				_selected = SelectedIndex;
				SelectedIndex = -1;
			}

#if TODO_XAML
			protected override void OnSelectionChanged(swc.SelectionChangedEventArgs e)
			{
				if (_selected == null)
					base.OnSelectionChanged(e);
			}
#endif

			protected override void OnItemsChanged(object e)
			{
				base.OnItemsChanged(e);
#if TODO_XAML
				if (IsLoaded)
#endif
				{
					InvalidateMeasure();
				}
			}

			void ComboBoxEx_Loaded(object sender, sw.RoutedEventArgs e)
			{
				if (_selected != null)
				{
					SelectedIndex = _selected.Value;
					_selected = null;
				}
			}

			protected override wf.Size MeasureOverride(wf.Size constraint)
			{
				var size = base.MeasureOverride(constraint);
				var popup = GetTemplateChild("PART_Popup") as swc.Primitives.Popup;
#if TODO_XAML
				popup.Child.Measure(Conversions.PositiveInfinitySize); // force generating containers
				if (ItemContainerGenerator.Status == swc.Primitives.GeneratorStatus.ContainersGenerated)
				{
					double maxWidth = 0;
					foreach (var item in Items)
					{
						var comboBoxItem = (swc.ComboBoxItem)ItemContainerGenerator.ContainerFromItem(item);
						comboBoxItem.Measure(Conversions.PositiveInfinitySize);
						maxWidth = Math.Max(maxWidth, comboBoxItem.DesiredSize.Width);
					}
					var toggle = GetTemplateChild("toggleButton") as sw.UIElement;
					if (toggle != null)
						maxWidth += toggle.DesiredSize.Width; // add room for the toggle button
					else
						maxWidth += 20; // windows 7 doesn't name the toggle button, so hack it
					size.Width = Math.Max(maxWidth, size.Width);
				}
#endif
				return size;
			}
		}

		public ComboBoxHandler()
		{
			Control = new EtoComboBox();
			var str = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                "<TextBox Text=\"{Binding Text}\" />" +
				"</DataTemplate>";
			var template = (sw.DataTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(str);
			Control.ItemTemplate = template;
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }


		protected override void PostInitialize()
		{
			base.PostInitialize();
			Control.SelectionChanged += delegate
			{
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};
		}

		public IListStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				Control.ItemsSource = store as IEnumerable ?? store.AsEnumerable();
			}
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}
	}
}
