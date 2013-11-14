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
		new IProgressBar Handler { get { return (IProgressBar)base.Handler; } }

		public ProgressBar()
			: this((Generator)null)
		{
		}

		public ProgressBar (Generator generator)
			: this (generator, typeof(IProgressBar))
		{
		}
		
		protected ProgressBar (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		public int MaxValue {
			get { return Handler.MaxValue; }
			set { Handler.MaxValue = value; }
		}

		public int MinValue {
			get { return Handler.MinValue; }
			set { Handler.MinValue = value; }
		}

		public int Value {
			get { return Handler.Value; }
			set { Handler.Value = value; }
		}

		public bool Indeterminate {
			get { return Handler.Indeterminate; }
			set { Handler.Indeterminate = value; }
		}
	}
}

