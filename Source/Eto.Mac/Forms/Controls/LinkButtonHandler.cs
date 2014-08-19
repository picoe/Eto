using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
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
	/// <summary>
	/// LinkButton handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinkButtonHandler : MacButton<NSButton, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{
		readonly NSMutableAttributedString str;

		Font hoverFont;
		Font normalFont;

		class EtoButton : NSButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public LinkButtonHandler Handler
			{ 
				get { return (LinkButtonHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public LinkButtonHandler()
		{
			Control = new EtoButton
			{ 
				Handler = this,
				Cell = new ButtonHandler.EtoButtonCell(),
				Title = string.Empty,
				BezelStyle = NSBezelStyle.Inline,
				Bordered = false
			};
			Control.SetButtonType(NSButtonType.MomentaryPushIn);
			str = new NSMutableAttributedString();
		}

		protected override void Initialize()
		{
			base.Initialize();

			normalFont = Font;
			hoverFont = new Font(Font.Typeface, Font.Size, FontDecoration.Underline);
			TextColor = NSColor.Blue.ToEto();
			Cursor = Cursors.Pointer;
			Widget.MouseEnter += HandleMouseEnter;
			Widget.MouseLeave += HandleMouseLeave;
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetAttributes(true);
		}

		void HandleMouseLeave(object sender, MouseEventArgs e)
		{
			Font = normalFont;
		}

		void HandleMouseEnter(object sender, MouseEventArgs e)
		{
			if (Enabled)
				Font = hoverFont;
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as LinkButtonHandler;
			if (handler != null)
			{
				handler.Callback.OnClick(handler.Widget, EventArgs.Empty);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case LinkButton.ClickEvent:
					Control.Activated += HandleActivated;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override string Text
		{
			get { return str.Value; }
			set
			{
				str.SetString(new NSAttributedString(value));
				SetAttributes();
			}
		}

		Color? textColor;

		public Color TextColor
		{
			get { return textColor ?? NSColor.ControlText.ToEto(); }
			set
			{
				textColor = value;
				SetAttributes();
			}
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				SetAttributes();
			}
		}

		void SetAttributes(bool force = false)
		{
			if (Widget.Loaded || force)
			{
				if (str.Length > 0)
				{
					var range = new NSRange(0, (int)str.Length);
					var attr = new NSMutableDictionary();
					font.Apply(attr);
					//attr.Add(NSAttributedString.ParagraphStyleAttributeName, paragraphStyle);
					if (textColor != null)
						attr.Add(NSAttributedString.ForegroundColorAttributeName, textColor.Value.ToNSUI());
					str.SetAttributes(attr, range);
				}
				Control.AttributedTitle = str;
			}
		}

		public override Color BackgroundColor
		{
			get
			{
				var cell = (ButtonHandler.EtoButtonCell)Control.Cell;
				return cell.Color ?? Colors.Transparent;
			}
			set
			{
				var cell = (ButtonHandler.EtoButtonCell)Control.Cell;
				cell.Color = value.A > 0 ? (Color?)value : null;
				Control.SetNeedsDisplay();
			}
		}

	}
}
