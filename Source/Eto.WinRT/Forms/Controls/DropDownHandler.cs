using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using swd = Windows.UI.Xaml.Data;
using swa = Windows.UI.Xaml.Automation;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using System.Collections;
using Windows.UI.Xaml.Markup;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	public class EtoDropDown : swc.ComboBox
	{
		int? selected;

		public EtoDropDown()
		{
			Loaded += ComboBoxEx_Loaded;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			selected = SelectedIndex;
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
			if (selected != null)
			{
				SelectedIndex = selected.Value;
				selected = null;
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

	public class DropDownHandler : DropDownHandler<EtoDropDown, DropDown, DropDown.ICallback>
	{
	}

	/// <summary>
	/// Combobox handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class DropDownHandler<TControl, TWidget, TCallback> : WpfControl<TControl, TWidget, TCallback>, DropDown.IHandler
		where TControl: EtoDropDown
		where TWidget: DropDown
		where TCallback: DropDown.ICallback

{
	IEnumerable<object> store;

	public DropDownHandler()
	{
		Control = (TControl)new EtoDropDown();
		var str = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
		          "<TextBlock Text=\"{Binding Text}\" />" +
		          "</DataTemplate>";
		var template = (sw.DataTemplate) XamlReader.Load(str);
		Control.ItemTemplate = template;
	}

	public override bool UseMousePreview
	{
		get { return true; }
	}

	public override bool UseKeyPreview
	{
		get { return true; }
	}


	protected override void Initialize()
	{
		base.Initialize();
		Control.SelectionChanged += delegate
		{
			Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
		};
	}

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

	public Color TextColor
	{
		get { return Control.Foreground.ToEtoColor(); }
		set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
	}
}
}
