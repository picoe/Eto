using System;

namespace Eto.Forms
{
	public interface INumericUpDown : ICommonControl
	{
		bool ReadOnly { get; set; }

		double Value { get; set; }

		double MinValue { get; set; }

		double MaxValue { get; set; }
	}

	[Handler(typeof(INumericUpDown))]
	public class NumericUpDown : CommonControl
	{
		new INumericUpDown Handler { get { return (INumericUpDown)base.Handler; } }

		public event EventHandler<EventArgs> ValueChanged;

		protected virtual void OnValueChanged(EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged(this, e);
		}

		public NumericUpDown()
		{
		}

		[Obsolete("Use default constructor instead")]
		public NumericUpDown(Generator generator) : this(generator, typeof(INumericUpDown))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected NumericUpDown(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public bool ReadOnly
		{
			get { return Handler.ReadOnly; }
			set { Handler.ReadOnly = value; }
		}

		public double Value
		{
			get { return Handler.Value; }
			set { Handler.Value = value; }
		}

		public double MinValue
		{
			get { return Handler.MinValue; }
			set { Handler.MinValue = value; }
		}

		public double MaxValue
		{
			get { return Handler.MaxValue; }
			set { Handler.MaxValue = value; }
		}

		public ObjectBinding<NumericUpDown, double> ValueBinding
		{
			get
			{
				return new ObjectBinding<NumericUpDown, double>(
					this, 
					c => c.Value, 
					(c, v) => c.Value = v, 
					(c, h) => c.ValueChanged += h, 
					(c, h) => c.ValueChanged -= h
				)
				{
					SettingNullValue = 0
				};
			}
		}

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : CommonControl.ICallback
		{
			void OnValueChanged(NumericUpDown widget, EventArgs e);
		}

		protected class Callback : CommonControl.Callback, ICallback
		{
			public void OnValueChanged(NumericUpDown widget, EventArgs e)
			{
				widget.OnValueChanged(e);
			}
		}
	}
}
