using System;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class ToggleButtonHandler : ButtonHandler<ToggleButtonHandler.EtoButton, ToggleButton, ToggleButton.ICallback>, ToggleButton.IHandler
	{
		protected override Size GetDefaultMinimumSize() => new Size(23, 23);

		public class EtoButton : swf.CheckBox
		{
			public EtoButton()
			{
				Appearance = swf.Appearance.Button;
				TextImageRelation = swf.TextImageRelation.ImageBeforeText;
				AutoSize = true;
			}

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var size = base.GetPreferredSize(sd.Size.Empty);

				if (AutoSize && Image != null)
				{
					if (!string.IsNullOrEmpty(Text))
						// fix bug where text will wrap if it has both an image and text
						size.Width += 3;
					else
						// fix bug with image and no text
						size.Height += 1;
				}
				if (Image != null)
				{
					var imgSize = Image.Size.ToEto() + 8;
					size.Width = Math.Max(size.Width, imgSize.Width);
					size.Height = Math.Max(size.Height, imgSize.Height);
				}

				return size;
			}
		}

		protected override EtoButton CreateControl() => new EtoButton();

		public bool Checked
		{
			get => Control.Checked;
			set => Control.Checked = value;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ToggleButton.CheckedChangedEvent:
					Control.CheckedChanged += Control_CheckedChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void Control_CheckedChanged(object sender, EventArgs e)
		{
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}
	}
}
