using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System;
using System.Threading.Tasks;
using Eto.Drawing;

namespace Eto.WinForms.Forms
{
    public class DialogHandler : WindowHandler<swf.Form, Dialog, Dialog.ICallback>, Dialog.IHandler
    {
        Button button;
        swf.Panel panelContent;
        swf.TableLayoutPanel panelButtons;

        public DialogHandler()
        {
            Control = new swf.Form
			{
				StartPosition = swf.FormStartPosition.CenterParent,
				AutoSize = true,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				ShowInTaskbar = false,
				ShowIcon = false,
				MaximizeBox = false,
				MinimizeBox = false
			};

			panelContent = new swf.Panel
			{
				Dock = swf.DockStyle.Fill,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};

			panelButtons = new swf.TableLayoutPanel
			{
				Dock = swf.DockStyle.Bottom,
				Height = 0
			};
			panelButtons.RowStyles.Add(new swf.RowStyle(swf.SizeType.AutoSize));
			panelButtons.ColumnStyles.Add(new swf.ColumnStyle(swf.SizeType.Percent, 100));

			Control.Controls.Add(panelContent);
			Control.Controls.Add(panelButtons);
		}

		public override swf.Control ContainerContentControl => panelContent;

		protected override swf.FormBorderStyle DefaultWindowStyle
		{
			get { return swf.FormBorderStyle.FixedDialog; }
		}

		public Button AbortButton { get; set; }

		public Button DefaultButton
		{
			get
			{
				return button;
			}
			set
			{
				button = value;
				if (button != null)
				{
					var b = button.ControlObject as swf.IButtonControl;
					Control.AcceptButton = b;
				}
				else
					Control.AcceptButton = null;
			}
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public void ShowModal()
        {
            ReloadButtons();

            Control.ShowDialog();
			Control.Owner = null; // without this, the dialog is still active as part of the owner form

            ClearButtons();
        }

		protected override void Initialize()
		{
			base.Initialize();

			Widget.KeyDown += Widget_KeyDown;
		}

		void Widget_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape && AbortButton != null)
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

        private void ClearButtons()
        {
            while (panelButtons.ColumnStyles.Count > 1)
                panelButtons.ColumnStyles.RemoveAt(1);
            panelButtons.Controls.Clear();
        }

        private void ReloadButtons()
        {
            var negativeButtons = Widget.NegativeButtons;
            var positiveButtons = Widget.PositiveButtons;
            var height = 0;

            for (int i = positiveButtons.Count - 1; i >= 0; i--)
            {
                height = Math.Max(height, positiveButtons[i].Height);
                AddButton(positiveButtons.Count - i, positiveButtons[i]);
            }

            for (int i = 0; i < negativeButtons.Count; i++)
            {
                height = Math.Max(height, negativeButtons[i].Height);
                AddButton(positiveButtons.Count + 1 + i, negativeButtons[i]);
            }

            if (negativeButtons.Count + positiveButtons.Count > 0 && height <= 0)
                height = Controls.ButtonHandler.DefaultMinimumSize.Height + 12;

            panelButtons.Height = height;
        }

        private void AddButton(int pos, Button button)
        {
            var native = button.ToNative();
            native.Margin = new swf.Padding(0, 6, 6, 3);

            panelButtons.ColumnStyles.Add(new swf.ColumnStyle(swf.SizeType.Absolute, button.Width > 0 ? button.Width : Controls.ButtonHandler.DefaultMinimumSize.Width));
            panelButtons.Controls.Add(native, pos, 0);
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
    }
}
