using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class SeparatorToolItemHandler : ToolItemHandler<Gtk.SeparatorToolItem, SeparatorToolItem>, ISeparatorToolItem
	{
		SeparatorToolItemType type;
		bool expand;

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			Gtk.Toolbar tb = handler.Control;
			Control = new Gtk.SeparatorToolItem();
			Control.Expand = expand;
			tb.Insert(Control, index);
			if (tb.Visible)
				Control.ShowAll();
		}

		public SeparatorToolItemType Type
		{
			get { return type; }
			set
			{
				type = value;
				expand = type == SeparatorToolItemType.FlexibleSpace;
				if (Control != null)
					Control.Expand = expand;
			}
		}
	}
}
