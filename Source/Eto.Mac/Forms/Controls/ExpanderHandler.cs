using System;
using Eto.Drawing;
using Eto.Forms;
using System.Linq;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;

#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;

#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;

#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class ExpanderHandler : MacPanel<NSView, Expander, Expander.ICallback>, Expander.IHandler
	{
		readonly NSButton disclosureButton;
		readonly NSView header;
		readonly NSView content;

		static readonly object EnableAnimation_Key = new object();

		public bool EnableAnimation
		{
			get { return Widget.Properties.Get<bool>(EnableAnimation_Key, false); }
			set { Widget.Properties.Set(EnableAnimation_Key, value); }
		}

		static IntPtr selSetFrame = Selector.GetHandle("setFrame:");

		public ExpanderHandler()
		{
			Control = new MacEventView
			{ 
				Handler = this,
				AutoresizesSubviews = false
			};

			disclosureButton = new NSButton
			{
				Title = string.Empty,
				BezelStyle = NSBezelStyle.Disclosure
			};
			disclosureButton.SetButtonType(NSButtonType.PushOnPushOff);
			disclosureButton.SizeToFit();
			var size = disclosureButton.Frame.Size;
			Control.AddSubview(disclosureButton);

			header = new NSView();

			Control.AddSubview(header);
			content = new NSView();
			content.WantsLayer = true;
			Control.AddSubview(content);
			disclosureButton.Activated += (sender, e) =>
			{
				if (EnableAnimation)
				{
					var frame = content.Frame;
					var controlFrame = Control.Frame;
					if (Expanded)
					{
						frame.Y = 0;
						var newSize = GetPreferredSize(SizeF.MaxValue);
						//controlFrame.Y -= newSize.Height - controlFrame.Height;
						controlFrame.Height = newSize.Height;
						Control.Frame = controlFrame;
						SetSizes();
						frame.Height = newSize.Height - header.Frame.Height;
						//frame.Y -= frame.Height;
						content.Hidden = false;
					}
					else
					{
						frame.Y = Control.Frame.Height - header.Frame.Height;
						frame.Height = 0;
					}

					NSAnimationContext.RunAnimation(ctx =>
					{
						ctx.Duration = 0.2;
						((NSView)content.Animator).Frame = frame;
						//Messaging.bool_objc_msgSend_CGRect(Control.Animator.Handle, selSetFrame, controlFrame);
					}, new NSAction(() =>
					{
						content.Hidden = !Expanded;
						Callback.OnExpandedChanged(Widget, EventArgs.Empty);
						LayoutIfNeeded();
					})
					);
				}
				else
				{
					LayoutIfNeeded(force: true);
					Callback.OnExpandedChanged(Widget, EventArgs.Empty);
				}
			};
			content.Hidden = true;
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetSizes();
		}

		public override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			SetSizes();
		}

		void SetSizes()
		{
			var size = Control.Frame.Size;
			var disclosureSize = disclosureButton.Frame.Size;
			var headerSize = SizeF.Max(disclosureSize.ToEto(), Header.GetPreferredSize(Size));
			disclosureButton.SetFrameOrigin(new CGPoint(0, (nfloat)Math.Round(size.Height - disclosureSize.Height - ((headerSize.Height - disclosureSize.Height) / 2))));
			header.Frame = new CGRect(disclosureSize.Width, size.Height - headerSize.Height, size.Width - disclosureSize.Width, headerSize.Height);
			if (Expanded)
				content.Frame = new CGRect(0, 0, size.Width, size.Height - headerSize.Height);
			else
				content.Frame = new CGRect(0, size.Height - headerSize.Height, size.Width, 0);
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var disclosureSize = disclosureButton.Frame.Size;
			var headerSize = Header.GetPreferredSize(availableSize);
			headerSize = new SizeF(disclosureSize.Width + headerSize.Width, Math.Max(disclosureSize.Height, headerSize.Height));
			var contentSize = base.GetNaturalSize(availableSize);
			if (!Expanded)
			{
				headerSize.Width = Math.Max(contentSize.Width, headerSize.Width);
				return headerSize;
			}
			return new SizeF(Math.Max(headerSize.Width, contentSize.Width), headerSize.Height + contentSize.Height);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Expander.ExpandedChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public bool Expanded
		{
			get { return disclosureButton.State == NSCellStateValue.On; }
			set
			{
				if (value != Expanded)
				{
					var size = GetPreferredSize(SizeF.MaxValue);
					disclosureButton.State = value ? NSCellStateValue.On : NSCellStateValue.Off;
					Callback.OnExpandedChanged(Widget, EventArgs.Empty);
					LayoutIfNeeded(size);
				}
			}
		}

		static readonly object Header_Key = new object();

		public Control Header
		{
			get { return Widget.Properties.Get<Control>(Header_Key); }
			set
			{
				var size = GetPreferredSize(SizeF.MaxValue);
				Widget.Properties.Set(Header_Key, value, () =>
				{
					var subview = header.Subviews.FirstOrDefault();
					if (subview != null)
						subview.RemoveFromSuperview();

					subview = value.ToNative();
					subview.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
					subview.Frame = header.Bounds;
					header.AddSubview(subview);
					LayoutIfNeeded(size);
				});
			}
		}

		public override NSView ContainerControl
		{
			get { return Control; }
		}

		public override NSView ContentControl
		{
			get { return content; }
		}

		public override void LayoutChildren()
		{
			base.LayoutChildren();
			SetSizes();
		}
	}
}
