namespace Eto.Mac.Forms
{
	public class OpenFileDialogHandler : MacFileDialog<NSOpenPanel, OpenFileDialog>, OpenFileDialog.IHandler
	{

		protected override NSOpenPanel CreateControl()
		{
			return NSOpenPanel.OpenPanel;
		}

		protected override bool DisposeControl { get { return false; } }

		public bool MultiSelect
		{
			get { return Control.AllowsMultipleSelection; }
			set { Control.AllowsMultipleSelection = value; }
		}

		public IEnumerable<string> Filenames
		{
			get { return Control.Urls.Select(a => a.Path); }
		}
	}
}
