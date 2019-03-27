using System;
using System.Windows.Input;

namespace Eto.Forms
{
	/// <summary>
	/// Menu item to choose from a set of options
	/// </summary>
	/// <remarks>
	/// The RadioMenuItem works with other radio items to present a list of options that the user can select from.
	/// When a radio button is toggled on, all others that are linked together will be toggled off.
	/// 
	/// To link radio buttons together, use the <see cref="C:Eto.Forms.RadioMenuItem(RadioMenuItem)"/> constructor
	/// to specify the controller radio item, which can be created with the default constructor.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(RadioMenuItem.IHandler))]
	public class RadioMenuItem : MenuItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioMenuItem"/> class.
		/// </summary>
		public RadioMenuItem()
		{
			Handler.Create(null);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioMenuItem"/> class.
		/// </summary>
		/// <param name="controller">Controller radio menu item to link to, or null if no controller.</param>
		public RadioMenuItem(RadioMenuItem controller)
		{
			Handler.Create(controller);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioMenuItem"/> class with the specified command and controller.
		/// </summary>
		/// <param name="command">Command to initialize the menu item with.</param>
		/// <param name="controller">Controller radio menu item to link to, or null if no controller.</param>
		public RadioMenuItem(RadioCommand command, RadioMenuItem controller = null)
			: base(command)
		{
			Handler.Create(controller);
			Initialize();
			Handler.CreateFromCommand(command);
			HandleEvent(CheckedChangedEvent);
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

				// HACK: should be checked some other way, perhaps during initialize
				// this is why we don't call virtual methods from constructors..
				if (ControlObject != null)
					HandleEvent(CheckedChangedEvent);
			}
		}

		void ValueCommand_ValueChanged(object sender, EventArgs e)
		{
			if (Command is IValueCommand<bool> valueCommand)
				Checked = valueCommand.GetValue(CommandParameter);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.RadioMenuItem"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		/// <summary>
		/// Event identifier for the <see cref="CheckedChanged"/> event.
		/// </summary>
		public const string CheckedChangedEvent = "RadioMenuItem.CheckedChanged";

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
		/// <param name="e">E.</param>
		protected virtual void OnCheckedChanged(EventArgs e)
		{
			Properties.TriggerEvent(CheckedChangedEvent, this, e);

			if (Command is IValueCommand<bool> valueCommand)
				valueCommand.SetValue(CommandParameter, Checked);
		}

		/// <summary>
		/// Performs the click handler for this item which sets the check state to true.
		/// </summary>
		/// <remarks>
		/// This performs the click by calling <see cref="MenuItem.OnClick"/> which triggers the <see cref="MenuItem.Click"/> event.
		/// The <see cref="Checked"/> state will also be set to true.
		/// </remarks>
		public override void PerformClick()
		{
			Checked = true;
			base.PerformClick();
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
		/// Callback interface for the <see cref="RadioMenuItem"/>.
		/// </summary>
		public new interface ICallback : MenuItem.ICallback
		{
			/// <summary>
			/// Raises the checked changed event.
			/// </summary>
			void OnCheckedChanged(RadioMenuItem widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="RadioMenuItem"/>.
		/// </summary>
		protected new class Callback : MenuItem.Callback, ICallback
		{
			/// <summary>
			/// Raises the checked changed event.
			/// </summary>
			public void OnCheckedChanged(RadioMenuItem widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCheckedChanged(e);
			}
		}


		/// <summary>
		/// Handler interface for the <see cref="RadioMenuItem"/>.
		/// </summary>
		[AutoInitialize(false)]
		public new interface IHandler : MenuItem.IHandler
		{
			/// <summary>
			/// Creates the menu item with the specified controller.
			/// </summary>
			/// <param name="controller">Controller radio menu item to link to, or null if no controller.</param>
			void Create(RadioMenuItem controller);

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.RadioMenuItem"/> is checked.
			/// </summary>
			/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
			bool Checked { get; set; }
		}
	}
}