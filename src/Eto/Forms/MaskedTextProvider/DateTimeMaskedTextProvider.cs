using System.Globalization;
using System.Text;

namespace Eto.Forms;

enum DateTimeSegmentPart
{
	Year,
	Month,
	Day,
	Hour,
	Minute,
	Second,
	Designator
}

readonly struct DateTimeSegment
{
	public int Start { get; }
	public int Length { get; }
	public DateTimeSegmentPart Part { get; }

	public DateTimeSegment(int start, int length, DateTimeSegmentPart part)
	{
		Start = start;
		Length = length;
		Part = part;
	}

	public bool Contains(int index) => index >= Start && index < Start + Length;
}

readonly struct DateTimePatternInfo
{
	public string Format { get; }
	public string ParseFormat { get; }
	public string Mask { get; }
	public DateTimeSegment[] Segments { get; }

	public DateTimePatternInfo(string format, string parseFormat, string mask, DateTimeSegment[] segments)
	{
		Format = format;
		ParseFormat = parseFormat;
		Mask = mask;
		Segments = segments;
	}
}

/// <summary>
/// Masked text provider that edits a <see cref="DateTime"/> value using a fixed, culture-aware date and/or time format.
/// </summary>
/// <remarks>
/// This provider encapsulates all of the date/time masking, parsing, clamping, and stepping logic so it can be used
/// with either a <see cref="MaskedTextStepper{T}"/> or a <see cref="MaskedTextBox{T}"/>.
///
/// For a stepper, the <see cref="GetStepValue"/> helper can be used from <see cref="TextStepper"/>'s step handling to
/// increment or decrement the segment at the caret. For a text box, simply assigning the provider gives masked,
/// culture-aware date entry without stepping.
///
/// The <see cref="DateTimeMaskedTextStepper"/> control wraps this provider with the appropriate 
/// control configuration.
/// </remarks>
public class DateTimeMaskedTextProvider : IMaskedTextProvider<DateTime?>
{
	FixedMaskedTextProvider inner;
	DateTime? value;
	DateTime minDate = DateTime.MinValue;
	DateTime maxDate = DateTime.MaxValue;
	DateTimePickerMode mode;
	DateTimePatternInfo patternInfo;
	char promptChar = '_';
	CultureInfo culture;

	/// <summary>
	/// Initializes a new instance of the <see cref="DateTimeMaskedTextProvider"/> class.
	/// </summary>
	/// <param name="mode">Date and/or time editing mode.</param>
	/// <param name="culture">Culture to use for the format and separators. Defaults to <see cref="CultureInfo.CurrentCulture"/>.</param>
	public DateTimeMaskedTextProvider(DateTimePickerMode mode = DateTimePickerMode.Date, CultureInfo culture = null)
	{
		this.mode = mode;
		this.culture = culture ?? CultureInfo.CurrentCulture;
		patternInfo = CreatePatternInfo(mode, this.culture);
		inner = CreateInner();
	}

	FixedMaskedTextProvider CreateInner()
	{
		return new FixedMaskedTextProvider(patternInfo.Mask, culture)
		{
			PromptChar = promptChar
		};
	}

	void RecreateProvider()
	{
		var current = value;
		inner = CreateInner();
		Value = current;
	}

	/// <summary>
	/// Gets or sets the culture used for the format and separators.
	/// </summary>
	public CultureInfo Culture
	{
		get => culture;
		set
		{
			culture = value ?? CultureInfo.CurrentCulture;
			patternInfo = CreatePatternInfo(mode, culture);
			RecreateProvider();
		}
	}

	/// <summary>
	/// Gets or sets the visible date/time editing mode.
	/// </summary>
	public DateTimePickerMode Mode
	{
		get => mode;
		set
		{
			if (mode == value)
				return;
			mode = value;
			patternInfo = CreatePatternInfo(mode, culture);
			RecreateProvider();
		}
	}

	/// <summary>
	/// Gets or sets the minimum allowed date/time value.
	/// </summary>
	public DateTime MinDate
	{
		get => minDate;
		set
		{
			minDate = value;
			if (maxDate < minDate)
				maxDate = minDate;
			if (this.value != null)
				Value = Clamp(this.value);
		}
	}

	/// <summary>
	/// Gets or sets the maximum allowed date/time value.
	/// </summary>
	public DateTime MaxDate
	{
		get => maxDate;
		set
		{
			maxDate = value;
			if (minDate > maxDate)
				minDate = maxDate;
			if (this.value != null)
				Value = Clamp(this.value);
		}
	}

	/// <summary>
	/// Gets or sets the prompt character used for unfilled edit positions.
	/// </summary>
	public char PromptChar
	{
		get => promptChar;
		set
		{
			if (promptChar == value)
				return;
			promptChar = value;
			RecreateProvider();
		}
	}

	/// <summary>
	/// Gets or sets the current value, parsing and clamping the masked text as needed.
	/// </summary>
	public DateTime? Value
	{
		get
		{
			var text = inner.Text;
			if (string.IsNullOrWhiteSpace(text) || inner.IsEmpty)
				return value = null;
			if (TryParse(text, out var parsed))
				return value = Clamp(parsed);
			return value;
		}
		set
		{
			var clamped = Clamp(value);
			this.value = clamped;
			inner.Text = ConvertToText(clamped);
		}
	}

	string ConvertToText(DateTime? value)
	{
		if (value == null)
			return string.Empty;
		return value.Value.ToString(patternInfo.Format, culture);
	}

	bool TryParse(string text, out DateTime result)
	{
		DateTime parsed = this.value ?? DateTime.Now;
		var didParse = false;
		var hasDesignator = patternInfo.Segments.Any(r => r.Part == DateTimeSegmentPart.Designator);
		int? hour12 = null;
		bool? isPm = null;
		foreach (var segment in patternInfo.Segments)
		{
			if (segment.Start + segment.Length > text.Length)
				break;
			var str = text.Substring(segment.Start, segment.Length);
			if (int.TryParse(str, NumberStyles.Integer, culture, out var intValue))
			{
				parsed = segment.Part switch
				{
					DateTimeSegmentPart.Year => new DateTime(intValue, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, parsed.Second),
					DateTimeSegmentPart.Month => new DateTime(parsed.Year, Math.Min(12, Math.Max(1, intValue)), parsed.Day, parsed.Hour, parsed.Minute, parsed.Second),
					DateTimeSegmentPart.Day => new DateTime(parsed.Year, parsed.Month, Math.Min(DateTime.DaysInMonth(parsed.Year, parsed.Month), Math.Max(1, intValue)), parsed.Hour, parsed.Minute, parsed.Second),
					DateTimeSegmentPart.Hour when hasDesignator => parsed,
					DateTimeSegmentPart.Hour => new DateTime(parsed.Year, parsed.Month, parsed.Day, Math.Min(23, Math.Max(0, intValue)), parsed.Minute, parsed.Second),
					DateTimeSegmentPart.Minute => new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, Math.Min(59, Math.Max(0, intValue)), parsed.Second),
					DateTimeSegmentPart.Second => new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, Math.Min(59, Math.Max(0, intValue))),
					_ => parsed
				};
				if (segment.Part == DateTimeSegmentPart.Hour && hasDesignator)
					hour12 = Math.Min(12, Math.Max(1, intValue));
				didParse = true;
			}
			else if (segment.Part == DateTimeSegmentPart.Designator && TryParseDesignator(str, culture, out var parsedIsPm))
			{
				isPm = parsedIsPm;
				didParse = true;
			}
		}

		if (!didParse)
		{
			result = default;
			return false;
		}

		if (hour12 != null)
		{
			var hour = hour12.Value % 12;
			if (isPm ?? parsed.Hour >= 12)
				hour += 12;
			parsed = new DateTime(parsed.Year, parsed.Month, parsed.Day, hour, parsed.Minute, parsed.Second);
		}

		switch (mode)
		{
			case DateTimePickerMode.Date:
			{
				var timeOfDay = value?.TimeOfDay ?? TimeSpan.Zero;
				result = parsed.Date + timeOfDay;
				break;
			}
			case DateTimePickerMode.Time:
			{
				var date = (value ?? DateTime.Today).Date;
				result = date + parsed.TimeOfDay;
				break;
			}
			default:
				result = parsed;
				break;
		}

		return true;
	}

	static bool TryParseDesignator(string text, CultureInfo culture, out bool isPm)
	{
		var designator = text.Trim();
		if (designator.Length > 0)
		{
			if (IsDesignatorMatch(designator, culture.DateTimeFormat.AMDesignator))
			{
				isPm = false;
				return true;
			}
			if (IsDesignatorMatch(designator, culture.DateTimeFormat.PMDesignator))
			{
				isPm = true;
				return true;
			}
		}
		isPm = false;
		return false;
	}

	static bool IsDesignatorMatch(string text, string designator)
	{
		if (string.IsNullOrEmpty(designator))
			return false;
		return designator.Equals(text, StringComparison.CurrentCultureIgnoreCase)
			|| designator.StartsWith(text, StringComparison.CurrentCultureIgnoreCase)
			|| text.StartsWith(designator, StringComparison.CurrentCultureIgnoreCase);
	}

	/// <summary>
	/// Clamps the specified value to the <see cref="MinDate"/> and <see cref="MaxDate"/> range.
	/// </summary>
	/// <param name="value">Value to clamp, or <c>null</c>.</param>
	/// <returns>The clamped value, or <c>null</c> if <paramref name="value"/> is <c>null</c>.</returns>
	public DateTime? Clamp(DateTime? value)
	{
		if (value == null)
			return null;
		if (value.Value < minDate)
			return minDate;
		if (value.Value > maxDate)
			return maxDate;
		return value;
	}

	/// <summary>
	/// Computes the value after stepping the segment at the specified caret index up or down.
	/// </summary>
	/// <remarks>
	/// This is intended to be called from a stepper's step handling. When there is no current value, the current
	/// date/time (clamped to the allowed range) is used as a starting point.
	/// </remarks>
	/// <param name="caretIndex">Caret index used to determine which segment to step.</param>
	/// <param name="delta">Amount to step, typically <c>1</c> for up or <c>-1</c> for down.</param>
	/// <param name="segmentStart">Outputs the start index of the segment that was stepped, for re-selecting the range.</param>
	/// <returns>The new value after stepping.</returns>
	public DateTime GetStepValue(int caretIndex, int delta, out int segmentStart)
	{
		var current = Value ?? Clamp(GetDefaultStepValue()) ?? GetDefaultStepValue();
		var segment = GetSegmentForCaret(caretIndex) ?? GetDefaultSegment();
		segmentStart = segment.Start;
		return ApplyStep(current, segment.Part, delta);
	}

	DateTime GetDefaultStepValue()
	{
		var current = DateTime.Now;
		return Clamp(current) ?? current;
	}

	DateTimeSegment GetDefaultSegment()
	{
		if (patternInfo.Segments.Length > 0)
			return patternInfo.Segments[0];
		return new DateTimeSegment(0, 0, mode == DateTimePickerMode.Time ? DateTimeSegmentPart.Hour : DateTimeSegmentPart.Day);
	}

	DateTimeSegment? GetSegmentForCaret(int caretIndex)
	{
		foreach (var segment in patternInfo.Segments)
		{
			if (segment.Contains(caretIndex) || caretIndex == segment.Start + segment.Length)
				return segment;
		}

		for (var i = patternInfo.Segments.Length - 1; i >= 0; i--)
		{
			var segment = patternInfo.Segments[i];
			if (caretIndex > segment.Start)
				return segment;
		}

		return null;
	}

	DateTime ApplyStep(DateTime current, DateTimeSegmentPart part, int delta)
	{
		return Clamp(part switch
		{
			DateTimeSegmentPart.Year => current.AddYears(delta),
			DateTimeSegmentPart.Month => current.AddMonths(delta),
			DateTimeSegmentPart.Day => current.AddDays(delta),
			DateTimeSegmentPart.Hour => current.AddHours(delta),
			DateTimeSegmentPart.Minute => current.AddMinutes(delta),
			DateTimeSegmentPart.Second => current.AddSeconds(delta),
			DateTimeSegmentPart.Designator => current.Hour >= 12 ? current.AddHours(-12) : current.AddHours(12),
			_ => current
		}) ?? current;
	}

	static DateTimePatternInfo CreatePatternInfo(DateTimePickerMode mode, CultureInfo culture)
	{
		var format = mode switch
		{
			DateTimePickerMode.Time => culture.DateTimeFormat.ShortTimePattern,
			DateTimePickerMode.DateTime => $"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}",
			_ => culture.DateTimeFormat.ShortDatePattern
		};

		// ICU-based cultures (e.g. en-US on macOS/Linux) separate the time from its AM/PM designator with a
		// narrow no-break space, which a mask can't contain - normalize the pattern the same way the mask is so
		// the text we format still lines up with the mask literals and can be set and parsed back.
		// See FixedMaskedTextProvider.NormalizeWhitespace for why the character can't be kept as-is.
		format = FixedMaskedTextProvider.NormalizeWhitespace(format);

		var formatBuilder = new StringBuilder();
		var parseBuilder = new StringBuilder();
		var maskBuilder = new StringBuilder();
		var segments = new List<DateTimeSegment>();

		for (var i = 0; i < format.Length; i++)
		{
			var ch = format[i];
			if (ch == '\'' || ch == '"')
			{
				var quote = ch;
				var start = ++i;
				while (i < format.Length && format[i] != quote)
					i++;
				AppendLiteral(formatBuilder, parseBuilder, maskBuilder, format.Substring(start, i - start));
				continue;
			}

			if (ch == '\\' && i + 1 < format.Length)
			{
				AppendLiteral(formatBuilder, parseBuilder, maskBuilder, format[++i].ToString());
				continue;
			}

			if (char.IsLetter(ch))
			{
				var tokenLength = 1;
				while (i + 1 < format.Length && format[i + 1] == ch)
				{
					tokenLength++;
					i++;
				}

				if (TryAppendToken(ch, tokenLength, culture, formatBuilder, parseBuilder, maskBuilder, segments))
					continue;

				AppendLiteral(formatBuilder, parseBuilder, maskBuilder, new string(ch, tokenLength));
				continue;
			}

			AppendLiteral(formatBuilder, parseBuilder, maskBuilder, ch.ToString());
		}

		return new DateTimePatternInfo(formatBuilder.ToString(), parseBuilder.ToString(), maskBuilder.ToString(), segments.ToArray());
	}

	static bool TryAppendToken(char formatChar, int tokenLength, CultureInfo culture, StringBuilder formatBuilder, StringBuilder parseBuilder, StringBuilder maskBuilder, List<DateTimeSegment> segments)
	{
		string normalizedToken;
		string parseToken;
		string maskToken;
		DateTimeSegmentPart? part;

		switch (formatChar)
		{
			case 'd':
				normalizedToken = "dd";
				maskToken = "00";
				parseToken = "d";
				part = DateTimeSegmentPart.Day;
				break;
			case 'M':
				normalizedToken = "MM";
				maskToken = "00";
				parseToken = "M";
				part = DateTimeSegmentPart.Month;
				break;
			case 'y':
				normalizedToken = tokenLength <= 2 ? "yy" : "yyyy";
				maskToken = new string('0', normalizedToken.Length);
				parseToken = tokenLength <= 2 ? "yy" : "yyyy";
				part = DateTimeSegmentPart.Year;
				break;
			case 'H':
				normalizedToken = "HH";
				maskToken = "00";
				parseToken = "H";
				part = DateTimeSegmentPart.Hour;
				break;
			case 'h':
				normalizedToken = "hh";
				maskToken = "00";
				parseToken = "h";
				part = DateTimeSegmentPart.Hour;
				break;
			case 'm':
				normalizedToken = "mm";
				maskToken = "00";
				parseToken = "m";
				part = DateTimeSegmentPart.Minute;
				break;
			case 's':
				normalizedToken = "ss";
				maskToken = "00";
				parseToken = "s";
				part = DateTimeSegmentPart.Second;
				break;
			case 't':
			{
				var designatorLength = Math.Max(1, Math.Max(culture.DateTimeFormat.AMDesignator?.Length ?? 0, culture.DateTimeFormat.PMDesignator?.Length ?? 0));
				normalizedToken = tokenLength == 1 ? "t" : "tt";
				maskToken = new string('L', designatorLength);
				parseToken = "t";
				part = DateTimeSegmentPart.Designator;
				break;
			}
			default:
				return false;
		}

		segments.Add(new DateTimeSegment(maskBuilder.Length, maskToken.Length, part.Value));
		formatBuilder.Append(normalizedToken);
		parseBuilder.Append(parseToken);
		maskBuilder.Append(maskToken);
		return true;
	}

	static void AppendLiteral(StringBuilder formatBuilder, StringBuilder parseBuilder, StringBuilder maskBuilder, string literal)
	{
		foreach (var ch in literal)
		{
			if (ch == '/')
			{
				formatBuilder.Append('/');
				parseBuilder.Append('/');
				maskBuilder.Append('/');
				continue;
			}

			if (ch == ':')
			{
				formatBuilder.Append(':');
				parseBuilder.Append(':');
				maskBuilder.Append(':');
				continue;
			}

			if (IsDateTimeFormatLiteralEscapeRequired(ch))
			{
				formatBuilder.Append('\\');
				parseBuilder.Append('\\');
			}
			formatBuilder.Append(ch);
			parseBuilder.Append(ch);

			if (IsMaskLiteralEscapeRequired(ch))
				maskBuilder.Append('\\');
			maskBuilder.Append(ch);
		}
	}

	static bool IsDateTimeFormatLiteralEscapeRequired(char ch)
	{
		return ch is 'd' or 'f' or 'F' or 'g' or 'h' or 'H' or 'K' or 'm' or 'M' or 's' or 't' or 'y' or 'z' or '%' or '\\' or '\'' or '"';
	}

	static bool IsMaskLiteralEscapeRequired(char ch)
	{
		return ch is '0' or '9' or '#' or 'L' or '?' or '&' or 'C' or 'A' or 'a' or '.' or ',' or '$' or '<' or '>' or '|' or '\\';
	}

	/// <inheritdoc/>
	public bool Insert(char character, ref int position) => inner.Insert(character, ref position);

	/// <inheritdoc/>
	public bool Replace(char character, ref int position) => inner.Replace(character, ref position);

	/// <inheritdoc/>
	public bool Delete(ref int position, int length, bool forward) => inner.Delete(ref position, length, forward);

	/// <inheritdoc/>
	public bool Clear(ref int position, int length, bool forward) => inner.Clear(ref position, length, forward);

	/// <inheritdoc/>
	public string DisplayText => inner.DisplayText;

	/// <inheritdoc/>
	public string Text
	{
		get => inner.Text;
		set => inner.Text = value;
	}

	/// <inheritdoc/>
	public bool MaskCompleted => inner.MaskCompleted;

	/// <inheritdoc/>
	public IEnumerable<int> EditPositions => inner.EditPositions;

	/// <inheritdoc/>
	public bool IsEmpty => inner.IsEmpty;

	/// <inheritdoc/>
	public void CommitText()
	{
		// re-parse and re-format the current text to normalize partially-entered values to a valid date/time
		Value = Value;
	}
}
