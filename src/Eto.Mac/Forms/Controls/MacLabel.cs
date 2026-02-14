using Eto.Mac.Drawing;
namespace Eto.Mac.Forms.Controls
{

	public class EtoLabelFieldCell : NSTextFieldCell
	{
		public EtoLabelFieldCell()
		{
		}

		public EtoLabelFieldCell(IntPtr handle)
			: base(handle)
		{
		}

		/// <summary>
		/// Draws background color manually, using a layer or the base.BackgroundColor does not draw allow us to align the text properly.
		/// </summary>
		public NSColor BetterBackgroundColor { get; set; }

		[Export("verticalAlignment")]
		public VerticalAlignment VerticalAlignment { get; set; }

		public override CGRect DrawingRectForBounds(CGRect theRect)
		{
			var rect = base.DrawingRectForBounds(theRect);

			if (VerticalAlignment == VerticalAlignment.Top || VerticalAlignment == VerticalAlignment.Stretch)
				return rect;

			nfloat offset = 0;
			if (VerticalAlignment == VerticalAlignment.Center)
			{
				var lineHeight = CellSizeForBounds(theRect).Height;
				offset = (nfloat)Math.Round((theRect.Height - lineHeight) / 2.0F);
			}
			else if (VerticalAlignment == VerticalAlignment.Bottom)
			{
				var lineHeight = CellSizeForBounds(theRect).Height;
				offset = (nfloat)Math.Round(theRect.Height - lineHeight);
			}
			offset = (nfloat)Math.Max(0, offset);
			rect.Y += offset;
			rect.Height -= offset;
			return rect;
		}

		public override void DrawWithFrame(CGRect cellFrame, NSView inView)
		{
			if (BetterBackgroundColor != null)
			{
				BetterBackgroundColor.SetFill();
				NSGraphics.RectFill(cellFrame);
			}
			base.DrawWithFrame(cellFrame, inView);
		}
	}

	public class EtoLabel : NSTextField, IMacControl
	{
		public WeakReference WeakHandler { get; set; }

		public object Handler
		{
			get { return WeakHandler.Target; }
			set { WeakHandler = new WeakReference(value); }
		}

		public EtoLabel()
		{
			Cell = new EtoLabelFieldCell();
			DrawsBackground = false;
			Bordered = false;
			Bezeled = false;
			Editable = false;
			Selectable = false;
			Alignment = NSTextAlignment.Left;
		}
	}

	public abstract class MacLabel<TControl, TWidget, TCallback> : MacView<TControl, TWidget, TCallback>
		where TControl : NSTextField
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		readonly MacMnemonicString _str = new();
		Size _availableSizeCached;

		public override NSView ContainerControl => Control;

		protected override bool DefaultUseAlignmentFrame => true;

		protected override bool ControlEnabled
		{
			get => Control.Enabled;
			set
			{
				if (value == Enabled)
					return;

				Control.Enabled = value;
				_str.Enabled = value;
				SetAttributes();
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			// set attributes if it hasn't been loaded yet, so we get the correct size.
			if (!Widget.Loaded)
				SetAttributes(true);

			if (float.IsPositiveInfinity(availableSize.Width))
			{
				if (NaturalSizeInfinity != null)
					return NaturalSizeInfinity.Value;

				var width = UserPreferredSize.Width;
				if (width < 0) width = int.MaxValue;
				if (UseAlignmentFrame && width < int.MaxValue)
					width = (int)Control.GetFrameForAlignmentRect(new CGRect(0, 0, width, int.MaxValue)).Size.Width;
				var size = Control.Cell.CellSizeForBounds(new CGRect(0, 0, width, int.MaxValue));
				if (UseAlignmentFrame)
					size = Control.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, size)).Size;

				NaturalSizeInfinity = Size.Ceiling(size.ToEto());
				return NaturalSizeInfinity.Value;
			}

			if (Widget.Loaded && Wrap != WrapMode.None)
			{
				if (UserPreferredSize.Width > 0)
				{
					/*if (!float.IsPositiveInfinity(availableSize.Width))
						availableSize.Width = Math.Max(Size.Width, availableSize.Width);
					else*/
					availableSize.Width = UserPreferredSize.Width;
					availableSize.Height = float.PositiveInfinity;
				}
			}

			var availableSizeTruncated = availableSize.TruncateInfinity();
			if (NaturalSize == null || _availableSizeCached != availableSizeTruncated)
			{
				// var size = _str.AttributedString.BoundingRectWithSize(availableSizeTruncated.ToNS(), 0).Size;
				if (UseAlignmentFrame)
					availableSizeTruncated = Control.GetFrameForAlignmentRect(new CGRect(CGPoint.Empty, availableSizeTruncated.ToNS())).Size.ToEtoSize();
				var size = Control.Cell.CellSizeForBounds(new CGRect(CGPoint.Empty, availableSizeTruncated.ToNS()));
				if (UseAlignmentFrame)
					size = Control.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, size)).Size;

				NaturalSize = Size.Ceiling(size.ToEto());
				_availableSizeCached = availableSizeTruncated;
			}

			return NaturalSize.Value;
		}

		protected MacLabel()
		{
			_str.Wrap = WrapMode.Word;
		}

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent(Eto.Forms.Control.SizeChangedEvent);
		}

		protected override TControl CreateControl()
		{
			return new EtoLabel() as TControl;
		}

		public Color TextColor
		{
			get => _str.TextColor ?? SystemColors.ControlText;
			set
			{
				_str.TextColor = value;
				SetAttributes();
			}
		}

		protected override void SetBackgroundColor(Color? color)
		{
			var cell = Control.Cell as EtoLabelFieldCell;
			if (cell != null)
			{
				cell.BetterBackgroundColor = color?.ToNSUI();
				Control.NeedsDisplay = true;
			}
			else
				base.SetBackgroundColor(color);
		}

		public WrapMode Wrap
		{
			get => _str.Wrap;
			set
			{
				_str.Wrap = value;
				SetAttributes();
				InvalidateMeasure();
			}
		}

		public string Text
		{
			get => _str.Text;
			set
			{
				_str.Text = value;
				SetAttributes();
				InvalidateMeasure();
			}
		}

		public TextAlignment TextAlignment
		{
			get { return _str.Alignment; }
			set
			{
				_str.Alignment = value;
				SetAttributes();
				InvalidateMeasure();
			}
		}

		public virtual Font Font
		{
			get => _str.Font ??= new Font(new FontHandler(Control.Font));
			set
			{
				if (_str.Font != value)
				{
					_str.Font = value;
					SetAttributes();
					InvalidateMeasure();
				}
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get => (Control.Cell as EtoLabelFieldCell)?.VerticalAlignment ?? VerticalAlignment.Top;
			set
			{
				if (Control.Cell is EtoLabelFieldCell cell)
					cell.VerticalAlignment = value;
				Control.NeedsDisplay = true;
			}
		}

		protected virtual void SetAttributes() => SetAttributes(false);

		void SetAttributes(bool force)
		{
			if (Widget.Loaded || force)
			{
				// Control.StringValue = _str.Text ?? string.Empty;
				Control.AttributedStringValue = _str.AttributedString;
			}
		}

		public bool UseMnemonic
		{
			get => _str.UseMnemonic;
			set
			{
				_str.UseMnemonic = value;
				SetAttributes();
			}
		}

		public bool AlwaysShowMnemonic
		{
			get => _str.AlwaysShowMnemonic;
			set
			{
				_str.AlwaysShowMnemonic = value;
				SetAttributes();
			}
		}

		protected virtual NSColor CurrentColor
		{
			get
			{
				var col = _str.TextColor;
				if (col != null)
					return col.Value.ToNSUI();
				return null;
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetAttributes(true);
			InvalidateMeasure();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
