using System;
using System.Collections;

namespace Eto.Forms
{
	public interface INumericUpDown : IControl
	{
		bool ReadOnly { get; set; }
		decimal Value { get; set; }
	}
	
	public class NumericUpDown : Control
	{
		INumericUpDown inner;
		
		public NumericUpDown() : this(Generator.Current) { }
		
		public NumericUpDown(Generator g) : base(g, typeof(INumericUpDown))
		{
			inner = (INumericUpDown)base.Handler;
		}
		
		public bool ReadOnly
		{
			get { return inner.ReadOnly; }
			set { inner.ReadOnly = value; }
		}
		
		public decimal Value
		{
			get { return inner.Value; }
			set { inner.Value = value; }
		}
	}
}
