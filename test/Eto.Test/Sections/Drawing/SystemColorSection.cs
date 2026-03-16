namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", typeof(SystemColors))]
	public class SystemColorSection : Panel, INotifyPropertyChanged
	{
		Scrollable controlContent;

		public event PropertyChangedEventHandler PropertyChanged;

		class ControlType
		{
			public static ControlType Create<T>(Action<T> mod = null, Action<T, string> set = null, Func<T, Color> getTextColor = null, Action<T, Color> setTextColor = null)
				where T: Control, new()
			{
				return new ControlType
				{
					CreateControl = () => {
						var c = new T();
						mod?.Invoke(c);
						return c;
					},
					SetText = (c, v) => set?.Invoke((T)c, v),
					GetTextColor = c => getTextColor?.Invoke((T)c),
					SetTextColor = (c, v) => setTextColor?.Invoke((T)c, v),
					Name = typeof(T).Name
				};
			}
			public Action<Control, string> SetText { get; set; }
			public Func<Control> CreateControl { get; set; }
			public Func<Control, Color?> GetTextColor { get; set; }
			public Action<Control, Color> SetTextColor { get; set; }
			public string Name { get; set; }

			public override string ToString() => Name;
		}

		IEnumerable<ControlType> GetControlTypes()
		{
			yield return ControlType.Create<Drawable>(c =>
			{
				c.Paint += (sender, e) =>
				{
					var color = c.BackgroundColor;
					var g = e.Graphics;
					var rect = new RectangleF(c.Size);
					rect.Inset(4);
					g.FillRectangle(color, rect.X, rect.Y, rect.Width / 2, rect.Height);
					g.FillRectangle(Color.FromArgb(color.ToArgb()), rect.X + rect.Width / 2, rect.Y, rect.Width / 2, rect.Height);
				};
				c.Size = new Size(100, 20);
			});
			yield return ControlType.Create<Label>(set: (c,v) => c.Text = v, getTextColor: c => c.TextColor, setTextColor: (c, v) => c.TextColor = v);
			yield return ControlType.Create<TextBox>(set: (c, v) => c.Text = v, getTextColor: c => c.TextColor, setTextColor: (c, v) => c.TextColor = v);
			yield return ControlType.Create<TextArea>(set: (c, v) => c.Text = v, getTextColor: c => c.TextColor, setTextColor: (c, v) => c.TextColor = v);
			yield return ControlType.Create<DropDown>(c => c.DataStore = new[] { "Item 1", "Item 2", "Item 3" });
			yield return ControlType.Create<LinkButton>(set: (c, v) => c.Text = v, getTextColor: c => c.TextColor, setTextColor: (c, v) => c.TextColor = v);
		}

		ControlType _selectedControlType;
		ControlType SelectedControlType
		{
			get => _selectedControlType;
			set
			{
				_selectedControlType = value;
				controlContent.Content = CreateControlContent(value);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedControlType)));
			}
		}

		public SystemColorSection()
		{
			controlContent = new Scrollable();

			var types = GetControlTypes().ToList();
			var typeDropDown = new DropDown { DataStore = types, ItemTextBinding = Binding.Property((ControlType m) => m.Name) };
			typeDropDown.SelectedValueBinding.Bind(this, c => c.SelectedControlType);
			SelectedControlType = types.First();

			var layout = new DynamicLayout();

			layout.BeginCentered();
			layout.AddRow("Control Type:", typeDropDown);
			layout.EndCentered();
			layout.Add(controlContent, yscale: true);

			Content = layout;
		}

		Control CreateControlContent(ControlType controlType)
		{
			if (controlType == null)
				return null;
			var layout = new DynamicLayout
			{
				DefaultSpacing = new Size(10, 10)
			};

			var type = typeof(SystemColors);

			var properties = type.GetRuntimeProperties();

			var skip = new List<PropertyInfo>();
			var colorProperties = properties.Where(r => r.PropertyType == typeof(Color)).OrderBy(r => r.Name).ToList();
			layout.BeginCentered();
			layout.AddRow("Panel", controlType.Name, "RGB");

			var defaultControl = controlType.CreateControl();
			var defaultLabel = new Label { Text = "Default" };
			var defaultTextColor = controlType.GetTextColor?.Invoke(defaultControl);
			var defaultTextColorName = $"bg:{defaultControl.BackgroundColor}";
			if (defaultTextColor != null)
				defaultTextColorName = $"{defaultTextColorName} fg:{defaultTextColor}";
			controlType.SetText?.Invoke(defaultControl, "Default");
			layout.AddRow(new Panel { Content = defaultLabel }, defaultControl, defaultTextColorName);
			foreach (var property in colorProperties)
			{
				if (skip.Contains(property))
					continue;
				var color = () => (Color)property.GetValue(null);
				var textColor = () => Colors.Black;
				var name = property.Name;
				var colorName = () => color().ToString();
				var colorNameText = colorName;
				var label = new Label();
				var panel = new Panel
				{
					Content = label,
					Padding = new Padding(10),
				};
				var control = controlType.CreateControl();

				bool isTextColor = property.Name.EndsWith("Text", StringComparison.Ordinal);

				if (isTextColor)
				{
					textColor = color;
					name = $"fg:{name}";
				}
				else
				{
					colorNameText = () => $"bg:{colorName()}";
					panel.BackgroundColor = color();
					panel.ThemeChanged += (sender, e) => panel.BackgroundColor = color();
					control.BackgroundColor = color();
					control.ThemeChanged += (sender, e) => control.BackgroundColor = color();
					var textProp = colorProperties.FirstOrDefault(r => r.Name == property.Name + "Text");
					if (textProp != null)
					{
						textColor = () => (Color)textProp.GetValue(null);
						name = $"{name}, {textProp.Name}";
						colorNameText = () => $"bg:{colorName()}, fg:{textColor()}";
						skip.Add(textProp);
					}
					else
					{
						textColor = () => color().ToHSB().B < 0.5 ? Colors.White : Colors.Black;
					}
				}
				controlType.SetTextColor?.Invoke(control, textColor());
				control.ThemeChanged += (sender, e) => controlType.SetTextColor?.Invoke(control, textColor());
				controlType.SetText?.Invoke(control, name);
				label.TextColor = textColor();
				label.ThemeChanged += (sender, e) => label.TextColor = textColor();
				label.Text = name;
				
				var colorNameLabel = new Label { Text = colorNameText() };
				colorNameLabel.ThemeChanged += (sender, e) => colorNameLabel.Text = colorNameText();

				layout.AddRow(panel, control, colorNameLabel);
			}
			layout.EndCentered();
			layout.AddSpace();

			return layout;
		}
	}
}

