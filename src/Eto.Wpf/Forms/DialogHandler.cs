using System.Windows.Automation.Peers;
using Eto.Wpf.Forms.Controls;
namespace Eto.Wpf.Forms
{
	class EtoWindowAutomationPeer : WindowAutomationPeer
	{
		public EtoWindowAutomationPeer(sw.Window owner) : base(owner)
		{
		}

		protected override string GetNameCore()
		{
			try
			{
				// due to windows message hooks, we can already be in error state which causes exceptions here.
#if NET
				Marshal.SetLastSystemError(0);
#else
				Win32.SetLastError(0);
#endif
				return base.GetNameCore();
			}
			catch (Win32Exception)
			{
				// See https://github.com/dotnet/wpf/issues/4181 and https://github.com/dotnet/wpf/pull/7345
				// Until that fix is in, we fix it ourselves to avoid random crashes
				return (Owner as sw.Window)?.Title ?? string.Empty;
			}
		}
	}


	public class EtoWindow : sw.Window
	{
		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new EtoWindowAutomationPeer(this);
		}
	}

	public class DialogHandler : WpfWindow<sw.Window, Dialog, Dialog.ICallback>, Dialog.IHandler
	{
		Button defaultButton;
		Rectangle? parentWindowBounds;
		swc.DockPanel dockMain;
		swc.Grid gridButtons;

		public DialogHandler() : this(new EtoWindow()) { }

		public DialogHandler(sw.Window window)
		{
			Control = window;
			Control.ShowInTaskbar = false;
			Control.PreviewKeyDown += Control_PreviewKeyDown;

			dockMain = new swc.DockPanel();

			gridButtons = new swc.Grid();
			gridButtons.RowDefinitions.Add(new swc.RowDefinition());
			gridButtons.Visibility = System.Windows.Visibility.Hidden;
			gridButtons.Margin = new sw.Thickness();
		}

		protected override void Initialize()
		{
			base.Initialize();

			Resizable = false;
			Minimizable = false;
			Maximizable = false;
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			this.content.Children.Add(dockMain);
			swc.DockPanel.SetDock(gridButtons, swc.Dock.Bottom);
			dockMain.Children.Add(gridButtons);
			dockMain.Children.Add(content);
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public void ShowModal()
		{
			ReloadButtons();

			var owner = Widget.Owner;

			if (LocationSet)
			{
				Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
			}
			else if (owner != null)
			{
				// CenterOwner does not work in certain cases (e.g. with autosizing)
				Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
				parentWindowBounds = owner.Bounds;
				Control.Loaded += HandleLoaded;
			}

			// if the owner doesn't have focus, windows changes the owner's z-order after the dialog closes.
			if (owner != null && !owner.HasFocus)
				owner.Focus();

			if (Control.IsLoaded)
			{
				Callback.OnLoadComplete(Widget, EventArgs.Empty);
				FireOnLoadComplete = false;
			}
			else
			{
				FireOnLoadComplete = true;
			}

			var _ = NativeHandle; // ensure SourceInitialized is called to get right size based on style flags

			Control.ShowDialog();
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;

			ClearButtons();
		}

		public override void SetOwner(Window owner)
		{
			// Dialogs can not change owner after shown
			if (!Widget.Loaded)
				base.SetOwner(owner);
		}

		void Control_PreviewKeyDown(object sender, sw.Input.KeyEventArgs e)
		{
			if (e.Key == sw.Input.Key.Escape && AbortButton != null)
			{
				AbortButton.PerformClick();
				e.Handled = true;
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

		void HandleLoaded(object sender, EventArgs e)
		{
			if (parentWindowBounds != null && !LocationSet)
			{
				var bounds = parentWindowBounds.Value;

				Location = bounds.Location + (bounds.Size - Control.GetSize()) / 2;
				parentWindowBounds = null;
			}
			LocationSet = false;
			Control.Loaded -= HandleLoaded;
		}

		private void ClearButtons()
		{
			gridButtons.ColumnDefinitions.Clear();
			gridButtons.Children.Clear();
		}

		private void ReloadButtons()
		{
			gridButtons.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = new sw.GridLength(100, sw.GridUnitType.Star) });

			var negativeButtons = Widget.NegativeButtons;
			var positiveButtons = Widget.PositiveButtons;
			var hasButtons = negativeButtons.Count + positiveButtons.Count > 0;

			for (int i = positiveButtons.Count - 1; i >= 0; i--)
				AddButton(positiveButtons.Count - i, positiveButtons[i]);

			for (int i = 0; i < negativeButtons.Count; i++)
				AddButton(positiveButtons.Count + 1 + i, negativeButtons[i]);

			gridButtons.Visibility = hasButtons ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
			gridButtons.Margin = new sw.Thickness(hasButtons ? 8 : 0);
		}

		private void AddButton(int pos, Button button)
		{
			var native = button.ToNative();
			native.Margin = new sw.Thickness(6, 0, 0, 0);

			swc.Grid.SetColumn(native, pos);

			gridButtons.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = new sw.GridLength(1, sw.GridUnitType.Auto) });
			gridButtons.Children.Add(native);
		}

		public void InsertDialogButton(bool positive, int index, Button item)
		{
			if (Widget.Visible)
			{
				ClearButtons();
				ReloadButtons();
			}
		}

		public void RemoveDialogButton(bool positive, int index, Button item)
		{
			if (Widget.Visible)
			{
				ClearButtons();
				ReloadButtons();
			}
		}

		public Button DefaultButton
		{
			get { return defaultButton; }
			set
			{
				if (defaultButton != null)
				{
					var handler = (ButtonHandler)defaultButton.Handler;
					handler.Control.IsDefault = false;
				}
				defaultButton = value;
				if (defaultButton != null)
				{
					var handler = (ButtonHandler)defaultButton.Handler;
					handler.Control.IsDefault = true;
				}
			}
		}

		public Button AbortButton { get; set; }
	}
}
