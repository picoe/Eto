using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IToolItem : IInstanceWidget
	{
		string Text { get; set; }
		string ToolTip { get; set; }
		Image Image { get; set; }
		bool Enabled { get; set; }

		void CreateFromCommand(Command command);
	}

	public abstract class ToolItem : InstanceWidget
	{
		new IToolItem Handler { get { return (IToolItem)base.Handler; } }

		public event EventHandler<EventArgs> Click;

		public void OnClick(EventArgs e)
		{
			if (Click != null) Click(this, e);
		}

		protected ToolItem(Command command, Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			Text = command.ToolBarText;
			ToolTip = command.ToolTip;
			Image = command.Image;
			Click += (sender, e) => command.OnExecuted(e);
			// CWEN: Need to unregister when removed from toolbar
			command.EnabledChanged += (sender, e) => Enabled = command.Enabled;
			Order = -1;
		}

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

		internal protected virtual void OnLoad(EventArgs e)
		{
		}

		internal protected virtual void OnUnLoad(EventArgs e)
		{
		}
	}
}
