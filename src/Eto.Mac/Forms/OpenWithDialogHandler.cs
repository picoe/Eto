namespace Eto.Mac.Forms
{
	public class OpenWithDialogHandler : WidgetHandler<NSOpenPanel, OpenWithDialog, OpenWithDialog.ICallback>, OpenWithDialog.IHandler
	{
		public string FilePath { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			Control = NSOpenPanel.OpenPanel;
			Control.DirectoryUrl = NSUrl.FromFilename("/Applications");
			Control.Prompt = Application.Instance.Localize(Widget, "Choose Application");
#if MACOS_NET
			Control.AllowedContentTypes = new [] { UniformTypeIdentifiers.UTTypes.Application };
#else
			Control.AllowedFileTypes = new[] { "app" };
#endif

			if (Control.RunModal() == 1)
				Process.Start("open", "-a \"" + Control.Url.Path +  "\" \"" + FilePath + "\"");

			return DialogResult.Ok;
		}
	}
}
