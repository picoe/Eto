using System;
using System.Windows.Input;

namespace Eto.Forms
{
	/// <summary>
	/// Tool item that can be toggled on or off.
	/// </summary>
	/// <remarks>
	/// Note that there is only a single group of radio buttons allowed on a toolbar.
	/// </remarks>
	[Handler(typeof(RadioToolItem.IHandler))]
	public class RadioToolItem : ToolItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Occurs when the <see cref="Checked"/> property is changed.
		/// </summary>
		public event EventHandler<EventArgs> CheckedChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioToolItem"/> class.
		/// </summary>
		public RadioToolItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioToolItem"/> class with the specified <paramref name="command"/>.
		/// </summary>
		/// <param name="command">Command for the tool item.</param>
		public RadioToolItem(CheckCommand command)
			: base(command)
		{
			Handler.CreateFromCommand(command);
		}

		internal override void SetCommand(ICommand oldValue, ICommand newValue)
		{
			if (oldValue is IValueCommand<bool> oldValueCommand)
				oldValueCommand.ValueChanged -= ValueCommand_ValueChanged;

			base.SetCommand(oldValue, newValue);
			if (newValue is IValueCommand<bool> valueCommand)
			{
				Checked = valueCommand.GetValue(CommandParameter);
				valueCommand.ValueChanged += ValueCommand_ValueChanged;
			}
		}

		void ValueCommand_ValueChanged(object sender, EventArgs e)
		{
			if (Command is IValueCommand<bool> valueCommand)
				Checked = valueCommand.GetValue(CommandParameter);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this item is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		/// <summary>
		/// Raises the <see cref="CheckedChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		public void OnCheckedChanged(EventArgs e)
		{
			CheckedChanged?.Invoke(this, e);

			if (Command is IValueCommand<bool> valueCommand)
				valueCommand.SetValue(CommandParameter, Checked);
		}

		/// <summary>
		/// Handler for the <see cref="RadioToolItem"/>.
		/// </summary>
		public new interface IHandler : ToolItem.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether this item is checked.
			/// </summary>
			/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
			bool Checked { get; set; }
		}
	}
}
