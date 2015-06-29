using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class NumericUpDownHandler : GtkControl<Gtk.SpinButton, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		class EtoSpinButton : Gtk.SpinButton
		{
			public EtoSpinButton()
				: base(double.MinValue, double.MaxValue, 1)
			{
			}

			#if GTK3
			protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
			{
				// gtk calculates size based on min/max value, so give sane defaults
				natural_width = 120;
				minimum_width = 0;
			}
			#endif
		}

		public NumericUpDownHandler()
		{
			Control = new EtoSpinButton();
			Control.WidthRequest = 120;
			Control.Wrap = true;
			Control.Adjustment.PageIncrement = 1;
			Value = 0;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.ValueChanged += Connector.HandleValueChanged;
		}

		protected new NumericUpDownConnector Connector { get { return (NumericUpDownConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new NumericUpDownConnector();
		}

		protected class NumericUpDownConnector : GtkControlConnector
		{
			public new NumericUpDownHandler Handler { get { return (NumericUpDownHandler)base.Handler; } }

			public void HandleValueChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnValueChanged(Handler.Widget, EventArgs.Empty);
			}
		}
		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly
		{
			get { return !Control.IsEditable; }
			set { Control.IsEditable = !value; }
		}

		public double Value
		{
			get { return Control.Value; }
			set { Control.Value = value; }
		}

		public double MaxValue
		{
			get { return Control.Adjustment.Upper; }
			set { Control.Adjustment.Upper = value; }
		}

		public double MinValue
		{
			get { return Control.Adjustment.Lower; }
			set { Control.Adjustment.Lower = value; }
		}

		public double Increment
		{
			get { return Control.Adjustment.StepIncrement; }
			set
			{
				Control.Adjustment.StepIncrement = value;
				Control.Adjustment.PageIncrement = value;
			}
		}

		public int DecimalPlaces
		{
			get { return (int)Control.Digits; }
			set { Control.Digits = (uint)value; }
		}

		public Color TextColor
		{
			get { return Control.GetTextColor(); }
			set { Control.SetTextColor(value); }
		}

		public override Color BackgroundColor
		{
			get { return Control.GetBase(); }
			set { Control.SetBase(value); }
		}
	}
}
