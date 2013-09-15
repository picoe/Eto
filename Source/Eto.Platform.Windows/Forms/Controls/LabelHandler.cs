using System;
using System.Reflection;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public class LabelHandler : WindowsControl<LabelHandler.MyLabel, Label>, ILabel
	{
		public class MyLabel : swf.Label
		{
			public LabelHandler Handler { get; set; }

			public WrapMode Wrap { get; set; }

			public HorizontalAlign HorizontalAlign { get; set; }

			public Eto.Forms.VerticalAlign VerticalAlign { get; set; }

			public MyLabel()
			{
				Wrap = WrapMode.Word;
			}

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				using (var format = CreateStringFormat())
				using (var g = sd.Graphics.FromHwnd(this.Handle))
				{
					var bordersAndPadding = this.Margin.Size; // this.SizeFromClientSize (SD.Size.Empty);
					proposedSize -= bordersAndPadding;
					proposedSize.Height = Math.Max(0, proposedSize.Height);
					if (proposedSize.Width <= 1)
						proposedSize.Width = int.MaxValue;

					var size = g.MeasureString(this.Text, this.Font, proposedSize.Width, format);

					size += bordersAndPadding;
					if (size.Width < MinimumSize.Width)
						size.Width = MinimumSize.Width;
					if (size.Height < MinimumSize.Height)
						size.Height = MinimumSize.Height;
					return sd.Size.Ceiling(size);
				}
			}

			sd.StringFormat CreateStringFormat()
			{
				var format = new sd.StringFormat();
				format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
				switch (this.Wrap)
				{
					case WrapMode.None:
						format.Trimming = System.Drawing.StringTrimming.None;
						format.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;
						break;
					case WrapMode.Word:
						format.Trimming = System.Drawing.StringTrimming.Word;
						format.FormatFlags = 0;
						break;
					case WrapMode.Character:
						format.Trimming = System.Drawing.StringTrimming.Character;
						format.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;
						break;
				}
				return format;
			}

			protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
			{
				using (var format = CreateStringFormat())
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
							e.Graphics.MeasureString(text, font, this.Bounds.Size, format, out charactersFitted, out linesFilled);

							e.Graphics.DrawString(text.Substring(0, charactersFitted), font, b, drawPoint, format);

							if (charactersFitted >= text.Length) break;
							text = text.Substring(charactersFitted);

							drawPoint.Y += height;
						}
					}
					else
					{
						var rect = new sd.RectangleF(Margin.Left, Margin.Top, this.Bounds.Width - Margin.Horizontal, this.Bounds.Height - Margin.Vertical);
						var size = e.Graphics.MeasureString(this.Text, this.Font, (int)rect.Width, format);

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

						switch (this.HorizontalAlign)
						{
							case HorizontalAlign.Right:
								format.Alignment = System.Drawing.StringAlignment.Far;
								break;
							case HorizontalAlign.Center:
								format.Alignment = System.Drawing.StringAlignment.Center;
								break;
						}

						e.Graphics.DrawString(this.Text, this.Font, b, rect, format);
					}
				}
			}
		}

		public LabelHandler()
		{
			Control = new MyLabel
			{
				Handler = this,
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
