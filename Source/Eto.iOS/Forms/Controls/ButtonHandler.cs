using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoTouch.UIKit;

namespace Eto.iOS.Forms.Controls
{
	public class ButtonHandler : IosButton<UIButton, Button, Button.ICallback>, Button.IHandler
	{
		public static Size MinimumSize = new Size(80, 23);
		
		class MyButton : UIButton {
			
			public ButtonHandler Handler { get; set; }
			
			public override System.Drawing.RectangleF Frame {
				get {
					var value = base.Frame;
					if (Handler.AutoSize)
					{
						value.Width = Math.Max(MinimumSize.Width, value.Width);
						value.Height = Math.Max(MinimumSize.Height, value.Height);
					}
					return value;
				}
				set {
					if (Handler.AutoSize)
					{
						value.Width = Math.Max(MinimumSize.Width, value.Width);
						value.Height = Math.Max(MinimumSize.Height, value.Height);
					}
					base.Frame = value;
				}
			}
		}

		public override Color BackgroundColor {
			get {
				return Control.Layer.BackgroundColor.ToEtoColor ();
			}
			set {
				Control.Layer.BackgroundColor = value.ToCGColor ();
			}
		}

		public ButtonHandler()
		{
			Control = UIButton.FromType(UIButtonType.RoundedRect);
			Control.SetTitle(string.Empty, UIControlState.Normal);
			//Control.ButtonType = UIButtonType.RoundedRect;
			Control.TouchUpInside += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
		}

		public Image Image
		{
			get;
			set;
		}

		public ButtonImagePosition ImagePosition
		{
			get;
			set;
		}
	}
}
