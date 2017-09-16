using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// Themed implementation of the <see cref="TextStepper"/> control composed of a <see cref="TextBox"/> and <see cref="Stepper"/>.
	/// </summary>
	public class ThemedTextStepperHandler : ThemedControlHandler<TableLayout, TextStepper, TextStepper.ICallback>, TextStepper.IHandler
	{
		readonly TextBox textBox;
		readonly Stepper stepper;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.ThemedControls.ThemedTextStepperHandler"/> class.
		/// </summary>
		public ThemedTextStepperHandler()
		{
			textBox = new TextBox();
			stepper = new Stepper();
			Control = TableLayout.Horizontal(
				new TableCell(new TableLayout(null, textBox, null), true),
				stepper
			);
			Control.EndInit();

			textBox.KeyDown += TextBox_KeyDown;
		}

		void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Up)
			{
				if (ValidDirection.HasFlag(StepperValidDirections.Up))
				{
					Callback.OnStep(Widget, new StepperEventArgs(StepperDirection.Up));
					e.Handled = true;
				}
			}
			else if (e.KeyData == Keys.Down)
			{
				if (ValidDirection.HasFlag(StepperValidDirections.Down))
				{
					Callback.OnStep(Widget, new StepperEventArgs(StepperDirection.Down));
					e.Handled = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the index of the current insertion point.
		/// </summary>
		/// <remarks>
		/// When there is selected text, this is usually the start of the selection.
		/// </remarks>
		/// <value>The index of the current insertion point.</value>
		public int CaretIndex
		{
			get { return textBox.CaretIndex; }
			set { textBox.CaretIndex = value; }
		}

		/// <summary>
		/// Gets or sets the font for the text of the control
		/// </summary>
		/// <value>The text font.</value>
		public Font Font
		{
			get { return textBox.Font; }
			set { textBox.Font = value; }
		}

		/// <summary>
		/// Gets or sets the maximum length of the text that can be entered in the control.
		/// </summary>
		/// <remarks>
		/// This typically does not affect the value set using <see cref="TextControl.Text"/>, only the limit of what the user can 
		/// enter into the control.
		/// </remarks>
		/// <value>The maximum length of the text in the control.</value>
		public int MaxLength
		{
			get { return textBox.MaxLength; }
			set { textBox.MaxLength = value; }
		}

		/// <summary>
		/// Gets or sets the placeholder text to show as a hint of what the user should enter.
		/// </summary>
		/// <remarks>
		/// Typically this will be shown when the control is blank, and will dissappear when the user enters text or if
		/// it has an existing value.
		/// </remarks>
		/// <value>The placeholder text.</value>
		public string PlaceholderText
		{
			get { return textBox.PlaceholderText; }
			set { textBox.PlaceholderText = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TextBox"/> is read only.
		/// </summary>
		/// <remarks>
		/// A user can selected and copied text when the read only, however the user will not be able to change any of the text.
		/// This differs from the <see cref="Control.Enabled"/> property, which disables all user interaction.
		/// </remarks>
		/// <value><c>true</c> if the control is read only; otherwise, <c>false</c>.</value>
		public bool ReadOnly
		{
			get { return textBox.ReadOnly; }
			set { textBox.ReadOnly = value; }
		}

		/// <summary>
		/// Gets or sets the current text selection.
		/// </summary>
		/// <value>The text selection.</value>
		public Range<int> Selection
		{
			get { return textBox.Selection; }
			set { textBox.Selection = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to show the control's border.
		/// </summary>
		/// <remarks>
		/// This is a hint to omit the border of the control and show it as plainly as possible.
		/// 
		/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
		/// </remarks>
		/// <value><c>true</c> to show the control border; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowBorder
		{
			get { return textBox.ShowBorder; }
			set { textBox.ShowBorder = value; }
		}

		/// <summary>
		/// Gets or sets the text of the control.
		/// </summary>
		/// <value>The text content.</value>
		public string Text
		{
			get { return textBox.Text; }
			set { textBox.Text = value; }
		}

		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <remarks>
		/// By default, the text will get a color based on the user's theme. However, this is usually black.
		/// </remarks>
		/// <value>The color of the text.</value>
		public Color TextColor
		{
			get { return textBox.TextColor; }
			set { textBox.TextColor = value; }
		}

		/// <summary>
		/// Gets or sets the valid directions the stepper will allow the user to click.
		/// </summary>
		/// <remarks>
		/// On some platforms, the up and/or down buttons will not appear disabled, but will not trigger any events when they are 
		/// not set as a valid direction.
		/// </remarks>
		/// <value>The valid directions for the stepper.</value>
		[DefaultValue(StepperValidDirections.Both)]
		public StepperValidDirections ValidDirection
		{
			get { return stepper.ValidDirection; }
			set { stepper.ValidDirection = value; }
		}

		/// <summary>
		/// Selects all of the text in the control.
		/// </summary>
		/// <remarks>
		/// When setting the selection, the control will be focussed and the associated keyboard may appear on mobile platforms.
		/// </remarks>
		public void SelectAll()
		{
			textBox.SelectAll();
		}

		/// <summary>
		/// Gets or sets the alignment of the text in the entry box.
		/// </summary>
		/// <value>The text alignment.</value>
		public TextAlignment TextAlignment
		{
			get { return textBox.TextAlignment; }
			set { textBox.TextAlignment = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Control"/> is visible to the user.
		/// </summary>
		/// <remarks>
		/// When the visibility of a control is set to false, it will still occupy space in the layout, but not be shown.
		/// The only exception is for controls like the <see cref="Splitter"/>, which will hide a pane if the visibility
		/// of one of the panels is changed.
		/// </remarks>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public bool ShowStepper
		{
			get { return stepper.Visible; }
			set { stepper.Visible = value; }
		}

		/// <summary>
		/// Gets the control used to attach keyboard and text input events
		/// </summary>
		/// <value>The keyboard control.</value>
		protected override Control KeyboardControl => textBox;

		/// <summary>
		/// Gets or sets the color for the background of the control
		/// </summary>
		/// <remarks>
		/// Note that on some platforms (e.g. Mac), setting the background color of a control can change the performance
		/// characteristics of the control and its children, since it must enable layers to do so.
		/// </remarks>
		/// <value>The color of the background.</value>
		public override Color BackgroundColor
		{
			get { return textBox.BackgroundColor; }
			set { textBox.BackgroundColor = value; }
		}

		/// <summary>
		/// Gets or sets the auto selection mode.
		/// </summary>
		/// <value>The auto selection mode.</value>
		public AutoSelectMode AutoSelectMode
		{
			get { return textBox.AutoSelectMode; }
			set { textBox.AutoSelectMode = value; }
		}

		/// <summary>
		/// Attaches the specified event to the platform-specific control
		/// </summary>
		/// <remarks>Implementors should override this method to handle any events that the widget
		/// supports. Ensure to call the base class' implementation if the event is not
		/// one the specific widget supports, so the base class' events can be handled as well.</remarks>
		/// <param name="id">Identifier of the event</param>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					textBox.TextChanged += (sender, e) => Callback.OnTextChanged(Widget, e);
					break;
				case TextBox.TextChangingEvent:
					textBox.TextChanging += (sender, e) => Callback.OnTextChanging(Widget, e);
					break;
				case TextStepper.StepEvent:
					stepper.Step += (sender, e) => Callback.OnStep(Widget, e);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
