using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.PixelLayoutSection
{
	enum Anchor
	{
		Top,
		Right,
		Bottom,
		Left
	}

	class AnchorSection : Panel
	{

		private Anchor anchor;
		public Anchor Anchor
		{
			get { return anchor; }
			set
			{
				anchor = value;
				SetButtonsPosition();
			}
		}

		private void SetButtonsPosition()
		{
			// remove the buttons 
			if (PixelLayout.Children.Contains(Buttons))
				PixelLayout.Remove(Buttons);

			var size = new Size(200, 200);
			var location = Point.Empty;

			// X
			if (Anchor == Anchor.Left ||
				Anchor == Anchor.Top ||
				Anchor == Anchor.Bottom)
				location.X = 0;
			else
				location.X = PixelLayout.Size.Width - size.Width;

			// Y
			if (Anchor == Anchor.Left ||
				Anchor == Anchor.Top ||
				Anchor == Anchor.Right)
				location.Y = 0;
			else
				location.Y = PixelLayout.Size.Height - size.Height;

			// Width
			if (Anchor == Anchor.Top ||
				Anchor == Anchor.Bottom)
				size.Width = PixelLayout.Size.Width;

			// Height
			if (Anchor == Anchor.Left ||
				Anchor == Anchor.Right)
				size.Height = PixelLayout.Size.Height;

			// At this point size and location are where
			// Buttons should be displayed.
			Buttons.Size = size;
			PixelLayout.Add(Buttons, location);
		}

		PixelLayout PixelLayout { get; set; }
		Control Buttons { get; set; }
		Size buttonsSize = new Size(200, 200);

		public AnchorSection()
		{
			Content = PixelLayout = new PixelLayout();		
			Buttons = CreateButtons();
			Anchor = Anchor.Bottom;
		}

		Control CreateButtons()
		{
			var table = new TableLayout(3, 3);
			Button top, right, bottom, left;

			table.Add(top = new Button { Text = "Top" }, 1, 0);
			table.Add(right = new Button { Text = "Right" }, 2, 1);
			table.Add(bottom = new Button { Text = "Bottom" }, 1, 2);
			table.Add(left = new Button { Text = "Left" }, 0, 1);

			top.Click += (s, e) => Anchor = Anchor.Top;
			right.Click += (s, e) => Anchor = Anchor.Right;
			bottom.Click += (s, e) => Anchor = Anchor.Bottom;
			left.Click += (s, e) => Anchor = Anchor.Left;

			return table;
		}
	}
}
