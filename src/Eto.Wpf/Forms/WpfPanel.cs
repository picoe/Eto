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

		public override bool Enabled
		{
			get => base.Enabled;
			set
			{
				base.Enabled = value;

				// some controls (Expander, GroupBox, Scrollable, etc) don't directly affect children for some (strange) reason.
				border.IsEnabled = value;
			}
		}

		public override Size ClientSize
		{
			get
			{
				if (!Control.IsLoaded)
					return clientSize ?? Size;
				// when the child of a border is null, it doesn't return the correct size
				if (border.Child == null)
					return Size;
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
					if (Widget.Loaded)
						SetContentScale(XScale, YScale);
				}
				else
					border.Child = null;
				Control.InvalidateMeasure();
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
