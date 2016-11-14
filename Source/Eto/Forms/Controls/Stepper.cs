using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Eto.Forms
{
	/// <summary>
	/// Direction of the stepper when it has been clicked
	/// </summary>
	public enum StepperDirection
	{
		/// <summary>
		/// The Up direction, which usually increases the value
		/// </summary>
		Up,
		/// <summary>
		/// The Down direction, which usually decreases the value
		/// </summary>
		Down
	}

	/// <summary>
	/// Valid stepper directions for the (typically) up/down buttons
	/// </summary>
	/// <remarks>
	/// Note that some platforms do not actually disable the up or down buttons, but just won't trigger the <see cref="Stepper.Step"/>
	/// event when it is not valid.
	/// </remarks>
	[Flags]
	public enum StepperValidDirections
	{
		/// <summary>
		/// Neither the up or down buttons are valid
		/// </summary>
		None = 0,
		/// <summary>
		/// Specifies that the up/increase button is a valid direction for the stepper
		/// </summary>
		Up = 1,
		/// <summary>
		/// Specifies that the down/decrease button is a valid direction for the stepper
		/// </summary>
		Down = 2,
		/// <summary>
		/// Combines both the <see cref="Up"/> and <see cref="Down"/> flags.
		/// </summary>
		Both = Up | Down
	}

	/// <summary>
	/// Arguments for the <see cref="Stepper"/> and <see cref="TextStepper"/> to give you the direction of the step.
	/// </summary>
	public class StepperEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the step direction, either up/increase, or down/decrease.
		/// </summary>
		/// <value>The step direction.</value>
		public StepperDirection Direction { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.StepperEventArgs"/> class.
		/// </summary>
		/// <param name="direction">Direction of the step that the user clicked.</param>
		public StepperEventArgs(StepperDirection direction)
		{
			Direction = direction;
		}
	}

	public class Stepper : Control
	{
		public static DependencyEvent<Stepper, StepperEventArgs> StepEvent = new DependencyEvent<Stepper, StepperEventArgs>((c, e) => c.OnStep(e));

		public event EventHandler<StepperEventArgs> Step
		{
			add { Properties.AddEvent(StepEvent, value); }
			remove { Properties.RemoveEvent(StepEvent, value); }
		}

		/// <summary>
		/// Triggers the <see cref="Step"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected void OnStep(StepperEventArgs e) => Properties.TriggerEvent(StepEvent, this, e);


		public static DependencyProperty<Stepper, StepperValidDirections> ValidDirectionProperty = new DependencyProperty<Stepper, StepperValidDirections>(StepperValidDirections.Both);

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
			get { return Properties.Get(ValidDirectionProperty); }
			set { Properties.Set(ValidDirectionProperty, value); }
		}
	}
}
