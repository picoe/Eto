using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface INumericUpDown : IControl
	{
		bool ReadOnly { get; set; }

		double Value { get; set; }

		double MinValue { get; set; }

		double MaxValue { get; set; }
		
		Font Font { get; set; }
	}
	
	public class NumericUpDown : Control
	{
		INumericUpDown inner;
		
		public event EventHandler<EventArgs> ValueChanged;
		
		public virtual void OnValueChanged (EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged (this, e);
		}
		
		public NumericUpDown () : this(Generator.Current)
		{
		}
		
		public NumericUpDown (Generator g) : base(g, typeof(INumericUpDown))
		{
			inner = (INumericUpDown)base.Handler;
		}
		
		public bool ReadOnly {
			get { return inner.ReadOnly; }
			set { inner.ReadOnly = value; }
		}
		
		public double Value {
			get { return inner.Value; }
			set { inner.Value = value; }
		}

		public double MinValue {
			get { return inner.MinValue; }
			set { inner.MinValue = value; }
		}

		public double MaxValue {
			get { return inner.MaxValue; }
			set { inner.MaxValue = value; }
		}
		
		public Font Font {
			get { return inner.Font; }
			set { inner.Font = value; }
		}
	}
}
