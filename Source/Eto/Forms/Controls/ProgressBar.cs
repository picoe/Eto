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
		IProgressBar handler;
		
		public ProgressBar ()
			: this (Generator.Current)
		{
		}

		public ProgressBar (Generator generator)
			: this (generator, typeof(IProgressBar))
		{
		}
		
		protected ProgressBar (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (IProgressBar)Handler;
		}

		public int MaxValue {
			get { return handler.MaxValue; }
			set { handler.MaxValue = value; }
		}

		public int MinValue {
			get { return handler.MinValue; }
			set { handler.MinValue = value; }
		}

		public int Value {
			get { return handler.Value; }
			set { handler.Value = value; }
		}

		public bool Indeterminate {
			get { return handler.Indeterminate; }
			set { handler.Indeterminate = value; }
		}
	}
}

