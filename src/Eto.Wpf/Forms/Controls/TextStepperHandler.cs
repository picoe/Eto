using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sw = System.Windows;
using swc = System.Windows.Controls;
using mwc = Xceed.Wpf.Toolkit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoButtonSpinner : mwc.ButtonSpinner, IEtoWpfControl
	{
		swc.TextBox TextBox => Content as swc.TextBox;

		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}

		/// <summary>
		/// Identifies the MouseWheelActiveTrigger dependency property
		/// </summary>
		public static readonly sw.DependencyProperty MouseWheelActiveTriggerProperty = sw.DependencyProperty.Register("MouseWheelActiveTrigger", typeof(mwc.Primitives.MouseWheelActiveTrigger), typeof(EtoButtonSpinner), new sw.UIPropertyMetadata(mwc.Primitives.MouseWheelActiveTrigger.FocusedMouseOver));

		/// <summary>
		/// Get or set when the mouse wheel event should affect the value.
		/// </summary>
		public mwc.Primitives.MouseWheelActiveTrigger MouseWheelActiveTrigger
		{
			get => (mwc.Primitives.MouseWheelActiveTrigger)GetValue(MouseWheelActiveTriggerProperty);
			set => SetValue(MouseWheelActiveTriggerProperty, value);
		}

		protected override void OnSpin(mwc.SpinEventArgs e)
		{
			var activeTrigger = this.MouseWheelActiveTrigger;
			bool spin = !e.UsingMouseWheel;
			spin |= (activeTrigger == mwc.Primitives.MouseWheelActiveTrigger.MouseOver);
			spin |= ((TextBox != null) && TextBox.IsFocused && (activeTrigger == mwc.Primitives.MouseWheelActiveTrigger.FocusedMouseOver));
			spin |= ((TextBox != null) && TextBox.IsFocused && (activeTrigger == mwc.Primitives.MouseWheelActiveTrigger.Focused) && (sw.Input.Mouse.Captured is Spinner));

			if (spin)
			{
				base.OnSpin(e);
			}
		}
	}

	public class TextStepperHandler : TextBoxHandler<mwc.ButtonSpinner, TextStepper, TextStepper.ICallback>, TextStepper.IHandler
	{
		public TextStepperHandler()
		{
			Control = new EtoButtonSpinner
			{
				Handler = this,
				IsTabStop = false,
				Focusable = false,

				Content = new mwc.WatermarkTextBox
				{
					KeepWatermarkOnGotFocus = true,
					BorderThickness = new sw.Thickness(0),
					BorderBrush = null,
					Background = null,
					Padding = new sw.Thickness(0),
				},
			};
		}

		public override sw.FrameworkElement TabControl => WatermarkTextBox;

		public override string PlaceholderText
		{
			get { return WatermarkTextBox.Watermark as string; }
			set { WatermarkTextBox.Watermark = value; }
		}

		public StepperValidDirections ValidDirection
		{
			get { return Control.ValidSpinDirection.ToEto(); }
			set { Control.ValidSpinDirection = value.ToWpf(); }
		}

		public bool ShowStepper
		{
			get { return Control.ShowButtonSpinner; }
			set { Control.ShowButtonSpinner = value; }
		}

		public override Color TextColor
		{
			get { return TextBox.Foreground.ToEtoColor(); }
			set { TextBox.Foreground = value.ToWpfBrush(TextBox.Foreground); }
		}

		mwc.WatermarkTextBox WatermarkTextBox => (mwc.WatermarkTextBox)Control.Content;

		protected override swc.TextBox TextBox => (swc.TextBox)Control.Content;

		protected override swc.Control BorderControl => Control;

		protected override sw.FrameworkElement FocusControl => TextBox;

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextStepper.StepEvent:
					Control.Spin += (sender, e) => Callback.OnStep(Widget, new StepperEventArgs(e.Direction == Xceed.Wpf.Toolkit.SpinDirection.Increase ? StepperDirection.Up : StepperDirection.Down));
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
