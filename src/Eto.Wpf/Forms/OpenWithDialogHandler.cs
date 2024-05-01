using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms
{
    public class OpenWithDialogHandler : WidgetHandler<System.Windows.Window, OpenWithDialog, OpenWithDialog.ICallback>, OpenWithDialog.IHandler
    {
		public string FilePath { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
			Process.Start("rundll32.exe", args + ",OpenAs_RunDLL " + FilePath);

			return DialogResult.Ok;
		}
    }
}
