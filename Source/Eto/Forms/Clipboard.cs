using System;
using Eto;
using Eto.Drawing;
using System.IO;

namespace Eto.Forms
{
	[Handler(typeof(Clipboard.IHandler))]
	public class Clipboard : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		public Clipboard()
		{
		}

		[Obsolete("Use default constructor instead")]
		public Clipboard (Generator generator)
			: base(generator, typeof(Clipboard.IHandler))
		{
		}
		
		public string[] Types {
			get { return Handler.Types; }
		}
		
		public void SetDataStream (Stream stream, string type)
		{
			var buffer = new byte[stream.Length];
			if (stream.CanSeek && stream.Position != 0)
				stream.Seek (0, SeekOrigin.Begin);
			stream.Read (buffer, 0, buffer.Length);
			SetData (buffer, type);
		}
		
		public void SetData (byte[] value, string type)
		{
			Handler.SetData (value, type);
		}
		
		public byte[] GetData (string type)
		{
			return Handler.GetData (type);
		}
		
		public Stream GetDataStream (string type)
		{
			var buffer = GetData (type);
			return buffer == null ? null : new MemoryStream(buffer, false);
		}
		
		public void SetString (string value, string type)
		{
			Handler.SetString (value, type);
		}
		
		public string GetString (string type)
		{
			return Handler.GetString (type);
		}
		
		public string Text {
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}
		
		public string Html {
			get { return Handler.Html; }
			set { Handler.Html = value; }
		}
		
		public Image Image {
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}
		
		public void Clear ()
		{
			Handler.Clear ();
		}

		public new interface IHandler : Widget.IHandler
		{
			string[] Types { get; }

			void SetString (string value, string type);

			void SetData (byte[] value, string type);

			string GetString (string type);

			byte[] GetData (string type);

			string Text { get; set; }

			string Html { get; set; }

			Image Image { get; set; }

			void Clear ();
		}
	}
}

