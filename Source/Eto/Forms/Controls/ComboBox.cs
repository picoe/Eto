using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Presents a combination of an editable text box and drop down to select from a list of items and enter text.
	/// </summary>
	[Handler(typeof(IHandler))]
	public class ComboBox : DropDown
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="TextChanged"/> event.
		/// </summary>
		public const string TextChangedEvent = "ComboBox.TextChanged";

		/// <summary>
		/// Occurs when the Text property is changed either by the user or programatically.
		/// </summary>
		public event EventHandler<EventArgs> TextChanged
		{
			add { Properties.AddHandlerEvent(TextChangedEvent, value); }
			remove { Properties.RemoveEvent(TextChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="TextChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
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
		/// Gets or sets the text of the ComboBox.
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
		/// When <c>true</c>, the user will still be able to select/copy the text, select items from the drop down, etc.
		/// To disable the control, use the <see cref="Control.Enabled"/> property.
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
		/// The autocomplete will be based off of the items in the combo box.
		/// </remarks>
		/// <value><c>true</c> to auto complete the text; otherwise, <c>false</c>.</value>
		public bool AutoComplete
		{
			get { return Handler.AutoComplete; }
			set { Handler.AutoComplete = value; }
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback()
		{
			return callback;
		}

		/// <summary>
		/// Callback interface for the <see cref="ComboBox"/>
		/// </summary>
		public new interface ICallback : DropDown.ICallback
		{
			/// <summary>
			/// Raises the text changed event.
			/// </summary>
			void OnTextChanged(ComboBox widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="ListControl"/>
		/// </summary>
		protected new class Callback : DropDown.Callback, ICallback
		{
			/// <summary>
			/// Raises the text changed event.
			/// </summary>
			public void OnTextChanged(ComboBox widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnTextChanged(e);
			}
		}


		/// <summary>
		/// Handler interface for the <see cref="ComboBox"/>
		/// </summary>
		public new interface IHandler : DropDown.IHandler
		{
			/// <summary>
			/// Gets or sets the text of the ComboBox.
			/// </summary>
			/// <value>The text content.</value>
			string Text { get; set; }

			/// <summary>
			/// Gets or sets the editable of ComboBox.
			/// </summary>
			bool ReadOnly { get; set; }

			/// <summary>
			/// Gets or sets a value indicating that the text should autocomplete when the user types in a value.
			/// </summary>
			/// <remarks>
			/// The autocomplete will be based off of the items in the combo box.
			/// </remarks>
			/// <value><c>true</c> to auto complete the text; otherwise, <c>false</c>.</value>
			bool AutoComplete { get; set; }
		}
	}
}
