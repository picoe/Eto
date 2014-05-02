using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System;

namespace Eto.Platform.Windows
{
	public class ModalWindowHandler : WindowHandler<swf.Form, ModalWindow>, IModalWindow
	{
		Button button;
		Button abortButton;


		public ModalWindowHandler()
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
					Control.CancelButton = b;
				}
				else
					Control.CancelButton = null;
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
					Control.AcceptButton = b;
				}
				else
					Control.AcceptButton = null;
			}
		}

		public ModalWindowDisplayMode DisplayMode { get; set; }

		public void ShowModal(Action completed, Control parent)
		{
			if (parent != null) Control.ShowDialog((swf.Control)parent.ControlObject);
			else Control.ShowDialog ();

			if (completed != null)
				completed();
		}
	}
}
