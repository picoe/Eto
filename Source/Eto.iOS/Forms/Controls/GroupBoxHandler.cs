using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Mac.Forms;
using Eto.Drawing;

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
				content.Frame = new System.Drawing.RectangleF(0, label.Frame.Height, frame.Width, Math.Max(0, frame.Height - label.Frame.Height));
			}
			else
			{
				label.Hidden = true;
				content.Frame = new System.Drawing.RectangleF(0, 0, frame.Width, frame.Height);
			}
		}

		public override Eto.Drawing.SizeF GetPreferredSize(Eto.Drawing.SizeF availableSize)
		{
			var size = base.GetPreferredSize(availableSize);
			if (!string.IsNullOrEmpty(Text))
				size.Height += label.Frame.Height;
			return size;
		}

		public Color TextColor
		{
			get { return label.TextColor.ToEto(); }
			set { label.TextColor = value.ToNSUI(); }
		}
	}
}
