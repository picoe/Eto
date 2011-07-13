using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms
{
	public class OpenFileDialogHandler : MacFileDialog<NSOpenPanel, OpenFileDialog>, IOpenFileDialog
	{

		public OpenFileDialogHandler()
		{
			Control = NSOpenPanel.OpenPanel;
		}

	}
}
