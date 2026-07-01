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

		public ClipboardHandler()
		{
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
				if (handler == null || !IsNewOwnerChange(e.Event))
					return;
				handler.Callback.OnChanged(handler.Widget, EventArgs.Empty);
			}

			// Fire only for owner-change events that hand the selection to a NEW owner, ignoring the ones that
			// merely tear an owner down (owner==NULL, reason DESTROY/CLOSE).
			//
			// Every real "clipboard now holds new content" is a single reason==NEW_OWNER event. But a selection
			// source that exits (short-lived tools, or GTK's own Wayland double-emit -- GNOME #775631) produces a
			// spurious owner==NULL teardown event first, and often a follow-up as a clipboard manager re-takes
			// ownership to persist the data. Firing on the teardown double-reports a single change. Gating on
			// NEW_OWNER drops exactly the teardown event without reading any clipboard content, so it also can't
			// suppress a genuine change to identical content (that is still a NEW_OWNER event) -- matching AHK's
			// "fire once per SetClipboard" semantics.
			//
			// GtkSharp's managed EventOwnerChange marshals owner/reason from the wrong struct offsets (owner always
			// reads 0, reason reads garbage), so we read the native GdkEventOwnerChange directly. Its 64-bit layout
			// is fixed ABI: type@0, GdkWindow* window@8, gint8 send_event@16, GdkWindow* owner@24, GdkOwnerChange
			// reason@32. owner==NULL and reason!=NEW_OWNER are perfectly correlated; we key on owner (a teardown is
			// exactly "no new owner").
			static bool IsNewOwnerChange(Gdk.EventOwnerChange ev)
			{
				// OwnerFieldOffset is the 64-bit (LP64) layout; on other word sizes fall back to firing so we
				// never silently drop a real change on an untested ABI.
				if (IntPtr.Size != 8)
					return true;
				try
				{
					var h = ev?.Handle ?? IntPtr.Zero;
					if (h == IntPtr.Zero)
						return true; // can't inspect -> don't drop a possibly-real change
					return Marshal.ReadIntPtr(h, OwnerFieldOffset) != IntPtr.Zero;
				}
				catch
				{
					return true;
				}
			}

			// Byte offset of GdkWindow* owner within GdkEventOwnerChange on 64-bit (LP64). See IsNewOwnerChange.
			const int OwnerFieldOffset = 24;
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
				SetEntry(
					ClipboardEntryKind.Text,
					null,
					value,
					(data, selection) => selection.Text = data.Data as string,
					(targetList, info) => targetList.AddTextTargets(info)
				);
			}
			get { return Control.WaitForText(); }
		}

		public Image Image
		{
			set
			{
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
			var data = GetSelectionData(type)?.Data;
			if (data != null)
			{
				return Encoding.UTF8.GetString(data);
			}
			return null;
		}

		public byte[] GetData(string type)
		{
			var selection = GetSelectionData(type);
			return selection != null && selection.Length > 0 ? selection.Data : null;
		}

		public void Clear()
		{
			Control.Clear();
			targets = new Gtk.TargetList();
			clipboard.Clear();
		}

		public bool Contains(string type)
		{
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


		public bool ContainsText => Control.WaitIsTextAvailable();

		public bool ContainsHtml => Contains("text/html");

		public bool ContainsImage => Control.WaitIsImageAvailable() || Contains("eto-icon");

		public bool ContainsUris => Contains("text/uri-list");

		public Uri[] Uris
		{
			set
			{
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
				var selection = GetSelectionData("text/uri-list");
				return selection?.GetSelectedUris()?.Select(r => new Uri(r)).ToArray();
			}
		}
	}
}
