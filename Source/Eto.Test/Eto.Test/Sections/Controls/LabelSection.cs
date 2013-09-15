using System;
using Eto.Forms;
using Eto.Drawing;
using VerticalAlign = Eto.Forms.VerticalAlign;

namespace Eto.Test.Sections.Controls
{
	public class LabelSection : Panel
	{
		public LabelSection()
		{
			var layout = new DynamicLayout(Padding.Empty, Size.Empty);
			
			layout.Add(NormalLabel(), true, true);
			layout.Add(FontLabel(), true, true);
			layout.Add(CenterLabel(), true, true);
			layout.Add(RightLabel(), true, true);
			layout.Add(MiddleLabel(), true, true);
			layout.Add(BottomLabel(), true, true);
			layout.Add(ColorLabel(), true, true);
			layout.Add(NoWrapLabel(), true, true);

			Content = layout;
		}

		Control NormalLabel()
		{
			return new Label
			{
				Text = "Normal Label"
			};
		}

		Control FontLabel()
		{
			return new Label
			{
				Text = "Font Label",
				Font = Fonts.Sans (12, FontStyle.Bold),
			};
		}

		Control CenterLabel()
		{
			return new Label
			{
				Text = "Center Align",
				//BackgroundColor = Color.FromArgb (0xa0a0a0),
				HorizontalAlign = HorizontalAlign.Center
			};
		}

		Control RightLabel()
		{
			return new Label
			{
				Text = "Right Align",
				HorizontalAlign = HorizontalAlign.Right
			};
		}

		Control MiddleLabel()
		{
			return new Label
			{
				Text = "Middle Center Align",
				HorizontalAlign = HorizontalAlign.Center,
				VerticalAlign = VerticalAlign.Middle
			};
		}

		Control BottomLabel()
		{
			return new Label
			{
				Text = "Bottom Center Align",
				HorizontalAlign = HorizontalAlign.Center,
				VerticalAlign = VerticalAlign.Bottom
			};
		}

		Control ColorLabel()
		{
			return new Label
			{
				Text = "Custom Color",
				TextColor = Color.FromArgb(0xFF00a000)
			};
		}

		Control NoWrapLabel()
		{
			return new Label
			{
				Text = "No wrapping on this label",
				Wrap = WrapMode.None
			};
		}
	}
}

