﻿using System;
using System.Diagnostics;
using Eto.Forms;



namespace Eto.Mac.Forms
{
	public class OpenWithDialogHandler : WidgetHandler<NSOpenPanel, OpenWithDialog, OpenWithDialog.ICallback>, OpenWithDialog.IHandler
	{
		public string FilePath { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			Control = new NSOpenPanel();
			Control.ReleasedWhenClosed = true;
			Control.DirectoryUrl = new NSUrl("/Applications");
			Control.Prompt = "Choose Application";
			Control.AllowedFileTypes = new[] { "app" };

			if (Control.RunModal() == 1)
				Process.Start("open", "-a \"" + Control.Url.Path +  "\" \"" + FilePath + "\"");

			return DialogResult.Ok;
		}
	}
}
