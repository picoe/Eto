using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms
{
	public class ClipboardHandler : WidgetHandler<Gtk.Clipboard, Clipboard>, IClipboard
	{
		public ClipboardHandler ()
		{
			Control = Gtk.Clipboard.Get (null);
		}

		#region IClipboard implementation
		public void SetString (string value, string type)
		{
			Control.Data [type] = value;
		}

		public string Html {
			set { Control.Data ["text/html"] = value; }
			get { return Control.Data ["text/html"] as string; }
		}

		public string Text {
			set { Control.Text = value; }
			get { return Control.WaitForText (); }
			
		}

		public Image Image {
			set {
				var pixbuf = value.ControlObject as Gdk.Pixbuf;
				if (pixbuf != null)
					Control.Image = pixbuf;
				else
					throw new NotSupportedException ();
			}
			get {
				var image = Control.WaitForImage ();
				if (image != null) {
					var handler = new BitmapHandler (image);
					return new Bitmap (Widget.Generator, handler);
				}
				return null;
			}
		}

		public void SetData (byte[] value, string type)
		{
			Control.Data [type] = value;
		}

		public string GetString (string type)
		{
			return Control.Data [type] as string;
		}

		public byte[] GetData (string type)
		{
			return Control.Data [type] as byte[];
		}

		public void Clear ()
		{
			Control.Clear ();
		}

		public string[] Types {
			get {
				return Control.Data.Keys.OfType<string> ().ToArray ();
			}
		}
		#endregion

		#region IWidget implementation

		#endregion
	}
}

