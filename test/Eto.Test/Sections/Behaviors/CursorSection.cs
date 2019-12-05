using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", typeof(Cursor))]
	public class CursorSection : Panel
	{
		class CursorRect
		{
			public Rectangle Rectangle { get; set; }
			public Cursor Cursor { get; set; }
			public string Text { get; set; }
		}

		class CursorDrawable : Drawable
		{
			public List<CursorRect> Rects { get; } = new List<CursorRect>();
			protected override void OnMouseMove(MouseEventArgs e)
			{
				base.OnMouseMove(e);
				Cursor = Cursors.Default; // should be able to set more than once.

				var loc = Rects.FirstOrDefault(r => r.Rectangle.Contains(Point.Round(e.Location)));
				var c = loc?.Cursor ?? Cursors.Default;
				Cursor = c;
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				base.OnPaint(e);
				var g = e.Graphics;
				foreach (var cursorRect in Rects)
				{
					var rect = cursorRect.Rectangle;
					g.FillRectangle(Colors.Silver, rect);
					var textSize = Size.Ceiling(g.MeasureString(SystemFonts.Default(), cursorRect.Text));

					var textLocation = rect.Center - textSize / 2;
					g.DrawText(SystemFonts.Default(), Colors.Black, textLocation, cursorRect.Text);
				}
			}
		}

		IEnumerable<(string name, Cursor cursor)> GetCursors()
		{
			foreach (var type in Enum.GetValues(typeof(CursorType)).OfType<CursorType?>())
			{
				yield return (type.ToString(), new Cursor(type.Value));
			}

			yield return ("Custom (.cur)", TestIcons.TestCursor);
			yield return ("Custom (.png)", new Cursor(new Bitmap(TestIcons.Logo, 32, 32), new PointF(16, 16)));
		}

		public CursorSection()
		{

			var layout = new DynamicLayout();
			layout.BeginCentered(spacing: new Size(10, 10), yscale: true);

			var drawable = new CursorDrawable();

			var rect = new Rectangle(0, 0, 100, 50);

			layout.Add("Label");
			layout.BeginVertical(spacing: new Size(20, 20));
			layout.BeginHorizontal();
			int count = 0;
			foreach (var type in GetCursors())
			{
				var cursor = type.cursor;
				var text = type.name;
				drawable.Rects.Add(new CursorRect { Rectangle = rect, Cursor = cursor, Text = text });
				rect.X += rect.Width + 20;

				var label = new Label
				{ 
					Size = new Size(100, 50), 
					Text = text,
					VerticalAlignment = VerticalAlignment.Center,
					TextAlignment = TextAlignment.Center,
					BackgroundColor = Colors.Silver
				};
				if (cursor == null)
					label.Cursor = null;
				else
					label.Cursor = cursor;
				layout.Add(label);

				if (count++ > 3)
				{
					count = 0;
					rect.X = 0;
					rect.Y += rect.Height + 20;
					layout.EndBeginHorizontal();
				}
			}
			layout.EndHorizontal();
			layout.EndVertical();

			layout.Add("Drawable with MouseMove");
			layout.Add(drawable);
			layout.EndCentered();

			drawable.Size = new Size(340, rect.Y + rect.Height + 20);

			Content = layout;

		}
	}
}

