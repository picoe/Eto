using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IToolItem : IWidget, ICommandItem
	{
		Image Image { get; set; }

		void CreateFromCommand(Command command);
	}

	public abstract class ToolItem : Widget, ICommandItemWidget
	{
		new IToolItem Handler { get { return (IToolItem)base.Handler; } }

		public event EventHandler<EventArgs> Click;

		public void OnClick(EventArgs e)
		{
			if (Click != null) Click(this, e);
		}

		protected ToolItem()
		{
		}

		protected ToolItem(Command command)
		{
			ID = command.ID;
			Text = command.ToolBarText;
			ToolTip = command.ToolTip;
			Image = command.Image;
			Click += (sender, e) => command.Execute();
			Enabled = command.Enabled;
			command.EnabledChanged += (sender, e) => Enabled = command.Enabled;
			Order = -1;
		}

		[Obsolete("Use ToolItem(Command) instead")]
		protected ToolItem(Command command, Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			ID = command.ID;
			Text = command.ToolBarText;
			ToolTip = command.ToolTip;
			Image = command.Image;
			Click += (sender, e) => command.Execute();
			Enabled = command.Enabled;
			command.EnabledChanged += (sender, e) => Enabled = command.Enabled;
			Order = -1;
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ToolItem(Generator g, Type type)
			: base(g, type)
		{
		}

		public int Order { get; set; }

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

		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		public bool Enabled
		{
			get { return Handler.Enabled; }
			set { Handler.Enabled = value; }
		}

		public object Tag { get; set; }

		internal protected virtual void OnLoad(EventArgs e)
		{
		}

		internal protected virtual void OnUnLoad(EventArgs e)
		{
		}
	}
}
