using System;
using System.Windows.Input;

namespace Eto.Forms
{
	/// <summary>
	/// Control to present a button to choose from a set of options
	/// </summary>
	/// <remarks>
	/// The RadioButton works with other radio buttons to present a list of options that the user can select from.
	/// When a radio button is toggled on, all others that are linked together will be toggled off.
	/// 
	/// To link radio buttons together, use the <see cref="C:Eto.Forms.RadioButton(RadioButton)"/> constructor
	/// to specify the controller radio button, which can be created with the default constructor.
	/// </remarks>
	/// <seealso cref="RadioButtonList"/>
	[Handler(typeof(RadioButton.IHandler))]
	public class RadioButton : TextControl
	{
		/// <summary>
		/// Occurs when the <see cref="Checked"/> property is changed.
		/// </summary>
		public event EventHandler<EventArgs> CheckedChanged;

		/// <summary>
		/// Occurs when the user clicks the radio button.
		/// </summary>
		public event EventHandler<EventArgs> Click;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
		}

		static readonly object Command_Key = new object();

		/// <summary>
		/// Gets or sets the command to invoke when the radio button is pressed.
		/// </summary>
		/// <remarks>
		/// This will invoke the specified command when the radio button is pressed.
		/// The <see cref="ICommand.CanExecute"/> will also used to set the enabled/disabled state of the button.
		/// </remarks>
		/// <value>The command to invoke.</value>
		public ICommand Command
		{
			get { return Properties.GetCommand(Command_Key); }
			set { Properties.SetCommand(Command_Key, value, e => Enabled = e, r => Click += r, r => Click -= r, () => CommandParameter); }
		}

		static readonly object CommandParameter_Key = new object();

		/// <summary>
		/// Gets or sets the parameter to pass to the <see cref="Command"/> when executing or determining its CanExecute state.
		/// </summary>
		/// <value>The command parameter.</value>
		public object CommandParameter
		{
			get { return Properties.Get<object>(CommandParameter_Key); }
			set { Properties.Set(CommandParameter_Key, value, () => Properties.UpdateCommandCanExecute(Command_Key)); }
		}

		/// <summary>
		/// Raises the <see cref="CheckedChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null)
				CheckedChanged(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioButton"/> class.
		/// </summary>
		public RadioButton()
		{
			Handler.Create(null);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioButton"/> class.
		/// </summary>
		/// <param name="controller">Controller radio button to link to, or null if no controller.</param>
		public RadioButton(RadioButton controller = null)
		{
			Handler.Create(controller);
			Initialize();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.RadioButton"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public virtual bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		/// <summary>
		/// Callback interface for the <see cref="RadioButton"/>
		/// </summary>
		public new interface ICallback : TextControl.ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			void OnClick(RadioButton widget, EventArgs e);

			/// <summary>
			/// Raises the checked changed event.
			/// </summary>
			void OnCheckedChanged(RadioButton widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="RadioButton"/>
		/// </summary>
		protected new class Callback : TextControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			public void OnClick(RadioButton widget, EventArgs e)
			{
				widget.OnClick(e);
			}

			/// <summary>
			/// Raises the checked changed event.
			/// </summary>
			public void OnCheckedChanged(RadioButton widget, EventArgs e)
			{
				widget.OnCheckedChanged(e);
			}
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Handler interface for the <see cref="RadioButton"/>
		/// </summary>
		/// <remarks>
		/// When using this handler, you must call <see cref="Eto.Widget.Initialize"/> in the constructor.
		/// </remarks>
		[AutoInitialize(false)]
		public new interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Used when creating a new instance of the RadioButton to specify the controller
			/// </summary>
			/// <param name="controller">Controller radio button to link to, or null if no controller.</param>
			void Create(RadioButton controller);

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.RadioButton"/> is checked.
			/// </summary>
			/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
			bool Checked { get; set; }
		}
	}
}
