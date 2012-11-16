using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class DialogHandler : GtkWindow<Gtk.Dialog, Dialog>, IDialog
	{
		public class MyDialog : Gtk.Dialog
		{
#if GTK3
			protected override void OnAdjustSizeAllocation (Gtk.Orientation orientation, out int minimum_size, out int natural_size, out int allocated_pos, out int allocated_size)
			{
				base.OnAdjustSizeAllocation (orientation, out minimum_size, out natural_size, out allocated_pos, out allocated_size);
				if (orientation == Gtk.Orientation.Horizontal)
					allocated_size = natural_size;
			}

#endif
		}

		public DialogHandler ()
		{
			Control = new MyDialog ();
#if GTK2
			Control.AllowShrink = false;
			Control.AllowGrow = false;
			vbox = Control.VBox;
			Control.HasSeparator = false;
#else
			Control.Resizable = false;
			Control.HasResizeGrip = false;
			Control.ActionArea.Add (actionvbox);
			Control.ContentArea.Add (vbox);
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
