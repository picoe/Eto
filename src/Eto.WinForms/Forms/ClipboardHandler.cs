using Eto.WinForms.Drawing;
namespace Eto.WinForms.Forms
{
	public class ClipboardHandler : DataObjectHandler<Clipboard, Clipboard.ICallback>, Clipboard.IHandler
	{
		ClipboardListener changeListener;

		public ClipboardHandler()
		{
			Control = new swf.DataObject();
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

		class ClipboardListener : swf.NativeWindow, IDisposable
		{
			readonly ClipboardHandler handler;
			bool registered;

			public ClipboardListener(ClipboardHandler handler)
			{
				this.handler = handler;
				CreateHandle(new swf.CreateParams());
				registered = Win32.AddClipboardFormatListener(Handle);
			}

			protected override void WndProc(ref swf.Message m)
			{
				if ((uint)m.Msg == (uint)Win32.WM.CLIPBOARDUPDATE)
					handler.Callback.OnChanged(handler.Widget, EventArgs.Empty);

				base.WndProc(ref m);
			}

			public void Dispose()
			{
				if (Handle != IntPtr.Zero)
				{
					if (registered)
					{
						Win32.RemoveClipboardFormatListener(Handle);
						registered = false;
					}
					DestroyHandle();
				}
			}
		}

		protected override bool InnerContainsFileDropList => swf.Clipboard.ContainsFileDropList();

		protected override StringCollection InnerGetFileDropList() => swf.Clipboard.GetFileDropList();

		public override bool ContainsText => swf.Clipboard.ContainsText();

		public override bool ContainsHtml => swf.Clipboard.ContainsText(swf.TextDataFormat.Html);

		protected override bool InnerContainsImage => swf.Clipboard.ContainsImage();

		public override string[] Types => swf.Clipboard.GetDataObject()?.GetFormats();

		public override string Html
		{
			set => base.Html = value;
			get => swf.Clipboard.ContainsText(swf.TextDataFormat.Html) ? swf.Clipboard.GetText(swf.TextDataFormat.Html)?.TrimEnd('\0') : null;
		}

		public override string Text
		{
			set => base.Text = value;
			get => swf.Clipboard.ContainsText() ? swf.Clipboard.GetText() : null;
		}

		public DataObject DataObject
		{
			get => swf.Clipboard.GetDataObject().ToEto();
			set
			{
				Control = value.ToSwf();
				Update();
			}
		}

		protected override sd.Image InnerGetImage() => swf.Clipboard.GetImage();

		protected override object InnerGetData(string type)
		{
#pragma warning disable WFDEV005
			return swf.Clipboard.GetData(type);
#pragma warning restore WFDEV005
		}


		protected override void Update() => swf.Clipboard.SetDataObject(Control);

		public override void Clear()
		{
			swf.Clipboard.Clear();
			Control = new swf.DataObject();
		}

		public override bool Contains(string type) => swf.Clipboard.ContainsData(type);
	}
}
