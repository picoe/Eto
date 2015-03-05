using System;

namespace Eto.Forms
{
	/// <summary>
	/// Menu item to choose from a set of options
	/// </summary>
	/// <remarks>
	/// The RadioMenuItem works with other radio items to present a list of options that the user can select from.
	/// When a radio button is toggled on, all others that are linked together will be toggled off.
	/// 
	/// To link radio buttons together, use the <see cref="C:Eto.Forms.RadioMenuItem(RadioMenuItem)"/> constructor
	/// to specify the controller radio item, which can be created with the default constructor.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(RadioMenuItem.IHandler))]
	public class RadioMenuItem : MenuItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioMenuItem"/> class.
		/// </summary>
		public RadioMenuItem()
		{
			Handler.Create(null);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioMenuItem"/> class.
		/// </summary>
		/// <param name="controller">Controller radio menu item to link to, or null if no controller.</param>
		public RadioMenuItem(RadioMenuItem controller)
		{
			Handler.Create(controller);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioMenuItem"/> class with the specified command and controller.
		/// </summary>
		/// <param name="command">Command to initialize the menu item with.</param>
		/// <param name="controller">Controller radio menu item to link to, or null if no controller.</param>
		public RadioMenuItem(RadioCommand command, RadioMenuItem controller = null)
			: base(command)
		{
			Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Handler.Create(controller);
			Initialize();
			Handler.CreateFromCommand(command);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.RadioMenuItem"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="RadioMenuItem"/>.
		/// </summary>
		[AutoInitialize(false)]
		public new interface IHandler : MenuItem.IHandler
		{
			/// <summary>
			/// Creates the menu item with the specified controller.
			/// </summary>
			/// <param name="controller">Controller radio menu item to link to, or null if no controller.</param>
			void Create(RadioMenuItem controller);

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.RadioMenuItem"/> is checked.
			/// </summary>
			/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
			bool Checked { get; set; }
		}
	}
}