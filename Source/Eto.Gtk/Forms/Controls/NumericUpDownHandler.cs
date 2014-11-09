using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class NumericUpDownHandler : GtkControl<Gtk.SpinButton, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		public NumericUpDownHandler()
		{
			Control = new Gtk.SpinButton(double.MinValue, double.MaxValue, 1);
			Control.WidthRequest = 80;
			Control.Wrap = true;
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
			get { 
				double step, page;
				Control.GetIncrements(out step, out page);
				return step;
			}
			set
			{
				Control.SetIncrements(value, value * 2);
			}
		}

		public int DecimalPlaces
		{
			get { return (int)Control.Digits; }
			set { Control.Digits = (uint)value; }
		}

		public Color TextColor
		{
			get { return Control.Style.Text(Gtk.StateType.Normal).ToEto(); }
			set { Control.ModifyText(Gtk.StateType.Normal, value.ToGdk()); }
		}

		public override Color BackgroundColor
		{
			get { return Control.Style.Base(Gtk.StateType.Normal).ToEto(); }
			set { Control.ModifyBase(Gtk.StateType.Normal, value.ToGdk()); }
		}
	}
}
