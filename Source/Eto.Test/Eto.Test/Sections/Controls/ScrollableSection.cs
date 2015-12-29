using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Scrollable))]
	public class ScrollableSection : Panel
	{
		public ScrollableSection()
		{
			var scrollable = CreateScrollable();

			var borderType = new EnumDropDown<BorderType>();
			borderType.SelectedValueBinding.Bind(scrollable, r => r.Border);

			var expandWidth = new CheckBox { Text = "Width" };
			expandWidth.CheckedBinding.Bind(scrollable, r => r.ExpandContentWidth);

			var expandHeight = new CheckBox { Text = "Height" };
			expandHeight.CheckedBinding.Bind(scrollable, r => r.ExpandContentHeight);

			var sizeMode = new DropDown
			{
				Items = { "Auto", "800x800", "30x30", "1x1" },
				SelectedIndex = 0
			};
			sizeMode.SelectedIndexChanged += (sender, e) => 
			{
				switch (sizeMode.SelectedIndex) {
                    case 0:
                        scrollable.Content.Size = new Size(-1, -1);
                        break;
                    case 1:
						scrollable.Content.Size = new Size(800, 800);
						break;
                    case 2:
                        scrollable.Content.Size = new Size(30, 30);
                        break;
                    case 3:
                        scrollable.Content.Size = new Size(1, 1);
                        break;

                }
            };

			var options = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Center,
				Spacing = 5,
				Items = { 
					"Border:",
					borderType,
					"Expand:",
					expandWidth,
					expandHeight,
					"Content Size:",
					sizeMode
				}
			};

			Content = new StackLayout
			{
				Padding = 10,
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{ 
					new StackLayoutItem(options, HorizontalAlignment.Center), 
					new StackLayoutItem(scrollable, true)
				}
			};
		}

		Scrollable CreateScrollable()
		{
			var scrollable = new Scrollable { Size = new Size(100, 200) };
			LogEvents(scrollable);

			var desc = new Label
			{ 
				Text = "Content",
				BackgroundColor = Colors.Green,
				Size = new Size(400, 400),
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			var content = new Panel
			{
				BackgroundColor = Colors.Red, // should not see red
				Content = desc
			};

			scrollable.Content = content;
			return scrollable;
		}

		void LogEvents(Scrollable control)
		{
			control.Scroll += (sender, e) => Log.Write(control, "Scroll, ScrollPosition: {0}", e.ScrollPosition);
		}
	}
}

