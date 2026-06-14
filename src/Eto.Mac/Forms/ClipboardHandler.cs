using Eto.Mac.Drawing;
namespace Eto.Mac.Forms
{
	public class ClipboardHandler : DataObjectHandler<Clipboard, Clipboard.ICallback>, Clipboard.IHandler
	{
		UITimer changeTimer;
		nint changeCount = -1;

		protected override NSPasteboard CreateControl() => NSPasteboard.GeneralPasteboard;

		protected override bool DisposeControl => false;

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Clipboard.ChangedEvent:
					changeCount = Control.ChangeCount;
					changeTimer ??= new UITimer { Interval = 0.2 };
					changeTimer.Elapsed -= ChangeTimer_Elapsed;
					changeTimer.Elapsed += ChangeTimer_Elapsed;
					changeTimer.Start();
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void ChangeTimer_Elapsed(object sender, EventArgs e)
		{
			var current = Control.ChangeCount;
			if (current == changeCount)
				return;

			changeCount = current;
			Callback.OnChanged(Widget, EventArgs.Empty);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && changeTimer != null)
			{
				changeTimer.Stop();
				changeTimer.Elapsed -= ChangeTimer_Elapsed;
				changeTimer.Dispose();
				changeTimer = null;
			}
			base.Dispose(disposing);
		}

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
