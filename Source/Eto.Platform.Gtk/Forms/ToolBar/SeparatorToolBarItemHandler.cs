using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class SeparatorToolBarItemHandler : ToolBarItemHandler<Gtk.SeparatorToolItem, SeparatorToolBarItem>, ISeparatorToolBarItem
	{
		SeparatorToolBarItemType type;
		bool expand;
		
		public override void CreateControl (ToolBarHandler handler)
		{
			Gtk.Toolbar tb = handler.Control;
			Control = new Gtk.SeparatorToolItem();
			Control.Expand = expand;
			tb.Insert(Control, -1);
			if (tb.Visible) Control.ShowAll();
		}
		
		
		public SeparatorToolBarItemType Type {
			get {
				return type;
			}
			set {
				type = value;
				expand = type == SeparatorToolBarItemType.FlexibleSpace;
				if (Control != null) Control.Expand = expand;
			}
		}
		
	}
}
