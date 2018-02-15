using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class RichTextAreaHandler : TextAreaHandler<Gtk.TextView, RichTextArea, RichTextArea.ICallback>, RichTextArea.IHandler, ITextBuffer
	{
		List<Gtk.TextTag> insertTags = new List<Gtk.TextTag>();
		List<Gtk.TextTag> removeTags = new List<Gtk.TextTag>();

		const string WeightTagName = "w";
		const string StyleTagName = "s";
		const string UnderlineTagName = "u";
		const string StrikethroughTagName = "t";
		const string FontTagName = "f";
		const string ForegroundTagName = "fg";
		const string BackgroundTagName = "bg";
		const string FamilyTagName = "fm";

		bool keepTags;

		protected override void Initialize()
		{
			base.Initialize();
			Control.Buffer.InsertText += HandleInsertText;
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

		void ApplySelectionTag(string name, string variation, Action<Gtk.TextTag> apply)
		{
			var prefix = name + "-";
			var tagName = prefix + variation;

			Gtk.TextIter start, end;
			var buffer = Control.Buffer;
			if (buffer.GetSelectionBounds(out start, out end))
			{
				// apply formatting to the selection
				ApplyTag(name, variation, start, end, apply);
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

		void ApplyTag(string name, string variation, Gtk.TextIter start, Gtk.TextIter end, Action<Gtk.TextTag> apply)
		{
			var buffer = Control.Buffer;
			var prefix = name + "-";
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

		void ApplyTag(string name, string variation, Range<int> range, Action<Gtk.TextTag> apply)
		{
			var buffer = Control.Buffer;
			var start = buffer.GetIterAtOffset(range.Start);
			var end = buffer.GetIterAtOffset(range.End + 1);
			ApplyTag(name, variation, start, end, apply);
		}

		bool HasFontAttribute(Func<Pango.FontDescription, bool> hasAttribute)
		{
			var selection = SelectionIter;
			const string prefix = FontTagName + "-";
			var fontTag = selection.Tags.LastOrDefault(r => r.Name != null && r.Name.StartsWith(prefix, StringComparison.Ordinal));
			if (fontTag == null)
				fontTag = insertTags.LastOrDefault(r => r.Name != null && r.Name.StartsWith(prefix, StringComparison.Ordinal));
			return fontTag != null && hasAttribute(fontTag.FontDesc);
		}

		Gtk.TextTag GetSelectionTag(string name)
		{
			var prefix = name + "-";
			var selection = SelectionIter;
			return selection.Tags.FirstOrDefault(r => r.Name != null && r.Name.StartsWith(prefix, StringComparison.Ordinal)) 
				?? insertTags.FirstOrDefault(r => r.Name != null && r.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
		}

		bool SelectionHasTag(string name, string variation)
		{
			var prefix = name + "-";
			var tagName = prefix + variation;
			var selection = SelectionIter;
			return selection.Tags.Any(r => r.Name == tagName) || insertTags.Any(r => r.Name == tagName);
		}

		bool SelectionHasTag(string name, IEnumerable<string> variations)
		{
			var prefix = name + "-";
			var tags = variations.Select(r => prefix + r).ToList();
			var selection = SelectionIter;
			return selection.Tags.Any(r => tags.Contains(r.Name)) || insertTags.Any(r => tags.Contains(r.Name));
		}

		public void SetFont(Range<int> range, Font font)
		{
			var pangoFont = font.ToPango();
			var variation = pangoFont != null ? pangoFont.Handle.ToString() : string.Empty;
			ApplyTag(FamilyTagName, pangoFont.Family, range, tag =>
			{
				tag.Family = pangoFont.Family;
				tag.FamilySet = true;
			});
			ApplyTag(FontTagName, variation, range, tag =>
			{
				tag.Size = pangoFont.Size;
				tag.SizeSet = true;
				tag.Stretch = pangoFont.Stretch;
				tag.StretchSet = true;

			});
			ApplyTag(StyleTagName, pangoFont.Style.ToString(), range, tag =>
			{
				tag.Style = pangoFont.Style;
				tag.StyleSet = true;
			});
			ApplyTag(WeightTagName, pangoFont.Weight.ToString(), range, tag =>
			{
				tag.Weight = pangoFont.Weight;
				tag.WeightSet = true;
			});
			SetUnderline(range, font.FontDecoration.HasFlag(FontDecoration.Underline));
			SetStrikethrough(range, font.FontDecoration.HasFlag(FontDecoration.Strikethrough));
		}

		public void SetFamily(Range<int> range, FontFamily family)
		{
			var pangoFamily = family.ToPango().Name;
			ApplyTag(FamilyTagName, pangoFamily, range, tag =>
			{
				tag.Family = pangoFamily;
				tag.FamilySet = true;
			});
		}


		public void SetForeground(Range<int> range, Color color)
		{
			ApplyTag(ForegroundTagName, color.ToArgb().ToString(), range, tag =>
			{
				tag.ForegroundGdk = color.ToGdk();
				tag.ForegroundSet = true;
			});
		}

		public void SetBackground(Range<int> range, Color color)
		{
			ApplyTag(BackgroundTagName, color.ToArgb().ToString(), range, tag =>
			{
				tag.BackgroundGdk = color.ToGdk();
				tag.BackgroundSet = true;
			});
		}

		public void SetBold(Range<int> range, bool bold)
		{
			var weight = bold ? Pango.Weight.Bold : Pango.Weight.Normal;
			ApplyTag(WeightTagName, weight.ToString(), range, tag =>
			{
				tag.Weight = weight;
				tag.WeightSet = true;
			});
		}

		public void SetItalic(Range<int> range, bool italic)
		{
			var style = italic ? Pango.Style.Italic : Pango.Style.Normal;
			ApplyTag(StyleTagName, style.ToString(), range, tag =>
			{
				tag.Style = style;
				tag.StyleSet = true;
			});
		}

		public void SetUnderline(Range<int> range, bool underline)
		{
			ApplyTag(UnderlineTagName, underline.ToString(), range, tag =>
			{
				tag.Underline = underline ? Pango.Underline.Single : Pango.Underline.None;
				tag.UnderlineSet = true;
			});
		}

		public void SetStrikethrough(Range<int> range, bool strikethrough)
		{
			ApplyTag(StrikethroughTagName, strikethrough.ToString(), range, tag =>
			{
				tag.Strikethrough = strikethrough;
				tag.StrikethroughSet = true;
			});
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

		public Font SelectionFont
		{
			get
			{
				const string prefix = FontTagName + "-";
				var weightTag = GetSelectionTag(WeightTagName);
				var weight = weightTag != null ? weightTag.Weight : Pango.Weight.Normal;
				var styleTag = GetSelectionTag(StyleTagName);
				var style = styleTag != null ? styleTag.Style : Pango.Style.Normal;
				var decoration = FontDecoration.None;
				if (SelectionUnderline)
					decoration |= FontDecoration.Underline;
				if (SelectionStrikethrough)
					decoration |= FontDecoration.Strikethrough;
				Pango.FontDescription fontDesc = null;

				Pango.FontFamily family = null;
				Pango.Stretch stretch = Pango.Stretch.Normal;

				const string familyPrefix = FamilyTagName + "-";
				var familyTag = SelectionIter.Tags.LastOrDefault(r => r.Name.StartsWith(familyPrefix, StringComparison.Ordinal))
					?? insertTags.LastOrDefault(r => r.Name.StartsWith(familyPrefix, StringComparison.Ordinal));
				if (familyTag != null)
					family = FontFamilyHandler.GetFontFamily(familyTag.Family);

				var tag = SelectionIter.Tags.LastOrDefault(r => r.Name.StartsWith(prefix, StringComparison.Ordinal))
					?? insertTags.LastOrDefault(r => r.Name.StartsWith(prefix, StringComparison.Ordinal));
				if (family == null && tag != null && tag.FamilySet)
				{
					family = FontFamilyHandler.GetFontFamily(tag.Family);
					if (tag.StretchSet)
						stretch = tag.Stretch;
				}
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
					fontDesc = family.Faces[0].Describe();
				fontDesc.Size = tag != null ? tag.Size : (int)(Font.Size * Pango.Scale.PangoScale);
				return new Font(new FontHandler(fontDesc, decorations: decoration));
			}
			set
			{
				var pangoFont = value.ToPango();
				var variation = pangoFont != null ? pangoFont.Handle.ToString() : string.Empty;
				ApplySelectionTag(FontTagName, variation, tag => {
					tag.Family = pangoFont.Family;
					tag.FamilySet = true;
					tag.Size = pangoFont.Size;
					tag.SizeSet = true;
					tag.Stretch = pangoFont.Stretch;
					tag.StretchSet = true;
				});
				SelectionBold = value.Bold;
				SelectionItalic = value.Italic;
				SelectionUnderline = value.FontDecoration.HasFlag(FontDecoration.Underline);
				SelectionStrikethrough = value.FontDecoration.HasFlag(FontDecoration.Strikethrough);
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
				ApplySelectionTag(ForegroundTagName, value.ToArgb().ToString(), tag =>
				{
					tag.ForegroundGdk = value.ToGdk();
					tag.ForegroundSet = true;
				});
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
				ApplySelectionTag(BackgroundTagName, value.ToArgb().ToString(), tag =>
				{
					tag.BackgroundGdk = value.ToGdk();
					tag.BackgroundSet = true;
				});
			}
		}

		public bool SelectionBold
		{
			get
			{
				return SelectionHasTag(WeightTagName, boldWeights.Select(r => r.ToString()));
			}
			set
			{
				var weight = value ? Pango.Weight.Bold : Pango.Weight.Normal;
				ApplySelectionTag(WeightTagName, weight.ToString(), tag =>
				{
					tag.Weight = weight;
					tag.WeightSet = true;
				});
			}
		}

		public bool SelectionItalic
		{
			get
			{
				return SelectionHasTag(StyleTagName, new [] { Pango.Style.Italic.ToString(), Pango.Style.Oblique.ToString() });
			}
			set
			{
				var style = value ? Pango.Style.Italic : Pango.Style.Normal;
				ApplySelectionTag(StyleTagName, style.ToString(), tag =>
				{
					tag.Style = style;
					tag.StyleSet = true;
				});
			}
		}

		public bool SelectionUnderline
		{
			get { return SelectionHasTag(UnderlineTagName, true.ToString()); }
			set
			{
				ApplySelectionTag(UnderlineTagName, value.ToString(), tag =>
				{
					tag.Underline = value ? Pango.Underline.Single : Pango.Underline.None;
					tag.UnderlineSet = true;
				});
			}
		}

		public bool SelectionStrikethrough
		{
			get { return SelectionHasTag(StrikethroughTagName, true.ToString()); }
			set
			{
				ApplySelectionTag(StrikethroughTagName, value.ToString(), tag =>
				{
					tag.Strikethrough = value;
					tag.StrikethroughSet = true;
				});
			}
		}

		public FontFamily SelectionFamily
		{
			get
			{
				const string prefix = FamilyTagName + "-";
				var tag = SelectionIter.Tags.LastOrDefault(r => r.Name.StartsWith(prefix, StringComparison.Ordinal))
					?? insertTags.LastOrDefault(r => r.Name.StartsWith(prefix, StringComparison.Ordinal));
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
				ApplySelectionTag(FamilyTagName, pangoFamily, tag =>
				{
					tag.Family = pangoFamily;
					tag.FamilySet = true;
				});
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
	}
}

