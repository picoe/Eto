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
		
		protected Gtk.Image GtkImage { get; set; }
		Icon icon;
        Image image;

		public abstract void CreateControl(ToolBarHandler handler);
		
		public string Text { get; set; }
		
		public string ToolTip { get; set; }

		public Icon Icon
		{
			get { return icon; }
			set
			{
				this.icon = value;
				
				if (icon != null)
				{
					GtkImage = new Gtk.Image((Gtk.IconSet)icon.ControlObject, Gtk.IconSize.Button);
				}
				else GtkImage = null;
			}
		}

        // ADDED
        public Image Image
        {
            get { return image; }
            set
            {
                this.image = value;

                if (image != null)
                {
                    GtkImage = (Gtk.Image)(image.ControlObject);
                }
                else GtkImage = null;
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
