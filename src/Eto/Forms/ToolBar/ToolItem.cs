using System;
using Eto.Drawing;
using System.Windows.Input;
using sc = System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Base tool item class for a <see cref="ToolBar"/>.
	/// </summary>
	[sc.TypeConverter(typeof(ToolItemConverter))]
	public abstract class ToolItem : Tool, ICommandItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static readonly object Command_Key = new object();

		/// <summary>
		/// Gets or sets the command to invoke when the tool item is pressed.
		/// </summary>
		/// <remarks>
		/// This will invoke the specified command when the tool item is pressed.
		/// The <see cref="ICommand.CanExecute"/> will also used to set the enabled/disabled state of the tool item.
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
		/// Occurs when the user clicks on the item.
		/// </summary>
		public event EventHandler<EventArgs> Click;

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		public void OnClick(EventArgs e)
		{
			if (Click != null) Click(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ToolItem"/> class.
		/// </summary>
		protected ToolItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ToolItem"/> class with the specified <paramref name="command"/>.
		/// </summary>
		/// <param name="command">Command to initialize the tool item with.</param>
		protected ToolItem(Command command)
		{
			ID = command.ID;
			Text = command.ToolBarText;
			ToolTip = command.ToolTip;
			Image = command.Image;
			Command = command;
		}

		/// <summary>
		/// Gets or sets the order of the tool item when adding to the <see cref="ToolItemCollection"/>.
		/// </summary>
		/// <value>The order when adding the item.</value>
		public int Order { get; set; }

		/// <summary>
		/// Gets or sets the text of the item, with mnemonic.
		/// </summary>
		/// <value>The text.</value>
		public string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Gets or sets the tool tip to show when hovering the mouse over the item.
		/// </summary>
		/// <value>The tool tip.</value>
		public string ToolTip
		{
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}

		/// <summary>
		/// Gets or sets the image for the tool item.
		/// </summary>
		/// <value>The image.</value>
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.ToolItem"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Enabled
		{
			get { return Handler.Enabled; }
			set { Handler.Enabled = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.ToolItem"/> is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public bool Visible
		{
			get { return Handler.Visible; }
			set { Handler.Visible = value; }
		}

		/// <summary>
		/// Gets or sets a user-defined tag for the tool item.
		/// </summary>
		/// <value>The user-defined tag.</value>
		public object Tag { get; set; }

		/// <summary>
		/// Handler interface for the <see cref="ToolItem"/>.
		/// </summary>
		public new interface IHandler : Tool.IHandler
		{
			/// <summary>
			/// Gets or sets the image for the tool item.
			/// </summary>
			/// <value>The image.</value>
			Image Image { get; set; }

			/// <summary>
			/// Creates the item from a command instance.
			/// </summary>
			/// <remarks>
			/// This is useful when using a platform-defined command. It allows you to create the item in a specific
			/// way based on the command it is created from.
			/// </remarks>
			/// <param name="command">Command the item is created from.</param>
			void CreateFromCommand(Command command);

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
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.ToolItem"/> is enabled.
			/// </summary>
			/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
			bool Enabled { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.ToolItem"/> is visible.
			/// </summary>
			/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
			bool Visible { get; set; }
		}
	}
}
