using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Eto.WinUI.Forms.Controls;

public sealed partial class EtoBindingTextBlock : UserControl
{
	public EtoBindingTextBlock()
	{
		this.InitializeComponent();
		DataContextChanged += EtoBindingTextBlock_DataContextChanged;
		Loaded += EtoBindingTextBlock_Loaded;
	}

	private void EtoBindingTextBlock_Loaded(object sender, RoutedEventArgs e)
	{
		Content.Text = Binding?.GetValue(DataContext);
	}

	private void EtoBindingTextBlock_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
	{
		Content.Text = Binding?.GetValue(DataContext);
	}

	private IIndirectBinding<string> _binding;
	IIndirectBinding<string> Binding => _binding ??= FindBinding();

	private IIndirectBinding<string> FindBinding()
	{
		var parent = this.FindVisualParent<IEtoBindingSource<string>>();
		if (parent == null && BindingSource is mux.FrameworkElement element && element.Parent != null)
		{
			var item = VisualTreeHelper.GetParent(element.Parent); //.FindVisualParent<muc.ComboBoxItem>();
			if (item != null)
			{
				var itemsControl = ItemsControl.ItemsControlFromItemContainer(item);
				if (itemsControl != null)
				{
					parent = itemsControl.FindVisualParent<IEtoBindingSource<string>>();
				}
			}
		}
		return parent?.Binding;
	}

	public static readonly DependencyProperty BindingSourceProperty =
		DependencyProperty.Register(nameof(BindingSource), typeof(object), typeof(EtoBindingTextBlock), new PropertyMetadata(null));

	public object BindingSource
	{
		get => GetValue(BindingSourceProperty);
		set => SetValue(BindingSourceProperty, value);
	}

	public string Text
	{
		get => Binding?.GetValue(DataContext);
		set => Binding?.SetValue(DataContext, value);
	}
}
