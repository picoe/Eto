using System;
using System.Windows.Input;

namespace Eto.Forms
{
	/// <summary>
	/// Menu item that can be toggled on and off
	/// </summary>
	/// <remarks>
	/// Most platforms show a check box next to the item when selected.  Some platforms may not show the item's image.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(CheckMenuItem.IHandler))]
	public class CheckMenuItem : MenuItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckMenuItem"/> class.
		/// </summary>
		public CheckMenuItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckMenuItem"/> class with the specified command.
		/// </summary>
		/// <param name="command">Command to initialize the menu with.</param>
		public CheckMenuItem(CheckCommand command)
			: base(command)
		{
			Handler.CreateFromCommand(command);
		}

		internal override void SetCommand(ICommand oldValue, ICommand newValue)
		{
			if (oldValue is IValueCommand<bool> oldValueCommand)
			{
				oldValueCommand.ValueChanged -= ValueCommand_ValueChanged;
			}

			base.SetCommand(oldValue, newValue);

			if (newValue is IValueCommand<bool> valueCommand)
			{
				Checked = valueCommand.GetValue(CommandParameter);
				valueCommand.ValueChanged += ValueCommand_ValueChanged;
				HandleEvent(CheckedChangedEvent);
			}
		}

		void ValueCommand_ValueChanged(object sender, EventArgs e)
		{
			if (Command is IValueCommand<bool> valueCommand)
				Checked = valueCommand.GetValue(CommandParameter);
		}

		/// <summary>
		/// Event identifier for the <see cref="CheckedChanged"/> event.
		/// </summary>
		public const string CheckedChangedEvent = "CheckMenuItem.CheckedChanged";

		/// <summary>
		/// Event to handle when the <see cref="Checked"/> property changes.
		/// </summary>
		public event EventHandler<EventArgs> CheckedChanged
		{
			add { Properties.AddHandlerEvent(CheckedChangedEvent, value); }
			remove { Properties.RemoveEvent(CheckedChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="CheckedChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnCheckedChanged(EventArgs e)
		{
			Properties.TriggerEvent(CheckedChangedEvent, this, e);

			if (Command is IValueCommand<bool> valueCommand)
				valueCommand.SetValue(CommandParameter, Checked);
		}

		static readonly Callback callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback()
		{
			return callback;
		}

		/// <summary>
		/// Callback interface for the <see cref="CheckMenuItem"/> class.
		/// </summary>
		public new interface ICallback : MenuItem.ICallback
		{
			/// <summary>
			/// Raises the checked changed event.
			/// </summary>
			void OnCheckedChanged(CheckMenuItem widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="CheckMenuItem"/>.
		/// </summary>
		protected new class Callback : MenuItem.Callback, ICallback
		{
			/// <summary>
			/// Raises the checked changed event.
			/// </summary>
			public void OnCheckedChanged(CheckMenuItem widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCheckedChanged(e);
			}
		}

		/// <summary>
		/// Performs the click handler for this item which toggles the check state.
		/// </summary>
		/// <remarks>
		/// This performs the click by calling <see cref="MenuItem.OnClick"/> which triggers the <see cref="MenuItem.Click"/> event.
		/// The <see cref="Checked"/> state will also be toggled.
		/// </remarks>
		public override void PerformClick()
		{
			Checked = !Checked;
			base.PerformClick();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.CheckMenuItem"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="CheckMenuItem"/>.
		/// </summary>
		public new interface IHandler : MenuItem.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether the menu item is checked.
			/// </summary>
			/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
			bool Checked { get; set; }
		}
	}
}