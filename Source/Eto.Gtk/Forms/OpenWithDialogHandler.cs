﻿using System;
using System.Diagnostics;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class OpenWithDialogHandler : WidgetHandler<Gtk.Dialog, OpenWithDialog, OpenWithDialog.ICallback>, OpenWithDialog.IHandler
	{
		public string FilePath { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			var handle = parent == null ? IntPtr.Zero : (parent.ControlObject as Gtk.Window).Handle;
			var adialoghandle = NativeMethods.gtk_app_chooser_dialog_new(handle, 5, NativeMethods.g_file_new_for_path(FilePath));
			var adialog = new Gtk.AppChooserDialog(adialoghandle);

			if (adialog.Run() == (int)Gtk.ResponseType.Ok)
				Process.Start(adialog.AppInfo.Executable, "\"" + FilePath + "\"");
			adialog.Destroy();

			return DialogResult.Ok;
		}
	}
}
