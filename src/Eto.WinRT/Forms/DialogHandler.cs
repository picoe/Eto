#if TODO_XAML
using System;
using Eto.Forms;
using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;
using Eto.WinRT.Forms.Controls;

namespace Eto.WinRT.Forms
{
	public class DialogHandler : WpfWindow<sw.Window, Dialog>, IDialog
	{
		Button defaultButton;
		Button abortButton;

		public override sw.Window CreateControl ()
		{
			return new sw.Window ();
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Control.ShowInTaskbar = false;
			Resizable = false;
			Minimizable = false;
			Maximizable = false;
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public virtual DialogResult ShowDialog (Control parent)
		{
			if (parent != null) {
				var parentWindow = parent.ParentWindow;
				if (parentWindow != null) {
					var owner = ((IWpfWindow)parentWindow.Handler).Control;
					Control.Owner = owner;
					// CenterOwner does not work in certain cases (e.g. with autosizing)
					Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
					Control.SourceInitialized += HandleSourceInitialized;
				}
			}
			Control.ShowDialog ();
            WpfFrameworkElementHelper.ShouldCaptureMouse = false;
            return Widget.DialogResult;
		}

		void HandleSourceInitialized (object sender, EventArgs e)
		{
			var owner = Control.Owner;
			Control.Left = owner.Left + (owner.ActualWidth - Control.ActualWidth) / 2;
			Control.Top = owner.Top + (owner.ActualHeight - Control.ActualHeight) / 2;
			Control.SourceInitialized -= HandleSourceInitialized;
		}

		public Button DefaultButton
		{
			get { return defaultButton; }
			set
			{
				if (defaultButton != null) {
					var handler = (ButtonHandler)defaultButton.Handler;
					handler.Control.IsDefault = false;
				}
				defaultButton = value;
				if (defaultButton != null) {
					var handler = (ButtonHandler)defaultButton.Handler;
					handler.Control.IsDefault = true;
				}
			}
		}

		public Button AbortButton
		{
			get { return abortButton; }
			set
			{
				if (abortButton != null) {
					var handler = (ButtonHandler)abortButton.Handler;
					handler.Control.IsCancel = false;
				}
				abortButton = value;
				if (abortButton != null) {
					var handler = (ButtonHandler)abortButton.Handler;
					handler.Control.IsCancel = true;
				}
			}
		}
	}
}
#endif