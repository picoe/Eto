using Eto.Forms;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(ColorPicker))]
	public class ColorPickerSection : Panel, INotifyPropertyChanged
	{
		bool allowAlpha;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool AllowAlpha
		{
			get { return allowAlpha; }
			set
			{
				if (allowAlpha != value)
				{
					allowAlpha = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllowAlpha)));
				}
			}
		}

		public ColorPickerSection()
		{
			Content = new StackLayout
			{
				Spacing = 5,
				Padding = 10,
				Items = 
				{
					CreateAllowAlpha(),
					TableLayout.Horizontal(5, "Default", Default()),
					TableLayout.Horizontal(5, "Initial Value", InitialValue())
				}
			};
		}

		Control CreateAllowAlpha()
		{
			var checkBox = new CheckBox { Text = "AllowAlpha" };
			checkBox.CheckedBinding.Bind(this, c => c.AllowAlpha);
			return checkBox;
		}

		Control Default()
		{
			var control = new ColorPicker();
			control.Bind(c => c.AllowAlpha, this, c => c.AllowAlpha);
			LogEvents(control);
			return control;
		}

		Control InitialValue()
		{
			var control = new ColorPicker { Value = Colors.Blue };
			control.Bind(c => c.AllowAlpha, this, c => c.AllowAlpha);
			LogEvents(control);
			return control;
		}

		void LogEvents(ColorPicker control)
		{
			control.ValueChanged += delegate
			{
				Log.Write(control, "SelectedColorChanged, Color: {0}", control.Value);
			};
		}
	}
}

