using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Eto.Drawing;
using Eto.Forms;
using Windows.UI.Xaml.Markup;
using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;
using scm = System.ComponentModel;

namespace Eto.WinRT.Forms
{

	/// <summary>
	/// Control to display a tool bar containing a single <see cref="ToolBar"/> control
	/// </summary>
	/// <copyright>(c) 2015 by Nicolas Pöhlmann</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ToolBarViewHandler : WpfControl<swc.CommandBar, ToolBarView, ToolBarView.ICallback>, ToolBarView.IHandler
	{
		Control content;
		ContextMenu contextMenu;
		static readonly object minimumSizeKey = new object();

		public ToolBarViewHandler()
		{
			Control = new swc.CommandBar();
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		public Size ClientSize
		{
			get
			{
#if TODO_XAML
				if (!Control.IsLoaded && clientSize != null)
					return clientSize.Value;
#endif
				return Control.GetSize();
			}
			set
			{
				Control.SetSize(value);
			}
		}

		public Control Content
		{
			get { return content; }
			set
			{
				if (Widget.Loaded)
					SuspendLayout();

				if (content != null)
				{
					var contentHandler = content.GetWpfFrameworkElement();
					contentHandler.SetScale(false, false);
				}
				
				content = value;

				if (content != null)
				{
					var contentHandler = content.GetWpfFrameworkElement();
					SetContent(contentHandler.ContainerControl);
				}
				
				if (Widget.Loaded)
					ResumeLayout();
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
#if TODO_XAML
				Control.ContextMenu = contextMenu != null ? ((ContextMenuHandler)contextMenu.Handler).Control : null;
#else
				throw new NotImplementedException();
#endif
			}
		}
		
		public Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(minimumSizeKey) ?? Size.Empty; }
			set
			{
				if (value != MinimumSize)
				{
					Widget.Properties[minimumSizeKey] = value;
					Control.MinHeight = value.Height;
					Control.MinWidth = value.Width;
				}
			}
		}
		
		public virtual Padding Padding
		{
			get { return this.Control.Padding.ToEto(); }
			set { this.Control.Padding = value.ToWpf(); }
		}

		public virtual void SetContent(sw.FrameworkElement content)
		{
			//this.Control.Content = content as swc.AppBar;
			//this.Control = content as swc.AppBar;
			//Control = content as swc.AppBar;
			Control = content as swc.CommandBar;
		}
		
		public bool RecurseToChildren
		{
			get { return true; }
		}
	}
}
