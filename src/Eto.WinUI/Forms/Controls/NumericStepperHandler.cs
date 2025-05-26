using CommunityToolkit.WinUI;
using Eto.WinUI.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.WinUI.Forms.Controls;

public class NumericStepperHandler : WinUIBorderedControl<muc.NumberBox, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
{
	muc.TextBox _textBox;
	public muc.TextBox TextBox => _textBox ??= Control.FindVisualChild<muc.TextBox>();

	static readonly object ReadOnlyProperty = new object();

	public bool ReadOnly
	{
		get => GetNullableValue(ReadOnlyProperty, TextBox, t => t.IsReadOnly, false);
		set => SetNullableValue(ReadOnlyProperty, value, () => TextBox, (t, v) => t.IsReadOnly = v);
	}
	public override bool Enabled
	{
		get => Control.IsEnabled;
		set => Control.IsEnabled = value;
	}
	public double Value
	{
		get => Control.Value;
		set => Control.Value = value;
	}
	public double MinValue
	{
		get => Control.Minimum;
		set => Control.Minimum = value;
	}
	public double MaxValue
	{
		get => Control.Maximum;
		set => Control.Maximum = value;
	}
	public int DecimalPlaces { get; set; }
	public double Increment { get; set; }
	public int MaximumDecimalPlaces { get; set; }
	public string? FormatString { get; set; }
	public CultureInfo? CultureInfo { get; set; }
	public bool Wrap
	{
		get => Control.IsWrapEnabled;
		set => Control.IsWrapEnabled = value;
	}
	public Color TextColor { get; set; }
	public Font Font { get; set; }

	protected override muc.NumberBox CreateControl() => new muc.NumberBox();

	protected override void Initialize()
	{
		base.Initialize();
		Control.SpinButtonPlacementMode = muc.NumberBoxSpinButtonPlacementMode.Inline;
		Control.ValueChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
	}
}
