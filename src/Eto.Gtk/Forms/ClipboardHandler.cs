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
		bool changeQueued;

		public ClipboardHandler()
		{
			Control = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
		}

		// Debounce owner-change into a single Changed per event-loop turn, via a GLib idle callback that runs
		// once the current batch of events has drained. Reads no clipboard content. Invoked only on the Wayland
		// backend (see HandleOwnerChange) — X11 fires directly.
		//
		// This works around GNOME/GTK bug #775631 (https://gitlab.gnome.org/GNOME/gtk/-/issues/715): on Wayland
		// GTK emits SEVERAL NEW_OWNER owner-change signals for a single clipboard change, which would otherwise
		// raise Changed multiple times per copy. Remove this debounce once #775631 is fixed upstream. Compositors
		// that expose wl_data_control never reach this path (the native WaylandClipboardHandler is single-fire by
		// protocol).
		//
		// Known limitation, inherent to debouncing: the burst events are metadata-identical to genuine changes
		// (owner/time/selection_time are all 0 on Wayland), so nothing but the clipboard content distinguishes a
		// duplicate from a real change. Distinct writes issued back-to-back within one loop turn therefore
		// collapse into one Changed. In practice that is only synchronous UI-thread writes with no yield between
		// them -- and on Wayland only the final value of such a run ever becomes the live selection anyway; writes
		// spaced by any yield, or made from another thread, each fire.
		void QueueChanged()
		{
			if (changeQueued)
				return;
			changeQueued = true;
			GLib.Idle.Add(() =>
			{
				changeQueued = false;
				if (changedAttached)
					Callback.OnChanged(Widget, EventArgs.Empty);
				return false;
			});
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
				// Only debounce on Wayland, which is where the #775631 burst happens. X11 emits exactly one
				// NEW_OWNER per change, so fire directly there: the debounce would coalesce nothing real on X11,
				// while still merging synchronous back-to-back distinct writes — a regression on a backend that
				// has no burst to work around.
				if (Helper.IsWaylandBackend)
					handler.QueueChanged();
				else
					handler.Callback.OnChanged(handler.Widget, EventArgs.Empty);
			}

			// Whether this owner-change hands the selection to a NEW owner (GDK_OWNER_CHANGE_NEW_OWNER), as
			// opposed to merely tearing an owner down (DESTROY/CLOSE). A clipboard content change is reported as a
			// NEW_OWNER event; DESTROY/CLOSE just signal an owner going away, not new content, so we do not raise
			// Changed for them. This reads no clipboard content, so a genuine re-copy of identical content (still a
			// NEW_OWNER event) still fires.
			//
			// We key on the REASON, not the owner window: on Wayland there is no X11-style selection-owner window,
			// so GTK reports owner==NULL for EVERY owner-change (NEW_OWNER included); gating on owner!=NULL would
			// suppress all clipboard changes on Wayland. The reason field is NEW_OWNER on both backends.
			//
			// (On Wayland GTK emits several NEW_OWNER events per change — GNOME bug #775631 — which QueueChanged
			// debounces into one Changed; see its comment for the rationale and the known limitation.)
			//
			// GtkSharp's managed EventOwnerChange marshals owner/reason from the wrong struct offsets, so we read
			// the native GdkEventOwnerChange directly. Its 64-bit (LP64) layout is fixed ABI: type@0,
			// GdkWindow* window@8, gint8 send_event@16, GdkWindow* owner@24, GdkOwnerChange reason@32.
			static bool IsNewOwnerChange(Gdk.EventOwnerChange ev)
			{
				// On non-LP64 / unknown ABIs, fall back to firing so we never silently drop a real change.
				if (IntPtr.Size != 8)
					return true;
				try
				{
					var h = ev?.Handle ?? IntPtr.Zero;
					if (h == IntPtr.Zero)
						return true; // can't inspect -> don't drop a possibly-real change
					return Marshal.ReadInt32(h, ReasonFieldOffset) == GdkOwnerChangeNewOwner;
				}
				catch
				{
					return true;
				}
			}

			// GdkEventOwnerChange.reason byte offset (LP64) and the GDK_OWNER_CHANGE_NEW_OWNER enum value.
			const int ReasonFieldOffset = 32;
			const int GdkOwnerChangeNewOwner = 0;
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
				IntPtr atomPtrs;
				int count;
				var success = NativeMethods.gtk_clipboard_wait_for_targets(Control.Handle, out atomPtrs, out count);

				if (!success || count <= 0 || atomPtrs == IntPtr.Zero)
				{
					return new string[0];
				}

				try
				{
					var atoms = new Gdk.Atom[count];
					for (int i = 0; i < count; i++)
						atoms[i] = new Gdk.Atom(Marshal.ReadIntPtr(atomPtrs, i * IntPtr.Size));
					return atoms.Select(r => r.Name).ToArray();
				}
				finally
				{
					GLib.Marshaller.Free(atomPtrs);
				}
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
