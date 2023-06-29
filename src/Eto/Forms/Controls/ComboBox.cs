namespace Eto.Forms;

/// <summary>
/// Presents a combination of an editable text box and drop down to select from a list of items and enter text.
/// </summary>
/// <example>
/// Here is a short example on how to create an autocompleting check box.
/// <code>
/// var list = new List&lt;string&gt;() { "First item", "Second item", "Third item" };
/// var comboBox = new ComboBox() { AutoComplete = true, DataStore = list }; 
/// </code>
/// </example>
[Handler(typeof(IHandler))]
public class ComboBox : DropDown
{
	new IHandler Handler { get { return (IHandler)base.Handler; } }

	/// <summary>
	/// Event identifier for handlers when attaching the <see cref="TextChanged"/> event.
	/// </summary>
	public const string TextChangedEvent = "ComboBox.TextChanged";

	/// <summary>
	/// Occurs when the <see cref="Text"/> property is changed either by the user or programatically.
	/// </summary>
	public event EventHandler<EventArgs> TextChanged
	{
		add { Properties.AddHandlerEvent(TextChangedEvent, value); }
		remove { Properties.RemoveEvent(TextChangedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="TextChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnTextChanged(EventArgs e)
	{
		Properties.TriggerEvent(TextChangedEvent, this, e);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ComboBox"/> class.
	/// </summary>
	public ComboBox()
	{
	}

	/// <summary>
	/// Gets or sets the text of the combo box.
	/// </summary>
	/// <value>The text content.</value>
	public string Text
	{
		get { return Handler.Text; }
		set { Handler.Text = value; }
	}

	/// <summary>
	/// Gets or sets whether the user can change the text in the combo box.
	/// </summary>
	/// <remarks>
	/// When <see langword="true"/>, the user will still be able to select/copy the text or select items from the drop down.
	/// They will only be unable to type in different text. <br/>
	/// To fully disable the control, use the <see cref="Control.Enabled"/> property.
	/// </remarks>
	public bool ReadOnly
	{
		get { return Handler.ReadOnly; }
		set { Handler.ReadOnly = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating that the text should autocomplete when the user types in a value.
	/// </summary>
	/// <remarks>
	/// The autocomplete will be based off of the available items in the combo box.
	/// </remarks>
	/// <value><see langword="true"/> to auto complete the text; otherwise, <see langword="false"/>.</value>
	public bool AutoComplete
	{
		get { return Handler.AutoComplete; }
		set { Handler.AutoComplete = value; }
	}

	static readonly object callback = new Callback();

	/// <inheritdoc/>
	protected override object GetCallback()
	{
		return callback;
	}

	/// <summary>
	/// Callback interface for the <see cref="ComboBox"/>.
	/// </summary>
	public new interface ICallback : DropDown.ICallback
	{
		/// <summary>
		/// <inheritdoc cref="ComboBox.OnTextChanged" path="/summary"/>
		/// </summary>
		// TODO: document parameters
		void OnTextChanged(ComboBox widget, EventArgs e);
	}

	/// <summary>
	/// Callback implementation for handlers of <see cref="ComboBox"/>.
	/// </summary>
	protected new class Callback : DropDown.Callback, ICallback
	{
        /// <inheritdoc cref="ICallback.OnTextChanged"/>
		public void OnTextChanged(ComboBox widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnTextChanged(e);
		}
	}
	
	/// <summary>
	/// Handler interface for the <see cref="ComboBox"/>.
	/// </summary>
	public new interface IHandler : DropDown.IHandler
	{
		/// <inheritdoc cref="ComboBox.Text"/>
		string Text { get; set; }

		/// <inheritdoc cref="ComboBox.ReadOnly"/>
		bool ReadOnly { get; set; }

		/// <inheritdoc cref="ComboBox.AutoComplete"/>
		bool AutoComplete { get; set; }
	}
}