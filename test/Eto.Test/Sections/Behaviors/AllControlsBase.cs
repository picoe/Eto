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
			layout.AddRow(null, LabelControl(), ButtonControl(), ToggleButtonControl(), LinkButtonControl(), new Panel(), null);
			layout.AddRow(null, TextBoxControl(), PasswordBoxControl(), SegmentedButtonControl());

			layout.BeginHorizontal();
			layout.Add(null);
			layout.Add(TextAreaControl());
			if (Platform.Supports<ListBox>())
				layout.Add(ListBoxControl());
			layout.Add(PanelControl());
			layout.EndHorizontal();

			layout.AddRow(null, CheckBoxControl(), RadioButtonControl());
			layout.AddRow(null, DateTimeControl(), DropDownControl(), ComboBoxControl());
			layout.AddRow(null, NumericStepperControl(), TextStepperControl(), StepperControl());

			layout.AddRow(null, ColorPickerControl(), SliderControl());

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
			var control = new Label { Text = "Label" };
			LogEvents(control);
			return control;
		}

		Control ButtonControl()
		{
			var control = new Button { Text = "Button" };
			LogEvents(control);
			return control;
		}

		Control ToggleButtonControl()
		{
			var control = new ToggleButton { Text = "ToggleButton " };
			LogEvents(control);
			return control;
		}

		Control TextBoxControl()
		{
			var control = new TextBox { Text = "TextBox" };
			LogEvents(control);
			return control;
		}

		Control PasswordBoxControl()
		{
			var control = new PasswordBox { Text = "PasswordBox" };
			LogEvents(control);
			return control;
		}

		Control TextAreaControl()
		{
			var control = new TextArea { Text = "TextArea" };
			LogEvents(control);
			return control;
		}

		Control CheckBoxControl()
		{
			var control = new CheckBox { Text = "CheckBox" };
			LogEvents(control);
			return control;
		}

		Control RadioButtonControl()
		{
			var control = new RadioButton { Text = "RadioButton" };
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
			var control = new TextStepper { Text = "TextStepper" };
			LogEvents(control);
			return control;
		}

		Control StepperControl()
		{
			var control = new Stepper();
			LogEvents(control);
			return control;
		}

		Control DropDownControl()
		{
			var control = new DropDown();
			control.Items.Add(new ListItem { Text = "DropDown" });
			control.Items.Add(new ListItem { Text = "Item 2" });
			control.Items.Add(new ListItem { Text = "Item 3" });
			control.SelectedKey = "DropDown";
			LogEvents(control);
			return control;
		}

		Control ComboBoxControl()
		{
			var control = new ComboBox();
			control.Items.Add(new ListItem { Text = "ComboBox" });
			control.Items.Add(new ListItem { Text = "Item 2" });
			control.Items.Add(new ListItem { Text = "Item 3" });
			control.SelectedKey = "ComboBox";
			LogEvents(control);
			return control;
		}

		Control ListBoxControl()
		{
			var control = new ListBox();
			control.Items.Add(new ListItem { Text = "ListBox" });
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

		Control PanelControl()
		{
			var control = new Panel();
			control.Padding = 10;

			LogEvents(control);
			return new Panel { Content = control, BackgroundColor = Colors.Yellow };
		}



		Control DrawableControl()
		{
			var control = new Drawable { Size = new Size(100, 30), CanFocus = true };
			control.Paint += delegate(object sender, PaintEventArgs pe)
			{
				if (control.BackgroundColor.A <= 0)
					pe.Graphics.FillRectangle(Brushes.Blue, pe.ClipRectangle);
				var size = pe.Graphics.MeasureString(SystemFonts.Label(), "Drawable");
				pe.Graphics.DrawText(SystemFonts.Label(), Brushes.White, (PointF)((control.Size - size) / 2), "Drawable");
			};
			LogEvents(control);
			return control;
		}

		Control GroupBoxControl()
		{
			var control = new GroupBox { Text = "GroupBox" };
			control.Content = new Label { Text = "Content" };
			LogEvents(control);
			return control;
		}

		Control LinkButtonControl()
		{
			var control = new LinkButton { Text = "LinkButton" };
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

		Control SegmentedButtonControl()
		{
			var control = new SegmentedButton { Items = { "Item1", "Item2" } };
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

