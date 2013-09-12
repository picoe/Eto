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
			Control.AllowShrink = false;
			Control.AllowGrow = false;
			//control.SetSizeRequest(100,100);
			Control.HasSeparator = false;
			//control.Resizable = true;
		}
		protected override void Initialize()
		{
			base.Initialize();
			Control.Add(WindowContentControl);
		}


		public Button AbortButton {
			get;
			set;
		}
		
		public Button DefaultButton {
			get;
			set;
		}
		
		
		public DialogDisplayMode DisplayMode { get; set; }

		public DialogResult ShowDialog (Control parent)
		{
			Widget.OnPreLoad (EventArgs.Empty);
			
			if (parent != null) {
				Control.TransientFor = ((Gtk.Window)(parent.ParentWindow).ControlObject);
				Control.Modal = true;
			}
			Control.ShowAll ();
			Widget.OnLoad (EventArgs.Empty);

			if (DefaultButton != null) {
				var widget = DefaultButton.GetContainerWidget();
				if (widget != null)
				{
					widget.SetFlag(Gtk.WidgetFlags.CanDefault);
					Control.Default = widget;
				}
			}
			// TODO: implement cancel button somehow?
			
			Control.Run ();
			Control.HideAll ();
									
			return Widget.DialogResult;
		}

	}
}
