using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using sc = System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Interface to access common properties of both <see cref="MenuItem"/> and <see cref="ToolItem"/>.
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface ICommandItem
	{
		/// <summary>
		/// Occurs when the user clicks on the item.
		/// </summary>
		event EventHandler<EventArgs> Click;

		/// <summary>
		/// Gets or sets the text of the item, with mnemonic.
		/// </summary>
		/// <value>The text.</value>
		string Text { get; set; }

		/// <summary>
		/// Gets or sets the tool tip to show when hovering the mouse over the item.
		/// </summary>
		/// <value>The tool tip.</value>
		string ToolTip { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.ICommandItem"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		bool Enabled { get; set; }
	}

	/// <summary>
	/// Base class for items in a menu
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[sc.TypeConverter(typeof(MenuItemConverter))]
	public abstract class MenuItem : Menu, ICommandItem
	{
		/// <summary>
		/// Gets or sets the order that the menu item should use when inserted into a submenu.
		/// </summary>
		/// <remarks>
		/// The order can be used to sort your menu items when added in a different order.
		/// 
		/// This is useful when you have menu items added from different areas of your program.
		/// </remarks>
		/// <value>The order to use when inserting into the submenu.</value>
		public int Order { get; set; }

		static readonly object Command_Key = new object();

		/// <summary>
		/// Gets or sets the command to invoke when the menu item is pressed.
		/// </summary>
		/// <remarks>
		/// This will invoke the specified command when the menu item is pressed.
		/// The <see cref="ICommand.CanExecute"/> will also used to set the enabled/disabled state of the menu item.
		/// </remarks>
		/// <value>The command to invoke.</value>
		public ICommand Command
		{
			get => Properties.GetCommand(Command_Key);
			set
			{
				var oldValue = Command;
				if (!ReferenceEquals(oldValue, value))
					SetCommand(oldValue, value);
			}
		}

		internal virtual void SetCommand(ICommand oldValue, ICommand newValue)
		{
			Properties.SetCommand(Command_Key, newValue, e => Enabled = e, r => Click += r, r => Click -= r, () => CommandParameter);
			HandleEvent(ValidateEvent);
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


		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Occurs when the user clicks or selects the menu item.
		/// </summary>
		public event EventHandler<EventArgs> Click;

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Validate"/> event.
		/// </summary>
		public const string ValidateEvent = "MenuActionItem.ValidateEvent";

		/// <summary>
		/// Occurs when the menu item is validated.
		/// </summary>
		/// <remarks>
		/// This is used to allow enabling/disabling items before they are shown to the user.
		/// Usually, platforms will call validate on all items each time they are shown in a submenu or context menu.
		/// </remarks>
		public event EventHandler<EventArgs> Validate
		{
			add { Properties.AddHandlerEvent(ValidateEvent, value); }
			remove { Properties.RemoveEvent(ValidateEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Validate"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnValidate(EventArgs e)
		{
			Properties.TriggerEvent(ValidateEvent, this, e);

			var command = Command;
			if (command != null)
				Enabled = command.CanExecute(CommandParameter);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MenuItem"/> class.
		/// </summary>
		protected MenuItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MenuItem"/> class with the specified command.
		/// </summary>
		/// <remarks>
		/// This links the menu item with the specified command, and will trigger <see cref="Eto.Forms.Command.Execute"/>
		/// when the user clicks the item, and enable/disable the menu item based on <see cref="Eto.Forms.Command.Enabled"/>.
		/// 
		/// This is not a weak link, so you should not re-use the Command instance for other menu items if you are disposing
		/// this menu item.
		/// </remarks>
		/// <param name="command">Command to initialize the menu item with.</param>
		protected MenuItem(Command command)
		{
			ID = command.ID;
			Text = command.MenuText;
			ToolTip = command.ToolTip;
			Shortcut = command.Shortcut;
			Command = command;
		}

		static MenuItem()
		{
			EventLookup.Register<MenuItem>(c => c.OnValidate(null), MenuItem.ValidateEvent);
		}

		/// <summary>
		/// Gets or sets the text of the menu item, with mnemonics identified with &amp;.
		/// </summary>
		/// <value>The text with mnemonic of the menu item.</value>
		public string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Gets or sets a user-defined tag for the menu item.
		/// </summary>
		/// <value>The user-defined tag.</value>
		public object Tag { get; set; }

		/// <summary>
		/// Gets or sets the tool tip of the item.
		/// </summary>
		/// <value>The tool tip.</value>
		public string ToolTip
		{
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.MenuItem"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Enabled
		{
			get { return Handler.Enabled; }
			set { Handler.Enabled = value; }
		}

		/// <summary>
		/// Gets or sets the shortcut key the user can press to activate the menu item.
		/// </summary>
		/// <value>The shortcut key.</value>
		public Keys Shortcut
		{
			get { return Handler.Shortcut; }
			set { Handler.Shortcut = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.MenuItem"/> is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public bool Visible
		{
			get { return Handler.Visible; }
			set { Handler.Visible = value; }
		}

		/// <summary>
		/// Performs the click handler for this item.
		/// </summary>
		/// <remarks>
		/// This performs the click by calling <see cref="OnClick"/> which triggers the <see cref="Click"/> event.
		/// </remarks>
		public virtual void PerformClick()
		{
			OnClick(EventArgs.Empty);
		}

		/// <summary>
		/// Callback interface for the <see cref="MenuItem"/>
		/// </summary>
		public new interface ICallback : Menu.ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			void OnClick(MenuItem widget, EventArgs e);

			/// <summary>
			/// Raises the validate event.
			/// </summary>
			void OnValidate(MenuItem widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="MenuItem"/>
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			public void OnClick(MenuItem widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnClick(e);
			}

			/// <summary>
			/// Raises the validate event.
			/// </summary>
			public void OnValidate(MenuItem widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnValidate(e);
			}
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback()
		{
			return callback;
		}

		/// <summary>
		/// Handler interface for the <see cref="MenuItem"/>
		/// </summary>
		public new interface IHandler : Menu.IHandler
		{
			/// <summary>
			/// Gets or sets the shortcut key the user can press to activate the menu item.
			/// </summary>
			/// <value>The shortcut key.</value>
			Keys Shortcut { get; set; }

			/// <summary>
			/// Called when creating the menu item from a command.
			/// </summary>
			/// <remarks>
			/// This is used primarily when creating menu items for system commands that the platform returns
			/// via <see cref="MenuBar.SystemCommands"/>.
			/// </remarks>
			/// <param name="command">Command the menu item is created with.</param>
			void CreateFromCommand(Command command);

			/// <summary>
			/// Gets or sets the text of the menu item, with mnemonics identified with &amp;.
			/// </summary>
			/// <value>The text with mnemonic of the menu item.</value>
			string Text { get; set; }

			/// <summary>
			/// Gets or sets the tool tip of the item.
			/// </summary>
			/// <value>The tool tip.</value>
			string ToolTip { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.MenuItem"/> is enabled.
			/// </summary>
			/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
			bool Enabled { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.MenuItem"/> is visible.
			/// </summary>
			/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
			bool Visible { get; set; }
		}
	}

}
