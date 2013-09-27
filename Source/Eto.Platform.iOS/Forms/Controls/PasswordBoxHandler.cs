using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class PasswordBoxHandler : iosControl<UITextField, PasswordBox>, IPasswordBox
	{
		public override UITextField CreateControl()
		{
			return new UITextField();
		}

		protected override Size GetNaturalSize(Size availableSize)
		{
			return Size.Max(base.GetNaturalSize(availableSize), new Size(60, 0));
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.SecureTextEntry = true;
			MaxLength = Int32.MaxValue;
			Control.BorderStyle = UITextBorderStyle.RoundedRect;
			Control.ShouldChangeCharacters = delegate(UITextField textField, MonoTouch.Foundation.NSRange range, string replacementString)
			{
				var text = textField.Text;
				if (text.Length + replacementString.Length - range.Length > MaxLength)
				{

					if (range.Length > 0)
						text = text.Remove(range.Location, range.Length);
					replacementString = replacementString.Substring(0, MaxLength - text.Length + range.Length);
					text = text.Insert(range.Location, replacementString);
					//UIApplication.SharedApplication.BeginInvokeOnMainThread(delegate {

					//});
					textField.Text = text;
					return false;
				}
				return !ReadOnly;
			};
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public int MaxLength
		{
			get;
			set;
		}

		public char PasswordChar
		{
			get;
			set;
		}
	}
}

