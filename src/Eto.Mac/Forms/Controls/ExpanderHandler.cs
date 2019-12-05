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
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class ExpanderHandler : MacPanel<NSView, Expander, Expander.ICallback>, Expander.IHandler
	{
		NSButton disclosureButton;
		NSView header;
		NSView content;
		int suspendContentSizing;


		public class NSExpanderView : MacEventView
		{
			new ExpanderHandler Handler => base.Handler as ExpanderHandler;

			public override void Layout()
			{
				if (MacView.NewLayout)
					base.Layout();
				Handler?.PerformLayout();
				if (!MacView.NewLayout)
					base.Layout();
			}
		}

		static readonly object EnableAnimation_Key = new object();

		public bool EnableAnimation
		{
			get { return Widget.Properties.Get<bool>(EnableAnimation_Key, true); }
			set { Widget.Properties.Set(EnableAnimation_Key, value); }
		}

		static readonly object AnimationDuration_Key = new object();

		public double AnimationDuration
		{
			get { return Widget.Properties.Get<double>(AnimationDuration_Key, 0.2); }
			set { Widget.Properties.Set(AnimationDuration_Key, value); }
		}

		protected override NSView CreateControl() => new NSExpanderView();

		protected override void Initialize()
		{
			Control.AutoresizesSubviews = false;

			disclosureButton = new NSButton
			{
				Title = string.Empty,
				BezelStyle = NSBezelStyle.Disclosure
			};
			disclosureButton.Activated += DisclosureButton_Activated;
			disclosureButton.SetButtonType(NSButtonType.PushOnPushOff);
			disclosureButton.SizeToFit();

			header = new NSView();

			content = new MacPanelView
			{
				Handler = this,
				Hidden = true
			};

			Control.AddSubview(disclosureButton);
			Control.AddSubview(header);
			Control.AddSubview(content);

			base.Initialize();
		}

		protected override bool ControlEnabled
		{
			get => disclosureButton.Enabled;
			set
			{
				disclosureButton.Enabled = value;
				base.ControlEnabled = value;
			}
		}

		static void DisclosureButton_Activated(object sender, EventArgs e)
		{
			var handler = GetHandler((sender as NSView)?.Superview?.Superview) as ExpanderHandler;
			handler?.UpdateExpandedState();
		}

		void UpdateExpandedState()
		{
			if (!Widget.Loaded)
			{
				content.Hidden = !Expanded;
				return;
			}
			if (EnableAnimation)
			{
				var frame = content.Frame;
				var controlFrame = Control.Frame;

				if (Expanded)
				{
					frame.Y = 0;
					suspendContentSizing++;
					var newSize = GetPreferredSize(SizeF.PositiveInfinity);

					controlFrame.Height = newSize.Height;
					Control.Frame = controlFrame;
					InvalidateMeasure();
					Control.Window?.LayoutIfNeeded();
					content.Frame = new CGRect(0, controlFrame.Height - header.Frame.Height, controlFrame.Width, 0);
					suspendContentSizing--;
					frame.Height = newSize.Height - header.Frame.Height;
					content.Hidden = false;
				}
				else
				{
					frame.Y = Control.Frame.Height - header.Frame.Height;
					frame.Height = 0;
				}

				NSAnimationContext.RunAnimation(ctx =>
				{
					ctx.Duration = AnimationDuration;
					((NSView)content.Animator).Frame = frame;
				}, () =>
				{
					content.Hidden = !Expanded;
					Callback.OnExpandedChanged(Widget, EventArgs.Empty);
					InvalidateMeasure();
				}
				);
			}
			else
			{
				content.Hidden = !Expanded;
				InvalidateMeasure();
				Callback.OnExpandedChanged(Widget, EventArgs.Empty);
			}
		}

		void PerformLayout()
		{
			var size = Control.Frame.Size;
			var disclosureSize = disclosureButton.Frame.Size;
			var headerSize = SizeF.Max(disclosureSize.ToEto(), Header.GetPreferredSize(Size));
			disclosureButton.SetFrameOrigin(new CGPoint(0, (nfloat)Math.Round(size.Height - disclosureSize.Height - ((headerSize.Height - disclosureSize.Height) / 2))));
			header.Frame = new CGRect(disclosureSize.Width, size.Height - headerSize.Height, size.Width - disclosureSize.Width, headerSize.Height);
			if (suspendContentSizing <= 0)
			{
				if (Expanded)
					content.Frame = new CGRect(0, 0, size.Width, size.Height - headerSize.Height);
				else
					content.Frame = new CGRect(0, size.Height - headerSize.Height, size.Width, 0);
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var disclosureSize = disclosureButton.Frame.Size.ToEto();
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
					disclosureButton.State = value ? NSCellStateValue.On : NSCellStateValue.Off;
					UpdateExpandedState();
				}
			}
		}

		static readonly object Header_Key = new object();

		public Control Header
		{
			get { return Widget.Properties.Get<Control>(Header_Key); }
			set
			{
				Widget.Properties.Set(Header_Key, value, () =>
				{
					var subview = header.Subviews.FirstOrDefault();
					if (subview != null)
						subview.RemoveFromSuperview();

					subview = value.GetContainerView();
					subview.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
					subview.Frame = header.Bounds;
					header.AddSubview(subview);
					InvalidateMeasure();
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
	}
}
