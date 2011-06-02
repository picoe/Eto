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
		
		public string ID { get; set; }
		
		protected Gtk.Image Image { get; set; }
		Icon icon;

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
					Image = new Gtk.Image((Gtk.IconSet)icon.ControlObject, Gtk.IconSize.Button);
				}
				else Image = null;
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
