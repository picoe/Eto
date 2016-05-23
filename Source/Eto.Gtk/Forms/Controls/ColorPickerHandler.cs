using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ColorPickerHandler : GtkControl<Gtk.ColorButton, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
	{
		public ColorPickerHandler()
		{
			Control = new Gtk.ColorButton();
		}

		protected new ColorPickerConnector Connector { get { return (ColorPickerConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ColorPickerConnector();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ColorPicker.ColorChangedEvent:
					Control.ColorSet += Connector.HandleSelectedColorChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected class ColorPickerConnector : GtkControlConnector
		{
			new ColorPickerHandler Handler { get { return (ColorPickerHandler)base.Handler; } }

			public void HandleSelectedColorChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnColorChanged(Handler.Widget, EventArgs.Empty);
			}
		}

#if GTK2
		public Eto.Drawing.Color Color
		{
			get { return Control.Color.ToEto(); }
			set { Control.Color = value.ToGdk(); }
		}
#else
		public Eto.Drawing.Color Color
		{
			get { return Control.Rgba.ToEto(); }
			set { Control.Rgba = value.ToRGBA(); }
		}
#endif
	}
}

