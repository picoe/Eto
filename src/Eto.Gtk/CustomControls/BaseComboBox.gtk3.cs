#if GTK3
using System;
using Gtk;

namespace Eto.GtkSharp.CustomControls
{
	public class BaseComboBox : Gtk.EventBox
	{
		ComboEntry entry;
		public int ArrowWidth { get; set; } = 28;

		public BaseComboBox()
		{
			AddEvents((int)Gdk.EventMask.ButtonPressMask);
			AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
			ButtonPressEvent += HandleButtonPressEvent;

			entry = new ComboEntry { Host = this };
			Add(entry);
		}

		[GLib.ConnectBefore]
		private void HandleButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			if (args.Event.X > AllocatedWidth - ArrowWidth && Sensitive)
			{
				OnPopupButtonClicked(EventArgs.Empty);
			}
		}

		public Entry Entry => entry;

		public event EventHandler PopupButtonClicked;

		protected virtual void OnPopupButtonClicked(EventArgs e)
		{
			PopupButtonClicked?.Invoke(this, e);
		}

		class ComboEntry : Gtk.Entry
		{
			public BaseComboBox Host { get; set; }

			protected override void OnGetTextAreaSize(out int x, out int y, out int width, out int height)
			{
				base.OnGetTextAreaSize(out x, out y, out width, out height);
				width -= Host.ArrowWidth;
			}

			protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
			{
				base.OnGetPreferredWidth(out minimum_width, out natural_width);
				natural_width = Math.Max(natural_width, 180);
			}

			protected override bool OnDrawn(Cairo.Context cr)
			{
				bool ret = true;
				var rect = Allocation;
				if (rect.Width > 0 && rect.Height > 0)
				{
					var dropDownPos = rect.Width - Host.ArrowWidth;
					var arrowSize = 10;
					var arrowPos = dropDownPos + (Host.ArrowWidth - arrowSize - 2) / 2;

					StyleContext.Save();

					ret = base.OnDrawn(cr);

					StyleContext.RenderArrow(cr, Math.PI, arrowPos, (rect.Height - arrowSize) / 2, arrowSize);

					cr.SetSourceColor(new Cairo.Color(.8, .8, .8));
					cr.Rectangle(dropDownPos, 2, 1, rect.Height - 4);
					cr.Fill();

					StyleContext.Restore();

				}
				return ret;
			}
		}

	}
}
#endif
