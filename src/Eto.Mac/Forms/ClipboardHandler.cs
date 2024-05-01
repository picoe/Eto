using Eto.Mac.Drawing;
namespace Eto.Mac.Forms
{
	public class ClipboardHandler : DataObjectHandler<Clipboard, Clipboard.ICallback>, Clipboard.IHandler
	{
		protected override NSPasteboard CreateControl() => NSPasteboard.GeneralPasteboard;

		protected override bool DisposeControl => false;

		/*
		public DataObject DataObject
		{
			get
			{
				return new DataObject(new DataObjectHandler(Control.MutableCopy() as NSPasteboard));
			}
			set
			{
				Control.ClearContents();
				var handler = value?.Handler as IDataObjectHandler;
				handler?.Apply(Control);
			}
		}
		*/

	}
}

