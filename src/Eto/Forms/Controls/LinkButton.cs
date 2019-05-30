using System;
using Eto.Drawing;
using System.Windows.Input;

namespace Eto.Forms
{
	/// <summary>
	/// Button that is visually represented like a hyperlink on a web page.
	/// </summary>
	[Handler(typeof(LinkButton.IHandler))]
	public class LinkButton : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static LinkButton()
		{
			EventLookup.Register<LinkButton>(c => c.OnClick(null), ClickEvent);
		}

		/// <summary>
		/// Event identifier for the <see cref="Click"/> event.
		/// </summary>
		public const string ClickEvent = "LinkButton.Click";

		/// <summary>
		/// Occurs when an individual cell is clicked.
		/// </summary>
		public event EventHandler<EventArgs> Click
		{
			add { Properties.AddHandlerEvent(ClickEvent, value); }
			remove { Properties.RemoveEvent(ClickEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnClick(EventArgs e)
		{
			Properties.TriggerEvent(ClickEvent, this, e);
		}

		static readonly object Command_Key = new object();

		/// <summary>
		/// Gets or sets the command to invoke when the link button is pressed.
		/// </summary>
		/// <remarks>
		/// This will invoke the specified command when the link button is pressed.
		/// The <see cref="ICommand.CanExecute"/> will also used to set the enabled/disabled state of the link button.
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
		/// Gets or sets the color of the text when the control is disabled.
		/// </summary>
		/// <value>The color of the text when disabled.</value>
		public Color DisabledTextColor
		{
			get { return Handler.DisabledTextColor; }
			set { Handler.DisabledTextColor = value; }
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for <see cref="LinkButton"/>
		/// </summary>
		public new interface ICallback : TextControl.ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			void OnClick(LinkButton widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="LinkButton"/>
		/// </summary>
		protected new class Callback : TextControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			public void OnClick(LinkButton widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnClick(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="LinkButton"/> control
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Gets or sets the color of the text when the control is disabled.
			/// </summary>
			Color DisabledTextColor { get; set; }
		}
	}
}
