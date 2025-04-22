namespace Eto.Mac.Forms
{
	public class OpenFileDialogHandler : MacFileDialog<NSOpenPanel, OpenFileDialog>, OpenFileDialog.IHandler
	{

		protected override NSOpenPanel CreateControl() => NSOpenPanel.OpenPanel;

		public bool MultiSelect
		{
			get => Control.AllowsMultipleSelection;
			set => Control.AllowsMultipleSelection = value;
		}

		public IEnumerable<string> Filenames => Control.Urls.Select(a => a.Path);

		static readonly Selector selSetAccessoryViewDisclosed = new Selector("setAccessoryViewDisclosed:");

		internal override void Create()
		{
			base.Create();

			if (Control.AccessoryView != null && Control.RespondsToSelector(selSetAccessoryViewDisclosed))
			{
				// ensure accessory view is always disclosed
				// only available on NSOpenPanel.
				Messaging.void_objc_msgSend_bool(Control.Handle, selSetAccessoryViewDisclosed.Handle, true);
			}
		}
	}
}
