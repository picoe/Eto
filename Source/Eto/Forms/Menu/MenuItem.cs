using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	public interface ICommandItem
	{
		string Text { get; set; }

		string ToolTip { get; set; }

		bool Enabled { get; set; }
	}

	public interface ICommandItemWidget : ICommandItem
	{
		event EventHandler<EventArgs> Click;
	}

	public interface IMenuItem : IMenu, ICommandItem
	{
		Keys Shortcut { get; set; }

		void CreateFromCommand(Command command);
	}

	public abstract class MenuItem : Menu, ICommandItemWidget
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
			ID = command.ID;
			Text = command.MenuText;
			ToolTip = command.ToolTip;
			Shortcut = command.Shortcut;
			Click += (sender, e) => command.OnExecuted(e);
			Enabled = command.Enabled;
			command.EnabledChanged += (sender, e) => Enabled = command.Enabled;
			if (initialize)
				Handler.CreateFromCommand(command);
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
