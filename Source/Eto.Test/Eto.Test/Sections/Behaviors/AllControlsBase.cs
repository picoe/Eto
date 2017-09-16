using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Behaviors
{
	public class AllControlsBase : Panel
	{
		protected override void OnPreLoad(EventArgs e)
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			var options = CreateOptions();
			if (options != null)
				layout.Add(options);

			layout.BeginVertical();
			layout.AddRow(null, LabelControl(), ButtonControl(), LinkButtonControl(), new Panel(), null);
			layout.AddRow(null, TextBoxControl(), PasswordBoxControl());

			layout.BeginHorizontal();
			layout.Add(null);
			layout.Add(TextAreaControl());
			if (Platform.Supports<ListBox>())
				layout.Add(ListBoxControl());
			layout.EndHorizontal();

			layout.AddRow(null, CheckBoxControl(), RadioButtonControl());
			layout.AddRow(null, DateTimeControl(), DropDownControl(), ComboBoxControl());
			layout.AddRow(null, NumericStepperControl(), TextStepperControl());

			layout.BeginHorizontal();
			layout.Add(null);
			layout.Add(ColorPickerControl());
			layout.Add(SliderControl());
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(null);
			layout.Add(DrawableControl());
			layout.Add(ImageViewControl());
			if (Platform.Supports<GroupBox>())
				layout.Add(GroupBoxControl());
			layout.Add(TableLayoutControl());
			layout.EndHorizontal();
			layout.EndVertical();
			layout.Add(null);

			Content = layout;

			base.OnPreLoad(e);
		}

		protected virtual Control CreateOptions()
		{
			return null;
		}

		Control LabelControl()
		{
			var control = new Label { Text = "Label Control" };
			LogEvents(control);
			return control;
		}

		Control ButtonControl()
		{
			var control = new Button { Text = "Button Control" };
			LogEvents(control);
			return control;
		}

		Control TextBoxControl()
		{
			var control = new TextBox { Text = "TextBox Control" };
			LogEvents(control);
			return control;
		}

		Control PasswordBoxControl()
		{
			var control = new PasswordBox { Text = "PasswordBox Control" };
			LogEvents(control);
			return control;
		}

		Control TextAreaControl()
		{
			var control = new TextArea { Text = "TextArea Control" };
			LogEvents(control);
			return control;
		}

		Control CheckBoxControl()
		{
			var control = new CheckBox { Text = "CheckBox Control" };
			LogEvents(control);
			return control;
		}

		Control RadioButtonControl()
		{
			var control = new RadioButton { Text = "RadioButton Control" };
			LogEvents(control);
			return control;
		}

		Control DateTimeControl()
		{
			var control = new DateTimePicker();
			LogEvents(control);
			return control;
		}

		Control NumericStepperControl()
		{
			var control = new NumericStepper();
			LogEvents(control);
			return control;
		}

		Control TextStepperControl()
		{
			var control = new TextStepper();
			LogEvents(control);
			return control;
		}

		Control DropDownControl()
		{
			var control = new DropDown();
			control.Items.Add(new ListItem { Text = "Item 1" });
			control.Items.Add(new ListItem { Text = "Item 2" });
			control.Items.Add(new ListItem { Text = "Item 3" });
			control.SelectedKey = "Item 1";
			LogEvents(control);
			return control;
		}

		Control ComboBoxControl()
		{
			var control = new ComboBox();
			control.Items.Add(new ListItem { Text = "Item 1" });
			control.Items.Add(new ListItem { Text = "Item 2" });
			control.Items.Add(new ListItem { Text = "Item 3" });
			control.SelectedKey = "Item 1";
			LogEvents(control);
			return control;
		}

		Control ListBoxControl()
		{
			var control = new ListBox();
			control.Items.Add(new ListItem { Text = "Item 1" });
			control.Items.Add(new ListItem { Text = "Item 2" });
			control.Items.Add(new ListItem { Text = "Item 3" });
			LogEvents(control);
			return control;
		}

		Control TableLayoutControl()
		{
			Func<Panel> createPanel = () => new Panel { Size = new Size(50, 20), BackgroundColor = Colors.Green };

			var control = new TableLayout(
				new TableRow(createPanel(), createPanel()),
				new TableRow(createPanel(), createPanel()),
				new TableRow(createPanel(), createPanel()),
				new TableRow(createPanel(), createPanel())
			);

			control.Spacing = new Size(5, 5);
			control.Padding = 10;
			control.BackgroundColor = Colors.Blue;

			LogEvents(control);
			return control;
		}


		Control DrawableControl()
		{
			var control = new Drawable { Size = new Size(100, 30), CanFocus = true };
			control.Paint += delegate(object sender, PaintEventArgs pe)
			{
				pe.Graphics.FillRectangle(Brushes.Blue, pe.ClipRectangle);
				var size = pe.Graphics.MeasureString(SystemFonts.Label(), "Drawable");
				pe.Graphics.DrawText(SystemFonts.Label(), Brushes.White, (PointF)((control.Size - size) / 2), "Drawable");
			};
			LogEvents(control);
			return control;
		}

		Control GroupBoxControl()
		{
			var control = new GroupBox { Text = "Some Group Box" };
			control.Content = new Label { Text = "Content" };
			LogEvents(control);
			return control;
		}

		Control LinkButtonControl()
		{
			var control = new LinkButton { Text = "Link Button" };
			LogEvents(control);
			return control;
		}

		Control SliderControl()
		{
			var control = new Slider();
			LogEvents(control);
			return control;
		}

		Control ColorPickerControl()
		{
			var control = new ColorPicker();
			LogEvents(control);
			return TableLayout.AutoSized(control, centered: true);
		}

		Control ImageViewControl()
		{
			var control = new ImageView();
			control.Image = TestIcons.TestImage;
			LogEvents(control);
			return control;
		}

		protected virtual void LogEvents(Button control)
		{
			control.Click += delegate
			{
				Log.Write(control, "Click");
			};

			LogEvents((Control)control);
		}

		protected virtual void LogEvents(LinkButton control)
		{
			control.Click += delegate
			{
				Log.Write(control, "Click");
			};

			LogEvents((Control)control);
		}

		protected virtual void LogEvents(RadioButton control)
		{
			control.CheckedChanged += delegate
			{
				Log.Write(control, "CheckedChanged");
			};

			LogEvents((Control)control);
		}

		protected virtual void LogEvents(CheckBox control)
		{
			control.CheckedChanged += delegate
			{
				Log.Write(control, "CheckedChanged");
			};

			LogEvents((Control)control);
		}

		protected virtual void LogEvents(Slider control)
		{
			control.ValueChanged += delegate
			{
				Log.Write(control, "ValueChanged");
			};

			LogEvents((Control)control);
		}

		protected virtual void LogEvents(Control control)
		{
		}
	}
}

