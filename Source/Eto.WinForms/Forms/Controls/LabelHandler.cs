using System;
using System.IO;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class LabelHandler : WindowsControl<LabelHandler.EtoLabel, Label, Label.ICallback>, Label.IHandler
	{
		public class EtoLabel : swf.Label
		{
			WrapMode wrapMode;
			HorizontalAlign horizontalAlign;
			sd.SizeF? measuredSize;
			sd.Size proposedSizeCache;
			sd.SizeF? measuredSizeMax;
			VerticalAlign verticalAlign;
			swf.TextFormatFlags textFormat = swf.TextFormatFlags.Default;

			void ClearSize()
			{
				measuredSize = measuredSizeMax = null;
			}

			public override sd.Font Font
			{
				get { return base.Font; }
				set
				{
					ClearSize();
					base.Font = value;
				}
			}

			public override string Text
			{
				get { return base.Text; }
				set
				{
					ClearSize();
					base.Text = value;
				}
			}

			public WrapMode Wrap
			{
				get { return wrapMode; }
				set
				{
					wrapMode = value;
					SetStringFormat();
					ClearSize();
				}
			}

			public HorizontalAlign HorizontalAlign
			{
				get { return horizontalAlign; }
				set
				{
					horizontalAlign = value;
					SetStringFormat();
					ClearSize();
				}
			}

			public VerticalAlign VerticalAlign
			{
				get { return verticalAlign; }
				set
				{
					verticalAlign = value;
					SetStringFormat();
					ClearSize();
				}
			}

			public EtoLabel()
			{
				Wrap = WrapMode.Word;
			}

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var bordersAndPadding = Margin.Size; // this.SizeFromClientSize (SD.Size.Empty);
				if (proposedSize.Width <= 1)
					proposedSize.Width = int.MaxValue;

				if (proposedSize.Width == int.MaxValue)
				{
					if (measuredSizeMax == null && string.IsNullOrEmpty(Text))
					{
						var emptySize = swf.TextRenderer.MeasureText(" ", Font, new sd.Size(proposedSize.Width, int.MaxValue), textFormat);
						measuredSizeMax = new sd.SizeF(0, emptySize.Height);
					}
					else if (measuredSizeMax == null)
					{
						proposedSize -= bordersAndPadding;
						proposedSize.Height = Math.Max(0, proposedSize.Height);
						measuredSizeMax = swf.TextRenderer.MeasureText(Text, Font, new sd.Size(proposedSize.Width, int.MaxValue), textFormat);
					}
					measuredSize = measuredSizeMax;
				}
				else if (measuredSize == null || proposedSizeCache != proposedSize)
				{
					proposedSizeCache = proposedSize;
					proposedSize -= bordersAndPadding;
					proposedSize.Height = Math.Max(0, proposedSize.Height);
					measuredSize = swf.TextRenderer.MeasureText(Text, Font, new sd.Size(proposedSize.Width, int.MaxValue), textFormat);
				}
				var size = measuredSize.Value;
				size += bordersAndPadding;
				if (size.Width < MinimumSize.Width)
					size.Width = MinimumSize.Width;
				if (size.Height < MinimumSize.Height)
					size.Height = MinimumSize.Height;
				return sd.Size.Ceiling(size);
			}

			void SetStringFormat()
			{
				textFormat = swf.TextFormatFlags.Default;
				switch (Wrap)
				{
					case WrapMode.None:
						textFormat |= swf.TextFormatFlags.SingleLine;
						break;
					case WrapMode.Word:
						textFormat |= swf.TextFormatFlags.WordBreak;
						break;
					case WrapMode.Character:
						break;
				}
				switch (HorizontalAlign)
				{
					case HorizontalAlign.Left:
						textFormat |= swf.TextFormatFlags.Left;
						break;
					case HorizontalAlign.Right:
						textFormat |= swf.TextFormatFlags.Right;
						break;
					case HorizontalAlign.Center:
						textFormat |= swf.TextFormatFlags.HorizontalCenter;
						break;
				}
				switch (VerticalAlign)
				{
					case VerticalAlign.Top:
						textFormat |= swf.TextFormatFlags.Top;
						break;
					case VerticalAlign.Bottom:
						textFormat |= swf.TextFormatFlags.Bottom;
						break;
					case VerticalAlign.Middle:
						textFormat |= swf.TextFormatFlags.VerticalCenter;
						break;
				}

			}

			protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
			{
				var rect = new sd.Rectangle(Margin.Left, Margin.Top, Bounds.Width - Margin.Horizontal, Bounds.Height - Margin.Vertical);
				swf.TextRenderer.DrawText(e.Graphics, Text, Font, rect, ForeColor, BackColor, textFormat);
			}
		}

		public LabelHandler()
		{
			Control = new EtoLabel
			{
				AutoSize = true
			};
		}

		public override Color TextColor
		{
			get { return Control.ForeColor.ToEto(); }
			set { Control.ForeColor = value.ToSD(); }
		}

		public HorizontalAlign HorizontalAlign
		{
			get { return Control.HorizontalAlign; }
			set
			{
				if (Control.HorizontalAlign != value)
				{
					Control.HorizontalAlign = value;
					Control.Invalidate();
				}
				/*if (Control.Dock != SWF.DockStyle.None) {
					Control.Dock = DockStyle;
				}*/
			}
		}

		public WrapMode Wrap
		{
			get { return Control.Wrap; }
			set
			{
				if (value != Control.Wrap)
				{
					Control.Wrap = value;
					Control.Invalidate();
				}
			}
		}

		public VerticalAlign VerticalAlign
		{
			get { return Control.VerticalAlign; }
			set
			{
				if (Control.VerticalAlign != value)
				{
					Control.VerticalAlign = value;
					Control.Invalidate();
				}
			}
		}

		public override void SetFilledContent()
		{
			base.SetFilledContent();
			Control.AutoSize = false;
		}
	}
}
