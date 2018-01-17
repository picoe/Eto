using System;
using System.Collections.Generic;
using System.Linq;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;

namespace Eto.WinForms.CustomControls
{
	/// <summary>
	/// A DateTimePicker with support for background colors
	/// </summary>
	/// <remarks>
	/// Since the default DateTimePicker is rendered completely by the OS, there's no way to change its behaviour or color.
	/// This is a crude re-implementation in winforms with support for most of the existing functionality.
	/// </remarks>
	[DesignerCategory("Code")]
	public class ExtendedDateTimePicker : swf.DateTimePicker
	{
		static readonly sd.Image img = sd.Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Eto.WinForms.CustomControls.CalendarPicker.png"));

		int selectedSegment = -1;

		const swf.TextFormatFlags RenderTextFormat = swf.TextFormatFlags.SingleLine
													 | swf.TextFormatFlags.NoPrefix
													 | swf.TextFormatFlags.TextBoxControl
													 | swf.TextFormatFlags.Right
													 | swf.TextFormatFlags.VerticalCenter
													 | swf.TextFormatFlags.NoPadding;

		class Segment
		{
			public SegmentDef Def { get; set; }
			public string Format { get; set; }
			public int Start { get; set; }
			public int End { get { return Start + Width; } }
			public string StaticText { get; set; }
			public bool IsUpdatable { get { return Def != null; } }
			public int Width { get; set; }
		}

		readonly List<Segment> segments = new List<Segment>();

		public bool ExtendedMode
		{
			get { return GetStyle(swf.ControlStyles.UserPaint); }
			set
			{
				SetStyle(swf.ControlStyles.UserPaint | swf.ControlStyles.OptimizedDoubleBuffer, value);
				UpdateSegments();
			}
		}

		class SegmentDef
		{
			public char Char { get; set; }
			public Func<DateTime, int, DateTime> Update { get; set; }
			public Func<string, string> MaxWidth { get; set; }
		}

		static readonly SegmentDef[] defs =
		{
			new SegmentDef { Char = 'y', Update = (d,i) => d.AddYears(i), MaxWidth = s => new string('9', s.Length) },
			new SegmentDef { Char = 'M', Update = (d,i) => d.AddMonths(i), MaxWidth = s => "99" },
			new SegmentDef { Char = 'd', Update = (d,i) => d.AddDays(i), MaxWidth = s => "99" },
			new SegmentDef { Char = 'h', Update = (d,i) => d.AddHours(i), MaxWidth = s => "99" },
			new SegmentDef { Char = 'm', Update = (d,i) => d.AddMinutes(i), MaxWidth = s => "99" },
			new SegmentDef { Char = 's', Update = (d,i) => d.AddSeconds(i), MaxWidth = s => "99" },
			new SegmentDef { Char = 't', Update = (d,i) => d.AddHours(d.Hour > 12 ? -12 : 12), MaxWidth = s => CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator }
		};

		protected override void OnFormatChanged(EventArgs e)
		{
			base.OnFormatChanged(e);
			UpdateSegments();
		}

		string GetFormat()
		{
			switch (Format)
			{
				case swf.DateTimePickerFormat.Custom:
					return CustomFormat;
				case swf.DateTimePickerFormat.Long:
					return CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern;
				case swf.DateTimePickerFormat.Time:
					return CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
				default:
				case swf.DateTimePickerFormat.Short:
					return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
			}
		}

		void UpdateSegments()
		{
			segments.Clear();
			if (!ExtendedMode)
				return;
			var format = GetFormat();
			if (format == null)
				return;

			var pixelPos = 0;
			var pos = 0;
			while (pos < format.Length)
			{
				var ch = format[pos];
				var def = defs.FirstOrDefault(r => r.Char == ch);
				if (def != null)
				{
					var endPos = pos;
					while (endPos < format.Length - 1 && format[endPos + 1] == ch)
						endPos++;
					var str = def.MaxWidth(format.Substring(pos, (endPos - pos + 1)));
					var strSize = swf.TextRenderer.MeasureText(str, Font, sd.Size.Empty, RenderTextFormat);
					var segment = new Segment
					{
						Start = pixelPos,
						Width = strSize.Width,
						Format = format.Substring(pos, endPos - pos + 1),
						Def = def
					};
					segments.Add(segment);
					pos = endPos;
					pixelPos += segment.Width;
				}
				else
				{
					var strSize = swf.TextRenderer.MeasureText(ch.ToString(CultureInfo.InvariantCulture), Font, sd.Size.Empty, RenderTextFormat);
					var segment = new Segment
					{
						Start = pixelPos,
						Width = strSize.Width,
						StaticText = ch.ToString(CultureInfo.InvariantCulture)
					};
					segments.Add(segment);
					pixelPos += segment.Width;
				}
				pos++;
			}

		}

		protected override void OnKeyDown(swf.KeyEventArgs e)
		{
			if (ExtendedMode)
			{
				var key = e.KeyData;
				if (key == swf.Keys.Left)
				{
					if (Checked)
					{
						do
						{
							selectedSegment = Math.Max(-1, selectedSegment - 1);
						} while (selectedSegment >= 0 && !segments[selectedSegment].IsUpdatable);
					}
					e.Handled = true;
				}
				else if (key == swf.Keys.Right)
				{
					if (Checked)
					{
						do
						{
							selectedSegment = Math.Min(segments.Count - 1, selectedSegment + 1);
						} while (selectedSegment < segments.Count - 1 && !segments[selectedSegment].IsUpdatable);
					}

					e.Handled = true;
				}
				else if (key == swf.Keys.Up || key == swf.Keys.Down)
				{
					if (selectedSegment >= 0 && selectedSegment < segments.Count)
					{
						var seg = segments[selectedSegment];
						Value = seg.Def.Update(Value, key == swf.Keys.Up ? 1 : -1);
					}

				}
				if (e.Handled)
				{
					Invalidate();
					return;
				}
			}
			base.OnKeyDown(e);
		}

		protected override void WndProc(ref swf.Message m)
		{
			if (ExtendedMode && m.Msg == (int)Win32.WM.LBUTTONDOWN)
			{
				var mouse = PointToClient(MousePosition);

				if (Enabled)
				{
					using (var g = CreateGraphics())
					{
						var offset = 2;
						if (ShowCheckBox)
						{
							var checkSize = swf.CheckBoxRenderer.GetGlyphSize(g, swf.VisualStyles.CheckBoxState.UncheckedNormal);
							var checkOffset = (ClientSize.Height - checkSize.Height) / 2;
							offset += checkOffset + checkSize.Width + 1;
						}

						if (Checked)
						{
							var test = mouse;
							test.X -= offset;
							for (int i = 0; i < segments.Count; i++)
							{
								var segment = segments[i];
								if (test.X >= segment.Start && test.X <= segment.End)
								{
									Focus();
									selectedSegment = i;
									Invalidate();
									return;
								}
							}
						}

						if (mouse.X > offset && mouse.X < ClientSize.Width - img.Width - 14)
						{
							Focus();
							return;
						}
					}
				}
			}
			base.WndProc(ref m);
		}

		protected override void OnValueChanged(EventArgs eventargs)
		{
			base.OnValueChanged(eventargs);
			if (!Checked)
				selectedSegment = -1;
			Invalidate();
		}

		protected override void OnPaint(swf.PaintEventArgs e)
		{
			if (segments.Count == 0)
				UpdateSegments();
			var g = e.Graphics;

			// colors
			var foreColor = Enabled && Checked ? ForeColor : sd.SystemColors.GrayText;
			var hoverBorderCol = sd.Color.FromArgb(unchecked((int)0xFF6CA3E4));
			var focusBorderCol = sd.Color.FromArgb(unchecked((int)0xFF4688DE));
			var borderCol = Focused ? focusBorderCol
				: hover ? hoverBorderCol
				: sd.Color.FromArgb(unchecked((int)0xFF9A9CA3));

			var rect = ClientRectangle;
			rect.Width -= 1;
			rect.Height -= 1;

			// background color
			using (var bgBrush = new sd.SolidBrush(BackColor))
			{
				g.FillRectangle(bgBrush, e.ClipRectangle);
			}

			// calculate text location
			var font = Font;
			var fontSize = swf.TextRenderer.MeasureText(g, "9/", font, new sd.Size(int.MaxValue, int.MaxValue), RenderTextFormat);

			int textOffset = (ClientSize.Height - fontSize.Height) / 2;
			var textRect = rect;
			textRect.X += 2;
			textRect.Y += textOffset;
			textRect.Width -= img.Width + 3 + 2;
			textRect.Height -= textOffset * 2;

			// check box
			if (ShowCheckBox)
			{
				var checkState = GetCheckBoxState();
				var checkSize = swf.CheckBoxRenderer.GetGlyphSize(g, checkState);
				var checkOffset = (ClientSize.Height - checkSize.Height) / 2;
				swf.CheckBoxRenderer.DrawCheckBox(g, new sd.Point(checkOffset, (ClientRectangle.Height - checkSize.Height) / 2), checkState);

				// adjust text location
				textRect.X += checkSize.Width + checkOffset + 1;
				textRect.Width -= checkSize.Width + checkOffset + 1;
			}

			// text segments
			int currentSegment = 0;
			foreach (var segment in segments)
			{
				string str;
				var textColor = foreColor;
				if (segment.Def != null)
				{
					var s = Value.ToString(" " + segment.Format);
					str = s.Substring(1);
					if (Focused && currentSegment == selectedSegment)
					{
						using (var highlightBgBrush = new sd.SolidBrush(sd.SystemColors.Highlight))
						{
							g.FillRectangle(highlightBgBrush, new sd.RectangleF(textRect.X + segment.Start, textRect.Y, segment.Width, textRect.Height));
						}
						textColor = sd.SystemColors.HighlightText;
					}
				}
				else
					str = segment.StaticText;

				swf.TextRenderer.DrawText(g, str, font, new sd.Rectangle((int)(textRect.X + segment.Start), textRect.Y, segment.Width, textRect.Height), textColor, RenderTextFormat);
				currentSegment++;
			}

			// calendar button
			if (hover || calendarOpen)
			{
				var buttonRect = new sd.Rectangle(ClientRectangle.Width - img.Width - 14, 0, img.Width + 14, ClientRectangle.Height);
				if (calendarOpen || buttonRect.Contains(PointToClient(MousePosition)))
				{
					var startColor = sd.Color.FromArgb(unchecked((int)0xFFE7F1FB));
					var endColor = sd.Color.FromArgb(unchecked((int)0xFFD4E6FB));
					var gradient = new sd.Drawing2D.LinearGradientBrush(buttonRect, startColor, endColor, 90f);
					g.FillRectangle(gradient, buttonRect);
					buttonRect.Width -= 1;
					buttonRect.Height -= 1;
					var buttonBorderCol = calendarOpen ? focusBorderCol : hoverBorderCol;
					using (var p = new sd.Pen(buttonBorderCol))
					{
						g.DrawRectangle(p, buttonRect);
					}
				}
			}
			g.DrawImage(img, ClientRectangle.Width - img.Width - 7, (ClientRectangle.Height - img.Height) / 2 + 1);

			if (ShowBorder)
			{
				// border
				using (var p = new sd.Pen(borderCol))
				{
					g.DrawRectangle(p, rect);
				}
			}
		}

		private swf.VisualStyles.CheckBoxState GetCheckBoxState()
		{
			if (Enabled)
			{
				if (Checked)
				{
					if (Focused && selectedSegment == -1)
						return swf.VisualStyles.CheckBoxState.CheckedHot;
					return swf.VisualStyles.CheckBoxState.CheckedNormal;
				}
				if (Focused && selectedSegment == -1)
					return swf.VisualStyles.CheckBoxState.UncheckedHot;
				return swf.VisualStyles.CheckBoxState.UncheckedNormal;
			}
			if (Checked)
				return swf.VisualStyles.CheckBoxState.CheckedDisabled;
			return swf.VisualStyles.CheckBoxState.UncheckedDisabled;
		}

		bool calendarOpen;
		protected override void OnDropDown(EventArgs eventargs)
		{
			base.OnDropDown(eventargs);
			calendarOpen = true;
		}

		protected override void OnCloseUp(EventArgs eventargs)
		{
			base.OnCloseUp(eventargs);
			calendarOpen = false;
		}

		[Browsable(true)]
		public override sd.Color BackColor
		{
			get { return base.BackColor; }
			set
			{
				base.BackColor = value;
				ExtendedMode = true;
			}
		}

		bool showBorder = true;
		public bool ShowBorder
		{
			get { return showBorder; }
			set
			{
				if (showBorder != value)
				{
					showBorder = value;
					Invalidate();
				}
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			UpdateSegments();
		}

		bool hover;
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			hover = true;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			hover = false;
			Invalidate();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Invalidate();
		}
	}
}
