using System;
using Eto.Forms;
using System.Threading.Tasks;
using Eto.GtkSharp.Forms.Controls;

namespace Eto.GtkSharp.Forms
{
	public class DialogHandler : GtkWindow<Gtk.Dialog, Dialog, Dialog.ICallback>, Dialog.IHandler
	{
		public class MyDialog : Gtk.Dialog
		{
			#if GTK3
			protected override void OnAdjustSizeAllocation(Gtk.Orientation orientation, out int minimum_size, out int natural_size, out int allocated_pos, out int allocated_size)
			{
				base.OnAdjustSizeAllocation(orientation, out minimum_size, out natural_size, out allocated_pos, out allocated_size);
				if (orientation == Gtk.Orientation.Horizontal)
					allocated_size = natural_size;
			}
			#endif
		}

		public DialogHandler()
		{
			Control = new MyDialog();
#if GTK2
			Control.AllowShrink = false;
			Control.AllowGrow = false;
			Control.HasSeparator = false;
#else
			Control.Resizable = false;
			Control.HasResizeGrip = false;
#endif
		}

		protected override void Initialize()
		{
			base.Initialize();
#if GTK2
			Control.VBox.PackStart(WindowActionControl, false, true, 0);
			Control.VBox.PackStart(WindowContentControl, true, true, 0);
#else
			Control.ActionArea.Add(WindowActionControl);
			Control.ContentArea.Add(WindowContentControl);
#endif
		}

		public Button AbortButton { get; set; }

		public Button DefaultButton { get; set; }

		public DialogDisplayMode DisplayMode { get; set; }

		public void ShowModal(Control parent)
		{
			if (parent != null)
			{
				Control.TransientFor = ((Gtk.Window)(parent.ParentWindow).ControlObject);
				Control.Modal = true;
			}
			Control.ShowAll();

			if (DefaultButton != null)
			{
				var widget = DefaultButton.GetContainerWidget();
				if (widget != null)
				{
#if GTK2
					widget.SetFlag(Gtk.WidgetFlags.CanDefault);
#else
					widget.CanDefault = true;
#endif
					Control.Default = widget;
				}
			}
			// TODO: implement cancel button somehow?
			
			Control.Run();
			Control.Hide();
		}

		public Task ShowModalAsync(Control parent)
		{
			var tcs = new TaskCompletionSource<bool>();
			Application.Instance.AsyncInvoke(() =>
			{
				ShowModal(parent);
				tcs.SetResult(true);
			});

			return tcs.Task;
		}
	}
}
