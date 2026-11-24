namespace Eto.Test.Sections.Layouts
{
	[Section("Layouts", "LayoutDirection")]
	public class LayoutDirectionSection : Panel
	{
		public LayoutDirectionSection()
		{
			var sample = new Panel { Content = CreateSample() };

			var effective = new Label();
			void UpdateEffective() => effective.Text = $"Effective: {sample.EffectiveLayoutDirection} (application: {Application.Instance.EffectiveLayoutDirection})";
			sample.EffectiveLayoutDirectionChanged += (sender, e) => UpdateEffective();
			UpdateEffective();

			var appDirection = new EnumDropDown<LayoutDirection> { SelectedValue = Application.Instance.LayoutDirection };
			appDirection.SelectedValueChanged += (sender, e) =>
			{
				Application.Instance.LayoutDirection = appDirection.SelectedValue;
				UpdateEffective();
			};

			var sampleDirection = new EnumDropDown<LayoutDirection>();
			sampleDirection.SelectedValueBinding.Bind(sample, s => s.LayoutDirection);

			Content = new TableLayout
			{
				Padding = 10,
				Spacing = new Size(5, 5),
				Rows =
				{
					new StackLayout
					{
						Orientation = Orientation.Horizontal,
						VerticalContentAlignment = VerticalAlignment.Center,
						Spacing = 5,
						Items = { "Application:", appDirection, "Sample:", sampleDirection, effective }
					},
					new TableRow(sample) { ScaleHeight = true }
				}
			};
		}

		static Control Box(string text, Color color) => new Panel { BackgroundColor = color, Padding = new Padding(8, 4), Content = new Label { Text = text } };

		Control CreateSample()
		{
			var layout = new DynamicLayout { Padding = 10, Spacing = new Size(10, 10) };

			layout.BeginHorizontal();
			layout.BeginVertical(spacing: new Size(5, 5));
			layout.AddRow(new Label { Text = "Labels" });
			layout.AddRow(new Label { Text = "Left", TextAlignment = TextAlignment.Left, BackgroundColor = Colors.LightYellow, TextColor = Colors.Black });
			layout.AddRow(new Label { Text = "Center", TextAlignment = TextAlignment.Center, BackgroundColor = Colors.LightYellow, TextColor = Colors.Black });
			layout.AddRow(new Label { Text = "Right", TextAlignment = TextAlignment.Right, BackgroundColor = Colors.LightYellow, TextColor = Colors.Black });
			layout.AddRow(new TextBox { Text = "Left", TextAlignment = TextAlignment.Left });
			layout.AddRow(new TextBox { Text = "Right", TextAlignment = TextAlignment.Right });
			layout.AddRow(new NumericStepper { Value = 10 });
			layout.AddRow(new SearchBox { PlaceholderText = "Search" });
			layout.AddRow(new DateTimePicker { Value = DateTime.Now });
			layout.EndVertical();

			layout.BeginVertical(spacing: new Size(5, 5));
			layout.AddRow(new Label { Text = "Buttons" });
			layout.AddRow(new Button { Text = "Image Left", Image = TestIcons.TestIcon.WithSize(16, 16), ImagePosition = ButtonImagePosition.Left });
			layout.AddRow(new Button { Text = "Image Right", Image = TestIcons.TestIcon.WithSize(16, 16), ImagePosition = ButtonImagePosition.Right });
			layout.AddRow(new CheckBox { Text = "Check box" });
			layout.AddRow(new RadioButtonList { Items = { "One", "Two" } });
			var dropDown = new DropDown();
			dropDown.Items.Add(new ImageListItem { Text = "With image", Image = TestIcons.TestIcon.WithSize(16, 16) });
			dropDown.SelectedIndex = 0;
			layout.AddRow(dropDown);
			layout.AddRow(new ComboBox { Items = { "Combo box" }, SelectedIndex = 0 });
			layout.AddRow(new Slider { Value = 25 });
			layout.AddRow(new ProgressBar { Value = 25 });
			layout.EndVertical();

			layout.BeginVertical(spacing: new Size(5, 5));
			layout.AddRow(new Label { Text = "Layouts" });
			layout.AddRow(new TableLayout { Spacing = new Size(5, 5), Rows = { new TableRow(Box("1", Colors.LightBlue), Box("2", Colors.LightGreen), Box("3", Colors.Pink)) } });
			layout.AddRow(new StackLayout { Orientation = Orientation.Horizontal, Spacing = 5, Items = { Box("A", Colors.LightBlue), Box("B", Colors.LightGreen), Box("C", Colors.Pink) } });
			var pixel = new PixelLayout { Size = new Size(160, 60), BackgroundColor = Colors.WhiteSmoke };
			pixel.Add(Box("x=10", Colors.LightBlue), 10, 5);
			pixel.Add(Box("x=80", Colors.LightGreen), 80, 30);
			layout.AddRow(pixel);
			layout.AddRow(new Splitter { Size = new Size(200, 50), Position = 60, Panel1 = Box("Panel1", Colors.LightBlue), Panel2 = Box("Panel2", Colors.LightGreen) });
			layout.AddRow(new GroupBox { Text = "Group box", Padding = new Padding(20, 5, 5, 5), Content = "Padding left 20" });
			layout.AddRow(new Expander { Header = "Expander", Expanded = true, Content = "Expander content" });
			layout.EndVertical();
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(new TabControl
			{
				Size = new Size(250, 100),
				Pages =
				{
					new TabPage { Text = "First", Image = TestIcons.TestIcon.WithSize(16, 16), Padding = new Padding(20, 5, 5, 5), Content = "Padding left 20" },
					new TabPage { Text = "Second", Content = "Second page" }
				}
			});
			layout.Add(CreateGrid());
			layout.Add(CreateTreeGrid());
			layout.EndHorizontal();

			layout.BeginHorizontal();
			var listBox = new ListBox { Size = new Size(150, 80) };
			listBox.Items.Add(new ImageListItem { Text = "Item with image", Image = TestIcons.TestIcon.WithSize(16, 16) });
			listBox.Items.Add(new ImageListItem { Text = "Second item" });
			layout.Add(listBox);
			layout.Add(new DocumentControl
			{
				Size = new Size(250, 100),
				Pages =
				{
					new DocumentPage { Text = "Document", Image = TestIcons.TestIcon.WithSize(16, 16), Content = "Document page" },
					new DocumentPage { Text = "Other", Content = "Other page" }
				}
			});
			layout.Add(CreateDrawable());
			layout.EndHorizontal();
			layout.Add(null);

			return new Scrollable { Content = layout, Border = BorderType.None };
		}

		static Control CreateGrid()
		{
			var grid = new GridView { Size = new Size(250, 100) };
			grid.Columns.Add(new GridColumn { HeaderText = "Image text", DataCell = new ImageTextCell(0, 1) });
			grid.Columns.Add(new GridColumn { HeaderText = "Check", DataCell = new CheckBoxCell(2) });
			grid.Columns.Add(new GridColumn { HeaderText = "Right", HeaderTextAlignment = TextAlignment.Right, DataCell = new TextBoxCell(1) { TextAlignment = TextAlignment.Right } });
			var image = TestIcons.TestIcon.WithSize(16, 16);
			grid.DataStore = new[]
			{
				new object[] { image, "First", true },
				new object[] { image, "Second", false }
			};
			return grid;
		}

		static Control CreateTreeGrid()
		{
			var tree = new TreeGridView { Size = new Size(200, 100) };
			tree.Columns.Add(new GridColumn { HeaderText = "Tree", DataCell = new ImageTextCell(0, 1) });
			var image = TestIcons.TestIcon.WithSize(16, 16);
			var root = new TreeGridItem(image, "Parent") { Expanded = true };
			root.Children.Add(new TreeGridItem(image, "Child"));
			tree.DataStore = new TreeGridItemCollection { root, new TreeGridItem(image, "Sibling") };
			return tree;
		}

		static Control CreateDrawable()
		{
			var drawable = new Drawable { Size = new Size(200, 80), BackgroundColor = Colors.White };
			drawable.EffectiveLayoutDirectionChanged += (sender, e) => drawable.Invalidate();
			drawable.Paint += (sender, e) =>
			{
				// drawing is never mirrored, so this arrow always points right from x=0
				var g = e.Graphics;
				g.FillRectangle(Colors.LightBlue, 0, 0, 20, drawable.Height);
				g.DrawLine(Colors.Black, 20, 20, 120, 20);
				g.FillPolygon(Colors.Black, new PointF(120, 14), new PointF(132, 20), new PointF(120, 26));
				g.DrawText(SystemFonts.Default(), Colors.Black, 24, 30, "x=0 is on the left");
				g.DrawText(SystemFonts.Default(), Colors.Black, 24, 50, drawable.EffectiveLayoutDirection.ToString());
			};
			return drawable;
		}
	}
}
