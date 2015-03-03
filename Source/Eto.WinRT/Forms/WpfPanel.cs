using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using Eto.Drawing;
//using Eto.WinRT.Forms.Menu;

namespace Eto.WinRT.Forms
{
	/// <summary>
	/// Panel handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class WpfPanel<TControl, TWidget, TCallback> : WpfContainer<TControl, TWidget, TCallback>, Panel.IHandler
		where TControl : sw.FrameworkElement
		where TWidget : Panel
		where TCallback : Panel.ICallback
	{
		Control content;
		readonly swc.Border border;
		Size? clientSize;

		protected virtual bool UseContentSize { get { return true; } }

		public override Size ClientSize
		{
			get
			{
#if TODO_XAML
				if (!Control.IsLoaded && clientSize != null)
					return clientSize.Value;
#endif
				return border.GetSize();
			}
			set
			{
				clientSize = value;
				border.SetSize(value);
			}
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			SetContentScale(xscale, yscale);
		}

		protected virtual void SetContentScale(bool xscale, bool yscale)
		{
			var contentHandler = content.GetWpfFrameworkElement();
			if (contentHandler != null)
			{
				contentHandler.SetScale(xscale, yscale);
			}
		}

		public override wf.Size GetPreferredSize(wf.Size constraint)
		{
			var size = PreferredSize;
			if (double.IsNaN(size.Width) || double.IsNaN(size.Height))
			{
				wf.Size baseSize;
				if (UseContentSize)
				{
					var padding = border.Padding.Size().Add(ContainerControl.Margin.Size());
					var contentSize = constraint.Subtract(padding);
					var preferredSize = content.GetPreferredSize(contentSize);
					baseSize = new wf.Size(Math.Max(0, preferredSize.Width + padding.Width), Math.Max(0, preferredSize.Height + padding.Height));
				}
				else
					baseSize = base.GetPreferredSize(constraint);

				if (double.IsNaN(size.Width))
					size.Width = baseSize.Width;
				if (double.IsNaN(size.Height))
					size.Height = baseSize.Height;
			}
			return new wf.Size(Math.Max(0, size.Width), Math.Max(0, size.Height));
		}

		ContextMenu contextMenu;
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

		protected WpfPanel()
		{
			border = new swc.Border
			{
#if TODO_XAML
				SnapsToDevicePixels = true,
				Focusable = false,
#endif
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetContainerContent(border);
		}

		public Padding Padding
		{
			get { return border.Padding.ToEto(); }
			set { border.Padding = value.ToWpf(); }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				content = value;
				if (content != null)
				{
					var wpfelement = content.GetWpfFrameworkElement();
					var element = wpfelement.ContainerControl;
					element.VerticalAlignment = sw.VerticalAlignment.Stretch;
					element.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
					border.Child = element;
					SetContentScale(XScale, YScale);
				}
				else
					border.Child = null;
				UpdatePreferredSize();
			}
		}

		public abstract void SetContainerContent(sw.FrameworkElement content);

		public override void Remove(sw.FrameworkElement child)
		{
			if (border.Child == child)
			{
				content = null;
				border.Child = null;
				UpdatePreferredSize();
			}
		}
	}
}
