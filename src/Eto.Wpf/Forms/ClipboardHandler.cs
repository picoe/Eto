using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Drawing;
using System;
using sc = System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Threading;
using sw = System.Windows;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;
using System.Collections.Specialized;

namespace Eto.Wpf.Forms
{
	public class ClipboardHandler : DataObjectHandler<Clipboard, Clipboard.ICallback>, Clipboard.IHandler
	{
		public ClipboardHandler()
		{
			Control = new sw.DataObject();
		}

		public override string[] Types => sw.Clipboard.GetDataObject()?.GetFormats();

		protected override void Update()
		{
			// internally WPF retries here so no need to retry
			sw.Clipboard.SetDataObject(Control);
		}

		T Retry<T>(Func<T> getValue)
		{
			for (int i = 0; i < 10; i++)
			{
				try
				{
					return getValue();
				}
				catch (COMException ex)
				{
					// cannot open clipboard, so retry 10 times after 100ms
					// WPF sometimes throws this when trying to get a value
					// as it appears to retry when getting the data object, but not when 
					if (ex.HResult != unchecked((int)0x800401D0) || i == 9)
						throw;
				}
				Thread.Sleep(100);
			}
			throw new InvalidOperationException(); // should not get here
		}


		public override bool Contains(string type) => Retry(() => sw.Clipboard.ContainsData(type));

		public override bool ContainsText => Retry(() => sw.Clipboard.ContainsText());

		public override string Text
		{
			get { return Retry(() => sw.Clipboard.ContainsText() ? sw.Clipboard.GetText() : null); }
			set => base.Text = value;
		}

		public override bool ContainsHtml => Retry(() => sw.Clipboard.ContainsText(sw.TextDataFormat.Html));

		public override string Html
		{
			get { return Retry(() => sw.Clipboard.ContainsText(sw.TextDataFormat.Html) ? sw.Clipboard.GetText(sw.TextDataFormat.Html) : null); }
			set => base.Html = value;
		}

		public DataObject DataObject
		{
			get { return sw.Clipboard.GetDataObject().ToEto(); }
			set { sw.Clipboard.SetDataObject(value.ToWpf()); }
		}

		protected override bool InnerContainsImage => Retry(() => sw.Clipboard.ContainsImage());

		protected override bool InnerContainsFileDropList => Retry(() => sw.Clipboard.ContainsFileDropList());

		public override void Clear()
		{
			sw.Clipboard.Clear();
			Control = new sw.DataObject();
		}

		protected override swmi.BitmapSource InnerGetImage() => Retry(() => sw.Clipboard.GetImage());

		protected override StringCollection InnerGetFileDropList() => Retry(() => sw.Clipboard.GetFileDropList());

		protected override object InnerGetData(string type) => Retry(() => sw.Clipboard.GetData(type));
	}
}
