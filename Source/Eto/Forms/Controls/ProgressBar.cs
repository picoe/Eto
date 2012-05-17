using System;

namespace Eto.Forms
{
	
	public interface IProgressBar : IControl
	{
		int MaxValue { get; set; }

		int MinValue { get; set; }

		int Value { get; set; }

		bool Indeterminate { get; set; }
	}
	
	public class ProgressBar : Control
	{
		IProgressBar inner;
		
		public ProgressBar ()
			: this(Generator.Current)
		{
		}

		public ProgressBar(Generator generator)
			: base(generator, typeof(IProgressBar), true)
		{
			inner = (IProgressBar)Handler;
		}
	

		public int MaxValue {
			get { return inner.MaxValue; }
			set { inner.MaxValue = value; }
		}

		public int MinValue {
			get { return inner.MinValue; }
			set { inner.MinValue = value; }
		}

		public int Value {
			get { return inner.Value; }
			set { inner.Value = value; }
		}


		public bool Indeterminate {
			get { return inner.Indeterminate; }
			set { inner.Indeterminate = value; }
		}
	}
}

