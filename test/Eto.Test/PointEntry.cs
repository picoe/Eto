using Eto.Forms;
using Eto.Drawing;
using System;

namespace Eto.Test
{
	/// <summary>
	/// Easily bindable entry for a Size struct
	/// </summary>
    public class PointEntry : TableLayout
    {
        NumericStepper x;
        NumericStepper y;
        public event EventHandler<EventArgs> ValueChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        public Point Value
        {
            get => new Point((int)x.Value, (int)y.Value);
            set
            {
                x.Value = value.X;
                y.Value = value.Y;
            }
        }

        public PointEntry()
        {
            x = new NumericStepper { MinValue = -1, Value = -1, Width = 50 };
            x.ValueChanged += StepperValueChanged;
            y = new NumericStepper { MinValue = -1, Value = -1, Width = 50 };
            y.ValueChanged += StepperValueChanged;

            Styles.Inherit = false;
            Styles.Add<Label>(null, l => l.VerticalAlignment = VerticalAlignment.Center);

            Rows.Add(new TableRow("X:", x, "Y:", y, null));
        }

        void StepperValueChanged(object sender, EventArgs e)
        {
            OnValueChanged(e);
        }

        public BindableBinding<PointEntry, Point> ValueBinding =>
            new BindableBinding<PointEntry, Point>(
                this,
                c => c.Value,
                (c, v) => c.Value = v,
                (s, eh) => s.ValueChanged += eh,
                (s, eh) => s.ValueChanged -= eh
            );
    }
}
