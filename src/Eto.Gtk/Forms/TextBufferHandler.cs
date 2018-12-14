using System;
using System.Collections.Generic;
using System.IO;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public static class TextBufferExtensions
	{
		public const string WeightTagPrefix = "w-";
		public const string StyleTagPrefix = "s-";
		public const string StretchTagPrefix = "h-";
		public const string UnderlineTagPrefix = "u-";
		public const string StrikethroughTagPrefix = "t-";
		public const string FontSizePrefix = "f-";
		public const string ForegroundTagPrefix = "fg-";
		public const string BackgroundTagPrefix = "bg-";
		public const string FamilyTagPrefix = "fm-";

		public static void SetFont(this Gtk.TextBuffer buffer, Range<int> range, Font font)
		{
			var pangoFont = font.ToPango();
			var variation = pangoFont != null ? pangoFont.Handle.ToString() : string.Empty;
			buffer.ApplyTag(FamilyTagPrefix, pangoFont.Family, range, tag =>
			{
				tag.Family = pangoFont.Family;
				tag.FamilySet = true;
			});
			buffer.ApplyTag(FontSizePrefix, pangoFont.Size.ToString(), range, tag =>
			{
				tag.Size = pangoFont.Size;
				tag.SizeSet = true;
			});
			buffer.ApplyTag(StretchTagPrefix, variation, range, tag =>
			{
				tag.Stretch = pangoFont.Stretch;
				tag.StretchSet = true;
			});
			buffer.ApplyTag(StyleTagPrefix, pangoFont.Style.ToString(), range, tag =>
			{
				tag.Style = pangoFont.Style;
				tag.StyleSet = true;
			});
			buffer.ApplyTag(WeightTagPrefix, pangoFont.Weight.ToString(), range, tag =>
			{
				tag.Weight = pangoFont.Weight;
				tag.WeightSet = true;
			});
			buffer.SetUnderline(range, font.FontDecoration.HasFlag(FontDecoration.Underline));
			buffer.SetStrikethrough(range, font.FontDecoration.HasFlag(FontDecoration.Strikethrough));
		}

		public static void SetFamily(this Gtk.TextBuffer buffer, Range<int> range, FontFamily family)
		{
			var pangoFamily = family.ToPango().Name;
			buffer.ApplyTag(FamilyTagPrefix, pangoFamily, range, tag =>
			{
				tag.Family = pangoFamily;
				tag.FamilySet = true;
			});
		}

		public static void ApplySelectionTag(this Gtk.TextBuffer buffer, string prefix, string variation, Action<Gtk.TextTag> apply)
		{
			var tagName = prefix + variation;

			Gtk.TextIter start, end;
			if (buffer.GetSelectionBounds(out start, out end))
			{
				// apply formatting to the selection
				buffer.ApplyTag(prefix, variation, start, end, apply);
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

		public static void ApplyTag(this Gtk.TextBuffer buffer, string prefix, string variation, Gtk.TextIter start, Gtk.TextIter end, Action<Gtk.TextTag> apply)
		{
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

		public static void ApplyTag(this Gtk.TextBuffer buffer, string prefix, string variation, Range<int> range, Action<Gtk.TextTag> apply)
		{
			var start = buffer.GetIterAtOffset(range.Start);
			var end = buffer.GetIterAtOffset(range.End + 1);
			buffer.ApplyTag(prefix, variation, start, end, apply);
		}
	}

	public class TextBufferHandler : WidgetHandler<Gtk.TextBuffer, TextBuffer, TextBuffer.ICallback>, TextBuffer.IHandler
	{
		Gtk.TextTagTable table;
		public TextBufferHandler()
		{
			table = new Gtk.TextTagTable();
			Control = new Gtk.TextBuffer(table);
		}

		public IEnumerable<RichTextAreaFormat> SupportedFormats => throw new NotImplementedException();

		public int TextLength => Control.CharCount;

		public void Append(string text)
		{
			var iter = Control.EndIter;
			Control.Insert(ref iter, text);
		}

		public void Append(ITextBuffer buffer)
		{
			var iter = Control.EndIter;
			var buf = buffer.ToGtk();
			Control.InsertRange(ref iter, buf.StartIter, buf.EndIter);
		}

		public void BeginEdit()
		{
			Control.BeginUserAction();
		}

		public void Clear()
		{
			Control.Clear();
		}

		public void Delete(Range<int> range)
		{
			var start = Control.GetIterAtOffset(range.Start);
			var end = Control.GetIterAtOffset(range.Start + range.Length());
			Control.Delete(ref start, ref end);
		}

		public void EndEdit()
		{
			Control.EndUserAction();
		}

		public void Insert(int position, string text)
		{
			var iter = Control.GetIterAtOffset(position);
			Control.Insert(ref iter, text);
		}

		public void Insert(int position, ITextBuffer buffer)
		{
			var iter = Control.GetIterAtOffset(position);
			var buf = buffer.ToGtk();
			Control.InsertRange(ref iter, buf.StartIter, buf.EndIter);
		}

		public void Load(Stream stream, RichTextAreaFormat format)
		{

		}

		public void Save(Stream stream, RichTextAreaFormat format)
		{
		}

		public void SetBackground(Range<int> range, Color color)
		{
			ApplyTag(ForegroundTagPrefix, color.ToArgb().ToString(), range, tag =>
			{
				tag.ForegroundGdk = color.ToGdk();
				tag.ForegroundSet = true;
			});
		}

		public void SetBold(Range<int> range, bool bold)
		{
			throw new NotImplementedException();
		}

		public void SetFamily(Range<int> range, FontFamily family)
		{
			throw new NotImplementedException();
		}

		public void SetFont(Range<int> range, Font font)
		{
			throw new NotImplementedException();
		}

		public void SetForeground(Range<int> range, Color color)
		{
			throw new NotImplementedException();
		}

		public void SetItalic(Range<int> range, bool italic)
		{
			throw new NotImplementedException();
		}

		public void SetStrikethrough(Range<int> range, bool strikethrough)
		{
			throw new NotImplementedException();
		}

		public void SetUnderline(Range<int> range, bool underline)
		{
			throw new NotImplementedException();
		}
	}
}
