using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;
using Eto.Platform.GtkSharp.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Forms
{
	public class ClipboardHandler : WidgetHandler<Gtk.Clipboard, Clipboard>, IClipboard
	{
		delegate void GetClipboardData(ClipboardData data,Gtk.SelectionData selection);

		class ClipboardData
		{
			public Gtk.TargetEntry Target { get; set; }

			public object Data { get; set; }

			public GetClipboardData GetClipboardData { get; set; }

			public void GetData(Gtk.SelectionData selectionData)
			{
				if (GetClipboardData != null)
					GetClipboardData(this, selectionData);
			}
		}

		readonly List<ClipboardData> clipboard = new List<ClipboardData>();

		public ClipboardHandler()
		{
			Control = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
		}

		void Update()
		{
			var targets = clipboard.Select(r => r.Target);
			Control.SetWithData(targets.ToArray(), (clip, selectionData, info) =>
			{
				if (info < clipboard.Count)
				{
					var clipdata = clipboard[(int)info];
					clipdata.GetData(selectionData);
				}
			}, clip =>
			{
				
			});
		}

		void AddEntry(string type, object data, GetClipboardData getData)
		{
			clipboard.Add(new ClipboardData
			{
				Target = new Gtk.TargetEntry(type, 0, (uint)clipboard.Count),
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
				if (data != null)
					return data;
			}
			return null;
		}

		#region IClipboard implementation

		public void SetString(string value, string type)
		{
			AddEntry(type, value, delegate(ClipboardData data, Gtk.SelectionData selection)
			{
				selection.Text = data.Data as string;
			});
		}

		public string Html
		{
			set
			{ 
				AddEntry("text/html", value, delegate(ClipboardData data, Gtk.SelectionData selection)
				{
					selection.Text = data.Data as string;
				});
			}
			get
			{
				var selection = GetSelectionData("text/html");
				return selection != null ? selection.Text : null;
			}
		}

		public string Text
		{
			set
			{ 
				AddEntry("UTF8_STRING", value, delegate(ClipboardData data, Gtk.SelectionData selection)
				{
					selection.Text = data.Data as string;
				});
			}
			get { return Control.WaitForText(); }
			
		}

		public Image Image
		{
			set
			{
				var pixbuf = value.ControlObject as Gdk.Pixbuf;
				/* TODO: AddEntry(type, value, delegate(ClipboardData data, Gtk.SelectionData selection) {
					selection = value;	
				});*/
				if (pixbuf != null)
					Control.Image = pixbuf;
				else
					throw new NotSupportedException();
			}
			get
			{
				var image = Control.WaitForImage();
				if (image != null)
				{
					var handler = new BitmapHandler(image);
					return new Bitmap(Widget.Generator, handler);
				}
				return null;
			}
		}

		public void SetData(byte[] value, string type)
		{
			AddEntry(type, value, delegate(ClipboardData data, Gtk.SelectionData selection)
			{
				selection.Set(Gdk.Atom.Intern(type, false), 0, value);
			});
		}

		public string GetString(string type)
		{
			var selection = GetSelectionData(type);
			return selection != null ? selection.Text : null;
		}

		public byte[] GetData(string type)
		{
			var selection = GetSelectionData(type);
			return selection != null && selection.Length > 0 ? selection.Data : null;
		}

		public void Clear()
		{
			Control.Clear();
			clipboard.Clear();
		}

		public string[] Types
		{
			get
			{
				//return Control.Data.Keys.OfType<string> ().ToArray ();
				return null;
			}
		}

		#endregion

		#region IWidget implementation

		#endregion

	}
}

