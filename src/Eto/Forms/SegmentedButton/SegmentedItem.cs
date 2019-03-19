using System;
using System.ComponentModel;
using System.Windows.Input;
using Eto.Drawing;
using sc = System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for items of the <see cref="SegmentedButton"/> control.
	/// </summary>
    [sc.TypeConverter(typeof(SegmentedItemConverter))]
    public abstract class SegmentedItem : BindableWidget
    {
        new IHandler Handler => (IHandler)base.Handler;

		static SegmentedItem()
		{
			EventLookup.Register<SegmentedItem>(c => c.OnClick(null), ClickEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.SegmentedItem"/> class.
		/// </summary>
		protected SegmentedItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SegmentedItem"/> class with the specified command.
		/// </summary>
		/// <remarks>
		/// This links the segmented item with the specified command, and will trigger <see cref="Eto.Forms.Command.Execute"/>
		/// when the user clicks the item, and enable/disable the menu item based on <see cref="Eto.Forms.Command.Enabled"/>.
		/// 
		/// This also supports <see cref="RadioCommand"/> and <see cref="CheckCommand"/>, which may be appropriate depending
		/// on the current <see cref="SegmentedButton.SelectionMode"/>.  For <see cref="SegmentedSelectionMode.Multiple"/>, then
		/// CheckCommand would be preferred, whereas for <see cref="SegmentedSelectionMode.Single"/>, RadioCommand would make more
		/// sense. Otherwise, use Command if the mode is <see cref="SegmentedSelectionMode.None"/>.
		/// 
		/// This is not a weak link, so you should not reuse the Command instance for other menu items if you are disposing
		/// this segmented item.
		/// </remarks>
		/// <param name="command">Command to initialize the menu item with.</param>
		protected SegmentedItem(Command command)
		{
			ID = command.ID;
			Text = command.ToolBarText;
			ToolTip = command.ToolTip;
			Image = command.Image;
			Command = command;
		}

		/// <summary>
		/// Gets the parent button this item belongs to.
		/// </summary>
		/// <remarks>
		/// Note this is only set after adding your item to the <see cref="SegmentedButton.Items"/> collection.
		/// </remarks>
		/// <value>The parent control.</value>
		public new SegmentedButton Parent
		{
			get => base.Parent as SegmentedButton;
			internal set => base.Parent = value;
		}

		/// <summary>
		/// Gets or sets the text to display for this segment.
		/// </summary>
		/// <value>The segment text.</value>
		public string Text
        {
            get => Handler.Text;
            set => Handler.Text = value;
        }

		/// <summary>
		/// Gets or sets the ToolTip to display for this segment
		/// </summary>
		/// <value>The segment tooltip.</value>
        public string ToolTip
        {
            get => Handler.ToolTip;
            set => Handler.ToolTip = value;
        }

		/// <summary>
		/// Gets or sets the image to display in this segment.
		/// </summary>
		/// <value>The segment image.</value>
        public Image Image
        {
            get => Handler.Image;
            set => Handler.Image = value;
        }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Enabled
        {
            get => Handler.Enabled;
            set => Handler.Enabled = value;
        }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Visible
        {
            get => Handler.Visible;
            set => Handler.Visible = value;
        }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is selected.
		/// </summary>
		/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected
        {
            get => Handler.Selected;
            set => Handler.Selected = value;
        }

		/// <summary>
		/// Gets or sets the width of this segment, or -1 to auto size.
		/// </summary>
		/// <value>The width of this segment.</value>
		[DefaultValue(-1)]
        public int Width
        {
            get => Handler.Width;
            set => Handler.Width = value;
        }

		static readonly object Command_Key = new object();

		/// <summary>
		/// Gets or sets the command to invoke when the segmented item is pressed.
		/// </summary>
		/// <remarks>
		/// This will invoke the specified command when the segmented item is pressed.
		/// The <see cref="ICommand.CanExecute"/> will also used to set the enabled/disabled state of the segmented item.
		/// </remarks>
		/// <value>The command to invoke.</value>
		public ICommand Command
		{
			get { return Properties.GetCommand(Command_Key); }
			set
			{
				var oldValue = Command;
				if (!ReferenceEquals(oldValue, value))
					SetCommand(oldValue, value);
			}
		}

		internal virtual void SetCommand(ICommand oldValue, ICommand newValue)
		{
			if (oldValue is IValueCommand<bool> lastValueCommand)
			{
				lastValueCommand.ValueChanged -= ValueCommand_ValueChanged;
			}

			Properties.SetCommand(Command_Key, newValue, e => Enabled = e, r => Click += r, r => Click -= r, () => CommandParameter);

			if (newValue is IValueCommand<bool> valueCommand)
			{
				Selected = valueCommand.GetValue(CommandParameter);
				valueCommand.ValueChanged += ValueCommand_ValueChanged;
			}
		}

		void ValueCommand_ValueChanged(object sender, EventArgs e)
		{
			if (Command is IValueCommand<bool> command)
			{
				Selected = command.GetValue(CommandParameter);
			}
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

		#region Events

		/// <summary>
		/// Identifier for handlers to attach the <see cref="Click"/> event.
		/// </summary>
		public const string ClickEvent = "SegmentedItem.Click";

		/// <summary>
		/// Occurs when this segment is clicked.
		/// </summary>
        public event EventHandler<EventArgs> Click
        {
            add => Properties.AddHandlerEvent(ClickEvent, value);
            remove => Properties.RemoveEvent(ClickEvent, value);
        }

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		/// <param name="e">Event argments</param>
        protected virtual void OnClick(EventArgs e)
        {
            Properties.TriggerEvent(ClickEvent, this, e);
        }

		/// <summary>
		/// Identifier for handlers to attach the <see cref="SelectedChanged"/> event.
		/// </summary>
		public const string SelectedChangedEvent = "SegmentedItem.SelectedChanged";

		/// <summary>
		/// Occurs when this <see cref="Selected"/> property changes.
		/// </summary>
		public event EventHandler<EventArgs> SelectedChanged
		{
			add => Properties.AddHandlerEvent(SelectedChangedEvent, value);
			remove => Properties.RemoveEvent(SelectedChangedEvent, value);
		}

		/// <summary>
		/// Raises the <see cref="SelectedChanged"/> event.
		/// </summary>
		/// <param name="e">Event argments</param>
		protected virtual void OnSelectedChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedChangedEvent, this, e);

			if (Command is IValueCommand<bool> command)
			{
				command.SetValue(CommandParameter, Selected);
			}
		}

		#endregion

		static readonly ICallback s_callback = new Callback();

		/// <inheritdoc />
        protected override object GetCallback() => s_callback;

		/// <summary>
		/// Callback interface for handlers of the <see cref="SegmentedItem"/>.
		/// </summary>
        public new interface ICallback : Widget.ICallback
        {
			/// <summary>
			/// Raises the Click event.
			/// </summary>
            void OnClick(SegmentedItem widget, EventArgs e);

			/// <summary>
			/// Raises the SelectedChanged event.
			/// </summary>
			void OnSelectedChanged(SegmentedItem widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="SegmentedItem"/>.
		/// </summary>
		protected class Callback : ICallback
        {
			/// <summary>
			/// Raises the Click event.
			/// </summary>
            public void OnClick(SegmentedItem widget, EventArgs e)
            {
                using (widget.Platform.Context)
                    widget.OnClick(e);
            }

			/// <summary>
			/// Raises the SelectedChanged event.
			/// </summary>
			public void OnSelectedChanged(SegmentedItem widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnSelectedChanged(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="SegmentedItem"/>.
		/// </summary>
        public new interface IHandler : Widget.IHandler
        {
			/// <summary>
			/// Gets or sets the text to display for this segment.
			/// </summary>
			/// <value>The segment text.</value>
			string Text { get; set; }
			/// <summary>
			/// Gets or sets the ToolTip to display for this segment
			/// </summary>
			/// <value>The segment tooltip.</value>
			string ToolTip { get; set; }
			/// <summary>
			/// Gets or sets the image to display in this segment.
			/// </summary>
			/// <value>The segment image.</value>
			Image Image { get; set; }
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is enabled.
			/// </summary>
			/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
			bool Enabled { get; set; }
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is visible.
			/// </summary>
			/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
			bool Visible { get; set; }
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is selected.
			/// </summary>
			/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
			bool Selected { get; set; }
			/// <summary>
			/// Gets or sets the width of this segment, or -1 to auto size.
			/// </summary>
			/// <value>The width of this segment.</value>
			int Width { get; set; }
        }

		/// <summary>
		/// Implicitly converts a string to a segmented item.
		/// </summary>
		/// <remarks>
		/// This allows you to do things like the following:
		/// <code><![CDATA[
		/// new SegmentedButton { Items = { "First", "Second", "Third" } };
		/// ]]></code>
		/// </remarks>
		/// <returns>A segmented item with the specified text.</returns>
		/// <param name="text">Text for the segmented item.</param>
		public static implicit operator SegmentedItem(string text)
        {
            return new ButtonSegmentedItem { Text = text };
        }

		/// <summary>
		/// Implicitly converts an image to a segmented item.
		/// </summary>
		/// <remarks>
		/// This allows you to do things like the following:
		/// <code><![CDATA[
		/// new SegmentedButton { Items = { myImage1, myImage2, myImage3 } };
		/// ]]></code>
		/// </remarks>
		/// <returns>A segmented item with the specified image.</returns>
		/// <param name="image">Image for the segmented item.</param>
		public static implicit operator SegmentedItem(Image image)
        {
            return new ButtonSegmentedItem { Image = image };
		}

		/// <summary>
		/// Implicitly converts a command to a segmented item.
		/// </summary>
		/// <remarks>
		/// This can be a <see cref="RadioCommand"/>, <see cref="CheckCommand"/>, or <see cref="Command"/>
		/// depending on the <see cref="SegmentedButton.SelectionMode"/>.
		/// </remarks>
		/// <returns>A segmented item linked to the specified command.</returns>
		/// <param name="command">Command to create the segmented item.</param>
		public static implicit operator SegmentedItem(Command command)
		{
			return new ButtonSegmentedItem(command);
		}
	}
}
