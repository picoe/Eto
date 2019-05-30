using System;
using UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class PasswordBoxHandler : IosControl<UITextField, PasswordBox, PasswordBox.ICallback>, PasswordBox.IHandler
	{

		public PasswordBoxHandler()
		{
			Control = new UITextField();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return SizeF.Max(base.GetNaturalSize(availableSize), new SizeF(60, 0));
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.SecureTextEntry = true;
			MaxLength = Int32.MaxValue;
			Control.BorderStyle = UITextBorderStyle.RoundedRect;
			Control.ShouldChangeCharacters = delegate(UITextField textField, Foundation.NSRange range, string replacementString)
			{
				var text = textField.Text;
				if (text.Length + replacementString.Length - range.Length > MaxLength)
				{

					if (range.Length > 0)
						text = text.Remove((int)range.Location, (int)range.Length);
					replacementString = replacementString.Substring(0, (int)(MaxLength - text.Length + range.Length));
					text = text.Insert((int)range.Location, replacementString);
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

		public Color TextColor
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
}

