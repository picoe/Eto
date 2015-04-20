using System;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Control to display a panel containing a single <see cref="Panel.Content"/> control
	/// </summary>
	/// <remarks>
	/// This can be instantiated directly to provide padding around a control, and is also the base of other containers that have
	/// only a single child control.
	/// </remarks>
	/// <copyright>(c) 2015 by Nicolas Pöhlmann</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[ContentProperty("Items")]
	[Handler(typeof(DockView.IHandler))]
	public class DockView : Panel
	{
		internal new IHandler Handler { get { return (IHandler)base.Handler; } }

		DockViewCollection items;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DockView"/> class.
		/// </summary>
		public DockView()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DockView"/> class with the specified handler.
		/// </summary>
		/// <param name="handler">Handler to use for the implementation of the control.</param>
		protected DockView(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Gets a value indicating the <see cref="DockView"/> is supported in the platform
		/// </summary>
		/// <value><c>true</c> if supported; otherwise, <c>false</c>.</value>
		public static bool IsSupported
		{
			get { return Platform.Instance.Supports<IHandler>(); }
		}

		/// <summary>
		/// Gets the collection of controls in the <see cref="DockView"/>.
		/// </summary>
		/// <value>The controls collection.</value>
		public DockViewCollection Items
		{
			get
			{
				return items ?? (items = new DockViewCollection(this));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="DockView"/>
		/// </summary>
		public new interface IHandler : Panel.IHandler
		{
			/// <summary>
			/// Adds a control at the specified index to the <see cref="DockView"/>.
			/// </summary>
			/// <param name="control">Control to add.</param>
			/// <param name="index">Index in the DockView to add the control.</param>
			void AddControl(Control control, int index);

			/// <summary>
			/// Clears all controls from the <see cref="DockView"/>.
			/// </summary>
			void Clear();

			/// <summary>
			/// Removes the specified control from the <see cref="DockView"/>.
			/// </summary>
			/// <param name="control">Control to remove.</param>
			void RemoveControl(Control control);
		}
	}
}
