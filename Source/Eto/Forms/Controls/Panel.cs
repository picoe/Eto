using System;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Forms
{
	[ContentProperty("Content")]
	[Handler(typeof(Panel.IHandler))]
	public class Panel : Container
	{
		public Panel()
		{
		}

		protected Panel(IHandler handler)
			: base(handler)
		{
		}

		[Obsolete("Use default constructor instead")]
		public Panel(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

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

		public static Padding DefaultPadding = Padding.Empty;

		public override IEnumerable<Control> Controls
		{
			get
			{
				var content = Handler == null ? null : Handler.Content;
				if (content != null)
					yield return content;
			}
		}

		public Padding Padding
		{
			get { return Handler.Padding; }
			set { Handler.Padding = value; }
		}

		public Size MinimumSize
		{
			get { return Handler.MinimumSize; }
			set { Handler.MinimumSize = value; }
		}

		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

		public Control Content
		{
			get { return Handler.Content; }
			set
			{
				var old = Handler.Content;
				if (old != value)
				{
					if (old != null)
						RemoveParent(old);
					if (value != null)
					{
						SetParent(value, () => Handler.Content = value);
					}
					else
						Handler.Content = value;
				}
			}
		}

		public override void Remove(Control child)
		{
			if (ReferenceEquals(Content, child))
			{
				Content = null;
			}
		}

		public interface IHandler : Container.IHandler, IContextMenuHost
		{
			Control Content { get; set; }

			Padding Padding { get; set; }

			Size MinimumSize { get; set; }
		}

	}
}
