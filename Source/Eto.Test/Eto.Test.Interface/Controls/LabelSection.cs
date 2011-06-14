using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class LabelSection : Panel
	{
		public LabelSection ()
		{
			var layout = new TableLayout(this, 1, 10);
			layout.Spacing = Size.Empty; // inbetween cells
			layout.Padding = Padding.Empty; // around edges
			
			
			layout.Add (NormalLabel(), 0, 0, true, true);
			layout.Add (FontLabel(), 0, 1, true, true);
			layout.Add (CenterLabel(), 0, 2, true, true);
			layout.Add (RightLabel(), 0, 3, true, true);
			layout.Add (MiddleLabel(), 0, 4, true, true);
			layout.Add (BottomLabel(), 0, 5, true, true);
			layout.Add (ColorLabel(), 0, 6, true, true);
			layout.Add (NoWrapLabel(), 0, 7, true, true);
			
		}
		
		Control NormalLabel()
		{
			return new Label{
				Text = "Normal Label"
			};
		}
		
		Control FontLabel()
		{
			return new Label{
				Text = "Font Label",
				Font = new Font(FontFamily.Sans, 12){ Bold = true },
			};
		}
		
		Control CenterLabel()
		{
			return new Label{
				Text = "Center Align",
				//BackgroundColor = Color.FromArgb (0xa0a0a0),
				HorizontalAlign = HorizontalAlign.Center
			};
		}

		Control RightLabel()
		{
			return new Label{
				Text = "Right Align",
				HorizontalAlign = HorizontalAlign.Right
			};
		}

		Control MiddleLabel()
		{
			return new Label{
				Text = "Middle Center Align",
				HorizontalAlign = HorizontalAlign.Center,
				VerticalAlign = VerticalAlign.Middle
			};
		}
		
		Control BottomLabel()
		{
			return new Label{
				Text = "Bottom Center Align",
				HorizontalAlign = HorizontalAlign.Center,
				VerticalAlign = VerticalAlign.Bottom
			};
		}
		
		Control ColorLabel()
		{
			return new Label{
				Text = "Custom Color",
				TextColor = Color.FromArgb(0xFF00a000)
			};
		}

		Control NoWrapLabel()
		{
			return new Label{
				Text = "No wrapping on this label",
				Wrap = WrapMode.None
			};
		}
		
	}
}

