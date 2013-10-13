using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using MonoTouch.UIKit;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class NumericUpDownHandler : IosControl<UITextField, NumericUpDown>, INumericUpDown
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
			Control.KeyboardType = UIKeyboardType.NumberPad;
			Control.ReturnKeyType = UIReturnKeyType.Done;
			Control.BorderStyle = UITextBorderStyle.RoundedRect;
			Control.ShouldReturn = (textField) =>
			{
				textField.ResignFirstResponder();
				return true;
			};
			Control.EditingChanged += (sender, e) =>
			{
				Widget.OnValueChanged(EventArgs.Empty);
			};
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public double Value
		{
			get
			{
				double value;
				if (double.TryParse(Control.Text, out value))
					return value;
				return 0;
			}
			set
			{
				Control.Text = value.ToString();
			}
		}

		public double MinValue
		{
			get;
			set;
		}

		public double MaxValue
		{
			get;
			set;
		}
	}
}
