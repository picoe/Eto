using System;
using Eto.Drawing;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// A themed handler for the <see cref="FontPicker"/> control.
	/// </summary>
	public class ThemedFontPickerHandler : ThemedControlHandler<Button, FontPicker, FontPicker.ICallback>, FontPicker.IHandler
	{
		private Font font;
		private FontDialog dialog;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.ThemedControls.ThemedFontPickerHandler"/> class.
		/// </summary>
		public ThemedFontPickerHandler()
		{
			Control = new Button();
			Control.Click += Control_Click;
			Refresh();
		}

		/// <summary>
		/// Gets or sets the currently selected font.
		/// </summary>
		/// <value>The selected font.</value>
		public Font Value
		{
			get { return font; }
			set
			{
				font = value;
				Callback.OnValueChanged(Widget, EventArgs.Empty);
				Refresh();
			}
		}

		private void Refresh()
		{
			Control.Text = font == null ? "None" : font.FamilyName + " " + font.Size;
		}

		private void Control_Click(object sender, EventArgs e)
		{
			dialog = new FontDialog();
			dialog.Font = font;
			dialog.FontChanged += Dialog_FontChanged;

			dialog.ShowDialog(Widget);
		}

		void Dialog_FontChanged(object sender, EventArgs e)
		{
			font = dialog?.Font;
			Callback.OnValueChanged(Widget, EventArgs.Empty);
			Refresh();
		}

		/// <summary>
		/// Attaches control events.
		/// </summary>
		/// <param name="id">ID of the event to attach</param>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FontPicker.ValueChangedEvent:

					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
