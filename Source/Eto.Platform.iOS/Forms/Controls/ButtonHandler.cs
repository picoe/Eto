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
		
		
		public ButtonHandler ()
		{
			Control = UIButton.FromType(UIButtonType.RoundedRect); //new MyButton{ Handler = this };
			Control.SetTitle(string.Empty, UIControlState.Normal);
			//Control.ButtonType = UIButtonType.RoundedRect;
			Control.SetFrameSize(Generator.ConvertF(Button.DefaultSize));
			Control.TouchUpInside += delegate {
				Widget.OnClick(EventArgs.Empty);
			};
		}
		
	}
}
