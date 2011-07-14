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
	
	public class MenuActionItem : MenuItem
	{
		IMenuActionItem inner;
		
		public event EventHandler<EventArgs> Click;

		public MenuActionItem (Generator g, Type type)
			: base(g, type)
		{
			inner = (IMenuActionItem)Handler;
		}
		
		public string Text {
			get { return inner.Text; }
			set { inner.Text = value; }
		}
		
		public string ToolTip {
			get { return inner.ToolTip; }
			set { inner.ToolTip = value; }
		}
		
		public bool Enabled {
			get { return inner.Enabled; }
			set { inner.Enabled = value; }
		}

		public Key Shortcut {
			get { return inner.Shortcut; }
			set { inner.Shortcut = value; }
		}

		public virtual void OnClick (EventArgs e)
		{
			if (Click != null)
				Click (this, e);
		}
		
	}
}

