using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	public interface IMenuItem : IMenu
	{
		string Text { get; set; }

		string ToolTip { get; set; }

		Keys Shortcut { get; set; }

		bool Enabled { get; set; }

		void CreateFromCommand(Command command);
	}

	public abstract class MenuItem : Menu
	{
		public int Order { get; set; }

		new IMenuItem Handler { get { return (IMenuItem)base.Handler; } }

		public const string ValidateEvent = "MenuActionItem.ValidateEvent";

		public event EventHandler<EventArgs> Click;

		public void OnClick(EventArgs e)
		{
			if (Click != null) Click(this, e);
		}

		public event EventHandler<EventArgs> Validate
		{
			add { Properties.AddHandlerEvent(ValidateEvent, value); }
			remove { Properties.RemoveEvent(ValidateEvent, value); }
		}

		public virtual void OnValidate(EventArgs e)
		{
			Properties.TriggerEvent(ValidateEvent, this, e);
		}

		protected MenuItem(Command command, Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			Text = command.MenuText;
			ToolTip = command.ToolTip;
			Shortcut = command.Shortcut;
			Click += (sender, e) => command.OnExecuted(e);
			// CWEN: Need to unregister when removed from menu?
			command.EnabledChanged += (sender, e) => Enabled = command.Enabled;
		}

		protected MenuItem(Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
		}

		static MenuItem()
		{
			EventLookup.Register<MenuItem>(c => c.OnValidate(null), MenuItem.ValidateEvent);
		}

		public string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		public string ToolTip
		{
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}

		public bool Enabled
		{
			get { return Handler.Enabled; }
			set { Handler.Enabled = value; }
		}

		public Keys Shortcut
		{
			get { return Handler.Shortcut; }
			set { Handler.Shortcut = value; }
		}
	}

}
