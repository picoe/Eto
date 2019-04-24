using System;
namespace Eto.Forms
{
	/// <summary>
	/// Specialized Button that can be toggled on or off.
	/// </summary>
	/// <remarks>
	/// This is similar to the <see cref="CheckBox"/> but appears depressed or highlighted when "checked".
	/// </remarks>
	[Handler(typeof(IHandler))]
	public class ToggleButton : Button
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.ToggleButton"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked
		{
			get => Handler.Checked;
			set => Handler.Checked = value;
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="CheckedChanged"/> event.
		/// </summary>
		public const string CheckedChangedEvent = "ToggleButton.CheckedChanged";

		/// <summary>
		/// Occurs when the <see cref="Checked"/> value changes.
		/// </summary>
		public event EventHandler<EventArgs> CheckedChanged
		{
			add => Properties.AddHandlerEvent(CheckedChangedEvent, value);
			remove => Properties.RemoveEvent(CheckedChangedEvent, value);
		}

		/// <summary>
		/// Raises the <see cref="Checked"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnCheckedChanged(EventArgs e) => Properties.TriggerEvent(CheckedChangedEvent, this, e);

		/// <summary>
		/// Clicks the toggle button programatically, raising the same events and toggling the Checked state if Enabled and Visible.
		/// </summary>
		public override void PerformClick()
		{
			if (Enabled && Visible)
			{
				Checked = !Checked;
				OnClick(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Callback interface for handlers of the <see cref="ToggleButton"/>.
		/// </summary>
		public new interface ICallback : Button.ICallback
		{
			/// <summary>
			/// Raises the CheckedChanged event.
			/// </summary>
			void OnCheckedChanged(ToggleButton widget, EventArgs e);
		}

		static ICallback callback => new Callback();

		/// <summary>
		/// Gets the callback instance for the <see cref="ToggleButton"/>.
		/// </summary>
		/// <returns>The callback object.</returns>
		protected override object GetCallback() => callback;

		/// <summary>
		/// Callback implementation for the <see cref="ToggleButton"/>.
		/// </summary>
		protected new class Callback : Button.Callback, ICallback
		{
			/// <summary>
			/// Raises the CheckedChanged event.
			/// </summary>
			public void OnCheckedChanged(ToggleButton widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCheckedChanged(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="ToggleButton"/>
		/// </summary>
		public new interface IHandler : Button.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether the ToggleButton is checked.
			/// </summary>
			/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
			bool Checked { get; set; }
		}
	}
}
