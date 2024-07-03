using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Eto.WinUI.CustomControls;

public sealed class SpinButton : muc.Control
{
	public SpinButton()
	{
		DefaultStyleKey = typeof(SpinButton);
	}

	public event RoutedEventHandler? UpClicked;
	public event RoutedEventHandler? DownClicked;

	public static readonly mux.DependencyProperty UpEnabledProperty =
		mux.DependencyProperty.Register(nameof(UpEnabled), typeof(bool), typeof(SpinButton), new PropertyMetadata(true));
	public bool UpEnabled
	{
		get => (bool)GetValue(UpEnabledProperty);
		set => SetValue(UpEnabledProperty, value);
	}

	public static readonly mux.DependencyProperty DownEnabledProperty =
		mux.DependencyProperty.Register(nameof(DownEnabled), typeof(bool), typeof(SpinButton), new PropertyMetadata(true));
	public bool DownEnabled
	{
		get => (bool)GetValue(DownEnabledProperty);
		set => SetValue(DownEnabledProperty, value);
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		if (GetTemplateChild("UpButton") is muc.Button upBtn)
			upBtn.Click += (s, e) => UpClicked?.Invoke(this, e);

		if (GetTemplateChild("DownButton") is muc.Button downBtn)
			downBtn.Click += (s, e) => DownClicked?.Invoke(this, e);
	}

}
