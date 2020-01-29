using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Stepper with custom text entry field
	/// </summary>
	/// <remarks>
	/// This can be used to implement a custom stepper interface that is not entirely restricted to numeric values like <see cref="NumericStepper"/>.
	/// </remarks>
	[Handler(typeof(IHandler))]
	public class TextStepper : TextBox
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static TextStepper()
		{
			EventLookup.Register((TextStepper c) => c.OnStep(null), StepEvent);
		}

		/// <summary>
		/// Identifier for the <see cref="Step"/> event.
		/// </summary>
		public const string StepEvent = "TextStepper.Step";

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
		protected virtual void OnStep(StepperEventArgs e)
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

		/// <summary>
		/// Gets or sets a value indicating whether the Stepper will be shown.
		/// </summary>
		/// <remarks>
		/// This is a hint only, some platforms (currently Gtk) may ignore this setting.
		/// </remarks>
		/// <value><c>true</c> to show the stepper (default); otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowStepper
		{
			get { return Handler.ShowStepper; }
			set { Handler.ShowStepper = value; }
		}

		ICallback callback = new Callback();

		/// <summary>
		/// Gets the callback.
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() => callback;

		/// <summary>
		/// Callback interface for the TextStepper
		/// </summary>
		public new interface ICallback : TextBox.ICallback
		{
			/// <summary>
			/// Triggers the <see cref="Step"/> event.
			/// </summary>
			/// <param name="widget">Widget instance to trigger the event</param>
			/// <param name="e">Event arguments</param>
			void OnStep(TextStepper widget, StepperEventArgs e);
		}

		/// <summary>
		/// Callback implementation for the TextStepper
		/// </summary>
		protected new class Callback : TextBox.Callback, ICallback
		{
			/// <summary>
			/// Triggers the <see cref="Step"/> event.
			/// </summary>
			/// <param name="widget">Widget instance to trigger the event</param>
			/// <param name="e">Event arguments</param>
			public void OnStep(TextStepper widget, StepperEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnStep(e);
			}
		}

		/// <summary>
		/// Handler interface for platform implementations of the TextStepper
		/// </summary>
		public new interface IHandler : TextBox.IHandler
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

			/// <summary>
			/// Gets or sets a value indicating whether the Stepper will be shown.
			/// </summary>
			/// <remarks>
			/// This is a hint only, some platforms (currently Gtk) may ignore this setting.
			/// </remarks>
			/// <value><c>true</c> to show the stepper (default); otherwise, <c>false</c>.</value>
			bool ShowStepper { get; set; }
		}
	}
}
