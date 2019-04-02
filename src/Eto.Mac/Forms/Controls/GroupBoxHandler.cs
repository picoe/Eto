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
			set { Control.Title = value; }
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			var boundsSize = new SizeF(16, (float)TitleCell.CellSize.Height + 8);
			availableSize -= boundsSize;

			return base.GetPreferredSize(availableSize) + boundsSize;
		}

		NSTextFieldCell TitleCell { get { return (NSTextFieldCell)Control.TitleCell; } }

		public Color TextColor
		{
			get { return TitleCell.TextColor.ToEto(); }
			set
			{ 
				TitleCell.TextColor = value.ToNSUI(); 
				Control.SetNeedsDisplay();
			}
		}

		protected override bool UseNSBoxBackgroundColor => false;
	}
}
