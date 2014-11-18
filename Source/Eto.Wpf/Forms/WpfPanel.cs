using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Forms.Menu;

namespace Eto.Wpf.Forms
{
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
				if (!Control.IsLoaded)
					return clientSize ?? Size.Max(Size.Empty, base.Size - Padding.Size);
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

		public override sw.Size GetPreferredSize(sw.Size constraint)
		{
			var size = PreferredSize;
			if (double.IsNaN(size.Width) || double.IsNaN(size.Height))
			{
				sw.Size baseSize;
				if (UseContentSize)
				{
					var padding = border.Padding.Size().Add(ContainerControl.Margin.Size());
					var contentSize = constraint.Subtract(padding);
					var preferredSize = content.GetPreferredSize(contentSize);
					baseSize = new sw.Size(Math.Max(0, preferredSize.Width + padding.Width), Math.Max(0, preferredSize.Height + padding.Height));
				}
				else
					baseSize = base.GetPreferredSize(constraint);

				if (double.IsNaN(size.Width))
					size.Width = baseSize.Width;
				if (double.IsNaN(size.Height))
					size.Height = baseSize.Height;
			}
			return new sw.Size(Math.Max(0, size.Width), Math.Max(0, size.Height));
		}

		ContextMenu contextMenu;
		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
				Control.ContextMenu = contextMenu != null ? ((ContextMenuHandler)contextMenu.Handler).Control : null;
			}
		}

		protected WpfPanel()
		{
			border = new swc.Border
			{
				SnapsToDevicePixels = true,
				Focusable = false,
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
					element.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
					element.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
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
