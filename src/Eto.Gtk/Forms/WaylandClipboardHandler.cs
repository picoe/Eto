// Clipboard handler that routes all access through the Wayland data-control protocol
// (see WaylandClipboard) instead of GTK's focus-gated Gtk.Clipboard.
//
// It is selected at registration time (Platform.cs) only when WaylandClipboard.IsAvailable;
// otherwise the standard GTK ClipboardHandler is used. Availability is latched for the
// process lifetime, so choosing once at construction is equivalent to the previous per-call
// UseWayland check that this replaces.

#if NET6_0_OR_GREATER

namespace Eto.GtkSharp.Forms
{
	public class WaylandClipboardHandler : WidgetHandler<Gtk.Clipboard, Clipboard, Clipboard.ICallback>, Clipboard.IHandler
	{
		// Image mime types we probe when reading an image, best first.
		static readonly string[] WaylandImageTypes =
		{
			"image/png",
			"image/tiff",
			"image/bmp",
			"image/jpeg"
		};

		bool changedAttached;

		public WaylandClipboardHandler()
		{
			// Data operations never touch this control, but the base widget-handler plumbing
			// (Widget/Callback/Connector used for the Changed event) expects a valid control.
			Control = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
		}

		protected override bool DisposeControl => false;

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Clipboard.ChangedEvent:
					if (changedAttached)
						break;
					WaylandClipboard.SelectionChanged += Connector.HandleSelectionChanged;
					changedAttached = true;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && changedAttached)
			{
				WaylandClipboard.SelectionChanged -= Connector.HandleSelectionChanged;
				changedAttached = false;
			}
			base.Dispose(disposing);
		}

		protected new ClipboardConnector Connector => (ClipboardConnector)base.Connector;

		protected override WeakConnector CreateConnector()
		{
			return new ClipboardConnector();
		}

		protected class ClipboardConnector : WeakConnector
		{
			public new WaylandClipboardHandler Handler => (WaylandClipboardHandler)base.Handler;

			public void HandleSelectionChanged()
			{
				var handler = Handler;
				if (handler == null)
					return;

				var application = Eto.Forms.Application.Instance;
				if (application != null)
					application.AsyncInvoke(() => handler.Callback.OnChanged(handler.Widget, EventArgs.Empty));
				else
					handler.Callback.OnChanged(handler.Widget, EventArgs.Empty);
			}
		}

		public void SetString(string value, string type)
		{
			WaylandClipboard.SetData(type, Encoding.UTF8.GetBytes(value ?? string.Empty));
		}

		public string Html
		{
			set { SetString(value, "text/html"); }
			get { return GetString("text/html"); }
		}

		public string Text
		{
			set { WaylandClipboard.SetText(value); }
			get { return WaylandClipboard.GetText(); }
		}

		public Image Image
		{
			set
			{
				var bitmap = value as Bitmap ?? new Bitmap(value);
				WaylandClipboard.SetData("image/png", bitmap.ToByteArray(ImageFormat.Png));
			}
			get
			{
				var iconData = GetData("eto-icon");
				if (iconData != null)
				{
					// Guard the decode: malformed bytes should fall through to the image-mime probe below
					// rather than throw out of the getter (mirrors the try/catch on the image branch).
					try
					{
						return new Icon(new MemoryStream(iconData, false));
					}
					catch
					{
					}
				}
				foreach (var type in WaylandImageTypes)
				{
					var imageData = WaylandClipboard.GetData(type);
					if (imageData == null)
						continue;
					try
					{
						return new Bitmap(imageData);
					}
					catch
					{
					}
				}
				return null;
			}
		}

		public void SetData(byte[] value, string type)
		{
			WaylandClipboard.SetData(type, value);
		}

		public string GetString(string type) => WaylandClipboard.GetString(type);

		public byte[] GetData(string type) => WaylandClipboard.GetData(type);

		public void Clear()
		{
			WaylandClipboard.Clear();
		}

		public bool Contains(string type) => WaylandClipboard.Contains(type);

		public bool TrySetObject(object value, string type) => false;

		public bool TryGetObject(string type, Type objectType, out object value)
		{
			if (objectType == null || objectType == typeof(string))
			{
				if (DataObjectHandler.string_types.Contains(type, StringComparer.OrdinalIgnoreCase))
				{
					value = GetString(type);
					if (value != null)
						return true;
				}
			}
			if (objectType == null || objectType == typeof(Bitmap))
			{
				if (DataObjectHandler.image_types.Contains(type, StringComparer.OrdinalIgnoreCase))
				{
					// Only claim success when the data is actually present; new Bitmap(null) would throw out of
					// TryGetObject instead of letting the caller fall back.
					var data = GetData(type);
					if (data != null)
					{
						value = new Bitmap(data);
						return true;
					}
				}
			}

			value = null;
			return false;
		}

		public void SetObject(object value, string type) => Widget.SetObject(value, type);
		public T GetObject<T>(string type) => Widget.GetObject<T>(type);
		public object GetObject(string type, Type objectType) => Widget.GetObject(type, objectType);
		public object GetObject(string type) => Widget.GetObject(type);

		public string[] Types => WaylandClipboard.GetMimeTypes();

		public bool ContainsText => WaylandClipboard.ContainsText;

		public bool ContainsHtml => Contains("text/html");

		public bool ContainsImage
		{
			get
			{
				// Fetch the mime list once rather than calling Contains() (which re-fetches and clones the list)
				// up to five times.
				var types = WaylandClipboard.GetMimeTypes();
				return types.Contains("eto-icon", StringComparer.OrdinalIgnoreCase)
					|| WaylandImageTypes.Any(img => types.Contains(img, StringComparer.OrdinalIgnoreCase));
			}
		}

		public bool ContainsUris => Contains("text/uri-list");

		public Uri[] Uris
		{
			set
			{
				var list = value == null ? string.Empty : string.Join("\r\n", value.Select(r => r.AbsoluteUri));
				WaylandClipboard.SetData("text/uri-list", Encoding.UTF8.GetBytes(list));
			}
			get
			{
				var bytes = WaylandClipboard.GetData("text/uri-list");
				if (bytes == null)
					return null;
				return Encoding.UTF8.GetString(bytes)
					.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(l => l.Trim())
					.Where(l => l.Length > 0 && !l.StartsWith("#"))
					.Select(l => Uri.TryCreate(l, UriKind.Absolute, out var u) ? u : null)
					.Where(u => u != null)
					.ToArray();
			}
		}
	}
}

#endif
