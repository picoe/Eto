using System;
using System.Collections.Generic;
using Eto.Drawing;

#if XAML
using System.Windows.Markup;

#endif
namespace Eto.Forms
{
	public interface IDockContainer : IContainer
#if DESKTOP
		, IContextMenuHost
#endif
	{
		Control Content { get; set; }

		Padding Padding { get; set; }

		Size MinimumSize { get; set; }
	}

	[ContentProperty("Content")]
	public abstract class DockContainer : Container
	{
		new IDockContainer Handler { get { return (IDockContainer)base.Handler; } }

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

#if DESKTOP
		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}
#endif

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
						var load = SetParent(value);
						Handler.Content = value;
						if (load)
							value.OnLoadComplete(EventArgs.Empty);
					}
					else
						Handler.Content = value;
				}
			}
		}

		[Obsolete("Use Content property instead")]
		public Control Layout { get { return Content; } set { Content = value; } }

		protected DockContainer(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		protected DockContainer(Generator generator, IDockContainer handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
		}

		public override void Remove(Control child)
		{
			if (ReferenceEquals(Content, child))
			{
				Content = null;
			}
		}
	}
}
