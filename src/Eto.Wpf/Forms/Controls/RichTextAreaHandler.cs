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
using Eto.Wpf.CustomControls.FontDialog;
using System.Text.RegularExpressions;

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

		static readonly object AllowImages_Key = new object();

		/// <summary>
		/// Gets or sets a value indicating that the user can paste images into the editor
		/// </summary>
		public bool AllowImages
		{
			get { return Widget.Properties.Get<bool>(AllowImages_Key); }
			set { Widget.Properties.Set(AllowImages_Key, value); }
		}

		public RichTextAreaHandler()
		{
			// set default margin between paragraphs to match other platforms
			var style = new sw.Style { TargetType = typeof(swd.Paragraph) };
			style.Setters.Add(new sw.Setter(swd.Paragraph.MarginProperty, new sw.Thickness(0)));
			Control.Resources.Add(typeof(swd.Paragraph), style);

			// toggle underline command doesn't actually work properly in the default implementation
			// e.g. if you select only a portion of underlined text, you will have to toggle underline twice to remove the underline.
			Control.CommandBindings.Add(new swi.CommandBinding(swd.EditingCommands.ToggleUnderline, (sender, e) => SelectionUnderline = !SelectionUnderline));

			sw.DataObject.AddPastingHandler(Control, HandlePasting);
		}

		void HandlePasting(object sender, sw.DataObjectPastingEventArgs e)
		{
			if (e.FormatToApply == sw.DataFormats.Bitmap && !AllowImages)
			{
				// don't allow images by default
				e.CancelCommand();
			}
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
			// Bug in WPF: When using composition (IME), it doesn't use the selection attributes when inserting the text
			// so, we apply selection attributes to the composition range while composing.
			swi.TextCompositionManager.AddPreviewTextInputStartHandler(Control, OnPreviewTextInputStart);
			swi.TextCompositionManager.AddPreviewTextInputUpdateHandler(Control, OnPreviewTextInputUpdate);
			swi.TextCompositionManager.AddPreviewTextInputHandler(Control, OnPreviewTextInput);
		}

		static readonly object CompositionAttributes_Key = new object();

		Dictionary<sw.DependencyProperty, object> CompositionAttributes
		{
			get { return Widget.Properties.Get<Dictionary<sw.DependencyProperty, object>>(CompositionAttributes_Key); }
			set { Widget.Properties.Set(CompositionAttributes_Key, value); }
		}

		void OnPreviewTextInput(object sender, swi.TextCompositionEventArgs e)
		{
			// Apply to the final composition, needed for example if you type only one character.
			ApplyCompositionAttributes(e);
			// clear out composition attributes as we're now done
			CompositionAttributes = null;
		}

		void OnPreviewTextInputUpdate(object sender, swi.TextCompositionEventArgs e)
		{
			// need to update the composition attributes here so it shows the correct attributes while typing
			// and so each "part" of the composition gets the attributes applied.
			// TODO: still can't figure out how to apply it to the first character entered in the composition
			ApplyCompositionAttributes(e);
		}

		void ApplyCompositionAttributes(swi.TextCompositionEventArgs e)
		{
			// update the composition with the selection attributes, if any
			var rtcomp = e.TextComposition as swd.FrameworkRichTextComposition;
			var attributes = CompositionAttributes;
			if (rtcomp != null && attributes != null)
			{
				swd.TextRange range = null;
				if (rtcomp.CompositionStart != null && rtcomp.CompositionEnd != null)
					range = new swd.TextRange(rtcomp.CompositionStart, rtcomp.CompositionEnd);
				else if (rtcomp.ResultStart != null && rtcomp.ResultEnd != null)
					range = new swd.TextRange(rtcomp.ResultStart, rtcomp.ResultEnd);

				// need to async this as the styles still don't get applied properly at this stage. Yuck!
				if (range != null)
					Application.Instance.AsyncInvoke(() => ApplySelectionAttributes(range, attributes));
			}
		}

		void OnPreviewTextInputStart(object sender, swi.TextCompositionEventArgs e)
		{
			if (e.TextComposition is swd.FrameworkRichTextComposition)
			{
				// save the selection attributes, they get cleared out while the user types the composition
				CompositionAttributes = selectionAttributes;
			}
		}

		void ApplySelectionAttributes(swd.TextRange range, Dictionary<sw.DependencyProperty, object> attributes)
		{
			if (attributes == null)
				return;
			Control.BeginChange();
			foreach (var attribute in attributes.ToList())
			{
				range.ApplyPropertyValue(attribute.Key, attribute.Value);
			}
			Control.EndChange();
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

		protected swd.TextRange ContentRange => new swd.TextRange(Control.Document.ContentStart, Control.Document.ContentEnd);

		public override string Text
		{
			get { return ContentRange.Text.Replace("\r\n", "\n"); }
			set
			{
				SuppressSelectionChanged++;
				ContentRange.Text = value ?? string.Empty;
				SuppressSelectionChanged--;
				var end = Control.Document.ContentEnd;
				Control.Selection.Select(end, end);
			}
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
							ApplySelectionAttributes(Control.Selection, selectionAttributes);
						}
					};
					break;
				case TextArea.CaretIndexChangedEvent:
					int? lastCaretIndex = null;
					Control.SelectionChanged += (sender, e) =>
					{
						var caretIndex = CaretIndex;
						if (lastCaretIndex != caretIndex)
						{
							Callback.OnCaretIndexChanged(Widget, EventArgs.Empty);
							lastCaretIndex = caretIndex;
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
			else
			{
				decorations = (sw.TextDecorationCollection)decorationsObj;
				if (decorations.IsFrozen)
					decorations = new sw.TextDecorationCollection(decorations);
			}

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
				return Eto.Forms.Range.FromLength(sel.Start.GetTextOffset(), sel.GetLength()); // Fully qualified because System.Range was introduced in .NET Core 3.0
			}
			set
			{
				var contentStart = Control.Document.ContentStart;
				var start = contentStart.GetTextPositionAtOffset(value.Start);
				Control.Selection.Select(start, start.GetTextPositionAtOffset(value.Length()));
			}
		}

		public override int CaretIndex
		{
			get
			{
				// CaretPosition is at the end of selection, we should report the beginning
				return Control.Selection.Start.GetTextOffset();
			}
			set { Control.CaretPosition = Control.Document.ContentStart.GetTextPositionAtOffset(value); }
		}

		void ApplyFont(swm.FontFamily family, swm.Typeface typeface, sw.FontWeight? weight, sw.FontStretch? stretch, sw.FontStyle? style)
		{
			SetSelectionAttribute(swd.TextElement.FontFamilyProperty, family);
			SetSelectionAttribute(swd.TextElement.FontWeightProperty, weight ?? sw.FontWeights.Normal);
			SetSelectionAttribute(swd.TextElement.FontStyleProperty, style ?? sw.FontStyles.Normal);
			SetSelectionAttribute(swd.TextElement.FontStretchProperty, stretch ?? sw.FontStretches.Normal);
		}

		public virtual Font SelectionFont
		{
			get { return new Font(new FontHandler(Control.Selection, Control)); }
			set
			{
				var handler = ((FontHandler)value?.Handler);
				ApplyFont(OnTranslateFamily(handler?.WpfFamily), OnTranslateTypeface(handler?.WpfTypeface), handler?.WpfFontWeight, handler?.WpfFontStretch, handler?.WpfFontStyle);
				SetSelectionAttribute(swd.TextElement.FontSizeProperty, handler?.WpfSize);
				SetSelectionAttribute(swd.Inline.TextDecorationsProperty, handler?.WpfTextDecorationsFrozen);
			}
		}

		public virtual FontFamily SelectionFamily
		{
			get { return new FontFamily(new FontFamilyHandler(Control.Selection, Control)); }
			set
			{
				SetSelectionAttribute(swd.TextElement.FontFamilyProperty, OnTranslateFamily(((FontFamilyHandler)value?.Handler)?.Control));
			}
		}

		public virtual FontTypeface SelectionTypeface
		{
			get { return new FontTypeface(SelectionFamily, new FontTypefaceHandler(Control.Selection, Control)); }
			set
			{
				var typeface = (value?.Handler as FontTypefaceHandler)?.Control;
				ApplyFont(OnTranslateFamily(typeface?.FontFamily ?? Control.FontFamily), OnTranslateTypeface(typeface), typeface?.Weight, typeface?.Stretch, typeface?.Style);
			}
		}

		public virtual Color SelectionForeground
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

		public virtual Color SelectionBackground
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
			var start = content.GetTextPositionAtOffset(range.Start);
			return new swd.TextRange(start, start.GetTextPositionAtOffset(range.Length()));
		}

		void SetRange(Range<int> range, Action<swd.TextRange> action)
		{
			action(GetRange(range));
		}

		public virtual void SetFont(Range<int> range, Font font)
		{
			SetRange(range, tr => tr.SetEtoFont(font));
		}

		public virtual void SetFamily(Range<int> range, FontFamily family)
		{
			SetRange(range, tr => tr.SetEtoFamily(family));
		}

		public virtual void SetForeground(Range<int> range, Color color)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.ForegroundProperty, color.ToWpfBrush()));
		}

		public virtual void SetBackground(Range<int> range, Color color)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.BackgroundProperty, color.ToWpfBrush()));
		}

		public virtual bool SelectionBold
		{
			get
			{
				var fontWeight = Control.Selection.GetPropertyValue(swd.TextElement.FontWeightProperty) as sw.FontWeight? ?? sw.FontWeights.Normal;
				return fontWeight >= sw.FontWeights.Bold;
			}
			set
			{
				SetSelectionAttribute(swd.TextElement.FontWeightProperty, value ? sw.FontWeights.Bold : sw.FontWeights.Normal);
			}
		}

		public virtual void SetBold(Range<int> range, bool bold)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.FontWeightProperty, bold ? sw.FontWeights.Bold : sw.FontWeights.Normal));
		}


		public virtual bool SelectionItalic
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
				foreach (var element in range.GetInlineElements())
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

		public virtual bool SelectionUnderline
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

		public virtual bool SelectionStrikethrough
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

		public virtual void SetItalic(Range<int> range, bool italic)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.FontStyleProperty, italic ? sw.FontStyles.Italic : sw.FontStyles.Normal));
		}

		public virtual void SetUnderline(Range<int> range, bool underline)
		{
			SetRange(range, tr => SetDecorations(tr, sw.TextDecorations.Underline, underline));
		}

		public virtual void SetStrikethrough(Range<int> range, bool strikethrough)
		{
			SetRange(range, tr => SetDecorations(tr, sw.TextDecorations.Strikethrough, strikethrough));
		}

		public virtual IEnumerable<RichTextAreaFormat> SupportedFormats
		{
			get
			{
				yield return RichTextAreaFormat.Rtf;
				yield return RichTextAreaFormat.PlainText;
			}
		}

		protected virtual swm.FontFamily OnTranslateFamily(swm.FontFamily family)
		{
			return family;
		}

		protected virtual swm.Typeface OnTranslateTypeface(swm.Typeface typeface)
		{
			return typeface;
		}

		public virtual void Load(Stream stream, RichTextAreaFormat format)
		{
			SuppressSelectionChanged++;
			SuppressTextChanged++;
			InnerLoad(stream, format, ContentRange);
			SuppressTextChanged--;
			SuppressSelectionChanged--;
			Control.Selection.Select(Control.Document.ContentEnd, Control.Document.ContentEnd);
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		protected virtual void InnerLoad(Stream stream, RichTextAreaFormat format, swd.TextRange range)
		{
			switch (format)
			{
				case RichTextAreaFormat.Rtf:
					var ms = EncodeRtfFontNames(stream);

					range.Load(ms, sw.DataFormats.Rtf);
					UpdateFacesToFamilyVariant(range);
					break;
				case RichTextAreaFormat.PlainText:
					range.Load(stream, sw.DataFormats.Text);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		const string AmpersandPlaceholder = "!!amp!!";

		void UpdateFacesToFamilyVariant(swd.TextRange range)
		{
			foreach (var elem in range.GetInlineElements())
			{
				var family = swd.TextElement.GetFontFamily(elem);
				if (family == null)
					continue;

				var newFamily = OnTranslateFamily(family);
				if (!ReferenceEquals(newFamily, family))
				{
					family = newFamily;
					swd.TextElement.SetFontFamily(elem, family);
				}

				// ampersands in the font name crash WPF, so we replace it in the RTF before loading, then fix it up here.
				var ampPosition = family.Source.IndexOf(AmpersandPlaceholder, StringComparison.Ordinal);
				if (ampPosition >= 0)
				{
					var src = family.Source.Replace(AmpersandPlaceholder, "&");
					family = new swm.FontFamily(src);
					swd.TextElement.SetFontFamily(elem, family);
				}

				var typeface = ResolveWpfTypeface(family);
				if (typeface != null)
				{
					swd.TextElement.SetFontFamily(elem, typeface.FontFamily);
					// not in RTF, so should never be set but do a check anyway.
					// In some cases we need to set this anyway (e.g. Arial -> Arial Narrow), so we ensure it matches the current typeface
					if (!elem.PropertyIsInheritedOrLocal(swd.TextElement.FontStretchProperty) 
						|| (elem.GetValue(swd.TextElement.FontStretchProperty) as sw.FontStretch?) != typeface.Stretch)
						swd.TextElement.SetFontStretch(elem, typeface.Stretch);
					
					// in RTF, can be set so only set it to the typeface if not specified in RTF
					if (!elem.PropertyIsInheritedOrLocal(swd.TextElement.FontStyleProperty))
						swd.TextElement.SetFontStyle(elem, typeface.Style);

					// in RTF, we can have bold/normal, but the face could be Black, etc.
					if (!elem.PropertyIsInheritedOrLocal(swd.TextElement.FontWeightProperty)
						|| (
							typeface.Weight != sw.FontWeights.Bold
							&& typeface.Weight != sw.FontWeights.Normal
						))
						swd.TextElement.SetFontWeight(elem, typeface.Weight);
				}
			}
		}

		/// <summary>
		/// Finds the WPF typeface if the family is not known
		/// </summary>
		/// <param name="family">family to find the typeface for</param>
		/// <returns>An instance of the typeface to use, or null if the font family is correct or could not be found</returns>
		swm.Typeface ResolveWpfTypeface(swm.FontFamily family)
		{
			var familyName = NameDictionaryExtensions.GetEnglishName(family.FamilyNames);

			// if the resolved name is the same as the source, we're good
			if (string.Equals(familyName, family.Source, StringComparison.OrdinalIgnoreCase))
				return null;

			// can't find fonts where their Win32 name is different from the WPF name, so lookup based on the win32 name
			// do we need to cache this, or is it fast enough?
			foreach (var font in swm.Fonts.SystemFontFamilies)
			{
				foreach (var typeface in font.GetTypefaces())
				{
					if (typeface.TryGetGlyphTypeface(out var glyphTypeface))
					{
						if (string.Equals(family.Source, glyphTypeface.Win32FamilyNames.GetEnglishName(), StringComparison.OrdinalIgnoreCase))
						{
							// found it!
							return typeface;
						}
					}
				}
			}

			// old, probably not needed code, the above should theoretically handle everything now: 

			var newFamily = new swm.FontFamily(familyName);
			var typefaces = newFamily.GetTypefaces();

			// find based on the win32 family name (which RTF typically uses)
			foreach (var typeface in typefaces)
			{
				if (typeface.TryGetGlyphTypeface(out var glyphTypeface)
					&& string.Equals(family.Source, glyphTypeface.Win32FamilyNames.GetEnglishName(), StringComparison.OrdinalIgnoreCase))
				{
					return typeface;
				}
			}

			// check if the resolved font name has the same family prefix
			if (family.Source.Length <= familyName.Length + 1 || !family.Source.StartsWith(familyName, StringComparison.OrdinalIgnoreCase))
				return null;

			// extract the face part of the source.  E.g. "Arial Narrow" will result in "Narrow".
			var faceName = family.Source.Substring(familyName.Length + 1);

			// find based on the non-localized face name
			foreach (var typeface in typefaces)
			{
				var typefaceName = NameDictionaryExtensions.GetEnglishName(typeface.FaceNames);
				if (faceName == typefaceName)
					return typeface;
			}

			return null;
		}

		public virtual void Save(Stream stream, RichTextAreaFormat format)
		{
			var range = ContentRange;
			switch (format)
			{
				case RichTextAreaFormat.Rtf:
					var fd = new swd.FlowDocument();

					// use same base font for new document
					fd.FontFamily = Control.FontFamily;
					fd.FontSize = Control.FontSize;
					fd.FontStyle = Control.FontStyle;
					fd.FontWeight = Control.FontWeight;
					fd.FontStretch = Control.FontStretch;

					using (var ms = new MemoryStream())
					{
						range.Save(ms, sw.DataFormats.Xaml);
						var fdr = new swd.TextRange(fd.ContentStart, fd.ContentEnd);
						ms.Position = 0;
						fdr.Load(ms, sw.DataFormats.Xaml);
						UpdateFamilyVariantToFaces(fdr, out var needsEncodingFix);

						if (needsEncodingFix)
							UnencodeRtfFontNames(ms, stream, fdr);
						else
							fdr.Save(stream, sw.DataFormats.Rtf);
					}
					break;
				case RichTextAreaFormat.PlainText:
					range.Save(stream, sw.DataFormats.Text);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		static MemoryStream EncodeRtfFontNames(Stream stream)
		{
			var rtf = new StreamReader(stream).ReadToEnd();
			const string regFonttbl = @"(?<={\\fonttbl(\s*))(({[^}]+)}(\s*?))+(?=\s*})";
			var fontTblMatch = Regex.Match(rtf, regFonttbl, RegexOptions.Compiled);
			if (fontTblMatch.Success)
			{
				// only replace ampersands in the fonttbl section, leave everything else
				const string regExp = @"(?<={\\f\d+[^}]+?)&(?=[^}]+)";
				var fontTbl = Regex.Replace(fontTblMatch.Value, regExp, AmpersandPlaceholder, RegexOptions.Compiled);
				if (fontTbl.Length != fontTblMatch.Length)
				{
					// found ampersand in font name, replace fonttbl string
					rtf = rtf.Remove(fontTblMatch.Index, fontTblMatch.Length);
					rtf = rtf.Insert(fontTblMatch.Index, fontTbl);
				}
			}

			var ms = new MemoryStream();
			var writer = new StreamWriter(ms, Encoding.UTF8);
			writer.Write(rtf);
			writer.Flush();
			ms.Position = 0;
			return ms;
		}

		/// <summary>
		/// work around WPF bug that writes xml encoded font names.
		/// 
		/// This unencodes any improperly xml-encoded characters in the font names of the RTF.
		/// </summary>
		static void UnencodeRtfFontNames(MemoryStream ms, Stream stream, swd.TextRange fdr)
		{
			// use existing memory stream to save on memory.
			ms.SetLength(0);
			ms.Position = 0;
			fdr.Save(ms, sw.DataFormats.Rtf);
			ms.Position = 0;

			// use regex to replace the unencoded characters for fcharset's. not ideal.
			var rtf = new StreamReader(ms).ReadToEnd();
			var regExp = @"(?<={\\f\d+[^}]+?)&(amp|lt|gt|quot|apos);(?=[^}]+)";
			rtf = Regex.Replace(rtf, regExp, ReplaceEncodedCharacter, RegexOptions.Compiled);
			var writer = new StreamWriter(stream, Encoding.UTF8);
			writer.Write(rtf);
			writer.Flush();
		}

		static string ReplaceEncodedCharacter(Match match)
		{
			if (match.Value == "&amp;")
				return "&";
			if (match.Value == "&lt;")
				return "<";
			if (match.Value == "&gt;")
				return ">";
			if (match.Value == "&quot;")
				return "\"";
			if (match.Value == "&apos;")
				return "'";
			return match.Value;
		}

		static char[] s_encodedFontNameCharacters = { '&', '<', '>', '\'', '"' };

		void UpdateFamilyVariantToFaces(swd.TextRange range, out bool needsEncodingFix)
		{
			needsEncodingFix = false;
			foreach (var elem in range.GetInlineElements())
			{
				var family = swd.TextElement.GetFontFamily(elem);
				if (family == null)
					continue;


				// rtf supports italic and bold, so ignore those variants
				var style = swd.TextElement.GetFontStyle(elem);
				if (style == sw.FontStyles.Italic || style == sw.FontStyles.Oblique)
					style = sw.FontStyles.Normal;
				var weight = swd.TextElement.GetFontWeight(elem);
				var stretch = swd.TextElement.GetFontStretch(elem);

				var typeface = new swm.Typeface(family, style, weight, stretch);
				var familyName = NameDictionaryExtensions.GetEnglishName(family.FamilyNames);

				// use the win32 family name first if one is mapped
				if (typeface.TryGetGlyphTypeface(out var glyphTypeface))
				{
					// use windows font name in RTF, same as how other apps (e.g. wordpad) does it
					var win32FamilyName = glyphTypeface.Win32FamilyNames.GetEnglishName();
					needsEncodingFix |= win32FamilyName.IndexOfAny(s_encodedFontNameCharacters) >= 0;

					if (!string.Equals(win32FamilyName, familyName, StringComparison.OrdinalIgnoreCase))
					{
						family = new swm.FontFamily(win32FamilyName);
						swd.TextElement.SetFontFamily(elem, family);
						continue;
					}
				}
				else if (stretch != sw.FontStretches.Normal
					|| (weight != sw.FontWeights.Normal && weight != sw.FontWeights.Bold)
					|| style != sw.FontStyles.Normal
					)
				{
					// fallback to writing "<FamilyName> <FaceName>" if glyph typeface cannot be found
					// ensure that the new family source is the same? Correct?
					if (typeface != null && typeface.FontFamily.Source == familyName)
					{
						var faceName = NameDictionaryExtensions.GetEnglishName(typeface.FaceNames);
						var fullFontName = $"{familyName} {faceName}";
						needsEncodingFix |= fullFontName.IndexOfAny(s_encodedFontNameCharacters) >= 0;

						family = new swm.FontFamily(fullFontName);
						swd.TextElement.SetFontFamily(elem, family);
					}
				}
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
		public static int GetLength(this swd.TextRange range)
		{
			var length = 0;
			var position = range.Start;
			var end = range.End;
			while (position != null && position.CompareTo(end) != 0)
			{
				position = position.GetNextInsertionPosition(swd.LogicalDirection.Forward);
				if (position == null)
					break;
				length++;
			}

			return length;
		}

		public static IEnumerable<swd.Inline> GetInlineElements(this swd.TextRange range)
		{
			for (var position = range.Start;
			  position != null 
			  && position.CompareTo(range.End) <= 0;
			  position = position.GetNextContextPosition(swd.LogicalDirection.Forward))
			{
				var obj = position.Parent as sw.FrameworkContentElement;
				while (obj != null)
				{
					var elem = obj as swd.Inline;
					if (elem != null)
					{
						yield return elem;
						position = elem.ElementEnd;
					}
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
#pragma warning disable CS0618 // 'FormattedText.FormattedText(string, CultureInfo, FlowDirection, Typeface, double, Brush)' is obsolete: 'Use the PixelsPerDip override'
			var output = new swm.FormattedText(
			  GetText(runsAndParagraphs),
			  CultureInfo.CurrentCulture,
			  doc.FlowDirection,
			  new swm.Typeface(doc.FontFamily, doc.FontStyle, doc.FontWeight, doc.FontStretch),
			  doc.FontSize,
			  doc.Foreground,
			  null,
			  swm.TextOptions.GetTextFormattingMode(doc));
#pragma warning restore CS0618 // 'FormattedText.FormattedText(string, CultureInfo, FlowDirection, Typeface, double, Brush)' is obsolete: 'Use the PixelsPerDip override'

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
				offset++; // newline
			}

			return output;
		}

		public static string GetText(this swd.InlineCollection inlines)
		{
			var sb = new StringBuilder();

			foreach (var el in inlines)
			{
				var run = el as swd.Run;
				sb.Append(run == null ? "\n" : run.Text);
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
					sb.Append('\n');
			}
			return sb.ToString();
		}

		public static int GetTextOffset(this swd.TextPointer position)
		{
			/*
			System.Diagnostics.Debug.WriteLine($"Finding text offset");
			var p = position;
			while (p != null)
			{
				var ctx = p.GetPointerContext(swd.LogicalDirection.Backward);
				var adj = p.GetAdjacentElement(swd.LogicalDirection.Backward);
				var len = p.GetTextRunLength(swd.LogicalDirection.Backward);
				var text = p.GetTextInRun(swd.LogicalDirection.Backward);
				System.Diagnostics.Debug.WriteLine($"Context: {ctx}, Adjacent: {adj}, Length: {len}, Text: {text}");
				p = p.GetNextContextPosition(swd.LogicalDirection.Backward);
			}
			*/ 

			var offset = 0;
			while (position != null)
			{
				var ctx = position.GetPointerContext(swd.LogicalDirection.Backward);
				if (ctx == swd.TextPointerContext.Text)
				{
					offset += position.GetTextRunLength(swd.LogicalDirection.Backward);
				}
				else if (ctx == swd.TextPointerContext.ElementEnd)
				{
					var adj = position.GetAdjacentElement(swd.LogicalDirection.Backward);
					if (adj is swd.Paragraph)
					{
						offset++; // newline
					}
				}

				position = position.GetNextContextPosition(swd.LogicalDirection.Backward);
			}

			return offset;
		}

		public static swd.TextPointer GetTextPositionAtOffset(this swd.TextPointer position, int characterCount)
		{
			/*
			System.Diagnostics.Debug.WriteLine($"Finding position with {characterCount}");
			var p = position;
			while (p != null)
			{
				var ctx = p.GetPointerContext(swd.LogicalDirection.Forward);
				var adj = p.GetAdjacentElement(swd.LogicalDirection.Forward);
				var len = p.GetTextRunLength(swd.LogicalDirection.Forward);
				var text = p.GetTextInRun(swd.LogicalDirection.Forward);
				System.Diagnostics.Debug.WriteLine($"Context: {ctx}, Adjacent: {adj}, Length: {len}, Text: {text}");
				p = p.GetNextContextPosition(swd.LogicalDirection.Forward);
			}
			*/

			while (position != null && characterCount > 0)
			{
				var ctx = position.GetPointerContext(swd.LogicalDirection.Forward);
				if (ctx == swd.TextPointerContext.Text)
				{
					int count = position.GetTextRunLength(swd.LogicalDirection.Forward);
					if (count >= characterCount)
						return position.GetPositionAtOffset(characterCount);

					characterCount -= count;
				}
				else if (ctx == swd.TextPointerContext.ElementEnd)
				{
					var adj = position.GetAdjacentElement(swd.LogicalDirection.Forward);
					if (adj is swd.Paragraph)
					{
						characterCount--; // newline
					}
				}

				position = position.GetNextContextPosition(swd.LogicalDirection.Forward);
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

		public static bool PropertyIsInheritedOrLocal(this sw.DependencyObject obj, sw.DependencyProperty prop)
		{
			var source = sw.DependencyPropertyHelper.GetValueSource(obj, prop);
			return source.BaseValueSource == sw.BaseValueSource.Local || source.BaseValueSource == sw.BaseValueSource.Inherited;
		}

	}
}
