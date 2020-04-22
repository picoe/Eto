using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;
using System.Text;
using System.IO;
using System.Threading;

namespace Eto.GtkSharp.Forms
{
	public class DataFormatsHandler : DataFormats.IHandler
	{
		public string Text => "UTF8_STRING";

		public string Html => "text/html";

		public string Color => "color";
	}

	public class DataObjectData
	{
		Gdk.Atom _atom;

		public Gtk.TargetEntry Target { get; set; }

		public object Data { get; set; }

		public Gdk.Atom Atom => _atom ?? (_atom = Gdk.Atom.Intern(Target.Target, false));

		public Action<DataObjectData, Gtk.SelectionData> GetDataFunc { get; set; }

		public void GetData(Gtk.SelectionData selectionData) => GetDataFunc?.Invoke(this, selectionData);
	}

	public class DataObjectHandler : WidgetHandler<Dictionary<string, DataObjectData>, DataObject, DataObject.ICallback>, DataObject.IHandler
	{

		Gdk.DragContext _dragContext;
		Gtk.Widget _sourceWidget;
		uint _dragTime;
		Action<Gtk.SelectionData> _getData;

		public Gdk.DragContext DragContext => _dragContext;

		public DataObjectHandler()
		{
			Control = new Dictionary<string, DataObjectData>();
		}

		public DataObjectHandler(Gtk.Widget widget, Gdk.DragContext context, uint time)
			: this()
		{
			_sourceWidget = widget;
			_dragContext = context;
			_dragTime = time;
		}

		internal void SetDataReceived(Gtk.DragDataReceivedArgs args)
		{
			// return the data!
			_getData?.Invoke(args.SelectionData);
		}

		public Gtk.TargetList GetTargets()
		{
			var targets = new Gtk.TargetList();
			targets.AddTable(Control.Values.Select(r => r.Target).ToArray());
			return targets;
		}

		public void Apply(Gtk.SelectionData data)
		{
			foreach (var item in Control.Values)
			{
				if (item.Target.Target == data.Target.Name)
				{
					item.GetData(data);
					return;
				}
			}
		}

		void AddEntry(string type, object data, Action<DataObjectData, Gtk.SelectionData> getData)
		{
			Control[type] = new DataObjectData
			{
				Target = new Gtk.TargetEntry(type, 0, (uint)Control.Count),
				Data = data,
				GetDataFunc = getData
			};
		}

		T GetSelectionData<T>(string type, Func<Gtk.SelectionData, T> getData)
		{
			if (_dragContext != null)
			{
				var target = _dragContext.ListTargets().FirstOrDefault(r => r.Name == type);
				if (target != null)
				{
					object data = null;
					var sem = new ManualResetEventSlim(false);
					_getData = selection =>
					{
						data = (object)getData(selection);
						sem.Set();
					};
					Gtk.Drag.GetData(_sourceWidget, _dragContext, target, _dragTime);
					if (!sem.IsSet)
					{
						// sometimes data gets passed back at the next run loop, so we pump the loop here
						// is there a better way to wait for the data to arrive?
						for (int i = 0; i < 2; i++)
						{
							Gtk.Application.RunIteration();
							if (sem.Wait(100))
								break;
						}
					}


					_getData = null;
					return (T)data;
				}
			}
			return default(T);
		}

		public void SetString(string value, string type)
		{
			AddEntry(type, value, (data, selection) =>
			{
				// using selection.Text doesn't always seem to work
				selection.Set(data.Atom, 8, Encoding.UTF8.GetBytes(data.Data as string));
			});
		}

		T GetControlData<T>(string type, Func<DataObjectData, T> getValue)
		{
			if (Control.TryGetValue(type, out var value))
				return getValue(value);
			return default(T);
		}

		public string GetString(string type)
		{
			return
				GetControlData(type, d => d.Data as string)
				?? GetSelectionData(type, selection =>
				{
					// using selection.Text doesn't always seem to work
					var data = selection.Data;
					if (data != null)
						return Encoding.UTF8.GetString(data);
					else
						return null;
				});
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
				SetString(value, "UTF8_STRING");
				SetString(value, "TEXT");
			}
			get { return GetString("UTF8_STRING") ?? GetString("TEXT"); }
		}

		void AddImageEntry(string type, Bitmap bmp, ImageFormat format)
		{
			if (bmp == null)
				return;
			AddEntry(type, bmp, (data, selection) =>
			{
				using (var ms = new MemoryStream())
				{
					((Bitmap)data.Data).Save(ms, format);
					ms.Position = 0;
					selection.Set(data.Atom, 8, ms.ToArray());
				}
			});

		}

		public Image Image
		{
			set
			{
				AddEntry("image/pixbuf", value, (data, selection) => selection.SetPixbuf(((Image)data.Data).ToGdk()));
				var bmp = value as Bitmap ?? (value as Icon)?.GetFrame(1)?.Bitmap;
				if (bmp != null)
				{
					AddImageEntry("image/png", bmp, ImageFormat.Png);
					AddImageEntry("image/bmp", bmp, ImageFormat.Bitmap);
					AddImageEntry("image/tiff", bmp, ImageFormat.Tiff);
					AddImageEntry("image/jpeg", bmp, ImageFormat.Jpeg);
				}
			}
			get
			{
				return GetControlData("image/png", d => d.Data as Bitmap)
					?? GetSelectionData("image/pixbuf", selection => selection.Pixbuf.ToEto())
					?? GetSelectionData("image/png", selection => new Bitmap(selection.Data))
					?? GetSelectionData("image/tiff", selection => new Bitmap(selection.Data))
					?? GetSelectionData("image/bmp", selection => new Bitmap(selection.Data))
					?? GetSelectionData("image/jpeg", selection => new Bitmap(selection.Data));
			}
		}

		public void SetData(byte[] value, string type)
		{
			AddEntry(type, value, (data, selection) => selection.Set(Gdk.Atom.Intern(type, false), 8, value));
		}

		public byte[] GetData(string type)
		{
			return GetControlData(type, d => d.Data as byte[]) 
				?? GetSelectionData(type, selection => selection.Data);
		}

		public void Clear()
		{
			Control.Clear();
		}

		public bool Contains(string type)
		{
			return _dragContext?.ListTargets().Any(r => r.Name == type)
				?? Control.ContainsKey(type);
		}

		public bool Contains(params string[] types)
		{
			return _dragContext?.ListTargets().Any(r => types.Contains(r.Name))
				?? Control.Keys.Any(r => types.Contains(r));
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
				if (_dragContext != null)
					return _dragContext.ListTargets().Select(r => r.Name).ToArray();

				return Control.Values.Select(r => r.Target.Target).ToArray();
			}
		}

		public Uri[] Uris
		{
			set
			{
				var uris = value?.Select(r => r.AbsoluteUri).ToArray();
				AddEntry("text/uri-list", value, (data, selection) => selection.SetSelectedUris2(uris));
			}
			get
			{
				var urls = GetControlData("text/uri-list", d => d.Data as Uri[])
					?? GetSelectionData("text/uri-list", selection => selection.GetSelectedUris())?.Select(r => new Uri(r)).ToArray();
				return urls;
			}
		}

		public bool ContainsText => Contains("UTF8_STRING", "STRING");

		public bool ContainsHtml => Contains("text/html");

		public bool ContainsImage => Contains("image/pixbuf", "image/png", "image/tiff", "image/bmp", "image/jpeg");

		public bool ContainsUris => Contains("text/uri-list");
	}
}
