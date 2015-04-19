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
    [Handler(typeof(ToolBarView.IHandler))]
	public class ToolBarView : Panel
	{
        internal new IHandler Handler { get { return (IHandler)base.Handler; } }

		ToolBarCollection items;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ToolBarView"/> class.
		/// </summary>
		public ToolBarView()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ToolBarView"/> class with the specified handler.
		/// </summary>
		/// <param name="handler">Handler to use for the implementation of the control.</param>
        protected ToolBarView(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Gets the collection of items in the toolbar.
		/// </summary>
		/// <value>The tool item collection.</value>
		public ToolBarCollection Items
		{
			get
			{
				return items ?? (items = new ToolBarCollection(this));
			}
		}

		/// <summary>
		/// Gets a value indicating the <see cref="ToolBarView"/> is supported in the platform
		/// </summary>
		/// <value><c>true</c> if supported; otherwise, <c>false</c>.</value>
		public static bool IsSupported
		{
			get { return Platform.Instance.Supports<IHandler>(); }
		}

        /// <summary>
        /// Handler interface for the <see cref="ToolBarView"/>
        /// </summary>
		public new interface IHandler : Panel.IHandler
        {
			/// <summary>
			/// Adds a tool bar at the specified index.
			/// </summary>
			/// <param name="toolbar">Tool bar to add.</param>
			/// <param name="index">Index of the tool bar to add.</param>
			void AddToolBar(ToolBar toolbar, int index);

			/// <summary>
			/// Clears all tool bars from the <see cref="ToolBarView"/>
			/// </summary>
			void Clear();

			/// <summary>
			/// Removes the specified tool bar from the tool bar view.
			/// </summary>
			/// <param name="toolbar">Tool bar to remove.</param>
			void RemoveToolBar(ToolBar toolbar);
        }
    }
}
