using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Documents;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;
using System.Globalization;
using System.Collections;
using System.IO;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoRichTextBox : swc.RichTextBox, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}
	public class RichTextAreaHandler : TextAreaHandler<EtoRichTextBox, RichTextArea, RichTextArea.ICallback>, RichTextArea.IHandler, ITextBuffer
	{
		LanguageChangedListener _languageChangedListener;

		public RichTextAreaHandler()
		{
			// set default margin between paragraphs to match other platforms
			var style = new sw.Style { TargetType = typeof(swd.Paragraph) };
			style.Setters.Add(new sw.Setter(swd.Paragraph.MarginProperty, new sw.Thickness(0)));
			Control.Resources.Add(typeof(swd.Paragraph), style);

			// toggle underline command doesn't actually work properly in the default implementation
			// e.g. if you select only a portion of underlined text, you will have to toggle underline twice to remove the underline.
			Control.CommandBindings.Add(new swi.CommandBinding(swd.EditingCommands.ToggleUnderline, (sender, e) => SelectionUnderline = !SelectionUnderline));
		}

		protected override void Initialize()
		{
			base.Initialize();
			lastSelection = Selection;
			HandleEvent(RichTextArea.SelectionChangedEvent);

			FixLanguageSelectionAttributes();
		}

		static sw.Markup.XmlLanguage CurrentLanguage => sw.Markup.XmlLanguage.GetLanguage(swi.InputLanguageManager.Current.CurrentInputLanguage.IetfLanguageTag);

		class LanguageChangedListener : IDisposable
		{
			WeakReference _handler;
			RichTextAreaHandler Handler => _handler?.Target as RichTextAreaHandler;

			swi.InputLanguageManager _manager;

			~LanguageChangedListener()
			{
				Dispose(false);
			}

			public void LanguageChanged(object sender, swi.InputLanguageEventArgs e)
			{
				var h = Handler;
				if (h != null)
					h.Control.Language = CurrentLanguage;
			}

			public LanguageChangedListener(RichTextAreaHandler handler, swi.InputLanguageManager manager)
			{
				_handler = new WeakReference(handler);
				_manager = manager;
				_manager.InputLanguageChanged += LanguageChanged;
				handler.Control.Language = CurrentLanguage;
			}

			void Dispose(bool disposing)
			{
				if (_manager != null && !_manager.Dispatcher.HasShutdownStarted)
				{
					// when shutting down, this causes a com exception
					_manager.InputLanguageChanged -= LanguageChanged;
					_manager = null;
				}
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		void FixLanguageSelectionAttributes()
		{
			// BUG in WPF: When entering text from a different language (or direction) than the current selection, 
			// we lose all selection formatting.

			// By setting the language to match the input language, we don't lose selection formatting when entering text

			// This has a concequence where the spellcheck language will always match the input language, not the language
			// set for the operating system. Fortunately, this is probably a good thing.

			// only track changes to language when we have focus.
			Control.GotKeyboardFocus += (sender, e) =>
			{
				if (_languageChangedListener == null)
					_languageChangedListener = new LanguageChangedListener(this, swi.InputLanguageManager.Current);
			};
			Control.LostKeyboardFocus += (sender, e) =>
			{
				_languageChangedListener?.Dispose();
				_languageChangedListener = null;
			};
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_languageChangedListener?.Dispose();
				_languageChangedListener = null;
			}
			base.Dispose(disposing);
		}

		swd.TextRange ContentRange => new swd.TextRange(Control.Document.ContentStart, Control.Document.ContentEnd);

		public override string Text
		{
			get { return ContentRange.Text; }
			set { ContentRange.Text = value ?? string.Empty; }
		}

		bool wrap = true;
		public override bool Wrap
		{
			get { return wrap; }
			set
			{
				if (value != wrap)
				{
					if (!wrap)
					{
						Control.TextChanged -= Control_TextChangedSetPageWidth;
						Control.SizeChanged -= Control_TextChangedSetPageWidth;
						Control.Document.PageWidth = double.NaN;
					}
					wrap = value;
					if (!wrap)
					{
						Control.TextChanged += Control_TextChangedSetPageWidth;
						Control.SizeChanged += Control_TextChangedSetPageWidth;
						SetPageWidthToContent();
					}
				}
			}
		}

		void Control_TextChangedSetPageWidth(object sender, EventArgs e)
		{
			Control.Dispatcher.BeginInvoke(new Action(SetPageWidthToContent));
		}

		void SetPageWidthToContent()
		{
			// this can be invoked after Wrap property is actually set since we are using the dispatcher
			if (!wrap)
			{
				var formattedText = Control.Document.GetFormattedText();

				var width = Math.Ceiling(formattedText.WidthIncludingTrailingWhitespace + Control.Document.PagePadding.Horizontal());
				Control.Document.PageWidth = Math.Max(width, Control.ViewportWidth);
			}
		}

		public override string SelectedText
		{
			get { return Control.Selection.Text; }
			set { Control.Selection.Text = value ?? string.Empty; }
		}

		Dictionary<sw.DependencyProperty, object> selectionAttributes;
		Range<int> lastSelection;
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case RichTextArea.SelectionChangedEvent:
					Control.SelectionChanged += (sender, e) =>
					{
						if (lastSelection != Selection)
						{
							selectionAttributes = null;
							lastSelection = Selection;
							Callback.OnSelectionChanged(Widget, EventArgs.Empty);
						}
						else if (selectionAttributes != null)
						{
							// when the selection doesn't actually change, keep the attributes.
							// e.g. if the control already has focus and the user clicks on it but it
							// doesn't change the selection, all selected attributes are lost which is unexpected
							// as the state of the control has not actually changed at all.
							foreach (var item in selectionAttributes)
							{
								Control.Selection.ApplyPropertyValue(item.Key, item.Value);
							}
						}
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void SetSelectionAttribute(sw.DependencyProperty property, object value)
		{
			selectionAttributes = selectionAttributes ?? new Dictionary<sw.DependencyProperty, object>();
			if (value == null)
			{
				if (selectionAttributes.ContainsKey(property))
					selectionAttributes.Remove(property);
			}
			else
				selectionAttributes[property] = value;

			Control.Selection.ApplyPropertyValue(property, value);
		}

		void SetSelectionDecorationAttribute(sw.TextDecorationCollection setDecorations, bool value)
		{
			selectionAttributes = selectionAttributes ?? new Dictionary<sw.DependencyProperty, object>();
			object decorationsObj;
			sw.TextDecorationCollection decorations;
			if (!selectionAttributes.TryGetValue(swd.Inline.TextDecorationsProperty, out decorationsObj))
			{
				decorations = Control.Selection.GetPropertyValue(swd.Inline.TextDecorationsProperty) as sw.TextDecorationCollection;
				if (decorations == null)
					decorations = new sw.TextDecorationCollection();
				else
					decorations = new sw.TextDecorationCollection(decorations);
				selectionAttributes[swd.Inline.TextDecorationsProperty] = decorations;
			}
			else decorations = (sw.TextDecorationCollection)decorationsObj;

			foreach (var decoration in setDecorations)
			{
				if (value)
					decorations.Add(decoration);
				else
					decorations.Remove(decoration);
			}

			Control.Selection.ApplyPropertyValue(swd.Inline.TextDecorationsProperty, decorations);
		}

		public override Range<int> Selection
		{
			get
			{
				var sel = Control.Selection;
				return new Range<int>(sel.Start.GetTextOffset(), sel.End.GetTextOffset() - 1);
			}
			set
			{
				var contentStart = Control.Document.ContentStart;
				Control.Selection.Select(contentStart.GetTextPositionAtOffset(value.Start), contentStart.GetTextPositionAtOffset(value.End + 1));
			}
		}

		public override int CaretIndex
		{
			get { return Control.CaretPosition.GetTextOffset(); }
			set { Control.CaretPosition = Control.Document.ContentStart.GetTextPositionAtOffset(value); }
		}

		public Font SelectionFont
		{
			get { return new Font(new FontHandler(Control.Selection, Control)); }
			set
			{
				var handler = ((FontHandler)value?.Handler);
				SetSelectionAttribute(swd.TextElement.FontFamilyProperty, handler?.WpfFamily);
				SetSelectionAttribute(swd.TextElement.FontStyleProperty, handler?.WpfFontStyle);
				SetSelectionAttribute(swd.TextElement.FontWeightProperty, handler?.WpfFontWeight);
				SetSelectionAttribute(swd.TextElement.FontSizeProperty, handler?.PixelSize);
				SetSelectionAttribute(swd.Inline.TextDecorationsProperty, handler?.WpfTextDecorationsFrozen);
			}
		}

		public FontFamily SelectionFamily
		{
			get { return new FontFamily(new FontFamilyHandler(Control.Selection, Control)); }
			set
			{
				SetSelectionAttribute(swd.TextElement.FontFamilyProperty, ((FontFamilyHandler)value?.Handler)?.Control);
			}
		}

		public Color SelectionForeground
		{
			get
			{
				var brush = Control.Selection.GetPropertyValue(swd.TextElement.ForegroundProperty) as swm.Brush;
				return brush.ToEtoColor();
			}
			set
			{
				SetSelectionAttribute(swd.TextElement.ForegroundProperty, value.ToWpfBrush());
			}
		}

		public Color SelectionBackground
		{
			get
			{
				var brush = Control.Selection.GetPropertyValue(swd.TextElement.BackgroundProperty) as swm.Brush;
				return brush.ToEtoColor();
			}
			set
			{
				SetSelectionAttribute(swd.TextElement.BackgroundProperty, value.ToWpfBrush());
			}
		}

		swd.TextRange GetRange(Range<int> range)
		{
			var content = Control.Document.ContentStart;
			return new swd.TextRange(content.GetTextPositionAtOffset(range.Start), content.GetTextPositionAtOffset(range.End + 1));
		}

		void SetRange(Range<int> range, Action<swd.TextRange> action)
		{
			action(GetRange(range));
		}

		public void SetFont(Range<int> range, Font font)
		{
			SetRange(range, tr => tr.SetEtoFont(font));
		}

		public void SetFamily(Range<int> range, FontFamily family)
		{
			SetRange(range, tr => tr.SetEtoFamily(family));
		}

		public void SetForeground(Range<int> range, Color color)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.ForegroundProperty, color.ToWpfBrush()));
		}

		public void SetBackground(Range<int> range, Color color)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.BackgroundProperty, color.ToWpfBrush()));
		}

		public bool SelectionBold
		{
			get
			{
				
				var fontWeight = Control.Selection.GetPropertyValue(swd.TextElement.FontWeightProperty) as sw.FontWeight? ?? sw.FontWeights.Normal;
				return fontWeight == sw.FontWeights.Bold
					|| fontWeight == sw.FontWeights.DemiBold
					|| fontWeight == sw.FontWeights.ExtraBold
					|| fontWeight == sw.FontWeights.SemiBold
					|| fontWeight == sw.FontWeights.UltraBold;
			}
			set
			{
				SetSelectionAttribute(swd.TextElement.FontWeightProperty, value ? sw.FontWeights.Bold : sw.FontWeights.Normal);
			}
		}

		public void SetBold(Range<int> range, bool bold)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.FontWeightProperty, bold ? sw.FontWeights.Bold : sw.FontWeights.Normal));
		}


		public bool SelectionItalic
		{
			get
			{
				var fontStyle = Control.Selection.GetPropertyValue(swd.TextElement.FontStyleProperty) as sw.FontStyle? ?? sw.FontStyles.Normal;
				return fontStyle != sw.FontStyles.Normal;
			}
			set
			{
				SetSelectionAttribute(swd.TextElement.FontStyleProperty, value ? sw.FontStyles.Italic : sw.FontStyles.Normal);
			}
		}


		bool HasDecorations(swd.TextRange range, sw.TextDecorationCollection decorations)
		{
			swd.TextRange realRange;
			var existingDecorations = range.GetRealPropertyValue(swd.Inline.TextDecorationsProperty, out realRange) as sw.TextDecorationCollection;
			return existingDecorations != null && decorations.All(r => existingDecorations.Contains(r));
		}

		void SetDecorations(swd.TextRange range, sw.TextDecorationCollection decorations, bool value)
		{
			using (Control.DeclareChangeBlock())
			{
				// set the property to each element in the range so it keeps all other decorations
				foreach (var element in range.GetElements().OfType<swd.Inline>().Distinct())
				{
					var existingDecorations = element.GetValue(swd.Inline.TextDecorationsProperty) as sw.TextDecorationCollection;

					// need to keep the range before changing otherwise the range changes
					var elementRange = new swd.TextRange(element.ElementStart, element.ElementEnd);

					sw.TextDecorationCollection newDecorations = null;

					// remove decorations from the element
					element.SetValue(swd.Inline.TextDecorationsProperty, null);

					if (existingDecorations != null && existingDecorations.Count > 0)
					{
						// merge desired decorations with existing decorations.
						if (value)
							newDecorations = new sw.TextDecorationCollection(existingDecorations.Union(decorations));
						else
							newDecorations = new sw.TextDecorationCollection(existingDecorations.Except(decorations));

						// split up existing decorations to the parts of the element that don't fall within the range
						existingDecorations = new sw.TextDecorationCollection(existingDecorations); // copy so we don't update existing elements
						if (elementRange.Start.CompareTo(range.Start) < 0)
							new swd.TextRange(elementRange.Start, range.Start).ApplyPropertyValue(swd.Inline.TextDecorationsProperty, existingDecorations);
						if (elementRange.End.CompareTo(range.End) > 0)
							new swd.TextRange(range.End, elementRange.End).ApplyPropertyValue(swd.Inline.TextDecorationsProperty, existingDecorations);
					}
					else
					{
						// no existing decorations, just set the new value
						newDecorations = value ? decorations : null;
					}

					if (newDecorations != null && newDecorations.Count > 0)
					{
						// apply new decorations to the desired range, which may be a combination of existing decorations
						swd.TextPointer start = elementRange.Start.CompareTo(range.Start) < 0 ? range.Start : elementRange.Start;
						swd.TextPointer end = elementRange.End.CompareTo(range.End) > 0 ? range.End : elementRange.End;
						new swd.TextRange(start, end).ApplyPropertyValue(swd.Inline.TextDecorationsProperty, newDecorations);
					}
				}
			}
		}

		public bool SelectionUnderline
		{
			get
			{
				return HasDecorations(Control.Selection, sw.TextDecorations.Underline);
			}
			set
			{
				if (Selection.Length() == 0)
					SetSelectionDecorationAttribute(sw.TextDecorations.Underline, value);
				else
					SetDecorations(Control.Selection, sw.TextDecorations.Underline, value);
			}
		}

		public bool SelectionStrikethrough
		{
			get
			{
				return HasDecorations(Control.Selection, sw.TextDecorations.Strikethrough);
			}
			set
			{
				if (Selection.Length() == 0)
					SetSelectionDecorationAttribute(sw.TextDecorations.Strikethrough, value);
				else
					SetDecorations(Control.Selection, sw.TextDecorations.Strikethrough, value);
			}
		}

		public void SetItalic(Range<int> range, bool italic)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.FontStyleProperty, italic ? sw.FontStyles.Italic : sw.FontStyles.Normal));
		}

		public void SetUnderline(Range<int> range, bool underline)
		{
			SetRange(range, tr => SetDecorations(tr, sw.TextDecorations.Underline, underline));
		}

		public void SetStrikethrough(Range<int> range, bool strikethrough)
		{
			SetRange(range, tr => SetDecorations(tr, sw.TextDecorations.Strikethrough, strikethrough));
		}

		public IEnumerable<RichTextAreaFormat> SupportedFormats
		{
			get
			{
				yield return RichTextAreaFormat.Rtf;
				yield return RichTextAreaFormat.PlainText;
			}
		}

		public void Load(Stream stream, RichTextAreaFormat format)
		{
			var range = ContentRange;
			switch (format)
			{
				case RichTextAreaFormat.Rtf:
					range.Load(stream, sw.DataFormats.Rtf);
					break;
				case RichTextAreaFormat.PlainText:
					range.Load(stream, sw.DataFormats.Text);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void Save(Stream stream, RichTextAreaFormat format)
		{
			var range = ContentRange;
			switch (format)
			{
				case RichTextAreaFormat.Rtf:
					range.Save(stream, sw.DataFormats.Rtf);
					break;
				case RichTextAreaFormat.PlainText:
					range.Save(stream, sw.DataFormats.Rtf);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void Clear()
		{
			Text = null;
		}

		public void Delete(Range<int> range)
		{
			var textRange = GetRange(range);
			textRange.Text = null;
		}

		public void Insert(int position, string text)
		{
			var pos = Control.Document.ContentStart.GetTextPositionAtOffset(position);
			pos.InsertTextInRun(text);
		}

		public ITextBuffer Buffer
		{
			get { return this; }
		}

		public override TextAlignment TextAlignment
		{
			get { return base.TextAlignment; }
			set
			{
				base.TextAlignment = value;
				ContentRange.ApplyPropertyValue(swd.Block.TextAlignmentProperty, value.ToWpfTextAlignment());
			}
		}
	}

	static class FlowDocumentExtensions
	{
		static IEnumerable<swd.TextElement> GetRunsAndParagraphs(swd.FlowDocument doc)
		{
			for (var position = doc.ContentStart;
			  position != null && position.CompareTo(doc.ContentEnd) <= 0;
			  position = position.GetNextContextPosition(swd.LogicalDirection.Forward))
			{
				if (position.GetPointerContext(swd.LogicalDirection.Forward) == swd.TextPointerContext.ElementEnd)
				{
					var parent = position.Parent;
					if (parent is swd.Run || parent is swd.Paragraph || parent is swd.LineBreak)
						yield return parent as swd.TextElement;
				}
			}
		}

		public static IEnumerable<swd.TextElement> GetElements(this swd.TextRange range)
		{
			for (var position = range.Start;
			  position != null && position.CompareTo(range.End) <= 0;
			  position = position.GetNextContextPosition(swd.LogicalDirection.Forward))
			{
				var obj = position.Parent as sw.FrameworkContentElement;
				while (obj != null)
				{
					var elem = obj as swd.TextElement;
					if (elem != null)
						yield return elem;
					obj = obj.Parent as sw.FrameworkContentElement;
				}
			}
		}

		// https://social.msdn.microsoft.com/Forums/sharepoint/en-US/6cd49173-b06d-4749-85aa-f6ab46c7d4af/wpf-rich-text-box-width-size-adjust-to-text?forum=wpf
		public static swm.FormattedText GetFormattedText(this swd.FlowDocument doc)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");

			
			var runsAndParagraphs = GetRunsAndParagraphs(doc).ToList();
			var output = new swm.FormattedText(
			  GetText(runsAndParagraphs),
			  CultureInfo.CurrentCulture,
			  doc.FlowDirection,
			  new swm.Typeface(doc.FontFamily, doc.FontStyle, doc.FontWeight, doc.FontStretch),
			  doc.FontSize,
			  doc.Foreground,
			  null,
			  swm.TextOptions.GetTextFormattingMode(doc));

			int offset = 0;

			foreach (var el in runsAndParagraphs)
			{
				var run = el as swd.Run;

				if (run != null)
				{
					int count = run.Text.Length;

					output.SetFontFamily(run.FontFamily, offset, count);
					output.SetFontStyle(run.FontStyle, offset, count);
					output.SetFontWeight(run.FontWeight, offset, count);
					output.SetFontSize(run.FontSize, offset, count);
					output.SetForegroundBrush(run.Foreground, offset, count);
					output.SetFontStretch(run.FontStretch, offset, count);
					output.SetTextDecorations(run.TextDecorations, offset, count);

					offset += count;
					continue;
				}
				offset += Environment.NewLine.Length;
			}

			return output;
		}

		public static string GetText(this swd.InlineCollection inlines)
		{
			var sb = new StringBuilder();

			foreach (var el in inlines)
			{
				var run = el as swd.Run;
				sb.Append(run == null ? Environment.NewLine : run.Text);
			}
			return sb.ToString();
		}

		public static string GetText(this IEnumerable<swd.TextElement> runsAndParagraphs)
		{
			var sb = new StringBuilder();

			foreach (var el in runsAndParagraphs)
			{
				var run = el as swd.Run;
				if (run != null)
				{
					sb.Append(run.Text);
					continue;
				}
				if (el is swd.Paragraph || el is swd.LineBreak)
					sb.AppendLine();
			}
			return sb.ToString();
		}

		public static int GetTextOffset(this swd.TextPointer position)
		{
			var offset = 0;
			while (position != null)
			{
				if (position.GetPointerContext(swd.LogicalDirection.Backward) == swd.TextPointerContext.Text)
				{
					offset += position.GetTextRunLength(swd.LogicalDirection.Backward);
				}

				var nextContextPosition = position.GetNextContextPosition(swd.LogicalDirection.Backward);
				if (nextContextPosition == null)
					return offset;

				position = nextContextPosition;
			}

			return offset;
		}

		public static swd.TextPointer GetTextPositionAtOffset(this swd.TextPointer position, int characterCount)
		{
			while (position != null)
			{
				if (position.GetPointerContext(swd.LogicalDirection.Forward) == swd.TextPointerContext.Text)
				{
					int count = position.GetTextRunLength(swd.LogicalDirection.Forward);
					if (count >= characterCount)
					{
						return position.GetPositionAtOffset(characterCount);
					}

					characterCount -= count;
				}

				var nextContextPosition = position.GetNextContextPosition(swd.LogicalDirection.Forward);
				if (nextContextPosition == null)
					return position;

				position = nextContextPosition;
			}

			return position;
		}

		// Existing GetPropertyValue for the TextDecorationCollection will return the first collection, even if it is empty.
		// this skips empty collections so we can get the actual value.
		// slightly modified code from https://social.msdn.microsoft.com/Forums/vstudio/en-US/3ac626cf-60aa-427f-80e9-794f3775a70e/how-to-tell-if-richtextbox-selection-is-underlined?forum=wpf
		public static object GetRealPropertyValue(this swd.TextRange textRange, sw.DependencyProperty formattingProperty, out swd.TextRange fullRange)
		{
			object value = null;
			fullRange = null;
			var pointer = textRange.Start as swd.TextPointer;
			if (pointer != null)
			{
				var needsContinue = true;
				swd.TextElement text = null;
				sw.DependencyObject element = pointer.Parent as swd.TextElement;
				while (needsContinue && (element is swd.Inline || element is swd.Paragraph || element is swc.TextBlock))
				{
					value = element.GetValue(formattingProperty);
					text = element as swd.TextElement;
					var seq = value as IEnumerable;
					needsContinue = (seq == null) ? value == null : seq.Cast<object>().Count() == 0;
					element = element is swd.TextElement ? ((swd.TextElement)element).Parent : null;
					
				}
				if (text != null)
				{
					fullRange = new swd.TextRange(text.ElementStart, text.ElementEnd);
                }
            }
			return value;
		}

	}
}
