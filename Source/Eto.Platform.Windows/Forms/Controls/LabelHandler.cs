using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public class LabelHandler : WindowsControl<LabelHandler.MyLabel, Label>, ILabel
	{
		public class MyLabel : swf.Label
		{
			sd.StringFormat stringFormat;
			WrapMode wrapMode;
			HorizontalAlign horizontalAlign;
			sd.SizeF? measuredSize;
			sd.Size proposedSizeCache;
			VerticalAlign verticalAlign;

			public override sd.Font Font
			{
				get { return base.Font; }
				set
				{
					measuredSize = null;
					base.Font = value;
				}
			}

			public override string Text
			{
				get { return base.Text; }
				set
				{
					measuredSize = null;
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
					measuredSize = null;
				}
			}

			public HorizontalAlign HorizontalAlign
			{
				get { return horizontalAlign; }
				set
				{
					horizontalAlign = value;
					SetStringFormat();
					measuredSize = null;
				}
			}

			public VerticalAlign VerticalAlign
			{
				get { return verticalAlign; }
				set
				{
					verticalAlign = value;
					measuredSize = null;
				}
			}

			public MyLabel()
			{
				stringFormat = new sd.StringFormat();
				Wrap = WrapMode.Word;
			}

			static sd.Graphics graphics = sd.Graphics.FromHwnd(IntPtr.Zero);

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var bordersAndPadding = this.Margin.Size; // this.SizeFromClientSize (SD.Size.Empty);
				if (measuredSize == null || proposedSizeCache != proposedSize)
				{
					proposedSize -= bordersAndPadding;
					proposedSize.Height = Math.Max(0, proposedSize.Height);
					if (proposedSize.Width <= 1)
						proposedSize.Width = int.MaxValue;
					measuredSize = graphics.MeasureString(this.Text, this.Font, proposedSize.Width, stringFormat);
					proposedSizeCache = proposedSize;
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
				stringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
				switch (this.Wrap)
				{
					case WrapMode.None:
						stringFormat.Trimming = System.Drawing.StringTrimming.None;
						stringFormat.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;
						break;
					case WrapMode.Word:
						stringFormat.Trimming = System.Drawing.StringTrimming.Word;
						stringFormat.FormatFlags = 0;
						break;
					case WrapMode.Character:
						stringFormat.Trimming = System.Drawing.StringTrimming.Character;
						stringFormat.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;
						break;
				}
				switch (this.HorizontalAlign)
				{
					case HorizontalAlign.Right:
						stringFormat.Alignment = System.Drawing.StringAlignment.Far;
						break;
					case HorizontalAlign.Center:
						stringFormat.Alignment = System.Drawing.StringAlignment.Center;
						break;
				}

			}

			protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
			{
				using (var b = new sd.SolidBrush(this.ForeColor))
				{
					if (this.Wrap == WrapMode.Character)
					{
						// draw string one line at a time to trim to character..
						int charactersFitted, linesFilled;
						string text = this.Text;
						sd.PointF drawPoint = sd.PointF.Empty;
						var font = this.Font;
						var height = font.GetHeight(e.Graphics);
						while (!string.IsNullOrEmpty(text))
						{
							e.Graphics.MeasureString(text, font, this.Bounds.Size, stringFormat, out charactersFitted, out linesFilled);

							e.Graphics.DrawString(text.Substring(0, charactersFitted), font, b, drawPoint, stringFormat);

							if (charactersFitted >= text.Length) break;
							text = text.Substring(charactersFitted);

							drawPoint.Y += height;
						}
					}
					else
					{
						var rect = new sd.RectangleF(Margin.Left, Margin.Top, this.Bounds.Width - Margin.Horizontal, this.Bounds.Height - Margin.Vertical);
						var size = e.Graphics.MeasureString(this.Text, this.Font, (int)rect.Width, stringFormat);

						if (size.Height < rect.Height)
						{
							switch (this.VerticalAlign)
							{
								case Eto.Forms.VerticalAlign.Bottom:
									rect.Y += rect.Height - size.Height;
									rect.Height = size.Height;
									break;
								case Eto.Forms.VerticalAlign.Middle:
									rect.Y += (rect.Height - size.Height) / 2;
									rect.Height = size.Height;
									break;
							}
						}

						if (size.Width < rect.Width)
						{
							switch (this.HorizontalAlign)
							{
								case HorizontalAlign.Right:
									rect.X = rect.Width - size.Width - Margin.Top;
									rect.Width = size.Width;
									break;
								case HorizontalAlign.Center:
									rect.X = (rect.Width - size.Width) / 2 - Margin.Top;
									rect.Width = size.Width;
									break;
							}
						}

						e.Graphics.DrawString(this.Text, this.Font, b, rect, stringFormat);
					}
				}
			}
		}

		public LabelHandler()
		{
			Control = new MyLabel
			{
				AutoSize = true
			};
		}

		public Color TextColor
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

		public Eto.Forms.VerticalAlign VerticalAlign
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

	}
}
