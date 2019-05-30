using System;
using Eto.Drawing;
using System.Globalization;
using System.Windows.Input;

namespace Eto.Forms
{
	/// <summary>
	/// Command for a menu/tool item that can be checked on or off.
	/// </summary>
	public class CheckCommand : Command, IValueCommand<bool>
	{
#region Events

		/// <summary>
		/// Occurs when the <see cref="Checked"/> value has changed.
		/// </summary>
		public event EventHandler<EventArgs> CheckedChanged;

		/// <summary>
		/// Raises the <see cref="CheckedChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null)
				CheckedChanged(this, e);
		}

#endregion

#region Properties

		bool ischecked;

		/// <summary>
		/// Gets or sets a value indicating whether this command is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked
		{
			get { return ischecked; }
			set
			{
				if (ischecked != value)
				{
					ischecked = value;
					OnCheckedChanged(EventArgs.Empty);
				}
			}
		}

#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckCommand"/> class.
		/// </summary>
		public CheckCommand()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckCommand"/> class with the specified <paramref name="execute"/> handler.
		/// </summary>
		/// <param name="execute">Execute delegate.</param>
		public CheckCommand(EventHandler<EventArgs> execute)
			: base(execute)
		{
		}

		event EventHandler<EventArgs> IValueCommand<bool>.ValueChanged
		{
			add => CheckedChanged += value;
			remove => CheckedChanged -= value;
		}

		bool IValueCommand<bool>.GetValue(object parameter) => Checked;

		void IValueCommand<bool>.SetValue(object parameter, bool value) => Checked = value;

		/// <summary>
		/// Creates a new menu item attached to this command.
		/// </summary>
		/// <returns>The menu item for the command.</returns>
		public override MenuItem CreateMenuItem()
		{
			return new CheckMenuItem(this);
		}

		/// <summary>
		/// Creates a new tool item attached to this command.
		/// </summary>
		/// <returns>The tool item for the command.</returns>
		public override ToolItem CreateToolItem()
		{
			return new CheckToolItem(this);
		}
	}

	/// <summary>
	/// Command for a radio button for a tool or menu item.
	/// </summary>
	/// <remarks>
	/// A radio command works by using a <see cref="RadioCommand.Controller"/> which allows you to group the radio buttons
	/// together.
	/// </remarks>
	public class RadioCommand : CheckCommand
	{
		RadioMenuItem menuItem;

		/// <summary>
		/// Gets or sets the controller of the radio button group.
		/// </summary>
		/// <value>The radio button controller.</value>
		public RadioCommand Controller { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioCommand"/> class.
		/// </summary>
		public RadioCommand()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioCommand"/> class with the specified <paramref name="execute"/> handler.
		/// </summary>
		/// <param name="execute">Delegate to execute when the command triggers.</param>
		public RadioCommand(EventHandler<EventArgs> execute)
			: base(execute)
		{
		}

		/// <summary>
		/// Creates a new menu item attached to this command.
		/// </summary>
		/// <returns>The menu item for the command.</returns>
		public override MenuItem CreateMenuItem()
		{
			return menuItem = new RadioMenuItem(this, Controller != null ? Controller.menuItem : null);
		}

		/// <summary>
		/// Creates a new tool item attached to this command.
		/// </summary>
		/// <returns>The tool item for the command.</returns>
		public override ToolItem CreateToolItem()
		{
			return new RadioToolItem(this);
		}
	}

	/// <summary>
	/// Base command for use on either <see cref="ToolBar"/> or <see cref="MenuBar"/>
	/// </summary>
	/// <remarks>
	/// Commands allow you to create a single class that can be used for both menu and tool items.
	/// </remarks>
	public class Command : IBindable, ICommand
	{
#region Events

		/// <summary>
		/// Occurs when the <see cref="Enabled"/> property is changed.
		/// </summary>
		public event EventHandler<EventArgs> EnabledChanged;

		/// <summary>
		/// Raises the <see cref="EnabledChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnEnabledChanged(EventArgs e)
		{
			if (EnabledChanged != null)
				EnabledChanged(this, e);
		}

		/// <summary>
		/// Occurs when the command is executed from either the menu or toolbar.
		/// </summary>
		public event EventHandler<EventArgs> Executed;

		/// <summary>
		/// Raises the <see cref="Executed"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnExecuted(EventArgs e)
		{
			if (Executed != null)
				Executed(this, e);
		}

#endregion

#region Properties

		/// <summary>
		/// Gets or sets the ID of the command
		/// </summary>
		/// <remarks>
		/// This can be used to identify a command.
		/// </remarks>
		/// <value>The command ID.</value>
		public string ID { get; set; }

		bool enabled = true;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Command"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public virtual bool Enabled
		{
			get { return enabled; }
			set
			{
				if (enabled != value)
				{
					enabled = value;
					OnEnabledChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets a user-defined tag value for this instance.
		/// </summary>
		/// <value>The tag.</value>
		public object Tag { get; set; }

		/// <summary>
		/// Gets or sets the text when shown on the menu.
		/// </summary>
		/// <value>The menu text.</value>
		public string MenuText { get; set; }

		/// <summary>
		/// Gets or sets the tool bar text.
		/// </summary>
		/// <value>The tool bar text.</value>
		public string ToolBarText { get; set; }

		/// <summary>
		/// Gets or sets the tool tip on both the menu and toolbar.
		/// </summary>
		/// <value>The tool tip.</value>
		public string ToolTip { get; set; }

		/// <summary>
		/// Gets or sets the image for the menu or tool item.
		/// </summary>
		/// <remarks>
		/// On some platforms, the menu bar does not show the image by default (e.g. OS X). You can override this behaviour
		/// using a style on the handler.
		/// </remarks>
		/// <value>The image for menu or tool items.</value>
		public Image Image { get; set; }

		/// <summary>
		/// Gets or sets the shortcut to trigger this command.
		/// </summary>
		/// <value>The command shortcut.</value>
		public Keys Shortcut { get; set; }

#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Command"/> class.
		/// </summary>
		public Command()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Command"/> class with the specified <paramref name="execute"/> handler.
		/// </summary>
		/// <param name="execute">Delegate to execute when the command is triggered.</param>
		public Command(EventHandler<EventArgs> execute)
		{
			Executed += execute;
		}

		/// <summary>
		/// Execute the command programatically.
		/// </summary>
		public void Execute()
		{
			OnExecuted(EventArgs.Empty);
		}

		/// <summary>
		/// Creates a new tool item attached to this command.
		/// </summary>
		/// <returns>The tool item for the command.</returns>
		public virtual ToolItem CreateToolItem()
		{
			return new ButtonToolItem(this);
		}

		/// <summary>
		/// Creates a new menu item attached to this command.
		/// </summary>
		/// <returns>The menu item for the command.</returns>
		public virtual MenuItem CreateMenuItem()
		{
			return new ButtonMenuItem(this);
		}

		/// <summary>
		/// Implicitly converts the command to a menu item
		/// </summary>
		/// <param name="command">Command to convert.</param>
		public static implicit operator MenuItem(Command command)
		{
			return command.CreateMenuItem();
		}

		/// <summary>
		/// Implicitly converts the command to a tool item
		/// </summary>
		/// <param name="command">Command to convert.</param>
		public static implicit operator ToolItem(Command command)
		{
			return command.CreateToolItem();
		}

		PropertyStore properties;

		/// <summary>
		/// Gets the dictionary of properties for this widget
		/// </summary>
		public PropertyStore Properties
		{ 
			get { return properties ?? (properties = new PropertyStore(this)); } 
		}

		static readonly object Command_Key = new object();

		/// <summary>
		/// Gets or sets a command to delegate to when the command is invoked.
		/// </summary>
		/// <remarks>
		/// This allows you to use a command to define menu/toolbar items or other functionality, and use
		/// a delegated command to chain to when invoked.
		/// This is especially useful when binding to a view model, you can do something like the following:
		/// <code>
		/// var myCommand = new Command { MenuText = "My Command", Parent = myForm };
		/// myCommand.BindDataContext(c => c.DelegatedCommand, (MyModel m) => m.MyModelCommand);
		/// 
		/// //...
		/// 
		/// myForm.DataContext = new MyModel { MyModelCommand = ... };
		/// </code>
		/// The <see cref="ICommand.CanExecute"/> will also used to set the enabled/disabled state of the current command.
		/// </remarks>
		/// <value>The command to invoke.</value>
		public ICommand DelegatedCommand
		{
			get { return Properties.GetCommand(Command_Key); }
			set { Properties.SetCommand(Command_Key, value, e => Enabled = e, r => Executed += r, r => Executed -= r, () => CommandParameter); }
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

		event EventHandler ICommand.CanExecuteChanged
		{
			add { EnabledChanged += new EventHandler<EventArgs>(value.Invoke); }
			remove { EnabledChanged -= new EventHandler<EventArgs>(value.Invoke); }
		}

		bool ICommand.CanExecute(object parameter)
		{ 
			return Enabled;
		}

		void ICommand.Execute(object parameter)
		{
			Execute();
		}

#region IBindable implementation

		/// <summary>
		/// Event to handle when the <see cref="DataContext"/> has changed
		/// </summary>
		/// <remarks>
		/// This may be fired in the event of a parent in the hierarchy setting the data context.
		/// For example, the <see cref="Forms.Container"/> widget fires this event when it's event is fired.
		/// </remarks>
		public event EventHandler<EventArgs> DataContextChanged;

		/// <summary>
		/// Raises the <see cref="DataContextChanged"/> event
		/// </summary>
		/// <remarks>
		/// Implementors may override this to fire this event on child widgets in a heirarchy. 
		/// This allows a control to be bound to its own <see cref="DataContext"/>, which would be set
		/// on one of the parent control(s).
		/// </remarks>
		/// <param name="e">Event arguments</param>
		protected internal virtual void OnDataContextChanged(EventArgs e)
		{
			if (DataContextChanged != null)
				DataContextChanged(this, e);
		}

		static readonly object Parent_Key = new object();

		/// <summary>
		/// Gets the parent widget which this widget has been added to, if any
		/// </summary>
		/// <value>The parent widget, or null if there is no parent</value>
		public IBindable Parent
		{
			get { return Properties.Get<IBindable>(Parent_Key); }
			set
			{ 
				var old = Parent;
				Properties.Set(Parent_Key, value, () =>
				{
					if (old != null)
						old.DataContextChanged -= Value_DataContextChanged;
					if (value != null)
						value.DataContextChanged += Value_DataContextChanged;
					if (!Properties.ContainsKey(DataContext_Key))
						OnDataContextChanged(EventArgs.Empty);
				});
			}
		}

		void Value_DataContextChanged(object sender, EventArgs e)
		{
			OnDataContextChanged(e);
		}

		static readonly object DataContext_Key = new object();

		/// <summary>
		/// Gets or sets the data context for this widget for binding
		/// </summary>
		/// <remarks>
		/// Subclasses may override the standard behaviour so that hierarchy of widgets can be taken into account.
		/// 
		/// For example, a Control may return the data context of a parent, if it is not set explicitly.
		/// </remarks>
		public object DataContext
		{
			get
			{
				return Properties.Get(DataContext_Key, () =>
				{
					var bindable = Parent as IBindable;
					return bindable != null ? bindable.DataContext : null;
				});
			}
			set { Properties.Set(DataContext_Key, value, () => OnDataContextChanged(EventArgs.Empty)); }
		}

		static readonly  object Bindings_Key = new object();

		/// <summary>
		/// Gets the collection of bindings that are attached to this widget
		/// </summary>
		public BindingCollection Bindings
		{
			get { return Properties.Create(Bindings_Key, () => new BindingCollection()); }
		}

#endregion
	}
}
