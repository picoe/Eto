namespace Eto.Mac.Forms.Controls
{

	public abstract class EtoCenteredButtonCell : NSButtonCell
	{
		nfloat defaultHeight;

		protected abstract nfloat GetDefaultHeight();

		protected virtual nfloat Offset => 0f;

		public override void SetButtonType(NSButtonType aType)
		{
			base.SetButtonType(aType);
			defaultHeight = GetDefaultHeight();
		}

		public override NSControlSize ControlSize
		{
			get => base.ControlSize;
			set
			{
				base.ControlSize = value;
				defaultHeight = GetDefaultHeight();
			}
		}

		nfloat GetButtonOffset(CGRect rect)
		{
			// tahoe centers the button in ImageRectForBounds instead, see GetCenteringOffset
			if (MacVersion.IsAtLeast(26, 0))
				return 0;

			var titleSize = AttributedTitle.Size;
			// big sur and later calculate the position a wee differently..
			if (MacVersion.IsAtLeast(10, 16))
			{
				if (AttributedTitle.Length > 0)
					return (nfloat)Math.Ceiling((rect.Height - titleSize.Height) / 2);
				else
					return 0;
			}

			// catalina and older
			return (nfloat)Math.Max(0, Math.Ceiling((titleSize.Height - defaultHeight) / 2 - 1)) + Offset;
		}
		

		/// <summary>
		/// How far the button has to move to sit in the middle of the text on tahoe, which aligns it
		/// with the bottom of the text instead and ignores the DrawingRectForBounds shift used on older versions.
		/// Applies to the compatibility appearance too, which lays the button out the same way.
		/// </summary>
		nfloat GetCenteringOffset(CGRect bounds)
		{
			if (!MacVersion.IsAtLeast(26, 0) || AttributedTitle.Length == 0)
				return 0;
			var imageRect = base.ImageRectForBounds(bounds);
			var titleRect = base.TitleRectForBounds(bounds);
			if (imageRect.Height <= 0 || titleRect.Height <= 0)
				return 0;
			return (nfloat)Math.Round(titleRect.GetMidY() - imageRect.GetMidY());
		}

		bool suppressCenteringOffset;

		public override CGRect ImageRectForBounds(CGRect theRect)
		{
			var rect = base.ImageRectForBounds(theRect);
			if (!suppressCenteringOffset)
				rect.Y += GetCenteringOffset(theRect);
			return rect;
		}

		public override void DrawFocusRing(CGRect cellFrame, NSView inView)
		{
			var offset = GetCenteringOffset(cellFrame);
			if (offset == 0)
			{
				base.DrawFocusRing(cellFrame, inView);
				return;
			}

			// the ring follows ImageRectForBounds in the compatibility appearance but not under glass,
			// so draw it unshifted either way and move it along with the button ourselves.
			var context = NSGraphicsContext.CurrentContext;
			context.SaveGraphicsState();
			var transform = new NSAffineTransform();
			transform.Translate(0, offset);
			transform.Concat();
			suppressCenteringOffset = true;
			try
			{
				base.DrawFocusRing(cellFrame, inView);
			}
			finally
			{
				suppressCenteringOffset = false;
				context.RestoreGraphicsState();
			}
		}

		public override CGRect DrawingRectForBounds(CGRect theRect)
		{
			var rect = base.DrawingRectForBounds(theRect);
			// adjust drawing offset so the button goes in the right spot
			rect.Y += GetButtonOffset(theRect);
			return rect;
		}

		public override CGRect TitleRectForBounds(CGRect theRect)
		{
			var rect = base.TitleRectForBounds(theRect);
			// adjust text offset so it goes back to where it should be
			rect.Y -= GetButtonOffset(theRect);
			return rect;
		}
	}

	public class CheckBoxHandler : MacButton<NSButton, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		public class EtoCheckCenteredButtonCell : EtoCenteredButtonCell
		{
			// check boxes get clipped at the top in mini mode using the alignment rects. macOS 10.14.6
			// see Eto.Test.Mac.UnitTests.CheckBoxTests.ButtonShouldNotBeClipped()
			protected override nfloat Offset => ControlSize == NSControlSize.Mini ? 0.5f : 0;

			protected override nfloat GetDefaultHeight()
			{
				switch (ControlSize)
				{
					default:
					case NSControlSize.Regular:
						return 14;
					case NSControlSize.Small:
						return 12;
					case NSControlSize.Mini:
						return 10;
				}
			}

			public override void DrawWithFrame(CGRect cellFrame, NSView inView)
			{
				if (NSGraphicsContext.IsCurrentContextDrawingToScreen)
				{
					base.DrawWithFrame(cellFrame, inView);
				}
				else
				{
					DrawTitle(AttributedTitle, TitleRectForBounds(cellFrame), inView);
					var state = State;
					var text = state == NSCellStateValue.On ? "☑" : state == NSCellStateValue.Mixed ? "-" : "☐";
					var font = NSFont.SystemFontOfSize(NSFont.SystemFontSizeForControlSize(ControlSize));
					var attributes = NSDictionary.FromObjectAndKey(font, NSStringAttributeKey.Font);
					var str = new NSAttributedString(text, attributes);
					var frame = cellFrame;
					var size = str.Size;
					var offset = (nfloat)Math.Max(0, (frame.Height - size.Height) / 2);
					frame.Y += offset;
					frame.Height -= offset;
					str.DrawString(frame);
				}
			}
		}

		public class EtoCheckBoxButton : NSButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			EtoCheckCenteredButtonCell cell;

			public EtoCheckBoxButton()
			{
				Cell = cell = new EtoCheckCenteredButtonCell();
				Title = string.Empty;
				SetButtonType(NSButtonType.Switch);
			}
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSButton CreateControl() => new EtoCheckBoxButton();

		protected override void Initialize()
		{
			Control.Activated += HandleActivated;

			base.Initialize();
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as CheckBoxHandler;
			handler.TriggerMouseCallback();
			handler.Callback.OnCheckedChanged(handler.Widget, EventArgs.Empty);
		}

		public bool? Checked
		{
			get
			{ 
				switch (Control.State)
				{
					case NSCellStateValue.On:
						return true;
					case NSCellStateValue.Off:
						return false;
					default:
						return null;
				}
			}
			set
			{ 
				if (Checked != value)
				{
					if (value == null)
						Control.State = ThreeState ? NSCellStateValue.Mixed : NSCellStateValue.Off;
					else if (value.Value)
						Control.State = NSCellStateValue.On;
					else
						Control.State = NSCellStateValue.Off;
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public bool ThreeState
		{
			get { return Control.AllowsMixedState; }
			set { Control.AllowsMixedState = value; }
		}

	}
}
