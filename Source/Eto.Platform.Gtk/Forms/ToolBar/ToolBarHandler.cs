using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class ToolBarHandler : WidgetHandler<Gtk.Toolbar, ToolBar>, IToolBar
	{
		ToolBarDock dock = ToolBarDock.Top;
		
		public ToolBarHandler()
		{
			Control = new Gtk.Toolbar();
			//control.ToolbarStyle = Gtk.ToolbarStyle.Both;
		}


		#region IToolBar Members

		public ToolBarDock Dock
		{
			get { return dock; }
			set { dock = value; }
		}
		
		public void AddButton(ToolBarItem item)
		{
			((IToolBarItemHandler)item.Handler).CreateControl(this);
		}

		public void RemoveButton(ToolBarItem item)
		{
			if (item.ControlObject != null) Control.Remove((Gtk.Widget)item.ControlObject);
		}

		public void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
		}

		public ToolBarTextAlign TextAlign
		{
			get
			{
				switch (Control.ToolbarStyle)
				{
					case Gtk.ToolbarStyle.BothHoriz:
						return ToolBarTextAlign.Right;
					default:
					case Gtk.ToolbarStyle.Both:
						return ToolBarTextAlign.Underneath;
				}
			}
			set
			{
				switch (value)
				{
					case ToolBarTextAlign.Right:
						Control.ToolbarStyle = Gtk.ToolbarStyle.BothHoriz;
						break;
					default:
					case ToolBarTextAlign.Underneath:
						Control.ToolbarStyle = Gtk.ToolbarStyle.Both;
						break;
				}
			}
		}
		#endregion

	}
}
