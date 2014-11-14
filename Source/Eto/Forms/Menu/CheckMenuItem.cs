using System;

namespace Eto.Forms
{
	/// <summary>
	/// Menu item that can be toggled on and off
	/// </summary>
	/// <remarks>
	/// Most platforms show a check box next to the item when selected.  Some platforms may not show the item's image.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(CheckMenuItem.IHandler))]
	public class CheckMenuItem : MenuItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckMenuItem"/> class.
		/// </summary>
		public CheckMenuItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckMenuItem"/> class with the specified command.
		/// </summary>
		/// <param name="command">Command to initialize the menu with.</param>
		public CheckMenuItem(CheckCommand command)
			: base(command)
		{
			Checked = command.Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
			Handler.CreateFromCommand(command);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckMenuItem"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public CheckMenuItem(Generator generator)
			: this(generator, typeof(CheckMenuItem.IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckMenuItem"/> class.
		/// </summary>
		/// <param name="command">Command.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use constructor without generator instead")]
		public CheckMenuItem(CheckCommand command, Generator generator = null)
			: base(command, generator, typeof(CheckMenuItem.IHandler))
		{
			Checked = command.Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckMenuItem"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected CheckMenuItem(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.CheckMenuItem"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="CheckMenuItem"/>.
		/// </summary>
		public new interface IHandler : MenuItem.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether the menu item is checked.
			/// </summary>
			/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
			bool Checked { get; set; }
		}
	}
}