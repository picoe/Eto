using Eto.Mac.Drawing;



namespace Eto.Mac.Forms.Controls
{
	public class GroupBoxHandler : MacPanel<NSBox, GroupBox, GroupBox.ICallback>, GroupBox.IHandler
	{
		SizeF? _borderSize;
		
		/// <summary>
		/// Use a separate class so it doesn't get event methods added
		/// </summary>
		public class EtoContentView : MacPanelView
		{
			
		}

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
				ContentView = new EtoContentView { Handler = handler };
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

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var borderSize = _borderSize ?? (_borderSize = CalculateBorderSize()) ?? SizeF.Empty;
			return base.GetNaturalSize(availableSize - borderSize) + borderSize;
		}

		SizeF CalculateBorderSize()
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
			return (frame.Size - contentSize).ToEto();
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
			_borderSize = null;
		}

		protected override bool DefaultUseNSBoxBackgroundColor => false;
	}
}
