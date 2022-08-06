using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	/// <summary>
	/// Handler for <see cref="Button"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler : AndroidTextControl<aw.Button, Button, Button.ICallback>, Button.IHandler
	{
		private Padding originalPadding;

		public ButtonHandler()
		{
			Control = new aw.Button(Platform.AppContextThemed);
			Control.SetIncludeFontPadding(false);
			Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);

			originalPadding = Control.GetPadding();
		}

		Image image;
		ButtonImagePosition imagePosition;

		public Image Image
		{
			get { return image; }
			set
			{
				if (image == value)
					return;

				image = value;
				UpdateImage(value, ImagePosition);
			}
		}

		private void UpdateImage(Image image, ButtonImagePosition position)
		{
			AndroidHelpers.SetCompoundDrawable(Control, position, image);
		}

		public ButtonImagePosition ImagePosition
		{
			get { return imagePosition; }
			set
			{
				if (imagePosition == value)
					return;

				imagePosition = value;
				UpdateImage(Image, ImagePosition);
			}
		}

		protected override void ApplyBackgroundColor(Color? value)
		{
			// Try to preserve Android button styling whilst still changing the color

			if (value.HasValue)
				ContainerControl.BackgroundTintList = ac.Res.ColorStateList.ValueOf(value.Value.ToAndroid());

			else
				ContainerControl.BackgroundTintList = null;
		}

		public override Size Size
		{
			get => base.Size; 
			set
			{
				base.Size = value;

				if (value.Width == -1 && value.Height == -1)
					Control.SetPadding(originalPadding);

				else if (value.Width == -1)
					Control.SetPadding(originalPadding.Left, 0, originalPadding.Right, 0);

				else if (value.Height == -1)
					Control.SetPadding(0, originalPadding.Top, 0, originalPadding.Bottom);

				else
					Control.SetPadding(0);
			}
		}

		public Size MinimumSize
		{
			get { return new Size(Control.MinimumWidth, Control.MinimumHeight); }
			set
			{
				Control.SetMinimumWidth(Math.Max(value.Width, 0));
				Control.SetMinWidth(Math.Max(value.Width, 0));

				Control.SetMinimumHeight(Math.Max(value.Height, 0));
				Control.SetMinHeight(Math.Max(value.Height, 0));
			}
		}
	}
}
