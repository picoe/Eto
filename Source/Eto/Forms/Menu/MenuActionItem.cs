#if DESKTOP
using System;

namespace Eto.Forms
{
	public interface IMenuActionItem : IMenuItem
	{
		string Text { get; set; }

		string ToolTip { get; set; }

		Key Shortcut { get; set; }

		bool Enabled { get; set; }
	}
	
	public abstract class MenuActionItem : MenuItem
	{
		new IMenuActionItem Handler { get { return (IMenuActionItem)base.Handler; } }
		
		public const string ValidateEvent = "MenuActionItem.ValidateEvent";

		event EventHandler<EventArgs> _Validate;

		public event EventHandler<EventArgs> Validate {
			add {
				_Validate += value;
				HandleEvent (ValidateEvent);
			}
			remove { _Validate -= value; }
		}

		public virtual void OnValidate (EventArgs e)
		{
			if (_Validate != null)
				_Validate (this, e);
		}
		
		public event EventHandler<EventArgs> Click;

		protected MenuActionItem (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}
		
		public string Text {
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}
		
		public string ToolTip {
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}
		
		public bool Enabled {
			get { return Handler.Enabled; }
			set { Handler.Enabled = value; }
		}

		public Key Shortcut {
			get { return Handler.Shortcut; }
			set { Handler.Shortcut = value; }
		}

		public virtual void OnClick (EventArgs e)
		{
			if (Click != null)
				Click (this, e);
		}
		
	}
}
#endif