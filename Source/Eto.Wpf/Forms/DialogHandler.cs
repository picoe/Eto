using System;
using Eto.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Wpf.Forms.Controls;
using System.Threading.Tasks;
using Eto.Drawing;

namespace Eto.Wpf.Forms
{
	public class DialogHandler : WpfWindow<sw.Window, Dialog, Dialog.ICallback>, Dialog.IHandler
	{
		Button defaultButton;
		Rectangle? parentWindowBounds;
        swc.DockPanel dockMain;
        swc.Grid gridButtons;

		public DialogHandler()
		{
			Control = new sw.Window();
			Control.ShowInTaskbar = false;
			Resizable = false;
			Minimizable = false;
			Maximizable = false;
			Control.PreviewKeyDown += Control_PreviewKeyDown;

            dockMain = new swc.DockPanel();

            gridButtons = new swc.Grid();
            gridButtons.Margin = new sw.Thickness(8);
            gridButtons.RowDefinitions.Add(new swc.RowDefinition());
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

			if (Widget.Owner != null)
			{
				// CenterOwner does not work in certain cases (e.g. with autosizing)
				Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
				Control.SourceInitialized += HandleSourceInitialized;
				parentWindowBounds = Widget.Owner.Bounds;
			}
			Control.ShowDialog();
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;

            ClearButtons();
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

		void HandleSourceInitialized(object sender, EventArgs e)
		{
			if (parentWindowBounds != null && !LocationSet)
			{
				var bounds = parentWindowBounds.Value;
				Control.Left = bounds.Left + (bounds.Width - Control.ActualWidth) / 2;
				Control.Top = bounds.Top + (bounds.Height - Control.ActualHeight) / 2;
				parentWindowBounds = null;
			}
			LocationSet = false;
			Control.SourceInitialized -= HandleSourceInitialized;
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

            for (int i = negativeButtons.Count - 1; i >= 0; i--)
                AddButton(negativeButtons.Count - i, negativeButtons[i]);

            for (int i = 0;i < positiveButtons.Count;i++)
                AddButton(negativeButtons.Count + 1 + i, positiveButtons[i]);
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
            if(Widget.Visible)
            {
                ClearButtons();
                ReloadButtons();
            }
        }

        public void RemoveDialogButton(bool positive, int index)
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
