using System;
using System.Collections;

namespace Eto.Forms
{
	public interface ICheckBox : ITextControl
	{
		bool Checked { get; set; }
	}
	
	public class CheckBox : TextControl
	{
		public event EventHandler<EventArgs> CheckedChanged;
		private ICheckBox inner;

		public virtual void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null) CheckedChanged(this, e);
		}

		public CheckBox() : this(Generator.Current)
		{
		}
		
		public CheckBox(Generator g) : base(g, typeof(ICheckBox))
		{
			inner = (ICheckBox)base.Handler;
		}

		public virtual bool Checked
		{
			get { return inner.Checked; }
			set { inner.Checked = value; }
		}
		

	}
}
