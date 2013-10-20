using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class TextBoxHandler : IosControl<UITextField, TextBox>, ITextBox
	{
		public override UITextField CreateControl()
		{
			return new UITextField();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return SizeF.Max(base.GetNaturalSize(availableSize), new SizeF(60, 0));
		}

		protected override void Initialize()
		{
			base.Initialize();
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

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case Eto.Forms.Control.TextChangedEvent:
					Control.EditingChanged += (s, e) => Widget.OnTextChanged(e);
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
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

		public string PlaceholderText
		{
			get { return Control.Placeholder; }
			set { Control.Placeholder = value; }
		}

		public void SelectAll()
		{
			Control.SelectAll(Control);
		}
	}
}

