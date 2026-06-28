using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eto.GirCore.Forms;

public class DialogHandler : GirWindow<Gtk.Dialog, Dialog, Dialog.ICallback>, Dialog.IHandler
{
	Gtk.HeaderBar headerBar;
	Button defaultButton;
	

	public DialogHandler()
	{
		Control = new Gtk.Dialog();

		Resizable = false;
	}

	// protected override Gdk.WindowTypeHint DefaultTypeHint => Gdk.WindowTypeHint.Dialog;

	protected override void Initialize()
	{
		base.Initialize();
		Control.Modal = true;
		// Control.KeyPressEvent += Connector.Control_KeyPressEvent;

		var vbox = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
		vbox.Append(WindowActionControl);
		vbox.Append(WindowContentControl);

		var content = Gtk.Box.New(Gtk.Orientation.Vertical, 0);

		headerBar = Gtk.HeaderBar.New();

		var title = Control.Title;
		Control.Titlebar = headerBar;
		Control.Title = title;

		content.Append(vbox);
		Control.Child = content;
	}

	public Button AbortButton { get; set; }

	public Button DefaultButton
	{
		get
		{
			return defaultButton;
		}
		set
		{
			defaultButton?.ToNative()?.GetStyleContext().RemoveClass("suggested-action");
			defaultButton = value;

			if (value != null)
			{
				value.ToNative()?.GetStyleContext().AddClass("suggested-action");
				var widget = DefaultButton.GetContainerWidget();

				if (widget != null)
				{
					widget.SetReceivesDefault(true);
					Control.DefaultWidget = widget;
				}
			}
		}
	}

	public DialogDisplayMode DisplayMode { get; set; }

	public void ShowModal()
	{
		DisableAutoSizeUpdate++;
		ReloadButtons();

		// Control.Child?.ShowAll();
		// if (!Control.IsRealized)
		// {
		// 	Control.Realize();
		// }
		Control.QueueResize();
		Callback.OnLoadComplete(Widget, EventArgs.Empty);

		// Control.ShowAll();
		DisableAutoSizeUpdate--;

		if (!WasClosed)
		{
			do
			{
				Control.Show();
			} while (!WasClosed && !CloseWindow());
		}

		WasClosed = false;
		Control.Hide();
		Control.Unrealize();

		CleanupButtons();
	}

	public void CleanupButtons()
	{
		Gtk.Widget? child;
		while ((child = headerBar.GetFirstChild()) != null)
		{
			headerBar.Remove(child);
		}
	}

	public void ReloadButtons()
	{
		var negativeButtons = Widget.NegativeButtons;
		var positiveButtons = Widget.PositiveButtons;

		if (negativeButtons.Count + positiveButtons.Count > 0)
		{
			for (int i = positiveButtons.Count - 1; i >= 0; i--)
				headerBar.PackEnd(positiveButtons[i].ToNative());

			for (int i = negativeButtons.Count - 1; i >= 0; i--)
				headerBar.PackStart(negativeButtons[i].ToNative());
				
			// headerBar.ShowCloseButton = false;
		}
		else
		{
			// headerBar.ShowCloseButton = false;
		}
	}

	public void InsertDialogButton(bool positive, int index, Button item)
	{
		if (Widget.Visible)
		{
			CleanupButtons();
			ReloadButtons();
		}
	}

	public void RemoveDialogButton(bool positive, int index, Button item)
	{
		if (Widget.Visible)
		{
			CleanupButtons();
			ReloadButtons();
		}
	}

	public override void Close()
	{
		if (Widget.Loaded && CloseWindow())
		{
			Control.Hide();
			Control.Unrealize();
			WasClosed = true;
		}
	}

	// [GLib.ConnectBefore]
	// void Control_KeyPressEvent(object o, Gtk.KeyPressEventArgs args)
	// {
	// 	if (args.Event.Key == Gdk.Key.Escape && AbortButton != null)
	// 	{
	// 		AbortButton.PerformClick();
	// 		args.RetVal = true;
	// 	}
	// }

	public Task ShowModalAsync()
	{
		var tcs = new TaskCompletionSource<bool>();
		Application.Instance.AsyncInvoke(() =>
		{
			if (Widget.IsDisposed)
			{
				tcs.SetResult(false);
				return;
			}
			ShowModal();
			tcs.SetResult(true);
		});

		return tcs.Task;
	}

	protected new DialogConnector Connector => (DialogConnector)base.Connector;

	protected override WeakConnector CreateConnector() => new DialogConnector();

	protected class DialogConnector : GirWindowConnector
	{
		public new DialogHandler Handler => (DialogHandler)base.Handler;

		// internal void Control_KeyPressEvent(object o, Gtk.KeyPressEventArgs args) => Handler?.Control_KeyPressEvent(o, args);
	}
}
