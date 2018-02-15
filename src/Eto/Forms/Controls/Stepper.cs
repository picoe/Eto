using System;
using System.ComponentModel;


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

	/// <summary>
	/// Control that allows you to "step" through values, usually presented by two buttons arranged vertically with up and down arrows.
	/// </summary>
	[Handler(typeof(IHandler))]
	public class Stepper : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Identifier for the <see cref="Step"/> event.
		/// </summary>
		public const string StepEvent = "Stepper.Step";

		/// <summary>
		/// Event to handle when the user clicks on one of the step buttons, either up or down.
		/// </summary>
		public event EventHandler<StepperEventArgs> Step
		{
			add { Properties.AddHandlerEvent(StepEvent, value); }
			remove { Properties.RemoveEvent(StepEvent, value); }
		}

		/// <summary>
		/// Triggers the <see cref="Step"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected void OnStep(StepperEventArgs e)
		{
			Properties.TriggerEvent(StepEvent, this, e);
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
			get { return Handler.ValidDirection; }
			set { Handler.ValidDirection = value; }
		}

		ICallback callback = new Callback();

		/// <summary>
		/// Gets the callback.
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() => callback;

		/// <summary>
		/// Callback interface for the Stepper
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Triggers the <see cref="Step"/> event.
			/// </summary>
			/// <param name="widget">Widget instance to trigger the event</param>
			/// <param name="e">Event arguments</param>
			void OnStep(Stepper widget, StepperEventArgs e);
		}

		/// <summary>
		/// Callback implementation for the Stepper
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Triggers the <see cref="Step"/> event.
			/// </summary>
			/// <param name="widget">Widget instance to trigger the event</param>
			/// <param name="e">Event arguments</param>
			public void OnStep(Stepper widget, StepperEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnStep(e);
			}
		}

		/// <summary>
		/// Handler interface for the Stepper
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the valid directions the stepper will allow the user to click.
			/// </summary>
			/// <remarks>
			/// On some platforms, the up and/or down buttons will not appear disabled, but will not trigger any events when they are 
			/// not set as a valid direction.
			/// </remarks>
			/// <value>The valid directions for the stepper.</value>
			StepperValidDirections ValidDirection { get; set; }
		}
	}
}
