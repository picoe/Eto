using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Eto.GtkSharp.Drawing;
using Gtk;

namespace Eto.GtkSharp.Forms.Controls
{
	public class RichTextAreaHandler : TextAreaHandler<Gtk.TextView, RichTextArea, RichTextArea.ICallback>, RichTextArea.IHandler, ITextBuffer
	{
		List<Gtk.TextTag> insertTags = new List<Gtk.TextTag>();
		List<Gtk.TextTag> removeTags = new List<Gtk.TextTag>();

		const string WeightTagPrefix = "w-";
		const string StyleTagPrefix = "s-";
		const string StretchTagPrefix = "h-";
		const string UnderlineTagPrefix = "u-";
		const string StrikethroughTagPrefix = "t-";
		const string FontSizePrefix = "f-";
		const string ForegroundTagPrefix = "fg-";
		const string BackgroundTagPrefix = "bg-";
		const string FamilyTagPrefix = "fm-";

		bool keepTags;

		protected override void Initialize()
		{
			base.Initialize();
			Control.Buffer.InsertText += Connector.HandleInsertText;
			Widget.SelectionChanged += HandleSelectionChanged;
		}

		void HandleSelectionChanged(object sender, EventArgs e)
		{
			// when user moves cursor or changes selection, clear insertion formatting
			if (!keepTags)
			{
				insertTags.Clear();
				removeTags.Clear();
			}
			else
				keepTags = false;
		}

		void HandleInsertText(object o, Gtk.InsertTextArgs args)
		{
			// when text is inserted, apply insertion formatting
			var buffer = Control.Buffer;
#if GTK3
			var start = buffer.GetIterAtOffset(args.Pos.Offset - args.NewTextLength);
#elif GTK2
			var start = buffer.GetIterAtOffset(args.Pos.Offset - args.Length);
#endif
			foreach (var tag in removeTags)
			{
				buffer.RemoveTag(tag, start, args.Pos);
			}
			foreach (var tag in insertTags)
			{
				buffer.ApplyTag(tag, start, args.Pos);
			}
			keepTags = true;
		}

		void ApplySelectionTag(string prefix, string variation, Action<Gtk.TextTag> apply)
		{
			var tagName = prefix + variation;

			Gtk.TextIter start, end;
			var buffer = Control.Buffer;
			if (buffer.GetSelectionBounds(out start, out end))
			{
				// apply formatting to the selection
				ApplyTag(prefix, variation, start, end, apply);
			}
			else
			{
				// nothing selected, set insertion formatting
				buffer.TagTable.Foreach(t =>
				{
					if (t.Name != null && t.Name != tagName && t.Name.StartsWith(prefix, StringComparison.Ordinal) && !removeTags.Contains(t))
						removeTags.Add(t);
				});
				insertTags.RemoveAll(removeTags.Contains);
				removeTags.RemoveAll(r => r.Name == tagName);

				var tag = buffer.TagTable.Lookup(tagName);
				if (tag == null)
				{
					tag = new Gtk.TextTag(tagName);
					apply(tag);
					buffer.TagTable.Add(tag);
				}
				insertTags.Add(tag);
			}
		}

		void ApplyTag(string prefix, string variation, Gtk.TextIter start, Gtk.TextIter end, Action<Gtk.TextTag> apply)
		{
			var buffer = Control.Buffer;
			var tagName = prefix + variation;
			var tagsToRemove = new List<Gtk.TextTag>();
			buffer.TagTable.Foreach(t =>
			{
				if (t.Name != null && t.Name.StartsWith(prefix, StringComparison.Ordinal))
					tagsToRemove.Add(t);
			});
			foreach (var removeTag in tagsToRemove)
			{
				buffer.RemoveTag(removeTag, start, end);
			}

			var tag = buffer.TagTable.Lookup(tagName);
			if (tag == null)
			{
				tag = new Gtk.TextTag(tagName);
				apply(tag);
				buffer.TagTable.Add(tag);
			}
			buffer.ApplyTag(tag, start, end);
		}

		void ApplyTag(string prefix, string variation, Range<int> range, Action<Gtk.TextTag> apply)
		{
			var buffer = Control.Buffer;
			var start = buffer.GetIterAtOffset(range.Start);
			var end = buffer.GetIterAtOffset(range.End + 1);
			ApplyTag(prefix, variation, start, end, apply);
		}

		bool SelectionHasTag(string prefix, string variation)
		{
			var tagName = prefix + variation;
			var selection = SelectionIter;
			return selection.Tags.Any(r => r.Name == tagName) || insertTags.Any(r => r.Name == tagName);
		}

		bool SelectionHasTag(string prefix, IEnumerable<string> variations)
		{
			var tags = variations.Select(r => prefix + r).ToList();
			var selection = SelectionIter;
			return selection.Tags.Any(r => tags.Contains(r.Name)) || insertTags.Any(r => tags.Contains(r.Name));
		}

		public void SetFont(Range<int> range, Font font)
		{
			var pangoFont = font.ToPango();
			var variation = pangoFont != null ? pangoFont.Handle.ToString() : string.Empty;
			ApplyTag(FamilyTagPrefix, pangoFont.Family, range, tag =>
			{
				tag.Family = pangoFont.Family;
				tag.FamilySet = true;
			});
			ApplyTag(FontSizePrefix, pangoFont.Size.ToString(), range, tag =>
			{
				tag.Size = pangoFont.Size;
				tag.SizeSet = true;
			});
			ApplyTag(StretchTagPrefix, variation, range, tag =>
			{
				tag.Stretch = pangoFont.Stretch;
				tag.StretchSet = true;
			});
			ApplyTag(StyleTagPrefix, pangoFont.Style.ToString(), range, tag =>
			{
				tag.Style = pangoFont.Style;
				tag.StyleSet = true;
			});
			ApplyTag(WeightTagPrefix, pangoFont.Weight.ToString(), range, tag =>
			{
				tag.Weight = pangoFont.Weight;
				tag.WeightSet = true;
			});
			SetUnderline(range, font.FontDecoration.HasFlag(FontDecoration.Underline));
			SetStrikethrough(range, font.FontDecoration.HasFlag(FontDecoration.Strikethrough));
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		public void SetFamily(Range<int> range, FontFamily family)
		{
			var pangoFamily = family.ToPango().Name;
			ApplyTag(FamilyTagPrefix, pangoFamily, range, tag =>
			{
				tag.Family = pangoFamily;
				tag.FamilySet = true;
			});
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		public void SetTypeface(Range<int> range, FontTypeface typeface)
		{
			var pangoFace = typeface.ToPango();
			ApplyTag(FamilyTagPrefix, pangoFace.FaceName, range, tag =>
			{
				tag.Family = pangoFace.FaceName;
				tag.FamilySet = true;
			});
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		public void SetForeground(Range<int> range, Color color)
		{
			ApplyTag(ForegroundTagPrefix, color.ToArgb().ToString(), range, tag =>
			{
				tag.ForegroundGdk = color.ToGdk();
				tag.ForegroundSet = true;
			});
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		public void SetBackground(Range<int> range, Color color)
		{
			ApplyTag(BackgroundTagPrefix, color.ToArgb().ToString(), range, tag =>
			{
				tag.BackgroundGdk = color.ToGdk();
				tag.BackgroundSet = true;
			});
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		public void SetBold(Range<int> range, bool bold)
		{
			var weight = bold ? Pango.Weight.Bold : Pango.Weight.Normal;
			ApplyTag(WeightTagPrefix, weight.ToString(), range, tag =>
			{
				tag.Weight = weight;
				tag.WeightSet = true;
			});
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		public void SetItalic(Range<int> range, bool italic)
		{
			var style = italic ? Pango.Style.Italic : Pango.Style.Normal;
			ApplyTag(StyleTagPrefix, style.ToString(), range, tag =>
			{
				tag.Style = style;
				tag.StyleSet = true;
			});
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		public void SetUnderline(Range<int> range, bool underline)
		{
			ApplyTag(UnderlineTagPrefix, underline.ToString(), range, tag =>
			{
				tag.Underline = underline ? Pango.Underline.Single : Pango.Underline.None;
				tag.UnderlineSet = true;
			});
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		public void SetStrikethrough(Range<int> range, bool strikethrough)
		{
			ApplyTag(StrikethroughTagPrefix, strikethrough.ToString(), range, tag =>
			{
				tag.Strikethrough = strikethrough;
				tag.StrikethroughSet = true;
			});
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		Gtk.TextIter SelectionIter
		{
			get
			{
				Gtk.TextIter start, end;
				if (!Control.Buffer.GetSelectionBounds(out start, out end))
					start = Control.Buffer.GetIterAtMark(Control.Buffer.InsertMark);
				return start;
			}
		}

		static List<Pango.Weight> boldWeights = new List<Pango.Weight>
		{
			Pango.Weight.Bold,
			Pango.Weight.Heavy,
			Pango.Weight.Semibold,
			Pango.Weight.Ultrabold
		};

		Gtk.TextTag GetTag(string prefix) => SelectionIter.Tags.LastOrDefault(r => r.Name != null && r.Name.StartsWith(prefix, StringComparison.Ordinal))
					?? insertTags.LastOrDefault(r => r.Name != null && r.Name.StartsWith(prefix, StringComparison.Ordinal));

		public Font SelectionFont
		{
			get
			{
				var weightTag = GetTag(WeightTagPrefix);
				var weight = weightTag?.WeightSet == true ? weightTag.Weight : Pango.Weight.Normal;
				var styleTag = GetTag(StyleTagPrefix);
				var style = styleTag?.StyleSet == true ? styleTag.Style : Pango.Style.Normal;
				var stretchTag = GetTag(StretchTagPrefix);
				var stretch = stretchTag?.StretchSet == true ? stretchTag.Stretch : Pango.Stretch.Normal;

				var decoration = FontDecoration.None;
				if (SelectionUnderline)
					decoration |= FontDecoration.Underline;
				if (SelectionStrikethrough)
					decoration |= FontDecoration.Strikethrough;
				Pango.FontDescription fontDesc = null;

				Pango.FontFamily family = null;

				var familyTag = GetTag(FamilyTagPrefix);
				if (familyTag?.FamilySet == true)
					family = FontFamilyHandler.GetFontFamily(familyTag.Family);
				if (family == null)
					family = Font.Family.ToPango();


				foreach (var face in family.Faces)
				{
					var faceDesc = face.Describe();
					if (faceDesc.Weight == weight && faceDesc.Style == style && faceDesc.Stretch == stretch)
					{
						fontDesc = faceDesc;
						break;
					}
				}
				if (fontDesc == null)
					fontDesc = family.Faces[0]?.Describe();
				var fontSizeTag = GetTag(FontSizePrefix);
				fontDesc.Size = fontSizeTag != null ? fontSizeTag.Size : (int)(Font.Size * Pango.Scale.PangoScale);
				return new Font(new FontHandler(fontDesc, decorations: decoration));
			}
			set
			{
				var pangoFont = value.ToPango();
				var variation = pangoFont != null ? pangoFont.Handle.ToString() : string.Empty;
				ApplySelectionTag(FamilyTagPrefix, pangoFont.Family, tag =>
				{
					tag.Family = pangoFont.Family;
					tag.FamilySet = true;
				});
				ApplySelectionTag(FontSizePrefix, variation, tag =>
				{
					tag.Size = pangoFont.Size;
					tag.SizeSet = true;
				});
				ApplySelectionTag(StretchTagPrefix, pangoFont.Stretch.ToString(), tag =>
				{
					tag.Stretch = pangoFont.Stretch;
					tag.StretchSet = true;
				});
				ApplySelectionTag(WeightTagPrefix, pangoFont.Weight.ToString(), tag =>
				{
					tag.Weight = pangoFont.Weight;
					tag.WeightSet = true;
				});
				ApplySelectionTag(StyleTagPrefix, pangoFont.Style.ToString(), tag =>
				{
					tag.Style = pangoFont.Style;
					tag.StyleSet = true;
				});

				var underline = value.FontDecoration.HasFlag(FontDecoration.Underline);
				ApplySelectionTag(UnderlineTagPrefix, value.ToString(), tag =>
				{
					tag.Underline = underline ? Pango.Underline.Single : Pango.Underline.None;
					tag.UnderlineSet = true;
				});
				var strikethrough = value.FontDecoration.HasFlag(FontDecoration.Strikethrough);
				ApplySelectionTag(StrikethroughTagPrefix, value.ToString(), tag =>
				{
					tag.Strikethrough = strikethrough;
					tag.StrikethroughSet = true;
				});

				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public Color SelectionForeground
		{
			get
			{
				var tag = SelectionIter.Tags.LastOrDefault(r => r.ForegroundSet);
				if (tag == null)
					tag = insertTags.LastOrDefault(r => r.ForegroundSet);
				if (tag != null)
					return tag.ForegroundGdk.ToEto();
				return Colors.Black; // todo
			}
			set
			{
				ApplySelectionTag(ForegroundTagPrefix, value.ToArgb().ToString(), tag =>
				{
					tag.ForegroundGdk = value.ToGdk();
					tag.ForegroundSet = true;
				});
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public Color SelectionBackground
		{
			get
			{
				var tag = SelectionIter.Tags.LastOrDefault(r => r.BackgroundSet);
				if (tag == null)
					tag = insertTags.LastOrDefault(r => r.BackgroundSet);
				if (tag != null)
					return tag.BackgroundGdk.ToEto();
				return Colors.Transparent; // todo
			}
			set
			{
				ApplySelectionTag(BackgroundTagPrefix, value.ToArgb().ToString(), tag =>
				{
					tag.BackgroundGdk = value.ToGdk();
					tag.BackgroundSet = true;
				});
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public bool SelectionBold
		{
			get
			{
				return SelectionHasTag(WeightTagPrefix, boldWeights.Select(r => r.ToString()));
			}
			set
			{
				var weight = value ? Pango.Weight.Bold : Pango.Weight.Normal;
				ApplySelectionTag(WeightTagPrefix, weight.ToString(), tag =>
				{
					tag.Weight = weight;
					tag.WeightSet = true;
				});
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public bool SelectionItalic
		{
			get
			{
				return SelectionHasTag(StyleTagPrefix, new[] { Pango.Style.Italic.ToString(), Pango.Style.Oblique.ToString() });
			}
			set
			{
				var style = value ? Pango.Style.Italic : Pango.Style.Normal;
				ApplySelectionTag(StyleTagPrefix, style.ToString(), tag =>
				{
					tag.Style = style;
					tag.StyleSet = true;
				});
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public bool SelectionUnderline
		{
			get { return SelectionHasTag(UnderlineTagPrefix, true.ToString()); }
			set
			{
				ApplySelectionTag(UnderlineTagPrefix, value.ToString(), tag =>
				{
					tag.Underline = value ? Pango.Underline.Single : Pango.Underline.None;
					tag.UnderlineSet = true;
				});
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public bool SelectionStrikethrough
		{
			get { return SelectionHasTag(StrikethroughTagPrefix, true.ToString()); }
			set
			{
				ApplySelectionTag(StrikethroughTagPrefix, value.ToString(), tag =>
				{
					tag.Strikethrough = value;
					tag.StrikethroughSet = true;
				});
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public FontFamily SelectionFamily
		{
			get
			{
				var tag = GetTag(FamilyTagPrefix);
				if (tag != null)
				{
					var family = FontFamilyHandler.GetFontFamily(tag.Family);
					return new FontFamily(new FontFamilyHandler(family));
				}
				return SelectionFont.Family;
			}
			set
			{
				var pangoFamily = value.ToPango().Name;
				ApplySelectionTag(FamilyTagPrefix, pangoFamily, tag =>
				{
					tag.Family = pangoFamily;
					tag.FamilySet = true;
				});
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public FontTypeface SelectionTypeface
		{
			get => SelectionFont.Typeface;
			set
			{
				if (value == null)
					return;
				var pangoDesc = value.ToPango().Describe();
				ApplySelectionTag(FamilyTagPrefix, pangoDesc.Family, tag =>
				{
					tag.Family = pangoDesc.Family;
					tag.FamilySet = true;
				});
				ApplySelectionTag(StyleTagPrefix, pangoDesc.Style.ToString(), tag =>
				{
					tag.Style = pangoDesc.Style;
					tag.StyleSet = true;
				});
				ApplySelectionTag(WeightTagPrefix, pangoDesc.Weight.ToString(), tag =>
				{
					tag.Weight = pangoDesc.Weight;
					tag.WeightSet = true;
				});
				ApplySelectionTag(StretchTagPrefix, pangoDesc.Stretch.ToString(), tag =>
				{
					tag.Stretch = pangoDesc.Stretch;
					tag.StretchSet = true;
				});
				Callback.OnTextChanged(Widget, EventArgs.Empty);
			}
		}

		public IEnumerable<RichTextAreaFormat> SupportedFormats
		{
			get
			{
				yield return RichTextAreaFormat.PlainText;
			}
		}

		public void Load(System.IO.Stream stream, RichTextAreaFormat format)
		{
			switch (format)
			{
				case RichTextAreaFormat.PlainText:
					Control.Buffer.Clear();
					using (var reader = new StreamReader(stream))
					{
						Control.Buffer.Text = reader.ReadToEnd();
					}
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void Save(Stream stream, RichTextAreaFormat format)
		{
			switch (format)
			{
				case RichTextAreaFormat.PlainText:
					var bytes = Encoding.UTF8.GetBytes(Control.Buffer.Text);
					stream.Write(bytes, 0, bytes.Length);
					return;
				default:
					throw new NotSupportedException();
			}
		}

		public void Clear()
		{
			Control.Buffer.Clear();
		}

		public void Delete(Range<int> range)
		{
			var buffer = Control.Buffer;
			var start = buffer.GetIterAtOffset(range.Start);
			var end = buffer.GetIterAtOffset(range.End + 1);
			buffer.Delete(ref start, ref end);
		}

		public void Insert(int position, string text)
		{
			var buffer = Control.Buffer;
			var start = buffer.GetIterAtOffset(position);
			buffer.Insert(ref start, text);
		}

		public ITextBuffer Buffer
		{
			get { return this; }
		}

		protected new RichTextAreaConnector Connector => (RichTextAreaConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new RichTextAreaConnector();

		protected class RichTextAreaConnector : TextAreaConnector
		{
			public new RichTextAreaHandler Handler => (RichTextAreaHandler)base.Handler;

			public virtual void HandleInsertText(object o, InsertTextArgs args) => Handler?.HandleInsertText(o, args);
		}
	}
}

