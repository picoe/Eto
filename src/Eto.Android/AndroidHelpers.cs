using System;
using Android.Widget;
using Eto.Android.Forms;
using Eto.Drawing;
using Eto.Forms;

using av = Android.Views;
using ag = Android.Graphics;
using Android.Views;

namespace Eto.Android
{
	public static class AndroidHelpers
	{
		public static av.View ToNative(this Control control, bool attach = false)
		{
			if (attach)
				control.AttachNative();

			return control.GetContainerView();
		}

		public static Control ToEto(this av.View control)
		{
			return new Control(new NativeControlHandler(control));
		}

		/// <summary>
		/// Uses SetCompoundDrawable to set image on a control in the correct position, scaling correctly for DPI.
		/// </summary>
		public static void SetCompoundDrawable(TextView view, ButtonImagePosition position, Image image, Size? size = null)
		{
			ag.Drawables.Drawable Left = null, Top = null, Right = null, Bottom = null;

			if (image != null)
			{
				var Icon = image.ToAndroidDrawable(size ?? image.Size);

				// Ideally support padding, but InsetDrawable currently just makes the drawable disappear completely.
				//Icon = new ag.Drawables.InsetDrawable(Icon, Platform.DpToPx(4));

				if (position == ButtonImagePosition.Left || position == ButtonImagePosition.Overlay)
					Left = Icon;

				else if (position == ButtonImagePosition.Above)
					Top = Icon;

				else if (position == ButtonImagePosition.Right)
					Right = Icon;

				else if (position == ButtonImagePosition.Below)
					Bottom = Icon;
			}

			view.SetCompoundDrawables(Left, Top, Right, Bottom);
		}

		public static ViewGroup.LayoutParams CreateOrAdjustLayoutParameters(ViewGroup.LayoutParams layoutParams, int width, int height)
		{
			if (layoutParams == null)
				return new ViewGroup.LayoutParams(width, height);

			layoutParams.Width = width;
			layoutParams.Height = height;

			return layoutParams;
		}
	}

	public class NativeControlHandler : AndroidControl<av.View, Control, Control.ICallback>
	{
		public NativeControlHandler(av.View nativeControl)
		{
			Control = nativeControl;
		}

		public override av.View ContainerControl => Control;
	}
}
