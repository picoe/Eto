using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;

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
	public class GroupBoxHandler : MacPanel<NSBox, GroupBox, GroupBox.ICallback>, GroupBox.IHandler
	{
		SizeF? borderSize;

		public class EtoBox : NSBox, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoBox(GroupBoxHandler handler)
			{
				Title = string.Empty;
				TitlePosition = NSTitlePosition.NoTitle;
				ContentView = new MacPanelView { Handler = handler };
			}
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSBox CreateControl() => new EtoBox(this);

		protected override void Initialize()
		{
			Enabled = true;
			base.Initialize();
		}

		public override NSView ContainerControl => Control;

		public override NSView ContentControl => (NSView)Control.ContentView;

		public override Size ClientSize
		{
			get
			{
				var view = Control.ContentView as NSView;
				return view.Frame.Size.ToEtoSize();
			}
			set
			{
				Control.SetFrameFromContentFrame(new CGRect(0, 0, value.Width, value.Height));
			}
		}

		static readonly object Font_Key = new object();

		public Font Font
		{
			get
			{
				var font = Widget.Properties.Get<Font>(Font_Key);
				if (font == null)
				{
					font = new Font(new FontHandler(Control.TitleFont));
					Widget.Properties.Set(Font_Key, font);
				}
				return font;
			}
			set
			{
				Widget.Properties.Set(Font_Key, value);
				Control.TitleFont = (value?.Handler as FontHandler)?.Control;
				InvalidateMeasure();
			}
		}

		public virtual string Text
		{
			get { return Control.Title; }
			set 
			{
				Control.Title = value ?? string.Empty;
				Control.TitlePosition = string.IsNullOrEmpty(value) ? NSTitlePosition.NoTitle : NSTitlePosition.AtTop;
				InvalidateMeasure();
			}
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			if (borderSize == null)
			{
				var frame = Control.Frame;
				var contentSize = ContentControl.Frame.Size;
				if (contentSize.Width <= 10 || contentSize.Height <= 10)
				{
					contentSize = new CGSize(100, 100);
					var oldFrame = Control.Frame;
					Control.SetFrameFromContentFrame(new CGRect(CGPoint.Empty, contentSize));
					frame = Control.Frame;
					Control.Frame = oldFrame;
				}
				frame = Control.GetAlignmentRectForFrame(frame);
				borderSize = (frame.Size - contentSize).ToEto();
			}

			return base.GetPreferredSize(availableSize - borderSize.Value) + borderSize.Value;
		}

		NSTextFieldCell TitleCell => (NSTextFieldCell)Control.TitleCell;

		public Color TextColor
		{
			get { return TitleCell.TextColor.ToEto(); }
			set
			{ 
				TitleCell.TextColor = value.ToNSUI(); 
				Control.SetNeedsDisplay();
			}
		}

		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			borderSize = null;
		}

		protected override bool UseNSBoxBackgroundColor => false;
	}
}
