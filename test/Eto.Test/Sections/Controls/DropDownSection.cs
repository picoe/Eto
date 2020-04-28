using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(DropDown))]
	public class DropDownSection : Scrollable
	{
		bool AddWithImages { get; set; }

		public DropDownSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow("Default", Default(), null);

			layout.AddRow("Set Initial Value", TableLayout.AutoSized(SetInitialValue()));

			layout.AddRow("EnumDropDown<Key>", TableLayout.AutoSized(EnumCombo()));

			layout.AddRow("FormatItem", TableLayout.AutoSized(DropDownWithFonts()));

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new DropDown();
			LogEvents(control);

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			layout.Add(TableLayout.AutoSized(control));
			layout.AddSeparateRow(AddRowsButton(control), AddWithImagesCheckBox(), RemoveRowsButton(control), ClearButton(control), null);
			layout.AddSeparateRow(EnabledCheckBox(control), SetSelected(control), ClearSelected(control), null);

			return layout;
		}

		Control EnabledCheckBox(DropDown list)
		{
			var control = new CheckBox { Text = "Enabled" };
			control.CheckedBinding.Bind(list, l => l.Enabled);
			return control;
		}

		Control AddWithImagesCheckBox()
		{
			var control = new CheckBox { Text = "Add with images" };
			control.CheckedBinding.Bind(this, l => l.AddWithImages);
			return control;
		}


		Control AddRowsButton(DropDown list)
		{
			var control = new Button { Text = "Add Rows" };
			control.Click += (sender, e) =>
			{
				var image1 = TestIcons.Logo.WithSize(32, 32);
				var image2 = TestIcons.TestIcon.WithSize(16, 16);
				for (int i = 0; i < 10; i++)
				{
					if (AddWithImages)
						list.Items.Add(new ImageListItem { Text = "Item " + list.Items.Count, Image = i % 2 == 0 ? image1 : image2 });
					else
						list.Items.Add(new ListItem { Text = "Item " + list.Items.Count });
				}
			};
			return control;
		}

		Control RemoveRowsButton(DropDown list)
		{
			var control = new Button { Text = "Remove Rows" };
			control.Click += delegate
			{
				if (list.SelectedIndex >= 0)
					list.Items.RemoveAt(list.SelectedIndex);
			};
			return control;
		}

		Control ClearButton(DropDown list)
		{
			var control = new Button { Text = "Clear" };
			control.Click += delegate
			{
				list.Items.Clear();
			};
			return control;
		}

		Control SetSelected(DropDown list)
		{
			var control = new Button { Text = "Set Selected" };
			control.Click += delegate
			{
				if (list.Items.Count > 0)
					list.SelectedIndex = new Random().Next(list.Items.Count - 1);
			};
			return control;
		}


		Control ClearSelected(DropDown list)
		{
			var control = new Button { Text = "Clear Selected" };
			control.Click += delegate
			{
				list.SelectedIndex = -1;
			};
			return control;
		}

		DropDown Items()
		{
			var control = new DropDown();
			LogEvents(control);
			for (int i = 0; i < 20; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}
			return control;
		}

		Control DropDownWithFonts()
		{
			var fontCache = new Dictionary<FontFamily, Font>();
			var dropDown = new DropDown();
			dropDown.DataStore = Fonts.AvailableFontFamilies.OrderBy(r => r.LocalizedName).ToList();
			dropDown.ItemTextBinding = Binding.Property((FontFamily f) => f.LocalizedName);
			dropDown.FormatItem += (sender, e) =>
			{
				if (e.Item is FontFamily family)
				{
					if (!fontCache.TryGetValue(family, out var font))
					{
						if (Platform.IsGtk && !EtoEnvironment.Platform.IsLinux)
						{
							// gtksharp has issues getting font faces on !linux
							font = new Font(family, e.Font?.Size ?? SystemFonts.Default().Size);
						}
						else
						{
							var typeface = family.Typefaces.FirstOrDefault();
							if (typeface != null && !typeface.IsSymbol && typeface.HasCharacterRange(32, 126))
							{
								font = new Font(family, e.Font?.Size ?? SystemFonts.Default().Size);
							}
							else
								font = SystemFonts.Default();
						}
						fontCache[family] = font;
					}
					e.Font = font;
				}
			};

			return dropDown;
		}


		DropDown SetInitialValue()
		{
			var control = Items();
			control.SelectedKey = "Item 8";
			return control;
		}

		Control EnumCombo()
		{
			var control = new EnumDropDown<Keys>();
			control.Width = 100;
			LogEvents(control);
			control.SelectedKey = ((int)Keys.E).ToString();
			return control;
		}

		void LogEvents(DropDown control)
		{
			control.SelectedIndexChanged += (sender, e) =>  Log.Write(control, "SelectedIndexChanged, Value: {0}", control.SelectedIndex);

			control.DropDownOpening += (sender, e) => Log.Write(control, "DropDownOpening");
			control.DropDownClosed += (sender, e) => Log.Write(control, "DropDownClosed");
		}
	}
}

