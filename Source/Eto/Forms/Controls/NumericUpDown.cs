using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface INumericUpDown : ICommonControl
	{
		bool ReadOnly { get; set; }

		double Value { get; set; }

		double MinValue { get; set; }

		double MaxValue { get; set; }
	}
	
	public class NumericUpDown : CommonControl
	{
		INumericUpDown handler;
		
		public event EventHandler<EventArgs> ValueChanged;
		
		public virtual void OnValueChanged (EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged (this, e);
		}
		
		public NumericUpDown () : this (Generator.Current)
		{
		}
		
		public NumericUpDown (Generator g) : this (g, typeof(INumericUpDown))
		{
		}
		
		protected NumericUpDown (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (INumericUpDown)base.Handler;
		}
		
		public bool ReadOnly {
			get { return handler.ReadOnly; }
			set { handler.ReadOnly = value; }
		}
		
		public double Value {
			get { return handler.Value; }
			set { handler.Value = value; }
		}

		public double MinValue {
			get { return handler.MinValue; }
			set { handler.MinValue = value; }
		}

		public double MaxValue {
			get { return handler.MaxValue; }
			set { handler.MaxValue = value; }
		}
	}
}
