using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using Eto.Wpf;
using System.Globalization;
using System.Diagnostics;
using Microsoft.VisualStudio.Text.Tagging;
using System.Linq;

namespace Eto.Addin.VisualStudio.Adornments
{
	public class ColorAdornment
	{
		IAdornmentLayer layer;
		IWpfTextView view;
		bool? hasUsing;
		Span usingSpan;
		const string colorRegexPattern = @"
(
  (
    (new\s+(?<!\.|\w)\s*{0}(?<mode>ColorCMYK|ColorHSB|ColorHSL|Color))
    |((?<!\.|\w)(?<=\s*){0}Color\s*\.\s*(?<mode>FromArgb|FromRgb|FromGrayscale|FromElementId))
  )\s*
  \(\s*
	(
		(unchecked\s*\(\s*\(\s*int\s*\)\s*(?<val>0x[A-Fa-f0-9]+)\s*\))
		|
		(?<val>(0x[A-Fa-f0-9]+)|[\-\+]?\s*[\d\.]+\s*f?)
		(\s*,\s*(?<val>(0x[A-Fa-f0-9]+)|[\-\+]?\s*[\d\.]+\s*f?)){{0,4}}
	)
	\s*
  \)
)
|
((?<!\.|\w)(?<=\s*){0}(?<mode>Colors)[.](?<name>[a-zA-Z]+))
";
		const string namespacePrefix = @"Eto\s*\.\s*Drawing\s*\.\s*";

		const string usingPattern = @"using\s*Eto\s*\.\s*Drawing\s*;";

		Regex usingRegex = new Regex(usingPattern, RegexOptions.Compiled);

		const RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;
		Regex colorRegex = new Regex(string.Format(colorRegexPattern, namespacePrefix), options);
		Regex usingColorRegex = new Regex(string.Format(colorRegexPattern, "(" + namespacePrefix + ")?"), options);
		Dictionary<Color, Pen> pens = new Dictionary<Color, Pen>();

		Dictionary<string, Func<Match, Eto.Drawing.Color>> colorMatching = new Dictionary<string, Func<Match, Eto.Drawing.Color>>
		{
			{ "Color", m => new Eto.Drawing.Color(GetFloat(m, 0), GetFloat(m, 1), GetFloat(m, 2), GetFloat(m, 3, 1f))},
			{ "ColorHSB", m => new Eto.Drawing.ColorHSB(GetFloat(m, 0), GetFloat(m, 1), GetFloat(m, 2), GetFloat(m, 3, 1f))},
			{ "ColorHSL", m => new Eto.Drawing.ColorHSL(GetFloat(m, 0), GetFloat(m, 1), GetFloat(m, 2), GetFloat(m, 3, 1f))},
			{ "ColorCMYK", m => new Eto.Drawing.ColorCMYK(GetFloat(m, 0), GetFloat(m, 1), GetFloat(m, 2), GetFloat(m, 3), GetFloat(m, 4, 1f))},
			{ "FromRgb", m => Eto.Drawing.Color.FromRgb(GetInt(m, 0)) },
			{ "FromArgb", m => 
				{
					if (m.Groups["val"].Captures.Count == 1)
						return Eto.Drawing.Color.FromArgb(GetInt(m, 0));
					return Eto.Drawing.Color.FromArgb(GetInt(m, 0), GetInt(m, 1), GetInt(m, 2), GetInt(m, 3, byte.MaxValue));
				}
			},
			{ "FromGrayscale", m => Eto.Drawing.Color.FromGrayscale(GetFloat(m, 0), GetFloat(m, 1, 1f)) },
			{ "FromElementId", m => Eto.Drawing.Color.FromElementId(GetInt(m, 0), GetInt(m, 1, byte.MaxValue)) },
			{ "Colors", m => 
				{
					var type = typeof(Eto.Drawing.Colors);
					var property = type.GetProperty(m.Groups["name"].Value);
					if (property != null)
						return (Eto.Drawing.Color)property.GetValue(null);
					return Eto.Drawing.Colors.Transparent;
				}
			},
		};

		public ColorAdornment(IWpfTextView view)
		{
			this.view = view;
			layer = view.GetAdornmentLayer("Eto.ColorAdornment");

			//Listen to any event that changes the layout (text changes, scrolling, etc)
			view.LayoutChanged += OnLayoutChanged;
		}

		Pen GetPen(Color color)
		{
			Pen pen;
			if (pens.TryGetValue(color, out pen))
				return pen;

			var penBrush = new SolidColorBrush(color);
			penBrush.Freeze();
			pen = new Pen(penBrush, 2);
			pen.Freeze();
			pens.Add(color, pen);
			return pen;
		}

		void UpdateUsing(string text = null)
		{
			if (text == null)
				hasUsing = false;
			var match = usingRegex.Match(text ?? view.TextSnapshot.GetText());
			if (match.Success)
			{
				hasUsing = match.Success;
				if (match.Success)
					usingSpan = new Span(match.Index, match.Length);
			}
		}

		/// <summary>
		/// On layout change add the adornment to any reformatted lines
		/// </summary>
		void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
		{
			if (hasUsing == null)
				UpdateUsing();

			foreach (ITextViewLine line in e.NewOrReformattedLines)
			{
				if (usingSpan.IntersectsWith(Span.FromBounds(line.Start, line.End)))
					UpdateUsing();
				CreateVisuals(line);
			}
		}

		static float GetFloat(Match match, int pos, float defaultValue = 0f, string groupName = "val")
		{
			var group = match.Groups[groupName];
			if (group.Success && group.Captures.Count > pos)
			{
				var text = group.Captures[pos].Value.TrimEnd('f');
				float value;
				if (float.TryParse(text, out value))
					return value;
			}
			return defaultValue;
		}

		static int GetInt(Match match, int pos, int defaultValue = 0, string groupName = "val")
		{
			var group = match.Groups[groupName];
			if (group.Success && group.Captures.Count > pos)
			{
				var text = group.Captures[pos].Value;
				int value;
				if (text.StartsWith("0x") && int.TryParse(text.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
					return value;
				if (int.TryParse(text, out value))
					return value;
			}
			return defaultValue;
		}

		void CreateVisuals(ITextViewLine line)
		{
			var text = view.TextSnapshot.GetText(line.Start, line.Length);

			if (hasUsing == false)
				UpdateUsing(text);
			var regex = hasUsing == true ? usingColorRegex : colorRegex;

			var matches = regex.Matches(text);
			foreach (Match match in matches)
			{
				var mode = match.Groups["mode"].Value;

				Func<Match, Eto.Drawing.Color> translateColor;
				if (!colorMatching.TryGetValue(mode, out translateColor))
					continue;

				var color = translateColor(match).ToWpf();
				if (color.A <= 0)
					continue;

				var span = new SnapshotSpan(view.TextSnapshot, line.Start + match.Index, match.Length);
				var geometry = view.TextViewLines.GetMarkerGeometry(span);
				if (geometry == null
					|| !view.TextViewModel.IsPointInVisualBuffer(span.Start, PositionAffinity.Successor)
					|| !view.TextViewModel.IsPointInVisualBuffer(span.End, PositionAffinity.Predecessor))
					continue;

				var pen = GetPen(color);
				var bounds = geometry.Bounds;
				var underline = new LineGeometry(bounds.BottomLeft, bounds.BottomRight);
				underline.Freeze();
				var drawing = new GeometryDrawing(null, pen, underline);
				drawing.Freeze();

				var drawingImage = new DrawingImage(drawing);
				drawingImage.Freeze();

				var image = new Image();
				image.Source = drawingImage;

				Canvas.SetLeft(image, geometry.Bounds.Left);
				Canvas.SetTop(image, geometry.Bounds.Bottom - 2);

				layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
			}
		}
	}
}
