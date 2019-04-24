using Eto.Forms;

namespace Eto.GtkSharp.Forms.ToolBar
{
	public class SeparatorToolItemHandler : ToolItemHandler<Gtk.SeparatorToolItem, SeparatorToolItem>, SeparatorToolItem.IHandler
	{
		SeparatorToolItemType type;

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			Gtk.Toolbar tb = handler.Control;
			Control = new Gtk.SeparatorToolItem();
			Control.ShowAll();
			Control.NoShowAll = true;
			Control.Visible = Visible;
			SetType();
			tb.Insert(Control, index);
		}

		void SetType()
		{
			if (Control != null)
			{
				Control.Expand = type == SeparatorToolItemType.FlexibleSpace;
				Control.Draw = type == SeparatorToolItemType.Divider;
			}
		}

		public SeparatorToolItemType Type
		{
			get { return type; }
			set
			{
				type = value;
				SetType();
			}
		}
	}
}
