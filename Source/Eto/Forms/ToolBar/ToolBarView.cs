using System;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Control to display a tool bar containing a single <see cref="ToolBar"/> control
	/// </summary>
	/// <remarks>
	/// This can be instantiated directly to provide padding around a control, and is also the base of other containers that have
	/// only a single child control.
	/// </remarks>
	/// <copyright>(c) 2015 by Nicolas Pöhlmann</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[ContentProperty("Content")]
	[Handler(typeof(ToolBarView.IHandler))]
	public class ToolBarView : Panel
	{
		internal new IHandler Handler { get { return (IHandler)base.Handler; } }

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
		/// Gets or sets the docking position for the <see cref="ToolBarView"/>.
		/// </summary>
		/// <remarks>
		/// Most platforms will not respect this, and is usually only necessary for constrained devices like iOS and Android.
		/// Each platform may have a different default docking mode, depending on the type of device.
		/// </remarks>
		/// <value>The dock hint.</value>
		public DockPosition Dock
		{
			get { return Handler.Dock; }
			set { Handler.Dock = value; }
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
			/// Gets or sets the docking position for the <see cref="ToolBarView"/>.
			/// </summary>
			/// <remarks>
			/// Some platforms will not respect this, and is usually only necessary for constrained devices like iOS and Android.
			/// Each platform may have a different default docking mode, depending on the type of device.
			/// </remarks>
			/// <value>The dock hint.</value>
			DockPosition Dock { get; set; }
		}
	}
}
