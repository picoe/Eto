using Eto.GtkSharp.Drawing;
namespace Eto.GtkSharp.Forms
{
	public class ClipboardHandler : WidgetHandler<Gtk.Clipboard, Clipboard, Clipboard.ICallback>, Clipboard.IHandler
	{
		delegate void GetClipboardData(ClipboardData data,Gtk.SelectionData selection);

		enum ClipboardEntryKind
		{
			Exact,
			Text,
			Image,
			Uris
		}

		class ClipboardData
		{
			public ClipboardEntryKind Kind { get; set; }

			public string Type { get; set; }

			public object Data { get; set; }

			public GetClipboardData GetClipboardData { get; set; }

			public Action<Gtk.TargetList, uint> AddTargets { get; set; }

			public void GetData(Gtk.SelectionData selectionData)
			{
				GetClipboardData?.Invoke(this, selectionData);
			}
		}

		Gtk.TargetList targets = new Gtk.TargetList();

		readonly List<ClipboardData> clipboard = new List<ClipboardData>();
		bool changedAttached;
#if NET6_0_OR_GREATER
		bool changedAttachedToWayland;
#endif

		public ClipboardHandler()
		{
			Control = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
		}

		protected override bool DisposeControl => false;

#if NET6_0_OR_GREATER
		// On Wayland with the data-control protocol available, route clipboard access through
		// it instead of GTK's focus-gated Gtk.Clipboard. Falls back to GTK otherwise.
		static bool UseWayland => WaylandClipboard.IsAvailable;

		static readonly string[] WaylandImageTypes =
		{
			"image/png",
			"image/tiff",
			"image/bmp",
			"image/jpeg"
		};
#else
		static bool UseWayland => false;
#endif

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Clipboard.ChangedEvent:
					if (changedAttached)
						break;
#if NET6_0_OR_GREATER
					if (UseWayland)
					{
						WaylandClipboard.SelectionChanged += Connector.HandleWaylandSelectionChanged;
						changedAttachedToWayland = true;
					}
					else
#endif
						Control.OwnerChange += Connector.HandleOwnerChange;
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
#if NET6_0_OR_GREATER
				if (changedAttachedToWayland)
				{
					WaylandClipboard.SelectionChanged -= Connector.HandleWaylandSelectionChanged;
					changedAttachedToWayland = false;
				}
				else
#endif
					Control.OwnerChange -= Connector.HandleOwnerChange;
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
			public new ClipboardHandler Handler => (ClipboardHandler)base.Handler;

			public void HandleOwnerChange(object sender, Gtk.OwnerChangeArgs e)
			{
				var handler = Handler;
				if (handler != null)
					handler.Callback.OnChanged(handler.Widget, EventArgs.Empty);
			}

#if NET6_0_OR_GREATER
			public void HandleWaylandSelectionChanged()
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
#endif
		}

		void Update()
		{
			Control.SetWithData((Gtk.TargetEntry[])targets, (clip, selectionData, info) =>
			{
				if (info < clipboard.Count)
				{
					var clipdata = clipboard[(int)info];
					clipdata.GetData(selectionData);
				}
			}, clip =>
			{

			});
			
#if GTKCORE
			Control.CanStore = (Gtk.TargetEntry[])targets;
#endif
		}

		void RebuildTargets()
		{
			targets = new Gtk.TargetList();
			for (var i = 0; i < clipboard.Count; i++)
			{
				clipboard[i].AddTargets?.Invoke(targets, (uint)i);
			}
		}

		void SetEntry(ClipboardEntryKind kind, string type, object data, GetClipboardData getData, Action<Gtk.TargetList, uint> addTargets)
		{
			clipboard.RemoveAll(entry => entry.Kind == kind && (kind != ClipboardEntryKind.Exact || StringComparer.Ordinal.Equals(entry.Type, type)));
			clipboard.Add(new ClipboardData
			{
				Kind = kind,
				Type = type,
				Data = data,
				GetClipboardData = getData,
				AddTargets = addTargets
			});
			RebuildTargets();
			Update();
		}

		Gtk.SelectionData GetSelectionData(string type)
		{
			var target = Gdk.Atom.Intern(type, false);
			if (Control.WaitIsTargetAvailable(target))
			{
				var data = Control.WaitForContents(target);
				if (data != null && data.GetDataType() != null)
					return data;
			}
			return null;
		}

		public void SetString(string value, string type)
		{
#if NET6_0_OR_GREATER
			if (UseWayland)
			{
				WaylandClipboard.SetData(type, Encoding.UTF8.GetBytes(value ?? string.Empty));
				return;
			}
#endif
			SetEntry(
				ClipboardEntryKind.Exact,
				type,
				value,
				(data, selection) => selection.Set(Gdk.Atom.Intern(data.Type, false), 8, Encoding.UTF8.GetBytes(data.Data as string)),
				(targetList, info) => targetList.Add(type, 0, info)
			);
		}

		public string Html
		{
			set { SetString(value, "text/html"); }
			get { return GetString("text/html"); }
		}

		public string Text
		{
			set
			{
#if NET6_0_OR_GREATER
				if (UseWayland)
				{
					WaylandClipboard.SetText(value);
					return;
				}
#endif
				SetEntry(
					ClipboardEntryKind.Text,
					null,
					value,
					(data, selection) => selection.Text = data.Data as string,
					(targetList, info) => targetList.AddTextTargets(info)
				);
			}
			get
			{
#if NET6_0_OR_GREATER
				if (UseWayland)
					return WaylandClipboard.GetText();
#endif
				return Control.WaitForText();
			}
		}

		public Image Image
		{
			set
			{
#if NET6_0_OR_GREATER
				if (UseWayland)
				{
					var bitmap = value as Bitmap ?? new Bitmap(value);
					WaylandClipboard.SetData("image/png", bitmap.ToByteArray(ImageFormat.Png));
					return;
				}
#endif
				var icon = value as Icon;
				if (icon != null)
				{
					// todo: save as icon
					//SetData(data, "eto-icon");
				}
				var pixbuf = value.ToGdk();
				if (pixbuf == null)
					throw new NotSupportedException();
				SetEntry(
					ClipboardEntryKind.Image,
					null,
					pixbuf,
					(data, selection) => selection.SetPixbuf(data.Data as Gdk.Pixbuf),
					(targetList, info) => targetList.AddImageTargets(info, false)
				);
			}
			get
			{
				var iconData = GetData("eto-icon");
				if (iconData != null)
				{
					return new Icon(new MemoryStream(iconData, false));
				}
#if NET6_0_OR_GREATER
				if (UseWayland)
				{
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
#endif
				var image = Control.WaitForImage();
				if (image != null)
				{
					return new Bitmap(new BitmapHandler(image));
				}
				return null;
			}
		}

		public void SetData(byte[] value, string type)
		{
#if NET6_0_OR_GREATER
			if (UseWayland)
			{
				WaylandClipboard.SetData(type, value);
				return;
			}
#endif
			SetEntry(
				ClipboardEntryKind.Exact,
				type,
				value,
				(data, selection) => selection.Set(Gdk.Atom.Intern(type, false), 8, value),
				(targetList, info) => targetList.Add(type, 0, info)
			);
		}

		public string GetString(string type)
		{
#if NET6_0_OR_GREATER
			if (UseWayland)
				return WaylandClipboard.GetString(type);
#endif
			var data = GetSelectionData(type)?.Data;
			if (data != null)
			{
				return Encoding.UTF8.GetString(data);
			}
			return null;
		}

		public byte[] GetData(string type)
		{
#if NET6_0_OR_GREATER
			if (UseWayland)
				return WaylandClipboard.GetData(type);
#endif
			var selection = GetSelectionData(type);
			return selection != null && selection.Length > 0 ? selection.Data : null;
		}

		public void Clear()
		{
#if NET6_0_OR_GREATER
			if (UseWayland)
			{
				WaylandClipboard.Clear();
				return;
			}
#endif
			Control.Clear();
			targets = new Gtk.TargetList();
			clipboard.Clear();
		}

		public bool Contains(string type)
		{
#if NET6_0_OR_GREATER
			if (UseWayland)
				return WaylandClipboard.Contains(type);
#endif
			return Control.WaitIsTargetAvailable(Gdk.Atom.Intern(type, false));
		}

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
					value = new Bitmap(GetData(type));
					return true;
				}
			}

			value = null;
			return false;
		}

		public void SetObject(object value, string type) => Widget.SetObject(value, type);
		public T GetObject<T>(string type) => Widget.GetObject<T>(type);
		public object GetObject(string type, Type objectType) => Widget.GetObject(type, objectType);
		public object GetObject(string type) => Widget.GetObject(type);

		public string[] Types
		{
			get
			{
#if NET6_0_OR_GREATER
				if (UseWayland)
					return WaylandClipboard.GetMimeTypes();
#endif
				Gdk.Atom[] atoms;
				IntPtr atomPtrs;
				int count;
				var success = NativeMethods.gtk_clipboard_wait_for_targets(Control.Handle, out atomPtrs, out count);

				if (!success || count <= 0)
				{
					atoms = null;
					return new string[0];
				}

				atoms = new Gdk.Atom[count];
				unsafe
				{
					byte* p = (byte*)atomPtrs.ToPointer();
					for (int i = 0; i < count; i++)
					{
						atoms[i] = new Gdk.Atom(new IntPtr(*p));
						p += IntPtr.Size;
					}
				}

				return atoms.Select(r => r.Name).ToArray();
			}
		}


		public bool ContainsText
		{
			get
			{
#if NET6_0_OR_GREATER
				if (UseWayland)
					return WaylandClipboard.ContainsText;
#endif
				return Control.WaitIsTextAvailable();
			}
		}

		public bool ContainsHtml => Contains("text/html");

		public bool ContainsImage
		{
			get
			{
#if NET6_0_OR_GREATER
				if (UseWayland)
					return Contains("eto-icon") || WaylandImageTypes.Any(Contains);
#endif
				return Control.WaitIsImageAvailable() || Contains("eto-icon");
			}
		}

		public bool ContainsUris => Contains("text/uri-list");

		public Uri[] Uris
		{
			set
			{
#if NET6_0_OR_GREATER
				if (UseWayland)
				{
					var list = value == null ? string.Empty : string.Join("\r\n", value.Select(r => r.AbsoluteUri));
					WaylandClipboard.SetData("text/uri-list", Encoding.UTF8.GetBytes(list));
					return;
				}
#endif
				var uris = value?.Select(r => r.AbsoluteUri).ToArray();
				SetEntry(
					ClipboardEntryKind.Uris,
					"text/uri-list",
					value,
					(data, selection) => selection.SetSelectedUris2(uris),
					(targetList, info) => targetList.Add("text/uri-list", 0, info)
				);
			}
			get
			{
#if NET6_0_OR_GREATER
				if (UseWayland)
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
#endif
				var selection = GetSelectionData("text/uri-list");
				return selection?.GetSelectedUris()?.Select(r => new Uri(r)).ToArray();
			}
		}
	}
}
