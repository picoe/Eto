using System;

namespace Eto.GirCore.Forms;

	static class GtkPanel
	{
		public static readonly object MinimumSize_Key = new object();
	}

	public abstract class GirPanel<TControl, TWidget, TCallback> : GirContainer<TControl, TWidget, TCallback>
		where TControl: Gtk.Widget
		where TWidget: Panel
		where TCallback: Panel.ICallback
	{
		Control content;

		public override Gtk.Widget ContainerContentControl
		{
			get { return Control; }
		}

		protected GirPanel()
		{
		}

		protected virtual bool UseMinimumSizeRequested { get { return true; } }

		protected override void Initialize()
		{
			base.Initialize();
			// SetContainerContent(alignment);
		}

		protected new GirPanelEventConnector Connector { get { return (GirPanelEventConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new GirPanelEventConnector();
		}

		protected class GirPanelEventConnector : GirControlConnector
		{
			public new GirPanel<TControl, TWidget, TCallback> Handler => (GirPanel<TControl, TWidget, TCallback>)base.Handler;
		}

		public virtual Size MinimumSize
		{
			get => Widget.Properties.Get<Size>(GtkPanel.MinimumSize_Key);
			set
			{
				Widget.Properties.Set(GtkPanel.MinimumSize_Key, value);
				ContainerControl.QueueResize();
				SetSize(UserPreferredSize);
			}
		}

		protected override void SetSize(Size size)
		{
			var min = MinimumSize;
			if (min.Width > 0)
				size.Width = Math.Max(size.Width, min.Width);
			if (min.Height > 0)
				size.Height = Math.Max(size.Height, min.Height);

			base.SetSize(size);
		}

		public virtual Padding Padding
		{
			get => new Padding((int)Control.MarginStart, (int)Control.MarginTop, (int)Control.MarginEnd, (int)Control.MarginBottom);
			set
		{
				Control.SetMarginStart((int)value.Left);
				Control.SetMarginEnd((int)value.Right);
				Control.SetMarginTop((int)value.Top);
				Control.SetMarginBottom((int)value.Bottom);
			}
		}

		public Control Content
		{
			get { return content; }
			set
			{
				if (!ReferenceEquals(content, value))
				{
					if (content != null)
						RemoveContainerContent(content.GetContainerWidget());
					content = value;
					var widget = content.GetContainerWidget();
					if (widget != null)
					{
						SetContainerContent(widget);
					}
					InvalidateMeasure();					
				}
			}
		}

		public override Gtk.Widget BackgroundControl => Control;

		protected abstract void SetContainerContent(Gtk.Widget content);
		protected abstract void RemoveContainerContent(Gtk.Widget content);
	}
