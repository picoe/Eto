using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class FontPickerHandler : GtkControl<Gtk.FontButton, FontPicker, FontPicker.ICallback>, FontPicker.IHandler
	{
		public Font Value
		{
			get { return new Font(new FontHandler(Control.FontName)); }
			set
			{
				if (value == null)
					Control.FontName = string.Empty;
				else
				{
					var handler = value.Handler as FontHandler;
					Control.FontName = handler.Control.ToString();
				}
			}
		}

		public FontPickerHandler()
		{
			Control = new Gtk.FontButton();
		}

		protected new FontPickerConnector Connector { get { return (FontPickerConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new FontPickerConnector();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FontPicker.ValueChangedEvent:
					Control.FontSet += Connector.HandleValueChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected class FontPickerConnector : GtkControlConnector
		{
			new FontPickerHandler Handler { get { return (FontPickerHandler)base.Handler; } }

			public void HandleValueChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnValueChanged(Handler.Widget, EventArgs.Empty);
			}
		}
	}
}
