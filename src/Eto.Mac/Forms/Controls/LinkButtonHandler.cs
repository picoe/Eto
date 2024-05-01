using Eto.Mac.Drawing;
namespace Eto.Mac.Forms.Controls
{
	/// <summary>
	/// LinkButton handler.
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinkButtonHandler : MacLabel<LinkButtonHandler.EtoLinkLabel, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{
		static readonly object HoverFontKey = new object();
		static readonly object NormalFontKey = new object();

		Font HoverFont
		{
			get { return Widget.Properties.Get<Font>(HoverFontKey); }
			set { Widget.Properties[HoverFontKey] = value; }
		}

		Font NormalFont
		{
			get { return Widget.Properties.Get<Font>(NormalFontKey) ?? Font; }
			set { Widget.Properties[NormalFontKey] = value; }
		}

		public class EtoLinkLabel : EtoLabel
		{
			new LinkButtonHandler Handler { get { return (LinkButtonHandler)base.Handler; } set { base.Handler = value; } }

			public override bool AcceptsFirstResponder()
			{
				return Handler?.Enabled == true;
			}

			public override void DrawRect(CGRect dirtyRect)
			{
				var h = Handler;
				if (h != null && h.HasFocus)
				{
					NSGraphicsContext.CurrentContext.SaveGraphicsState();
					GraphicsExtensions.SetFocusRingStyle(NSFocusRingPlacement.RingOnly);
					NSGraphics.RectFill(this.Bounds);
					NSGraphicsContext.CurrentContext.RestoreGraphicsState();
				}

				base.DrawRect(dirtyRect);
			}

			public EtoLinkLabel()
			{
				FocusRingType = NSFocusRingType.Exterior;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			TextColor = SystemColors.LinkText;
			Cursor = Cursors.Pointer;
			Widget.MouseEnter += HandleMouseEnter;
			Widget.MouseLeave += HandleMouseLeave;
			SetFonts();
			Font = NormalFont;
		}

		public override Cursor CurrentCursor
		{
			get { return Enabled ? Cursor : null; }
		}

		protected override NSColor CurrentColor
		{
			get { return Enabled ? base.CurrentColor : DisabledTextColor.ToNSUI(); }
		}

		protected override EtoLinkLabel CreateControl()
		{
			return new EtoLinkLabel();
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				SetFonts();
			}
		}

		void SetFonts()
		{
			//HoverFont = new Font(Font.Typeface, Font.Size, FontDecoration.Underline);
			//NormalFont = new Font(Font.Typeface, Font.Size);
			base.Font = new Font(Font.Typeface, Font.Size, FontDecoration.Underline);
		}

		void HandleMouseLeave(object sender, MouseEventArgs e)
		{
			Font = NormalFont;
		}

		void HandleMouseEnter(object sender, MouseEventArgs e)
		{
			if (Enabled && HoverFont != null)
				Font = HoverFont;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case LinkButton.ClickEvent:
					HandleEvent(LinkButton.MouseUpEvent);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override MouseEventArgs TriggerMouseUp(NSObject obj, IntPtr sel, NSEvent theEvent)
		{
			var args = base.TriggerMouseUp(obj, sel, theEvent);
			if (!args.Handled && Enabled && args.Buttons == MouseButtons.Primary)
			{
				Callback.OnClick(Widget, EventArgs.Empty);
			}
			return args;
		}

		static readonly object DisabledTextColorKey = new object();

		public Color DisabledTextColor
		{
			get { return Widget.Properties.Get<Color?>(DisabledTextColorKey) ?? SystemColors.DisabledText; }
			set
			{
				if (value != DisabledTextColor)
				{
					Widget.Properties[DisabledTextColorKey] = value;
					SetAttributes();
				}
			}
		}
	}
}
