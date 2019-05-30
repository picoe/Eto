using System;
using System.Threading.Tasks;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class DialogHandler : GtkWindow<Gtk.Dialog, Dialog, Dialog.ICallback>, Dialog.IHandler
	{
		Gtk.Container btcontainer;
		Button defaultButton;

		public DialogHandler()
		{
			Control = new Gtk.Dialog("", null, Gtk.DialogFlags.DestroyWithParent);

			Resizable = false;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.KeyPressEvent += Connector.Control_KeyPressEvent;

#if GTK2
			Control.VBox.PackStart(WindowActionControl, false, true, 0);
			Control.VBox.PackStart(WindowContentControl, true, true, 0);

			btcontainer = Control.ActionArea;
#else
			Control.ContentArea.PackStart(WindowActionControl, false, true, 0);
			Control.ContentArea.PackStart(WindowContentControl, true, true, 0);

			Control.ActionArea.NoShowAll = true;
			Control.ActionArea.Hide();

#if GTKCORE
			if (Helper.UseHeaderBar)
			{
				btcontainer = new Gtk.HeaderBar();

				var title = Control.Title;
				Control.Titlebar = btcontainer;
				Control.Title = title;
			}
			else
#endif
				btcontainer = Control.ActionArea;
#endif
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
#if GTK3
				defaultButton?.ToNative().StyleContext.RemoveClass("suggested-action");
#endif
				defaultButton = value;

				if (value != null)
				{
#if GTK3
					value.ToNative().StyleContext.AddClass("suggested-action");
#endif
					var widget = DefaultButton.GetContainerWidget();

					if (widget != null)
					{
#if GTK2
						widget.SetFlag(Gtk.WidgetFlags.CanDefault);
#else
						widget.CanDefault = true;
#endif
						Control.Default = widget;
					}
				}
			}
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public void ShowModal()
		{
			ReloadButtons();

			Control.Modal = true;
			Control.ShowAll();

			do
			{
				Control.Run();
			} while (!WasClosed && !CloseWindow());

			WasClosed = false;
			Control.Hide();
			Control.Unrealize();

			CleanupButtons();
		}

		public void CleanupButtons()
		{
			var children = btcontainer.Children;
			foreach (var child in children)
				Control.ActionArea.Remove(child);
		}

		public void ReloadButtons()
		{
			var negativeButtons = Widget.NegativeButtons;
			var positiveButtons = Widget.PositiveButtons;

			if (negativeButtons.Count + positiveButtons.Count > 0)
			{
				if (!Helper.UseHeaderBar)
				{
					Control.ActionArea.NoShowAll = false;

					for (int i = negativeButtons.Count - 1; i >= 0; i--)
						Control.ActionArea.PackStart(negativeButtons[i].ToNative(), false, true, 1);

					foreach (var button in positiveButtons)
						Control.ActionArea.PackStart(button.ToNative(), false, true, 1);
				}
#if GTKCORE
				else
				{
					for (int i = positiveButtons.Count - 1; i >= 0; i--)
						(btcontainer as Gtk.HeaderBar).PackEnd(positiveButtons[i].ToNative());

					for (int i = negativeButtons.Count - 1; i >= 0; i--)
						(btcontainer as Gtk.HeaderBar).PackStart(negativeButtons[i].ToNative());
				}

				if (btcontainer is Gtk.HeaderBar)
					(btcontainer as Gtk.HeaderBar).ShowCloseButton = false;
#endif

				btcontainer.ShowAll();
			}
			else
			{
				Control.ActionArea.NoShowAll = true;
				if (!Helper.UseHeaderBar)
					btcontainer.Hide();
#if GTKCORE
				else
				{
					if (btcontainer is Gtk.HeaderBar)
						(btcontainer as Gtk.HeaderBar).ShowCloseButton = true;
				}
#endif
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

		static readonly object WasClosedKey = new object();

		bool WasClosed
		{
			get { return Widget.Properties.Get<bool>(WasClosedKey); }
			set { Widget.Properties.Set(WasClosedKey, value); }
		}

		public override void Close()
		{
			if (CloseWindow())
			{
				WasClosed = true;
				Control.Hide();
			}
		}

		[GLib.ConnectBefore]
		void Control_KeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Escape && AbortButton != null)
			{
				AbortButton.PerformClick();
				args.RetVal = true;
			}
		}

		public Task ShowModalAsync()
		{
			var tcs = new TaskCompletionSource<bool>();
			Application.Instance.AsyncInvoke(() =>
			{
				ShowModal();
				tcs.SetResult(true);
			});

			return tcs.Task;
		}

		protected new DialogConnector Connector => (DialogConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new DialogConnector();

		protected class DialogConnector : GtkWindowConnector
		{
			public new DialogHandler Handler => (DialogHandler)base.Handler;

			internal void Control_KeyPressEvent(object o, Gtk.KeyPressEventArgs args) => Handler.Control_KeyPressEvent(o, args);
		}
	}
}
