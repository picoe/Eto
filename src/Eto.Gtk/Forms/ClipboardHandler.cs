using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;
using Eto.GtkSharp.Drawing;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Eto.GtkSharp.Forms
{
	public class ClipboardHandler : WidgetHandler<Gtk.Clipboard, Clipboard>, Clipboard.IHandler
	{
		delegate void GetClipboardData(ClipboardData data,Gtk.SelectionData selection);

		class ClipboardData
		{
			public string Type { get; set; }

			public object Data { get; set; }

			public GetClipboardData GetClipboardData { get; set; }

			public void GetData(Gtk.SelectionData selectionData)
			{
				GetClipboardData?.Invoke(this, selectionData);
			}
		}

		Gtk.TargetList targets = new Gtk.TargetList();

		readonly List<ClipboardData> clipboard = new List<ClipboardData>();

		public ClipboardHandler()
		{
			Control = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
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

		void AddEntry(string type, object data, GetClipboardData getData)
		{
			targets.Add(type, 0, (uint)clipboard.Count);
			clipboard.Add(new ClipboardData
			{
				Type = type,
				Data = data,
				GetClipboardData = getData
			});
			Update();
		}

		void AddTextEntry(string data, GetClipboardData getData)
		{
			targets.AddTextTargets((uint)clipboard.Count);
			clipboard.Add(new ClipboardData
			{
				Data = data,
				GetClipboardData = getData
			});
			Update();
		}

		void AddImageEntry(object data, GetClipboardData getData)
		{
			targets.AddImageTargets((uint)clipboard.Count, false);
			clipboard.Add(new ClipboardData
			{
				Data = data,
				GetClipboardData = getData
			});
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
			AddEntry(type, value, (data, selection) => selection.Set(Gdk.Atom.Intern(data.Type, false), 8, Encoding.UTF8.GetBytes(data.Data as string)));
		}

		public string Html
		{
			set { SetString(value, "text/html"); }
			get { return GetString("text/html"); }
		}

		public string Text
		{
			set { AddTextEntry(value, (data, selection) => selection.Text = data.Data as string); }
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
				AddImageEntry(pixbuf, (data, selection) => selection.SetPixbuf(data.Data as Gdk.Pixbuf));
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
			AddEntry(type, value, (data, selection) => selection.Set(Gdk.Atom.Intern(type, false), 8, value));
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
			Update();
		}

		public bool Contains(string type)
		{
			return Control.WaitIsTargetAvailable(Gdk.Atom.Intern(type, false));
		}

		public bool TrySetObject(object value, string type) => false;

		public bool TryGetObject(string type, out object value)
		{
			value = null;
			return false;
		}

		public void SetObject(object value, string type) => Widget.SetObject(value, type);

		public T GetObject<T>(string type) => Widget.GetObject<T>(type);

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
				AddEntry("text/uri-list", value, (data, selection) => selection.SetSelectedUris2(uris));
			}
			get
			{
				var selection = GetSelectionData("text/uri-list");
				return selection?.GetSelectedUris()?.Select(r => new Uri(r)).ToArray();
			}
		}
	}
}

