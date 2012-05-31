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
		IMenuActionItem handler;
		
		public event EventHandler<EventArgs> Click;

		protected MenuActionItem (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			handler = (IMenuActionItem)Handler;
		}
		
		public string Text {
			get { return handler.Text; }
			set { handler.Text = value; }
		}
		
		public string ToolTip {
			get { return handler.ToolTip; }
			set { handler.ToolTip = value; }
		}
		
		public bool Enabled {
			get { return handler.Enabled; }
			set { handler.Enabled = value; }
		}

		public Key Shortcut {
			get { return handler.Shortcut; }
			set { handler.Shortcut = value; }
		}

		public virtual void OnClick (EventArgs e)
		{
			if (Click != null)
				Click (this, e);
		}
		
	}
}

