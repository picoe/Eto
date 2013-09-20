using System;
using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;

namespace Eto.Test.Sections.Controls
{
	public class ButtonSection : Scrollable, INotifyPropertyChanged
	{
		Bitmap smallImage = new Bitmap(Bitmap.FromResource("Eto.Test.TestImage.png"), 16, 16);
		Bitmap largeImage = Bitmap.FromResource("Eto.Test.TestImage.png");
		ButtonImagePosition imagePosition;

		public ButtonImagePosition ImagePosition
		{
			get { return imagePosition; }
			set
			{
				imagePosition = value;
				OnPropertyChanged(new PropertyChangedEventArgs("ImagePosition"));
			}
		}

		public ButtonSection()
		{
			ExpandContentWidth = true;
			ExpandContentHeight = true;
			var layout = new DynamicLayout();

			layout.AddAutoSized(NormalButton(), centered: true);
			layout.AddAutoSized(LongerButton(), centered: true);
			layout.AddAutoSized(DefaultSizeButton(), centered: true);
			layout.AddAutoSized(ColourButton(), centered: true);
			layout.AddAutoSized(DisabledButton(), centered: true);
			layout.Add(StretchedButton());
			layout.AddSeparateRow(null, new Label { Text = "Image Position:", VerticalAlign = VerticalAlign.Middle }, ImagePositionControl(), null);
			layout.AddSeparateRow(null, TableLayout.AutoSized(ImageButton(smallImage)), TableLayout.AutoSized(ImageTextButton(smallImage)), null);
			layout.AddSeparateRow(null, TableLayout.AutoSized(ImageButton(largeImage)), TableLayout.AutoSized(ImageTextButton(largeImage)), null);

			layout.Add(null);

			Content = layout;
		}

		Control NormalButton()
		{
			var control = new Button { Text = "Click Me" };
			LogEvents(control);
			return control;
		}

		Control StretchedButton()
		{
			var control = new Button { Text = "A stretched button" };
			LogEvents(control);
			return control;
		}

		Control LongerButton()
		{
			var control = new Button { Text = "This is a long(er) button title" };
			LogEvents(control);
			return control;
		}

		Control DefaultSizeButton()
		{
			var old = Button.DefaultSize;
			Button.DefaultSize = new Size(50, 50);
			var control = new Button { Text = "B" };
			LogEvents(control);
			var control2 = new Button { Text = "Button With Text" };
			LogEvents(control2);
			Button.DefaultSize = old;

			var layout = new DynamicLayout(Padding.Empty);
			layout.AddRow(new Label { Text = "With Default Size of 50x50:", VerticalAlign = VerticalAlign.Middle }, control, control2);
			return layout;
		}

		Control ColourButton()
		{
			var control = new Button { Text = "Button with Color", BackgroundColor = Colors.Lime };
			LogEvents(control);
			return control;
		}

		Control DisabledButton()
		{
			var control = new Button { Text = "Disabled Button", Enabled = false };
			LogEvents(control);
			return control;
		}

		Control ImageButton(Image image)
		{
			var control = new Button { Image = image };
			control.Bind(r => r.ImagePosition, this, r => r.ImagePosition);
			LogEvents(control);
			return control;
		}

		Control ImageTextButton(Image image)
		{
			var control = new Button { Text = "Image && Text Button", Image = image };
			control.Bind(r => r.ImagePosition, this, r => r.ImagePosition);
			LogEvents(control);
			return control;
		}

		Control ImagePositionControl()
		{
			var control = new EnumComboBox<ButtonImagePosition>();
			control.Bind(r => r.SelectedValue, this, r => r.ImagePosition);
			return control;
		}

		void LogEvents(Button button)
		{
			button.Click += delegate
			{
				Log.Write(button, "Click");
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}

