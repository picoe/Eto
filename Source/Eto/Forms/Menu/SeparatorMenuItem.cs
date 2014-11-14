using System;

namespace Eto.Forms
{
	/// <summary>
	/// Menu item to separate menu items
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(SeparatorMenuItem.IHandler))]
	public class SeparatorMenuItem : MenuItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SeparatorMenuItem"/> class.
		/// </summary>
		public SeparatorMenuItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SeparatorMenuItem"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public SeparatorMenuItem(Generator generator) : this(generator, typeof(SeparatorMenuItem.IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SeparatorMenuItem"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SeparatorMenuItem(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="SeparatorMenuItem"/>
		/// </summary>
		public new interface IHandler : MenuItem.IHandler
		{
		}
	}
}