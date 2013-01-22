using System;
using Eto;
using Eto.Drawing;
using System.IO;

namespace Eto.Forms
{
	public interface IClipboard : IInstanceWidget
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
	
	public class Clipboard : InstanceWidget
	{
		IClipboard handler;
		
		public Clipboard ()
			: this(Generator.Current)
		{
		}
		
		public Clipboard (Generator generator)
			: base(generator, typeof(IClipboard))
		{
			handler = (IClipboard)Handler;
		}

        public string[] Types {
			get { return handler.Types; }
		}
		
		public void SetDataStream (Stream stream, string type)
		{
			byte[] buffer = new byte[stream.Length];
			if (stream.CanSeek && stream.Position != 0)
				stream.Seek (0, SeekOrigin.Begin);
			stream.Read (buffer, 0, buffer.Length);
			SetData (buffer, type);
		}
		
		public void SetData (byte[] value, string type)
		{
			handler.SetData (value, type);
		}
		
		public byte[] GetData (string type)
		{
			return handler.GetData (type);
		}
		
		public Stream GetDataStream (string type)
		{
			var buffer = GetData (type);
			if (buffer != null)
				return new MemoryStream (buffer, false);
			else
				return null;
		}
		
		public void SetString (string value, string type)
		{
			handler.SetString (value, type);
		}
		
		public string GetString (string type)
		{
			return handler.GetString (type);
		}
		
		public string Text {
			get { return handler.Text; }
			set { handler.Text = value; }
		}
		
		public string Html {
			get { return handler.Html; }
			set { handler.Html = value; }
		}
		
		public Image Image {
			get { return handler.Image; }
			set { handler.Image = value; }
		}
		
		public void Clear ()
		{
			handler.Clear ();
		}
		
	}
}

