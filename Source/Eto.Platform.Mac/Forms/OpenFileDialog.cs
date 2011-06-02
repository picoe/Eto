using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class OpenFileDialogHandler : MacFileDialog<NSOpenPanel, OpenFileDialog>, IOpenFileDialog
	{

		public OpenFileDialogHandler()
		{
			Control = NSOpenPanel.OpenPanel;
		}

	}
}
