#if DESKTOP
using System;

namespace Eto.Forms
{
	public interface IMenuActionItem : IMenuItem
	{
		string Text { get; set; }

		string ToolTip { get; set; }

		Keys Shortcut { get; set; }

		bool Enabled { get; set; }
	}

	public abstract class MenuActionItem : MenuItem
	{
		new IMenuActionItem Handler { get { return (IMenuActionItem)base.Handler; } }

		public const string ValidateEvent = "MenuActionItem.ValidateEvent";

		public event EventHandler<EventArgs> Validate
		{
			add { Properties.AddHandlerEvent(ValidateEvent, value); }
			remove { Properties.RemoveEvent(ValidateEvent, value); }
		}

		public virtual void OnValidate(EventArgs e)
		{
			Properties.TriggerEvent(ValidateEvent, this, e);
		}

		public event EventHandler<EventArgs> Click
		{
			add { Properties.AddEvent(ClickKey, value); }
			remove { Properties.RemoveEvent(ClickKey, value); }
		}

		static readonly object ClickKey = new object();

		public virtual void OnClick(EventArgs e)
		{
			Properties.TriggerEvent(ClickKey, this, e);
		}

		protected MenuActionItem(Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
		}

		static MenuActionItem()
		{
			EventLookup.Register(typeof(MenuActionItem), "OnValidate", MenuActionItem.ValidateEvent);
		}

		public override string Text
		{
			get { return Handler.Text; }
			set
			{
				base.Text = value;  // Call the base setter. This parses the fields, if any.
				Handler.Text = MenuText; // retrieve the value from the parsed MenuText.
			}
		}

		public override string ToolTip
		{
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}

		public override bool Enabled
		{
			get { return Handler.Enabled; }
			set
			{
				base.Enabled = value;
				Handler.Enabled = value;
			}
		}

		public Keys Shortcut
		{
			get { return Handler.Shortcut; }
			set { Handler.Shortcut = value; }
		}
	}
}
#endif