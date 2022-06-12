using System;
using Eto.Drawing;
using System.Windows.Input;

namespace Eto.Forms
{
	/// <summary>
	/// Button image position
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum ButtonImagePosition
	{
		/// <summary>
		/// Positions the image to the left of the text
		/// </summary>
		Left,

		/// <summary>
		/// Positions the image to the right of the text
		/// </summary>
		Right,

		/// <summary>
		/// Positions the image on top of the text
		/// </summary>
		Above,

		/// <summary>
		/// Positions the image below the text
		/// </summary>
		Below,

		/// <summary>
		/// Positions the image behind the text
		/// </summary>
		Overlay
	}

	/// <summary>
	/// Button control
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(Button.IHandler))]
	public class Button : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static readonly object Click_Key = new object();

		/// <summary>
		/// Event to handle when the user clicks the button
		/// </summary>
		public event EventHandler<EventArgs> Click
		{
			add { Properties.AddEvent(Click_Key, value); }
			remove { Properties.RemoveEvent(Click_Key, value); }
		}

		/// <summary>
		/// Raises the <see cref="Click"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnClick(EventArgs e)
		{
			Properties.TriggerEvent(Click_Key, this, e);
		}

		static readonly object Command_Key = new object();

		/// <summary>
		/// Gets or sets the command to invoke when the button is pressed.
		/// </summary>
		/// <remarks>
		/// This will invoke the specified command when the button is pressed.
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
		/// Gets or sets the image to display on the button
		/// </summary>
		/// <value>The image to display</value>
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		/// <summary>
		/// Gets or sets the position of the image relative to the text
		/// </summary>
		/// <value>The image position</value>
		public ButtonImagePosition ImagePosition
		{
			get { return Handler.ImagePosition; }
			set { Handler.ImagePosition = value; }
		}

		/// <summary>
		/// Gets or sets the minimum size for the button.
		/// </summary>
		/// <remarks>
		/// Each platform may have a different initial minimum size set for buttons to match their standard sizes.
		/// 
		/// Setting this to <see cref="Eto.Drawing.Size.Empty"/> is useful when you want the button to shrink to fit the size
		/// of the specified <see cref="Image"/> and/or <see cref="TextControl.Text"/>.
		/// </remarks>
		public Size MinimumSize
		{
			get { return Handler.MinimumSize; }
			set { Handler.MinimumSize = value; }
		}

		/// <summary>
		/// Gets or sets the size of the control. Use -1 to specify auto sizing for either the width and/or height.
		/// </summary>
		/// <value>The size of the control.</value>
		public override Size Size
		{
			get { return base.Size; }
			set
			{
				base.Size = value;
				// Ensure minimum size is at least as small as the desired explicit size
				if (value.Width != -1 || value.Height != -1)
				{
					var min = MinimumSize;
					var size = Size.Min(value, min);
					if (size.Width == -1)
						size.Width = min.Width;
					if (size.Height == -1)
						size.Height = min.Height;
					if (size != min)
						MinimumSize = size;
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of the control size.
		/// </summary>
		public override int Width
		{
			get => base.Width;
			set
			{
				base.Width = value;
				if (value != -1 && value < MinimumSize.Width)
				{
					MinimumSize = new Size(value, MinimumSize.Height);
				}
			}
		}

		/// <summary>
		/// Gets or sets the height of the control size.
		/// </summary>
		public override int Height
		{
			get => base.Height;
			set
			{
				base.Height = value;
				if (value != -1 && value < MinimumSize.Height)
				{
					MinimumSize = new Size(MinimumSize.Width, value);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Button"/> class.
		/// </summary>
		public Button()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Button"/> class with the specified <paramref name="click"/> handler.
		/// </summary>
		/// <remarks>
		/// This is a convenience constructor to set up the click event.
		/// </remarks>
		/// <param name="click">Delegate to handle when the button is clicked.</param>
		public Button(EventHandler<EventArgs> click)
		{
			Click += click;
		}

		/// <summary>
		/// Triggers the <see cref="Click"/> event for the button, if the button is visible and enabled.
		/// </summary>
		public virtual void PerformClick()
		{
			if (Enabled && Visible)
				OnClick(EventArgs.Empty);
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for <see cref="Button"/>
		/// </summary>
		public new interface ICallback : TextControl.ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			void OnClick(Button widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="Button"/>
		/// </summary>
		protected new class Callback : TextControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			public void OnClick(Button widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnClick(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="Button"/> control
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Gets or sets the image to display on the button
			/// </summary>
			/// <value>The image to display</value>
			Image Image { get; set; }

			/// <summary>
			/// Gets or sets the image position
			/// </summary>
			/// <value>The image position</value>
			ButtonImagePosition ImagePosition { get; set; }

			/// <summary>
			/// Gets or sets the minimum size for the button.
			/// </summary>
			/// <remarks>
			/// Each platform may have a different initial minimum size set for buttons to match their standard sizes.
			/// 
			/// Setting this to <see cref="Eto.Drawing.Size.Empty"/> is useful when you want the button to shrink to fit the size
			/// of the specified <see cref="Image"/> and/or <see cref="TextControl.Text"/>.
			/// </remarks>
			Size MinimumSize { get; set;}
		}
	}
}
