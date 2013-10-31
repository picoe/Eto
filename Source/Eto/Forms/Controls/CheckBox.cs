using System;

namespace Eto.Forms
{
	public interface ICheckBox : ITextControl
	{
		bool? Checked { get; set; }
		
		bool ThreeState { get; set; }
	}
	
	public class CheckBox : TextControl
	{
		public event EventHandler<EventArgs> CheckedChanged;

		new ICheckBox Handler { get { return (ICheckBox)base.Handler; } }

		public virtual void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null)
				CheckedChanged(this, e);
		}

		public CheckBox() : this (Generator.Current)
		{
		}
		
		public CheckBox(Generator g) : this (g, typeof(ICheckBox))
		{
		}
		
		protected CheckBox(Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
		}

		public virtual bool? Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}
		
		public bool ThreeState
		{
			get { return Handler.ThreeState; }
			set { Handler.ThreeState = value; }
		}

		public ObjectBinding<CheckBox, bool?> CheckedBinding
		{
			get
			{
				return new ObjectBinding<CheckBox, bool?>(
					this, 
					c => c.Checked, 
					(c, v) => c.Checked = v, 
					(c, h) => c.CheckedChanged += h, 
					(c, h) => c.CheckedChanged -= h
				);
			}
		}

	}
}
