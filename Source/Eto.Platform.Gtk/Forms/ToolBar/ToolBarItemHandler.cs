using System;
using System.Reflection;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public interface IToolBarItemHandler
	{
		void CreateControl(ToolBarHandler handler);
		
	}

	public abstract class ToolBarItemHandler<T, W> : WidgetHandler<T, W>, IToolBarItem, IToolBarItemHandler
		where T: Gtk.Widget
		where W: ToolBarItem
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
				this.image = value;
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
