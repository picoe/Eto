using a = Android;
using av = Android.Views;

namespace Eto.Android.Forms
{
	public interface IAndroidControl
	{
		av.View ContainerControl { get; }
	}

	/// <summary>
	/// Base handler for <see cref="Control"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class AndroidControl<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IAndroidControl
		where TControl: av.View
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public abstract av.View ContainerControl { get; }

		private Boolean attachedTouchEvent;
		private Boolean attachedMouseDown;
		private Boolean attachedMouseUp;

		public override void AttachEvent(String id)
		{
			switch (id)
			{
				case Eto.Forms.Control.MouseDownEvent:
					if(!attachedTouchEvent)
						Control.Touch += Control_Touch;
					attachedMouseDown = attachedTouchEvent = true;
					break;

				case Eto.Forms.Control.MouseUpEvent:
					if (!attachedTouchEvent)
						Control.Touch += Control_Touch;
					attachedMouseUp = attachedTouchEvent = true;
					break;

				case Eto.Forms.Control.SizeChangedEvent:
					Control.LayoutChange += Control_LayoutChange;
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected virtual void Control_Touch(Object sender, av.View.TouchEventArgs e)
		{
			if (e.Event.Action == av.MotionEventActions.Down)
			{
				if (attachedMouseDown)
				{
					var DownEvent = e.Event.ToEto();
					Callback.OnMouseDown(Widget, DownEvent);
				}

				// If we don't 'handle' touch down, we won't get a touch up event
				e.Handled = true;
				return;
			}

			if (e.Event.Action == av.MotionEventActions.Up)
			{
				if (attachedMouseUp)
				{
					var UpEvent = e.Event.ToEto();
					Callback.OnMouseUp(Widget, UpEvent);
					e.Handled = UpEvent.Handled;
				}

				return;
			}
		}

		private void Control_LayoutChange(Object sender, av.View.LayoutChangeEventArgs e)
		{
			Callback.OnSizeChanged(Widget, EventArgs.Empty);
		}

		public SizeF GetPreferredSize(SizeF availableSize)
		{
			return availableSize;
		}

		public void Invalidate(bool invalidateChildren)
		{
			Control.Invalidate();
		}

		public virtual void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			Control.Invalidate(rect.ToAndroid());
		}

		public void SuspendLayout()
		{
		}

		public void ResumeLayout()
		{
		}

		public virtual void Focus()
		{
			Control.RequestFocus();
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		public virtual void SetParent(Container parent)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
			throw new NotImplementedException();
		}

		public PointF PointToScreen(PointF point)
		{
			throw new NotImplementedException();
		}

		Color? backgroundColor;

		public Color BackgroundColor
		{
			get { return backgroundColor ?? Colors.Transparent; }
			set
			{
				backgroundColor = value;
				ApplyBackgroundColor(value);
			}
		}

		protected virtual void ApplyBackgroundColor(Color? value)
		{
			ContainerControl.SetBackgroundColor((value ?? Colors.Transparent).ToAndroid());
		}

		Color? foregroundColor;

		public Color ForegroundColor
		{
			get { return foregroundColor ?? Colors.Transparent; }
			set { foregroundColor = value; }
		}

		private Size UserPreferredSize = new Size(-1, -1);

		public virtual Size Size
		{
			get
			{
				if (!Widget.Loaded)
					return UserPreferredSize;
				
				var SizeInDp = new Size(ContainerControl.Width, ContainerControl.Height);
				return Platform.PxToDp(SizeInDp);
			}
			set
			{
				if (UserPreferredSize == value)
					return;

				UserPreferredSize = value;

				value = Platform.DpToPx(value);

				Control.SetMinimumWidth(value.Width);
				Control.SetMinimumHeight(value.Height);
			}
		}

		public int Width
		{
			get => Size.Width;
			set => Size = new Size(value, UserPreferredSize.Height);
		}

		public int Height
		{
			get => Size.Height;
			set => Size = new Size(UserPreferredSize.Width, value);
		}


		public virtual bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public bool HasFocus
		{
			get { return Control.IsFocused; }
		}

		public bool Visible
		{
			get { return Control.Visibility == av.ViewStates.Visible; }
			set 
			{
				var Requested = value ? av.ViewStates.Visible : av.ViewStates.Gone;

				if (Control.Visibility == Requested)
					return;

				Control.Visibility = Requested;

				// Layouts do not share common base class or interface, so need to check for each...
				//(this.Widget as ILayout)?.Update();
				if (Widget is Layout layout)
					layout.Update();
				else if (Widget is StackLayout stackLayout)
					LayoutUpdateContainers(stackLayout);
				// else other kinds of non-Layout layouts
			}
		}

		/// <summary>
		/// This is copied from <see cref="Layout.UpdateContainers"/>, but that is private and cannot be called from here
		/// </summary>
		private void LayoutUpdateContainers(Container container)
		{
			foreach (var c in container.VisualControls.OfType<Layout>())
			{
				c.Update();
			}
		}

		public virtual Point Location
		{
			get
			{
				return new Point(ContainerControl.Left, ContainerControl.Top);
			}
			set { }
		}

		public IEnumerable<string> SupportedPlatformCommands
		{
			get { return Enumerable.Empty<string>(); }
		}

		public void MapPlatformCommand(string systemAction, Command action)
		{
			throw new NotImplementedException();
		}

		// TODO: Implement ToolTip
		public string ToolTip { get; set; }

		// TODO: Implement Cursor
		public Cursor Cursor { get; set; }

		// TODO: Implement ShowBorder
		public bool ShowBorder { get; set; }

		// TODO: Implement TabIndex
		public int TabIndex { get; set; }

		public virtual IEnumerable<Control> VisualControls => Enumerable.Empty<Control>();

		// TODO: Implement AllowDrop
		public Boolean AllowDrop { get; set; }

		public void DoDragDrop(DataObject data, DragEffects allowedEffects)
		{
			throw new NotImplementedException();
		}

		public void SetParent(Container oldParent, Container newParent)
		{
		}

		public void DoDragDrop(DataObject data, DragEffects allowedEffects, Image image, PointF cursorOffset)
		{
			throw new NotImplementedException();
		}

		public Window GetNativeParentWindow()
		{
			throw new NotImplementedException();
		}

		public virtual void UpdateLayout()
		{
		}
	}
}
