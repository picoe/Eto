using System;
using System.Linq;
using Eto.Forms;
using MonoMac.AppKit;
using System.Collections.Generic;

namespace Eto.Platform.Mac.Forms
{
	public class OpenFileDialogHandler : MacFileDialog<NSOpenPanel, OpenFileDialog>, IOpenFileDialog
	{

		public OpenFileDialogHandler()
		{
			Control = NSOpenPanel.OpenPanel;
		}

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
