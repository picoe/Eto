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
		delegate void GetClipboardData(ClipboardData data, Gtk.SelectionData selection);
		
		class ClipboardData
		{
			public Gtk.TargetEntry Target { get; set; }
			public object Data { get; set; }
			public GetClipboardData GetClipboardData { get; set; }
			
			public void GetData(Gtk.SelectionData selection_data)
			{
				if (GetClipboardData != null) GetClipboardData(this, selection_data);
			}
		}
		List<ClipboardData> clipboard = new List<ClipboardData>();
		
		public ClipboardHandler ()
		{
			Control = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false));
		}
		
		void Update()
		{
			var targets = clipboard.Select (r => r.Target);
			Control.SetWithData(targets.ToArray (), delegate(Gtk.Clipboard clip, Gtk.SelectionData selection_data, uint info) {
				if (info < clipboard.Count) {
					var clipdata = clipboard[(int)info];
					clipdata.GetData(selection_data);
				}
			}, delegate(Gtk.Clipboard clip) {
				
			});
		}
		
		void AddEntry(string type, object data, GetClipboardData getData)
		{
			clipboard.Add (new ClipboardData{
				Target = new Gtk.TargetEntry(type, 0, (uint)clipboard.Count),
				Data = data,
				GetClipboardData = getData
			});
			Update ();
		}
		
		Gtk.SelectionData GetSelectionData(string type)
		{
			var target = Gdk.Atom.Intern (type, false);
			if (Control.WaitIsTargetAvailable (target))
			{
				var data = Control.WaitForContents(target);
				if (data != null) return data;
			}
			return null;
		}

		#region IClipboard implementation
		public void SetString (string value, string type)
		{
			AddEntry(type, value, delegate(ClipboardData data, Gtk.SelectionData selection) {
				selection.Text = data.Data as string;
			});
		}

		public string Html {
			set { 
				AddEntry("text/html", value, delegate(ClipboardData data, Gtk.SelectionData selection) {
					selection.Text = data.Data as string;
				});
			}
			get {
				var selection = GetSelectionData ("text/html");
				if (selection != null) return selection.Text;
				else return null;
			}
		}

		public string Text {
			set { 
				AddEntry ("UTF8_STRING", value, delegate(ClipboardData data, Gtk.SelectionData selection) {
					selection.Text = data.Data as string;
				});
			}
			get { return Control.WaitForText (); }
			
		}

		public Image Image {
			set {
				var pixbuf = value.ControlObject as Gdk.Pixbuf;
				/* TODO: AddEntry(type, value, delegate(ClipboardData data, Gtk.SelectionData selection) {
					selection = value;	
				});*/
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
			AddEntry(type, value, delegate(ClipboardData data, Gtk.SelectionData selection) {
				selection.Set (Gdk.Atom.Intern (type, false), 0, value);
			});
		}

		public string GetString (string type)
		{
			var selection = GetSelectionData (type);
			if (selection != null) return selection.Text;
			else return null;
		}

		public byte[] GetData (string type)
		{
			var selection = GetSelectionData (type);
			if (selection != null && selection.Length > 0) return selection.Data;
			else return null;
		}

		public void Clear ()
		{
			Control.Clear ();
			clipboard.Clear ();
		}

		public string[] Types {
			get {
				//return Control.Data.Keys.OfType<string> ().ToArray ();
				return null;
			}
		}
		#endregion

		#region IWidget implementation

		#endregion
	}
}

