using System;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class ButtonHandler : ButtonHandler<ButtonHandler.EtoButton, Button, Button.ICallback>, Button.IHandler
	{

		// windows guidelines specify default height of 23
		public static Size DefaultMinimumSize = new Size(80, 23);

		protected override Size GetDefaultMinimumSize() => DefaultMinimumSize;

		public class EtoButton : swf.Button
		{
			public EtoButton()
			{
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink;
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

	}

	public abstract class ButtonHandler<TControl, TWidget, TCallback> : WindowsControl<TControl, TWidget, TCallback>, Button.IHandler
		where TControl: swf.ButtonBase
		where TWidget: Button
		where TCallback: Button.ICallback
	{
		protected abstract Size GetDefaultMinimumSize();

		public override Size? GetDefaultSize(Size availableSize) => MinimumSize;

		protected override void Initialize()
		{
			base.Initialize();
			Control.Click += Control_Click;
		}

		void Control_Click(object sender, EventArgs e)
		{
			Callback.OnClick(Widget, EventArgs.Empty);
		}

		public override string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				SetAlign();
			}
		}

		static readonly object Image_Key = new object();

		public Image Image
		{
			get { return Widget.Properties.Get<Image>(Image_Key); }
			set { Widget.Properties.Set(Image_Key, value, () => Control.Image = value.ToSD()); }
		}

		void SetAlign()
		{
			if (string.IsNullOrEmpty(base.Text))
			{
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
			get { return Control.TextImageRelation.ToEto(); }
			set
			{
				Control.TextImageRelation = value.ToSD();
				SetAlign();
			}
		}

		static readonly object MinimumSize_Key = new object();

		public Size MinimumSize
		{
			get { return Widget.Properties.Get(MinimumSize_Key, GetDefaultMinimumSize); }
			set
			{
				if (MinimumSize != value)
				{
					Widget.Properties[MinimumSize_Key] = value;
					ClearCachedDefaultSize();
					SetMinimumSize(true);
				}
			}
		}

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
