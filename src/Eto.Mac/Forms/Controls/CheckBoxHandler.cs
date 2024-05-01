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
