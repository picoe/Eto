using System;
using System.Reflection;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler : WindowsControl<ButtonHandler.EtoButton, Button>, IButton
	{
		Image image;
		Size defaultSize;

		public class EtoButton : swf.Button
		{
			public override sd.Size GetPreferredSize (sd.Size proposedSize)
			{
				var size = base.GetPreferredSize (sd.Size.Empty);
				// fix bug where text will wrap if it has both an image and text
				if (AutoSize && Image != null) {
					size.Width += 3;
				}
				return size;
			}
		}

		public override Size? DefaultSize
		{
			get { return defaultSize; }
		}

		public ButtonHandler ()
		{
			Control = new EtoButton
			{
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink,
				TextImageRelation = swf.TextImageRelation.ImageBeforeText,
				AutoSize = true
			};
			Control.Click += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
			defaultSize = Button.DefaultSize;
		}

		public override string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				SetAlign ();
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Image = image.ToSD ();
			}
		}

		void SetAlign ()
		{
			if (string.IsNullOrEmpty (base.Text)) {
				if (Control.TextImageRelation == swf.TextImageRelation.ImageBeforeText)
					Control.ImageAlign = sd.ContentAlignment.MiddleLeft;
				else if (Control.TextImageRelation == swf.TextImageRelation.TextBeforeImage)
					Control.ImageAlign = sd.ContentAlignment.MiddleRight;
				else
					Control.ImageAlign = sd.ContentAlignment.MiddleCenter;
			}
			else
				Control.ImageAlign = sd.ContentAlignment.MiddleCenter;
		}

		public ButtonImagePosition ImagePosition
		{
			get { return Control.TextImageRelation.ToEto (); }
			set {
				Control.TextImageRelation = value.ToSD ();
				SetAlign ();
			}
		}
	}
}
