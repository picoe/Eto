using System;
using Eto.Drawing;

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

		/// <summary>
		/// The default minimum size for buttons
		/// </summary>
		/// <remarks>
		/// You can set this size to ensure that all buttons are at least of this size
		/// </remarks>
		[Obsolete("This is no longer supported. Set the size of your buttons directly")]
		public static Size DefaultSize = new Size(80, 26);

		EventHandler<EventArgs> click;

		/// <summary>
		/// Event to handle when the user clicks the button
		/// </summary>
		public event EventHandler<EventArgs> Click
		{
			add { click += value; }
			remove { click -= value; }
		}

		/// <summary>
		/// Raises the <see cref="Click"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnClick(EventArgs e)
		{
			if (click != null)
				click(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Button"/> class.
		/// </summary>
		public Button()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Button"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the button</param>
		[Obsolete("Use default constructor instead")]
		public Button(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Button"/> class
		/// </summary>
		/// <remarks>
		/// Used by subclasses of a button to allow them to define a different handler interface
		/// to create as the platform handler of the button
		/// </remarks>
		/// <param name="generator">Generator to create the button</param>
		/// <param name="type">Type of the button handler to use for the subclass</param>
		/// <param name="initialize">True to initialize the button, false if you will initialize after constructor logic</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Button(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
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
				widget.Platform.Invoke(() => widget.OnClick(e));
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
		}
	}
}
