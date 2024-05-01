using Eto.WinForms.Drawing;
namespace Eto.WinForms.Forms
{
	public class ClipboardHandler : DataObjectHandler<Clipboard, Clipboard.ICallback>, Clipboard.IHandler
	{
		public ClipboardHandler()
		{
			Control = new swf.DataObject();
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

		protected override object InnerGetData(string type) => swf.Clipboard.GetData(type);


		protected override void Update() => swf.Clipboard.SetDataObject(Control);

		public override void Clear()
		{
			swf.Clipboard.Clear();
			Control = new swf.DataObject();
		}

		public override bool Contains(string type) => swf.Clipboard.ContainsData(type);
	}
}

