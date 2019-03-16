using Eto.Forms;

namespace Eto.GtkSharp.Forms.ToolBar
{
	public class ToolBarHandler : WidgetHandler<Gtk.Toolbar, Eto.Forms.ToolBar>, Eto.Forms.ToolBar.IHandler
	{
		ToolBarDock dock = ToolBarDock.Top;

		#if GTK2
		Gtk.RadioButton radioGroup;
		public GLib.SList RadioGroup
		{
			get { return (radioGroup ?? (radioGroup = new Gtk.RadioButton("g"))).Group; }
		}
		#elif GTK3
		Gtk.RadioToolButton radioGroup;
		public Gtk.RadioToolButton RadioGroup
		{
			get { return (radioGroup ?? (radioGroup = new Gtk.RadioToolButton(new Gtk.RadioToolButton[0]))); }
		}
		#endif
		
		public ToolBarHandler()
		{
			Control = new Gtk.Toolbar();
			//control.ToolbarStyle = Gtk.ToolbarStyle.Both;
		}

		public ToolBarDock Dock
		{
			get { return dock; }
			set { dock = value; }
		}
		
		public void AddButton(ToolItem item, int index)
		{
			((IToolBarItemHandler)item.Handler).CreateControl(this, index);
		}

		public void RemoveButton(ToolItem item, int index)
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
						Control.ToolbarStyle = Gtk.ToolbarStyle.Both;
						break;
				}
			}
		}
	}
}
