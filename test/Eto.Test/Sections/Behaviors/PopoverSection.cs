using Eto.Forms.ThemedControls;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", typeof(Popover))]
	public class PopoverSection : Panel
	{
		public PopoverPosition Position { get; set; } = PopoverPosition.Bottom;
		public PopoverBehavior Behavior { get; set; } = PopoverBehavior.Transient;
		public bool UseThemed { get; set; }

		Popover _nativePopover;
		Popover _themedPopover;
		static Platform s_themedPlatform;

		public PopoverSection()
		{
			var positionDropDown = new EnumDropDown<PopoverPosition>();
			positionDropDown.SelectedValueBinding.Bind(this, c => c.Position);

			var behaviorDropDown = new EnumDropDown<PopoverBehavior>();
			behaviorDropDown.SelectedValueBinding.Bind(this, c => c.Behavior);

			var useThemedCheckBox = new CheckBox { Text = "Use themed popover" };
			useThemedCheckBox.CheckedBinding.Bind(this, c => c.UseThemed);

			// click anywhere to point at that spot, to try areas and placement near the edges of the screen
			var area = new Drawable { Size = new Size(300, 150), BackgroundColor = SystemColors.ControlBackground };
			area.Paint += (sender, e) => e.Graphics.DrawText(SystemFonts.Default(), SystemColors.ControlText, 10, 10, "Click to show a popover here");
			area.MouseDown += (sender, e) =>
			{
				if (e.Buttons != MouseButtons.Primary)
					return;
				e.Handled = true;
				Show(area, new RectangleF(e.Location - 2, new SizeF(4, 4)));
			};

			Content = new StackLayout
			{
				Padding = 10,
				Spacing = 5,
				Items =
				{
					new TableLayout
					{
						Spacing = new Size(5, 5),
						Rows =
						{
							new TableRow("Position", positionDropDown),
							new TableRow("Behavior", behaviorDropDown),
							new TableRow(null, useThemedCheckBox)
						}
					},
					new StackLayoutItem(new TableLayout
					{
						Spacing = new Size(5, 5),
						Rows =
						{
							new TableRow(CreateButton(), null, CreateButton()),
							new TableRow(null, area, null),
							new TableRow(CreateButton(), null, CreateButton())
						}
					}, HorizontalAlignment.Stretch)
				}
			};
		}

		Button CreateButton()
		{
			var button = new Button { Text = "Show Popover" };
			button.Click += (sender, e) => Show(button, null);
			return button;
		}

		void Show(Control relativeTo, RectangleF? rectangle)
		{
			var popover = UseThemed ? (_themedPopover ??= CreatePopover(true)) : (_nativePopover ??= CreatePopover(false));
			popover.Behavior = Behavior;
			popover.Show(relativeTo, rectangle, Position);
		}

		Popover CreatePopover(bool themed)
		{
			Popover popover;
			if (themed)
			{
				if (s_themedPlatform == null)
				{
					s_themedPlatform = (Platform)Activator.CreateInstance(Platform.Instance.GetType());
					s_themedPlatform.Add<Popover.IHandler>(() => new ThemedPopoverHandler());
				}
				using (s_themedPlatform.Context)
					popover = new Popover();
			}
			else
				popover = new Popover();

			var closeButton = new Button { Text = "Close" };
			closeButton.Click += (sender, e) => popover.Close();

			var slider = new Slider { Width = 150 };
			var valueLabel = new Label();
			slider.ValueChanged += (sender, e) => valueLabel.Text = slider.Value.ToString();

			popover.Content = new TableLayout
			{
				Padding = 10,
				Spacing = new Size(5, 5),
				Rows =
				{
					new Label { Text = "Popover content", Font = SystemFonts.Bold() },
					new TextBox { PlaceholderText = "Type here" },
					new TableLayout(new TableRow(slider, valueLabel)),
					new CheckBox { Text = "A check box" },
					new TableLayout(new TableRow(null, closeButton))
				}
			};
			popover.Content.Load += (sender, e) => Log.Write(popover, "Content.Load");
			popover.Content.UnLoad += (sender, e) => Log.Write(popover, "Content.UnLoad");
			popover.Shown += (sender, e) => Log.Write(popover, "Shown");
			popover.Closed += (sender, e) => Log.Write(popover, "Closed");
			return popover;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_nativePopover?.Dispose();
				_themedPopover?.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
