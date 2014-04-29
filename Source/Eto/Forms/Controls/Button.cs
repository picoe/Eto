using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Handler interface for the <see cref="Button"/> control
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IButton : ITextControl
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

	/// <summary>
	/// Button image position
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
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
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class Button : TextControl
	{
		new IButton Handler { get { return (IButton)base.Handler; } }

		/// <summary>
		/// The default minimum size for buttons
		/// </summary>
		/// <remarks>
		/// You can set this size to ensure that all buttons are at least of this size
		/// </remarks>
		[Obsolete("This is no longer supported. Set the size of your buttons directly")]
		public static Size DefaultSize = new Size (80, 26);

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
		public virtual void OnClick (EventArgs e)
		{
			if (click != null)
				click (this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Button"/> class.
		/// </summary>
		public Button ()
			: this (null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Button"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the button</param>
		public Button (Generator generator)
			: this (generator, typeof (IButton))
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
		protected Button (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
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
	}
}
