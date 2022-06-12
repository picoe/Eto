using Eto.Forms;
using Eto.Drawing;
using System;

namespace Eto.Test
{
	/// <summary>
	/// Easily bindable entry for a Size struct
	/// </summary>
    public class SizeEntry : TableLayout
    {
        NumericStepper widthText;
        NumericStepper heightText;
        public event EventHandler<EventArgs> ValueChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        public Size Value
        {
            get => new Size((int)widthText.Value, (int)heightText.Value);
            set
            {
                widthText.Value = value.Width;
                heightText.Value = value.Height;
            }
        }

        public SizeEntry()
        {
            widthText = new NumericStepper { MinValue = -1, Value = -1 };
            widthText.ValueChanged += StepperValueChanged;
            heightText = new NumericStepper { MinValue = -1, Value = -1 };
            heightText.ValueChanged += StepperValueChanged;

            Styles.Inherit = false;
            Styles.Add<Label>(null, l => l.VerticalAlignment = VerticalAlignment.Center);

            Rows.Add(new TableRow("Width:", widthText, "Height:", heightText, null));
        }

        void StepperValueChanged(object sender, EventArgs e)
        {
            OnValueChanged(e);
        }

        public BindableBinding<SizeEntry, Size> ValueBinding =>
            new BindableBinding<SizeEntry, Size>(
                this,
                c => c.Value,
                (c, v) => c.Value = v,
                (s, eh) => s.ValueChanged += eh,
                (s, eh) => s.ValueChanged -= eh
            );
    }

	/// <summary>
	/// Easily bindable entry for a SizeF struct
	/// </summary>
	public class SizeFEntry : TableLayout
	{
		NumericStepper widthText;
		NumericStepper heightText;
		public event EventHandler<EventArgs> ValueChanged;

		protected virtual void OnValueChanged(EventArgs e)
		{
			ValueChanged?.Invoke(this, e);
		}

		public SizeF Value
		{
			get => new SizeF((float)widthText.Value, (float)heightText.Value);
			set
			{
				widthText.Value = value.Width;
				heightText.Value = value.Height;
			}
		}

		public SizeFEntry()
		{
			widthText = new NumericStepper { MinValue = -1, Value = -1 };
			widthText.ValueChanged += StepperValueChanged;
			heightText = new NumericStepper { MinValue = -1, Value = -1 };
			heightText.ValueChanged += StepperValueChanged;

			Styles.Inherit = false;
			Styles.Add<Label>(null, l => l.VerticalAlignment = VerticalAlignment.Center);

			Rows.Add(new TableRow("Width:", widthText, "Height:", heightText, null));
		}

		void StepperValueChanged(object sender, EventArgs e)
		{
			OnValueChanged(e);
		}

		public BindableBinding<SizeFEntry, SizeF> ValueBinding =>
			new BindableBinding<SizeFEntry, SizeF>(
				this,
				c => c.Value,
				(c, v) => c.Value = v,
				(s, eh) => s.ValueChanged += eh,
				(s, eh) => s.ValueChanged -= eh
			);
	}
}
