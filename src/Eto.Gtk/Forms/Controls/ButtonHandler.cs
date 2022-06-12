using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ButtonHandler : ButtonHandler<Gtk.Button, Button, Button.ICallback>
	{
		public static int MinimumWidth = 80;

		public class EtoButton : Gtk.Button
		{
			WeakReference _reference;
			public ButtonHandler Handler
			{
				get => _reference?.Target as ButtonHandler;
				set => _reference = new WeakReference(value);
			}
#if GTK3
			protected override void OnAdjustSizeRequest(Gtk.Orientation orientation, out int minimum_size, out int natural_size)
			{
				base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
				var h = Handler;
				if (h == null)
					return;
				h.MinimumSize.AdjustMinimumSizeRequest(orientation, ref minimum_size, ref natural_size);
			}
#endif
		}

		protected override int DefaultMinimumWidth => MinimumWidth;

		internal static readonly object Image_Key = new object();
		internal static readonly object ImagePosition_Key = new object();
		internal static readonly object MinimumSize_Key = new object();

		protected override Gtk.Button CreateControl() => new EtoButton { Handler = this };
	}
}
