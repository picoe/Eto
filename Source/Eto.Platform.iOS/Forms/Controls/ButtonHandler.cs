using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ButtonHandler : iosButton<UIButton, Button>, IButton
	{
		
		class MyButton : UIButton {
			
			public ButtonHandler Handler { get; set; }
			
			public override System.Drawing.RectangleF Frame {
				get {
					var value = base.Frame;
					if (Handler.AutoSize)
					{
						var defaultSize = Button.DefaultSize;
						if (value.Width < defaultSize.Width) value.Width = defaultSize.Width;
						if (value.Height < defaultSize.Height) value.Height = defaultSize.Height;
					}
					return value;
				}
				set {
					if (Handler.AutoSize)
					{
						var defaultSize = Button.DefaultSize;
						if (value.Width < defaultSize.Width) value.Width = defaultSize.Width;
						if (value.Height < defaultSize.Height) value.Height = defaultSize.Height;
					}
					base.Frame = value;
				}
			}
		}

		public override void AttachEvent (string handler)
		{
			base.AttachEvent (handler);
		}

		public override Color BackgroundColor {
			get {
				return Control.Layer.BackgroundColor.ToEtoColor ();
			}
			set {
				Control.Layer.BackgroundColor = value.ToCGColor ();
			}
		}

		public override UIButton CreateControl ()
		{
			//return UIButton.FromType(UIButtonType.Custom);
			return UIButton.FromType(UIButtonType.RoundedRect);
		}

		public override void Initialize ()
		{
			base.Initialize ();
			/**
			Control.SetTitleColor (UIColor.Black, UIControlState.Normal);
			Control.BackgroundColor = UIColor.White;
			Control.Layer.BorderColor = UIColor.Black.CGColor;
			Control.Layer.BorderWidth = 0.5f;
			Control.Layer.CornerRadius = 7f;
			/**/
			Control = UIButton.FromType(UIButtonType.RoundedRect);
			/**/
			Control.SetTitle(string.Empty, UIControlState.Normal);
			//Control.ButtonType = UIButtonType.RoundedRect;
			Control.SetFrameSize(Button.DefaultSize.ToSDSizeF());
			Control.TouchUpInside += delegate {
				Widget.OnClick(EventArgs.Empty);
			};
		}
		
		#region IButton implementation
		public IImage Image {
			get {throw new NotImplementedException(); }
			set {throw new NotImplementedException(); }
		}
		#endregion
	}
}
