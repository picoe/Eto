using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class DialogHandler : GtkWindow<Gtk.Dialog, Dialog>, IDialog
	{
		Button button;
		
		public DialogHandler()
		{
			Control = new Gtk.Dialog();
			Control.AllowShrink = false;
			Control.AllowGrow = false;
			//control.SetSizeRequest(100,100);
			vbox = Control.VBox;
			Control.HasSeparator = false;
			//control.Resizable = true;
		}
		
		public Button DefaultButton {
			get {
				return button;	
			}
			set {
				button = value;
			}
		}
		
		
		/*
		private Gtk.Window FindParentWindow(Gtk.Widget widget)
		{
			while (widget != null && !(widget is Gtk.Window))
			{
				widget = widget.Parent;
			}
			return (Gtk.Window)widget;
			
		}
		 */

		public DialogResult ShowDialog(Control parent)
		{
			if (parent != null)
			{
				Control.TransientFor = ((Gtk.Window)(parent.ParentWindow).ControlObject); //FindParentWindow((Gtk.Widget)parent.ControlObject);
				Control.Modal = true;
			}
			Control.ShowAll();
			Widget.OnLoad(EventArgs.Empty);

			if (button != null)
			{
				var widget = button.ControlObject as Gtk.Widget;
				widget.SetFlag (Gtk.WidgetFlags.CanDefault);
				Control.Default = widget;
			}
			Control.Run();
			Control.HideAll();
									
			return Widget.DialogResult; // Generator.Convert((Gtk.ResponseType)result);
		}

	}
}
