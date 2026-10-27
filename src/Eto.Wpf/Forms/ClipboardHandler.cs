using Eto.Wpf.Drawing;
namespace Eto.Wpf.Forms
{
	public class ClipboardHandler : DataObjectHandler<Clipboard, Clipboard.ICallback>, Clipboard.IHandler
	{
		const string CfHtmlVersion = "Version:0.9";
		const string StartHtmlPrefix = "StartHTML:";
		const string EndHtmlPrefix = "EndHTML:";
		const string StartFragmentPrefix = "StartFragment:";
		const string EndFragmentPrefix = "EndFragment:";
		ClipboardListener changeListener;

		public ClipboardHandler()
		{
			Control = new sw.DataObject();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Clipboard.ChangedEvent:
					changeListener ??= new ClipboardListener(this);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && changeListener != null)
			{
				changeListener.Dispose();
				changeListener = null;
			}
			base.Dispose(disposing);
		}

		class ClipboardListener : IDisposable
		{
			readonly ClipboardHandler handler;
			readonly swin.HwndSource source;
			bool registered;

			public ClipboardListener(ClipboardHandler handler)
			{
				this.handler = handler;

				var parameters = new swin.HwndSourceParameters("EtoClipboardListener")
				{
					Width = 0,
					Height = 0,
					WindowStyle = 0
				};
				source = new swin.HwndSource(parameters);
				source.AddHook(WndProc);
				registered = Win32.AddClipboardFormatListener(source.Handle);
			}

			IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
			{
				if ((uint)msg == (uint)Win32.WM.CLIPBOARDUPDATE)
					handler.Callback.OnChanged(handler.Widget, EventArgs.Empty);

				return IntPtr.Zero;
			}

			public void Dispose()
			{
				if (registered)
				{
					Win32.RemoveClipboardFormatListener(source.Handle);
					registered = false;
				}
				source.RemoveHook(WndProc);
				source.Dispose();
			}
		}

		public override sw.IDataObject ReadingDataObject => sw.Clipboard.GetDataObject();

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
			get
			{
				return Retry(() =>
				{
					if (!sw.Clipboard.ContainsText(sw.TextDataFormat.Html))
						return null;

					var html = sw.Clipboard.GetText(sw.TextDataFormat.Html);
					return FromClipboardHtmlFormat(html);
				});
			}
			set => base.Html = ToClipboardHtmlFormat(value);
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

		static string FromClipboardHtmlFormat(string html)
		{
			if (string.IsNullOrEmpty(html))
				return html;

			if (!html.StartsWith("Version:", StringComparison.OrdinalIgnoreCase))
				return html;

			var htmlStart = FindOffset(html, StartHtmlPrefix);
			var htmlEnd = FindOffset(html, EndHtmlPrefix);
			if (htmlStart >= 0 && htmlEnd > htmlStart && htmlEnd <= html.Length)
				return html.Substring(htmlStart, htmlEnd - htmlStart);

			var fragmentStart = FindOffset(html, StartFragmentPrefix);
			var fragmentEnd = FindOffset(html, EndFragmentPrefix);
			if (fragmentStart >= 0 && fragmentEnd > fragmentStart && fragmentEnd <= html.Length)
			{
				var fragment = html.Substring(fragmentStart, fragmentEnd - fragmentStart);
				return "<html><body>" + fragment + "</body></html>";
			}

			return html;
		}

		static string ToClipboardHtmlFormat(string html)
		{
			if (string.IsNullOrEmpty(html))
				return html;

			if (html.StartsWith("Version:", StringComparison.OrdinalIgnoreCase))
				return html;

			var fragment = html;
			if (html.IndexOf("<html", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				var startMarkerIndex = html.IndexOf("<!--StartFragment-->", StringComparison.OrdinalIgnoreCase);
				var endMarkerIndex = html.IndexOf("<!--EndFragment-->", StringComparison.OrdinalIgnoreCase);
				if (startMarkerIndex >= 0 && endMarkerIndex > startMarkerIndex)
				{
					startMarkerIndex += "<!--StartFragment-->".Length;
					fragment = html.Substring(startMarkerIndex, endMarkerIndex - startMarkerIndex);
				}
			}
			else
			{
				html = "<html><body><!--StartFragment-->" + html + "<!--EndFragment--></body></html>";
			}

			const string startMarker = "<!--StartFragment-->";
			const string endMarker = "<!--EndFragment-->";
			var startFragment = html.IndexOf(startMarker, StringComparison.OrdinalIgnoreCase);
			if (startFragment < 0)
			{
				var bodyIndex = html.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
				if (bodyIndex >= 0)
				{
					var bodyEnd = html.IndexOf('>', bodyIndex);
					if (bodyEnd >= 0)
					{
						html = html.Insert(bodyEnd + 1, startMarker).Insert(html.IndexOf("</body>", StringComparison.OrdinalIgnoreCase), endMarker);
						startFragment = html.IndexOf(startMarker, StringComparison.OrdinalIgnoreCase);
					}
				}
			}

			if (startFragment < 0)
			{
				html = "<html><body>" + startMarker + fragment + endMarker + "</body></html>";
				startFragment = html.IndexOf(startMarker, StringComparison.Ordinal);
			}

			var endFragment = html.IndexOf(endMarker, StringComparison.OrdinalIgnoreCase);
			var header =
				$"{CfHtmlVersion}\r\n" +
				$"{StartHtmlPrefix}0000000000\r\n" +
				$"{EndHtmlPrefix}0000000000\r\n" +
				$"{StartFragmentPrefix}0000000000\r\n" +
				$"{EndFragmentPrefix}0000000000\r\n";

			var startHtml = header.Length;
			var endHtml = startHtml + html.Length;
			var startFragmentOffset = startHtml + startFragment + startMarker.Length;
			var endFragmentOffset = startHtml + endFragment;

			var fixedHeader =
				$"{CfHtmlVersion}\r\n" +
				$"{StartHtmlPrefix}{startHtml:D10}\r\n" +
				$"{EndHtmlPrefix}{endHtml:D10}\r\n" +
				$"{StartFragmentPrefix}{startFragmentOffset:D10}\r\n" +
				$"{EndFragmentPrefix}{endFragmentOffset:D10}\r\n";

			return fixedHeader + html;
		}

		static int FindOffset(string html, string prefix)
		{
			var index = html.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
			if (index < 0)
				return -1;

			index += prefix.Length;
			var end = index;
			while (end < html.Length && char.IsDigit(html[end]))
				end++;

			if (end == index)
				return -1;

			return int.TryParse(html.Substring(index, end - index), out var offset) ? offset : -1;
		}
	}
}
