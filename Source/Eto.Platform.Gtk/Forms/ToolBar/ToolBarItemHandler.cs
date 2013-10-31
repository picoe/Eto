using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public interface IToolBarItemHandler
	{
		void CreateControl(ToolBarHandler handler);
		
	}

	public abstract class ToolBarItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IToolBarItem, IToolBarItemHandler
		where TControl: Gtk.Widget
		where TWidget: ToolBarItem
	{
		bool enabled = true;
		Image image;

		protected Gtk.Image GtkImage { get; set; }

		public abstract void CreateControl(ToolBarHandler handler);
		
		public string Text { get; set; }
		
		public string ToolTip { get; set; }

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				GtkImage = image.ToGtk (Gtk.IconSize.Button);
			}
		}

		public bool Enabled 
		{
			get { return enabled; }
			set { 
				enabled = value;
				if (Control != null)
					Control.Sensitive = value;
			}
		}
	}
}
