using System;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Control to display a panel containing a single <see cref="Panel.Content"/> control
	/// </summary>
	/// <remarks>
	/// This can be instantiated directly to provide padding around a control, and is also the base of other containers that have
	/// only a single child control.
	/// </remarks>
	[ContentProperty("Content")]
	[Handler(typeof(Panel.IHandler))]
	public class Panel : Container
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Panel"/> class.
		/// </summary>
		public Panel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Panel"/> class with the specified handler.
		/// </summary>
		/// <param name="handler">Handler to use for the implementation of the control.</param>
		protected Panel(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Panel"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public Panel(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Panel"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Panel(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{

		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		[Obsolete("Use Panel(IHandler) instead")]
		protected Panel(Generator generator, IHandler handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
		}

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// The default padding for panels.
		/// </summary>
		[Obsolete("Set the padding of your panel directly or use styles")]
		public static Padding DefaultPadding = Padding.Empty;

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public override IEnumerable<Control> Controls
		{
			get
			{
				var content = Handler == null ? null : Handler.Content;
				if (content != null)
					yield return content;
			}
		}

		/// <summary>
		/// Gets or sets the padding around the <see cref="Content"/> of the panel.
		/// </summary>
		/// <value>The padding around the content.</value>
		public Padding Padding
		{
			get { return Handler.Padding; }
			set { Handler.Padding = value; }
		}

		/// <summary>
		/// Gets or sets the minimum size of the panel.
		/// </summary>
		/// <value>The minimum size.</value>
		public Size MinimumSize
		{
			get { return Handler.MinimumSize; }
			set { Handler.MinimumSize = value; }
		}

		/// <summary>
		/// Gets or sets the context menu for the panel.
		/// </summary>
		/// <remarks>
		/// The context menu is usually shown when the user right clicks the control, or in mobile platforms when the
		/// user taps and holds their finger down on the control.
		/// </remarks>
		/// <value>The context menu.</value>
		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

		/// <summary>
		/// Gets or sets the main content of the panel.
		/// </summary>
		/// <remarks>
		/// The main content will be offset by the <see cref="Padding"/> value specified, and will take up the entire
		/// content area of the control.
		/// Some controls may have decorations that will surround the content, such as a <see cref="GroupBox"/>, <see cref="TabControl"/>,
		/// or <see cref="Window"/>
		/// </remarks>
		/// <value>The child content of the control.</value>
		public Control Content
		{
			get { return Handler.Content; }
			set
			{
				SetParent(value, () => Handler.Content = value, Handler.Content);
			}
		}

		/// <summary>
		/// Removes the specified child from the container
		/// </summary>
		/// <param name="child">Child to remove.</param>
		public override void Remove(Control child)
		{
			if (ReferenceEquals(Content, child))
			{
				Content = null;
			}
		}

		/// <summary>
		/// Handler interface fot the <see cref="Panel"/>
		/// </summary>
		public new interface IHandler : Container.IHandler, IContextMenuHost
		{
			/// <summary>
			/// Gets or sets the main content of the panel.
			/// </summary>
			/// <remarks>
			/// The main content will be offset by the <see cref="Padding"/> value specified, and will take up the entire
			/// content area of the control.
			/// Some controls may have decorations that will surround the content, such as a <see cref="GroupBox"/>, <see cref="TabControl"/>,
			/// or <see cref="Window"/>
			/// </remarks>
			/// <value>The child content of the control.</value>
			Control Content { get; set; }

			/// <summary>
			/// Gets or sets the padding around the <see cref="Content"/> of the panel.
			/// </summary>
			/// <value>The padding around the content.</value>
			Padding Padding { get; set; }

			/// <summary>
			/// Gets or sets the minimum size of the panel.
			/// </summary>
			/// <value>The minimum size.</value>
			Size MinimumSize { get; set; }
		}
	}
}
