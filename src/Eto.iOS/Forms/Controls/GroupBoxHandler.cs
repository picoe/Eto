using System;
using UIKit;
using Eto.Forms;
using Eto.Mac.Forms;
using Eto.Drawing;
using CoreGraphics;

namespace Eto.iOS.Forms.Controls
{
	public class GroupBoxHandler : MacPanel<UIView, GroupBox, GroupBox.ICallback>, GroupBox.IHandler
	{
		UIView content;
		UILabel label;

		public override UIView ContainerControl { get { return Control; } }

		public override UIView ContentControl { get { return content; } }

		protected override void Initialize()
		{
			base.Initialize();
			Control = new UIView();
			Control.BackgroundColor = UIColor.White;

			content = new UIView();
			content.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			Control.AddSubview(content);

			label = new UILabel();
			label.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleBottomMargin;
			Control.AddSubview(label);
		}

		public string Text
		{
			get { return label.Text; }
			set
			{
				var size = GetPreferredSize(SizeF.MaxValue);
				label.Text = value ?? string.Empty;
				SetSize();
				LayoutIfNeeded(size);
			}
		}

		void SetSize()
		{
			var frame = Control.Frame;
			if (!string.IsNullOrEmpty(Text))
			{
				label.SizeToFit();
				label.Hidden = false;
				content.Frame = new CGRect(0, label.Frame.Height, frame.Width, (nfloat)Math.Max(0, frame.Height - label.Frame.Height));
			}
			else
			{
				label.Hidden = true;
				content.Frame = new CoreGraphics.CGRect(0, 0, frame.Width, frame.Height);
			}
		}

		public override Eto.Drawing.SizeF GetPreferredSize(Eto.Drawing.SizeF availableSize)
		{
			var size = base.GetPreferredSize(availableSize);
			if (!string.IsNullOrEmpty(Text))
				size.Height += (float)label.Frame.Height;
			return size;
		}

		public Color TextColor
		{
			get { return label.TextColor.ToEto(); }
			set { label.TextColor = value.ToNSUI(); }
		}
	}
}
