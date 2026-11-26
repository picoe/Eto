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
			public bool IsUpdatable { get { return Def?.Update != null; } }
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
			public Func<string, DateTimeFormatInfo, IEnumerable<string>> MaxWidth { get; set; }
		}

		static IEnumerable<string> Digits(int count) => new[] { new string('9', count) };

		static readonly SegmentDef[] defs =
		{
			new SegmentDef { Char = 'y', Update = (d,i) => d.AddYears(i), MaxWidth = (s, f) => Digits(Math.Max(2, s.Length)) },
			new SegmentDef { Char = 'M', Update = (d,i) => d.AddMonths(i), MaxWidth = (s, f) => s.Length == 3 ? f.AbbreviatedMonthNames : s.Length > 3 ? f.MonthNames : Digits(2) },
			new SegmentDef { Char = 'd', Update = (d,i) => d.AddDays(i), MaxWidth = (s, f) => s.Length == 3 ? f.AbbreviatedDayNames : s.Length > 3 ? f.DayNames : Digits(2) },
			new SegmentDef { Char = 'h', Update = (d,i) => d.AddHours(i), MaxWidth = (s, f) => Digits(2) },
			new SegmentDef { Char = 'H', Update = (d,i) => d.AddHours(i), MaxWidth = (s, f) => Digits(2) },
			new SegmentDef { Char = 'm', Update = (d,i) => d.AddMinutes(i), MaxWidth = (s, f) => Digits(2) },
			new SegmentDef { Char = 's', Update = (d,i) => d.AddSeconds(i), MaxWidth = (s, f) => Digits(2) },
			new SegmentDef { Char = 't', Update = (d,i) => d.AddHours(d.Hour >= 12 ? -12 : 12), MaxWidth = (s, f) => new[] { f.AMDesignator, f.PMDesignator } },
			// the era is shown but can't be stepped
			new SegmentDef { Char = 'g', MaxWidth = (s, f) => f.Calendar.Eras.Select(f.GetEraName) }
		};

		CultureInfo culture;

		/// <summary>
		/// Gets or sets the culture to show the date and time in, or null to use the current culture.
		/// </summary>
		public CultureInfo Culture
		{
			get => culture ?? CultureInfo.CurrentCulture;
			set
			{
				culture = value;
				UpdateSegments();
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the custom format, using the same format as <see cref="DateTime.ToString(string)"/>.
		/// </summary>
		public new string CustomFormat
		{
			get => base.CustomFormat;
			set
			{
				base.CustomFormat = value;
				UpdateSegments();
				Invalidate();
			}
		}

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
					return Culture.DateTimeFormat.LongDatePattern;
				case swf.DateTimePickerFormat.Time:
					return Culture.DateTimeFormat.ShortTimePattern;
				default:
				case swf.DateTimePickerFormat.Short:
					return Culture.DateTimeFormat.ShortDatePattern;
			}
		}

		/// <summary>
		/// Gets the width needed to show all of the text along with the check box and calendar button.
		/// </summary>
		public int PreferredWidth
		{
			get
			{
				if (segments.Count == 0)
					UpdateSegments();
				var textWidth = segments.Count > 0 ? segments[segments.Count - 1].End : 0;
				// left margin and calendar button, as laid out in OnPaint, plus a little room
				var width = 2 + textWidth + img.Width + 14 + 2;
				if (ShowCheckBox)
				{
					// measure against the screen, as CreateGraphics() would create the handle while laying out
					using (var g = sd.Graphics.FromHwnd(IntPtr.Zero))
					{
						var checkSize = swf.CheckBoxRenderer.GetGlyphSize(g, swf.VisualStyles.CheckBoxState.UncheckedNormal);
						var checkOffset = (ClientSize.Height - checkSize.Height) / 2;
						width += checkSize.Width + checkOffset + 1;
					}
				}
				return width;
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

			var formatInfo = Culture.DateTimeFormat;
			var pixelPos = 0;
			void AddStatic(string text)
			{
				if (string.IsNullOrEmpty(text))
					return;
				var strSize = swf.TextRenderer.MeasureText(text, Font, sd.Size.Empty, RenderTextFormat);
				segments.Add(new Segment { Start = pixelPos, Width = strSize.Width, StaticText = text });
				pixelPos += strSize.Width;
			}

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
					var token = format.Substring(pos, endPos - pos + 1);
					var width = def.MaxWidth(token, formatInfo)
						.Where(r => !string.IsNullOrEmpty(r))
						.Select(r => swf.TextRenderer.MeasureText(r, Font, sd.Size.Empty, RenderTextFormat).Width)
						.DefaultIfEmpty(0)
						.Max();
					segments.Add(new Segment { Start = pixelPos, Width = width, Format = token, Def = def });
					pixelPos += width;
					pos = endPos + 1;
				}
				else if (ch == '\'' || ch == '"')
				{
					// quoted literal text
					var endPos = format.IndexOf(ch, pos + 1);
					if (endPos < 0)
						endPos = format.Length;
					AddStatic(format.Substring(pos + 1, endPos - pos - 1));
					pos = endPos + 1;
				}
				else if (ch == '\\' && pos + 1 < format.Length)
				{
					AddStatic(format[pos + 1].ToString());
					pos += 2;
				}
				else
				{
					AddStatic(ch == '/' ? formatInfo.DateSeparator : ch == ':' ? formatInfo.TimeSeparator : ch.ToString());
					pos++;
				}
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
						// clicking can select a part that can't be stepped, such as a literal or the era
						if (seg.IsUpdatable)
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
					var s = Value.ToString(" " + segment.Format, Culture);
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
