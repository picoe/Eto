using Eto.Wpf.Forms.Menu;

namespace Eto.Wpf.Forms
{
	public abstract class WpfPanel<TControl, TWidget, TCallback> : WpfContainer<TControl, TWidget, TCallback>, Panel.IHandler
		where TControl : sw.FrameworkElement
		where TWidget : Panel
		where TCallback : Panel.ICallback
	{
		Control _content;
		swc.Border _border;
		Size? _clientSize;

		public override bool Enabled
		{
			get => base.Enabled;
			set
			{
				base.Enabled = value;

				// some controls (Expander, GroupBox, Scrollable, etc) don't directly affect children for some (strange) reason.
				ContentBorder.IsEnabled = value;
			}
		}

		public override Size ClientSize
		{
			get
			{
				if (!Control.IsLoaded)
					return _clientSize ?? Size;
				// when the child of a border is null, it doesn't return the correct size
				if (ContentBorder.Child == null)
					return Size;
				return ContentBorder.GetSize();
			}
			set
			{
				_clientSize = value;
				ContentBorder.SetSize(value);
			}
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			var preferredSize = UserPreferredSize;
			SetContentScale(xscale || !double.IsNaN(preferredSize.Width), yscale || !double.IsNaN(preferredSize.Height));
		}

		protected virtual void SetContentScale(bool xscale, bool yscale)
		{
			var contentHandler = _content.GetWpfFrameworkElement();
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
		}

		swc.Border ContentBorder => _border ??= CreateContentBorder();

		protected virtual swc.Border CreateContentBorder()
		{
			return new swc.Border
			{
				SnapsToDevicePixels = true,
				Focusable = false,
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetContainerContent(ContentBorder);
		}

		public Padding Padding
		{
			get { return ContentBorder.Padding.ToEto(); }
			set { ContentBorder.Padding = value.ToWpf(); }
		}

		public Control Content
		{
			get { return _content; }
			set
			{
				_content = value;
				if (_content != null)
				{
					var wpfelement = _content.GetWpfFrameworkElement();
					var element = wpfelement.ContainerControl;
					element.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
					element.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
					ContentBorder.Child = element;
					if (Widget.Loaded)
						SetContentScale(XScale, YScale);
				}
				else
					ContentBorder.Child = null;
				Control.InvalidateMeasure();
				OnChildPreferredSizeUpdated();
			}
		}
		
		public abstract void SetContainerContent(sw.FrameworkElement content);

		public override void Remove(sw.FrameworkElement child)
		{
			if (ContentBorder.Child == child)
			{
				_content = null;
				ContentBorder.Child = null;
				OnChildPreferredSizeUpdated();
			}
		}
	}
}
