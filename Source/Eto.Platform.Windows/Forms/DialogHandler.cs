using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class DialogHandler : WindowHandler<swf.Form, Dialog>, IDialog
	{
		Button button;
		Button abortButton;

		
		public DialogHandler()
		{
			Control = new swf.Form {
				StartPosition = swf.FormStartPosition.CenterParent,
				AutoSize = true,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				ShowInTaskbar = false,
				ShowIcon = false,
				MaximizeBox = false,
				MinimizeBox = false
			};
		}

        protected override swf.FormBorderStyle DefaultWindowStyle
        {
            get { return swf.FormBorderStyle.FixedDialog; }
        }

		public Button AbortButton {
			get {
				return abortButton;
			}
			set {
				abortButton = value;
				if (abortButton != null) {
					var b = abortButton.ControlObject as swf.IButtonControl;
					this.Control.CancelButton = b;
				}
				else
					this.Control.CancelButton = null;
			}
		}
		
		public Button DefaultButton
		{
			get {
				return button;
			}
			set {
				button = value;
				if (button != null) {
					var b = button.ControlObject as swf.IButtonControl;
					this.Control.AcceptButton = b;
				}
				else
					this.Control.AcceptButton = null;
			}
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public DialogResult ShowDialog(Control parent)
		{
			if (parent != null) Control.ShowDialog((swf.Control)parent.ControlObject);
			else Control.ShowDialog ();

			return Widget.DialogResult;
		}
	}
}
