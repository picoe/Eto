using System;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// Themed version of the <see cref="Stepper"/> control for platforms that have no support for such a control.
	/// </summary>
	/// <remarks>
	/// Currently used for Gtk and WinForms.  Mac and Wpf Toolkit have controls that have this functionality.
	/// 
	/// To use this implementation for all platforms, add this before you start your app:
	/// <code>
	/// Platform.Detect.Add&gt;Stepper.IHandler&lt;(() => new Eto.Forms.ThemedControls.ThemedStepperHandler());
	/// </code>
	/// </remarks>
	public class ThemedStepperHandler : ThemedControlHandler<Panel, Stepper, Stepper.ICallback>, Stepper.IHandler
	{
		StepperValidDirections validDirection = StepperValidDirections.Both;
		Font font = SystemFonts.Default(4);
		Button upButton;
		Button downButton;
		Orientation orientation = Orientation.Vertical;

		/// <summary>
		/// Gets or sets the text for the up/increase button
		/// </summary>
		public string UpText
		{
			get { return upButton.Text; }
			set { upButton.Text = value; }
		}

		/// <summary>
		/// Gets or sets the text for the down/decrease button
		/// </summary>
		public string DownText
		{
			get { return downButton.Text; }
			set { downButton.Text = value; }
		}

		/// <summary>
		/// Gets or sets the font for the text in the buttons
		/// </summary>
		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				upButton.Font = font;
				downButton.Font = font;
			}
		}

		/// <summary>
		/// Initializes a new instance of the ThemedStepperHandler
		/// </summary>
		public ThemedStepperHandler()
		{
			upButton = new Button { Text = "\u25B2", MinimumSize = Size.Empty, Font = font };
			downButton = new Button { Text = "\u25BC", MinimumSize = Size.Empty, Font = font };

			Control = new Panel { Height = 28, Width = 17 };
			Setup();
		}

		/// <summary>
		/// Gets or sets the orientation of the stepper
		/// </summary>
		[DefaultValue(Orientation.Vertical)]
		public Orientation Orientation
		{
			get { return orientation; }
			set
			{
				if (orientation != value)
				{
					orientation = value;
					Setup();
				}
			}
		}

		void Setup()
		{
			if (Orientation == Orientation.Vertical)
				Control.Content = new TableLayout(true, upButton, downButton);
			else
				Control.Content = TableLayout.HorizontalScaled(downButton, upButton);
		}

		/// <summary>
		/// Gets or sets the valid directions for the stepper
		/// </summary>
		public StepperValidDirections ValidDirection
		{
			get { return validDirection; }
			set
			{
				if (validDirection != value)
				{
					validDirection = value;
					UpdateButtonState();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating that the control is enabled
		/// </summary>
		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				base.Enabled = value;
				UpdateButtonState();
			}
		}

		void UpdateButtonState()
		{
			upButton.Enabled = Enabled && validDirection.HasFlag(StepperValidDirections.Up);
			downButton.Enabled = Enabled && validDirection.HasFlag(StepperValidDirections.Down);
		}

		/// <summary>
		/// Attaches control events
		/// </summary>
		/// <param name="id">ID of the event to attach</param>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Stepper.StepEvent:
					upButton.Click += (sender, e) => Callback.OnStep(Widget, new StepperEventArgs(StepperDirection.Up));
					downButton.Click += (sender, e) => Callback.OnStep(Widget, new StepperEventArgs(StepperDirection.Down));
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
