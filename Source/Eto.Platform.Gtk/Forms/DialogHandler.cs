using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class DialogHandler : GtkWindow<Gtk.Dialog, Dialog>, IDialog
	{
		public DialogHandler ()
		{
			Control = new Gtk.Dialog ();
#if GTK2
			Control.AllowShrink = false;
			Control.AllowGrow = false;
			vbox = Control.VBox;
			Control.HasSeparator = false;
#else
			Control.Resizable = false;
			Control.HasResizeGrip = false;
#endif
		}
		
		public Button AbortButton {
			get;
			set;
		}
		
		public Button DefaultButton {
			get;
			set;
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

		public DialogDisplayMode DisplayMode { get; set; }

		public DialogResult ShowDialog (Control parent)
		{
			Widget.OnPreLoad (EventArgs.Empty);
			
			if (parent != null) {
				Control.TransientFor = ((Gtk.Window)(parent.ParentWindow).ControlObject); //FindParentWindow((Gtk.Widget)parent.ControlObject);
				Control.Modal = true;
			}
			Control.ShowAll ();
			Widget.OnLoad (EventArgs.Empty);

			if (DefaultButton != null) {
				var widget = DefaultButton.ControlObject as Gtk.Widget;
#if GTK2
				widget.SetFlag (Gtk.WidgetFlags.CanDefault);
#else
				widget.CanDefault = true;
#endif
				Control.Default = widget;
			}
			// TODO: implement cancel button somehow?
			
			Control.Run ();
			Control.Hide ();
									
			return Widget.DialogResult; // Generator.Convert((Gtk.ResponseType)result);
		}

	}
}
