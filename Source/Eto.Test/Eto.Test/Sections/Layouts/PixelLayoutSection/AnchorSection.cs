using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.PixelLayoutSection
{
	[Flags]
	public enum Anchor
	{
		Left = 0x01,
		Right = 0x02,
		Top = 0x04,
		Bottom = 0x08,
		Horizontal = Left | Right,
		Vertical = Top | Bottom
	}

	[Section("PixelLayout", "Anchor")]
	public class AnchorSection : Panel
	{
		Anchor anchor;

		public Anchor Anchor
		{
			get { return anchor; }
			set
			{
				anchor = value;
				SetButtonsPosition();
			}
		}

		void SetButtonsPosition()
		{
			// remove the buttons 
			if (PixelLayout.Children.Contains(Buttons))
				PixelLayout.Remove(Buttons);

			var size = new Size(200, 200);
			var location = Point.Empty;
			var containerSize = PixelLayout.Size;

			// X
			if (Anchor.HasFlag(Anchor.Right) && !Anchor.HasFlag(Anchor.Left))
				location.X = containerSize.Width - size.Width;

			// Y
			if (Anchor.HasFlag(Anchor.Bottom) && !Anchor.HasFlag(Anchor.Top))
				location.Y = containerSize.Height - size.Height;

			// Width
			if (Anchor.HasFlag(Anchor.Left) && Anchor.HasFlag(Anchor.Right))
				size.Width = containerSize.Width;

			// Height
			if (Anchor.HasFlag(Anchor.Top) && Anchor.HasFlag(Anchor.Bottom))
				size.Height = containerSize.Height;

			// At this point size and location are where
			// Buttons should be displayed.
			Buttons.Size = size;
			PixelLayout.Add(Buttons, location);
		}

		PixelLayout PixelLayout { get; set; }

		Control Buttons { get; set; }

		public AnchorSection()
		{
			Content = PixelLayout = new PixelLayout();

			Buttons = CreateButtons();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			SetButtonsPosition(); // update child position when the size changes
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			SetButtonsPosition(); // set position when we're shown and know our size
		}

		static Anchor Flip(Anchor flag, Anchor value)
		{
			return flag.HasFlag(value) ? flag &= ~value : flag |= value;
		}

		Control CreateButtons()
		{
			var table = new TableLayout(3, 3);
			CheckBox top, right, bottom, left;

			table.Add(top = new CheckBox { Text = "Top", BackgroundColor = Colors.Blue }, 1, 0, true, true);
			table.Add(right = new CheckBox { Text = "Right", BackgroundColor = Colors.Red }, 2, 1, true, true);
			table.Add(bottom = new CheckBox { Text = "Bottom", BackgroundColor = Colors.Blue }, 1, 2, true, true);
			table.Add(left = new CheckBox { Text = "Left", BackgroundColor = Colors.Red }, 0, 1, true, true);

			top.CheckedChanged += (s, e) => Anchor = Flip(Anchor, Anchor.Top);
			right.CheckedChanged += (s, e) => Anchor = Flip(Anchor, Anchor.Right);
			bottom.CheckedChanged += (s, e) => Anchor = Flip(Anchor, Anchor.Bottom);
			left.CheckedChanged += (s, e) => Anchor = Flip(Anchor, Anchor.Left);

			return table;
		}
	}
}
