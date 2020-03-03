using System;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Forms
{
	interface IThemedControlHandler
	{
	}

	/// <summary>
	/// Base class for implementing Eto control handlers using other Eto controls.
	/// </summary>
	/// <remarks>
	/// This allows a single implementation to be used in multiple platforms and is useful in a couple of scenarios:
	/// 
	/// a) Creating default implementations of a control on platforms that do not support the control natively.
	/// b) Implementing a control with a non-native look and feel that is consistent across platforms.
	/// </remarks>
	/// <typeparam name="TControl">The Eto control used to create the custom implementation, e.g. Panel</typeparam>
	/// <typeparam name="TWidget">The control being implemented, e.g. TabControl</typeparam>
	/// <typeparam name="TCallback">The callback inferface for the control, e.g. TabControl.ICallback</typeparam>
	public class ThemedControlHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IThemedControlHandler
		where TControl : Control
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		/// <summary>
		/// Called to initialize this widget after it has been constructed
		/// </summary>
		/// <remarks>
		/// Override this to initialize any of the platform objects.  This is called
		/// in the widget constructor, after all of the widget's constructor code has been called.
		/// </remarks>
		protected override void Initialize()
		{
			base.Initialize();
			Control.IsVisualControl = true;
		}

		/// <summary>
		/// Gets or sets the color for the background of the control
		/// </summary>
		/// <remarks>Note that on some platforms (e.g. Mac), setting the background color of a control can change the performance
		/// characteristics of the control and its children, since it must enable layers to do so.</remarks>
		/// <value>The color of the background.</value>
		public virtual Color BackgroundColor
		{
			get { return Control.BackgroundColor; }
			set { Control.BackgroundColor = value; }
		}

		/// <summary>
		/// Gets an enumeration of controls that are in the visual tree.
		/// </summary>
		/// <remarks>
		/// This is used to specify which controls are contained by this instance that are part of the visual tree.
		/// This should include all controls including non-logical Eto controls used for layout. 
		/// </remarks>
		/// <value>The visual controls.</value>
		public virtual IEnumerable<Control> VisualControls => new[] { Control };

		/// <summary>
		/// Gets a value indicating whether <see cref="Control.PreLoad"/>/<see cref="Control.Load"/>/<see cref="Control.LoadComplete"/>/<see cref="Control.UnLoad"/>
		/// events are propagated to the inner control
		/// </summary>
		/// <remarks>
		/// Typically this should be true so that the events are propagated, but when you set the control hierarchy 
		/// manually, such as a <see cref="TabPage"/> on a <see cref="TabControl"/>, you can return false here
		/// since the load events will be handled automatically by the internal eto controls.
		/// </remarks>
		/// <value><c>true</c> if propagate load events; otherwise, <c>false</c>.</value>
		public virtual bool PropagateLoadEvents { get { return true; } }

		/// <summary>
		/// Gets or sets the size of the control. Use -1 to specify auto sizing for either the width and/or height.
		/// </summary>
		/// <value>The size.</value>
		public virtual Size Size
		{
			get { return Control.Size; }
			set { Control.Size = value; }
		}

		/// <summary>
		/// Gets or sets the width of the control size.
		/// </summary>
		public virtual int Width
		{
			get => Control.Width;
			set => Control.Width = value;
		}

		/// <summary>
		/// Gets or sets the height of the control size.
		/// </summary>
		public virtual int Height
		{
			get => Control.Height;
			set => Control.Height = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this control is enabled
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public virtual bool Enabled
		{ 
			get { return Control.Enabled; } 
			set { Control.Enabled = value; }
		}

		/// <summary>
		/// Queues a repaint of the entire control on the screen
		/// </summary>
		/// <remarks>This is only useful when the control is visible.</remarks>
		/// <param name="invalidateChildren"><c>True</c> to invalidate all children, <c>false</c> to only invalidate the container</param>
		public virtual void Invalidate(bool invalidateChildren)
		{
			Control.Invalidate(invalidateChildren);
		}

		/// <summary>
		/// Queues a repaint of the specified <paramref name="rect"/> of the control
		/// </summary>
		/// <remarks>
		/// This is only useful when the control is visible.
		/// </remarks>
		/// <param name="rect">Rectangle to repaint</param>
		/// <param name="invalidateChildren"><c>True</c> to invalidate all children, <c>false</c> to only invalidate the container</param>
		public virtual void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			Control.Invalidate(rect, invalidateChildren);
		}

		/// <summary>
		/// Suspends the layout of child controls
		/// </summary>
		/// <remarks>
		/// This can be used to optimize some platforms while adding, removing, or changing many child controls at once.
		/// It disables the calculation of control positioning until <see cref="ResumeLayout"/> is called.
		/// Each call to SuspendLayout() must be balanced with a call to <see cref="ResumeLayout"/>.
		/// </remarks>
		public virtual void SuspendLayout()
		{
			Control.SuspendLayout();
		}

		/// <summary>
		/// Resumes the layout after it has been suspended, and performs a layout
		/// </summary>
		/// <remarks>
		/// This can be used to optimize some platforms while adding, removing, or changing many child controls at once.
		/// Each call to ResumeLayout() must be balanced with a call to <see cref="SuspendLayout"/> before it.
		/// </remarks>
		public virtual void ResumeLayout()
		{
			Control.ResumeLayout();
		}

		/// <summary>
		/// Attempts to set the keyboard input focus to this control, or the first child that accepts focus
		/// </summary>
		public virtual void Focus()
		{
			Control.Focus();
		}

		/// <summary>
		/// Gets a value indicating whether this instance has the keyboard input focus.
		/// </summary>
		/// <value><c>true</c> if this instance has focus; otherwise, <c>false</c>.</value>
		public virtual bool HasFocus
		{
			get { return Control.HasFocus; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this control is visible to the user.
		/// </summary>
		/// <remarks>
		/// When the visibility of a control is set to false, it will still occupy space in the layout, but not be shown.
		/// The only exception is for controls like the <see cref="Splitter"/>, which will hide a pane if the visibility
		/// of one of the panels is changed.
		/// </remarks>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public virtual bool Visible
		{
			get { return Control.Visible; }
			set { Control.Visible = value; }
		}

		/// <summary>
		/// Called before the control is loaded on a form
		/// </summary>
		/// <param name="e">Event arguments</param>
		/// <seealso cref="OnLoadComplete"></seealso>
		/// <seealso cref="OnLoad"></seealso>
		/// <seealso cref="OnUnLoad"></seealso>
		public virtual void OnPreLoad(EventArgs e)
		{
			if (PropagateLoadEvents)
				Control.TriggerPreLoad(e);
		}

		/// <summary>
		/// Called when the control is loaded on a form
		/// </summary>
		/// <param name="e">Event arguments</param>
		/// <seealso cref="OnPreLoad"></seealso>
		/// <seealso cref="OnLoadComplete"></seealso>
		/// <seealso cref="OnUnLoad"></seealso>
		public virtual void OnLoad(EventArgs e)
		{
			if (PropagateLoadEvents)
				Control.TriggerLoad(e);
		}

		/// <summary>
		/// Called after all other controls have been loaded
		/// </summary>
		/// <param name="e">Event arguments</param>
		/// <seealso cref="OnPreLoad"></seealso>
		/// <seealso cref="OnLoad"></seealso>
		/// <seealso cref="OnUnLoad"></seealso>
		public virtual void OnLoadComplete(EventArgs e)
		{
			if (PropagateLoadEvents)
				Control.TriggerLoadComplete(e);
		}

		/// <summary>
		/// Called when the control is unloaded, which is when it is not currently on a displayed window
		/// </summary>
		/// <param name="e">Event arguments</param>
		/// <seealso cref="OnPreLoad"></seealso>
		/// <seealso cref="OnLoad"></seealso>
		/// <seealso cref="OnLoadComplete"></seealso>
		public virtual void OnUnLoad(EventArgs e)
		{
			if (PropagateLoadEvents)
				Control.TriggerUnLoad(e);
		}

		/// <summary>
		/// Called when the parent of the control has been set
		/// </summary>
		/// <param name="oldParent">Old parent for the control, or null if the control is added</param>
		/// <param name="newParent">New parent for the control, or null if the parent was removed</param>
		public virtual void SetParent(Container oldParent, Container newParent)
		{
			Control.VisualParent = newParent;
		}

		/// <summary>
		/// Converts a point from screen space to control space.
		/// </summary>
		/// <returns>The point in control space</returns>
		/// <param name="point">Point in screen space</param>
		public virtual PointF PointFromScreen(PointF point)
		{
			return Control.PointFromScreen(point);
		}

		/// <summary>
		/// Converts a point from control space to screen space
		/// </summary>
		/// <returns>The point in screen space</returns>
		/// <param name="point">Point in control space</param>
		public virtual PointF PointToScreen(PointF point)
		{
			return Control.PointToScreen(point);
		}

		/// <summary>
		/// Gets the supported platform commands that can be used to hook up system functions to user defined logic
		/// </summary>
		/// <value>The supported platform commands.</value>
		public virtual IEnumerable<string> SupportedPlatformCommands
		{
			get { yield break; }
		}

		/// <summary>
		/// Specifies a command to execute for a platform-specific command
		/// </summary>
		/// <param name="systemAction">System action.</param>
		/// <param name="action">Action.</param>
		public virtual void MapPlatformCommand(string systemAction, Command action)
		{
			Control.MapPlatformCommand(systemAction, action);
		}

		/// <summary>
		/// Gets the location of the control as positioned by the container
		/// </summary>
		/// <remarks>A control's location is set by the container.
		/// This can be used to determine where the control is for overlaying floating windows, menus, etc.</remarks>
		/// <value>The current location of the control</value>
		public virtual Point Location
		{
			get { return Control.Location; }
		}

		/// <summary>
		/// Gets or sets the tool tip to show when the mouse is hovered over the control
		/// </summary>
		/// <value>The tool tip.</value>
		public virtual string ToolTip
		{
			get { return Control.ToolTip; }
			set { Control.ToolTip = value; }
		}

		/// <summary>
		/// Gets or sets the type of cursor to use when the mouse is hovering over the control
		/// </summary>
		/// <value>The mouse cursor</value>
		public virtual Cursor Cursor
		{
			get { return Control.Cursor; }
			set { Control.Cursor = value; }
		}

		/// <summary>
		/// Gets the instance of the platform-specific object
		/// </summary>
		/// <value>The control object.</value>
		public virtual object ControlObject
		{
			get { return Control; }
		}

		/// <summary>
		/// Gets the control used to attach keyboard and text input events
		/// </summary>
		/// <value>The keyboard control.</value>
		protected virtual Control KeyboardControl => Control;

		/// <summary>
		/// Gets the control used to attach mouse events
		/// </summary>
		/// <value>The mouse control.</value>
		protected virtual Control MouseControl => Control;

		/// <summary>
		/// Gets the control used to attach focus events.
		/// </summary>
		/// <value>The focus control.</value>
		protected virtual Control FocusControl => KeyboardControl;

		/// <summary>
		/// Gets or sets the tab index order for this control within its container.
		/// </summary>
		/// <remarks>
		/// This sets the order when using the tab key to cycle through controls
		/// 
		/// Note that some platforms (Gtk and WinForms) may not support setting the context of the tab order to StackLayout 
		/// or DynamicLayout containers and may not behave exactly as expected. Use the 
		/// <see cref="PlatformFeatures.TabIndexWithCustomContainers"/> flag to determine if it is supported.
		/// </remarks>
		/// <value>The index of the control in the tab order.</value>
		public int TabIndex
		{
			get { return FocusControl.TabIndex; }
			set { FocusControl.TabIndex = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this control can serve as drop target.
		/// </summary>
		public virtual bool AllowDrop
		{
			get { return Control.AllowDrop; }
			set { Control.AllowDrop = value; }
		}

		/// <summary>
		/// Starts drag operation using this control as drag source.
		/// </summary>
		/// <param name="data">Drag data.</param>
		/// <param name="allowedAction">Allowed action.</param>
		/// <param name="image">Custom drag image</param>
		/// <param name="cursorOffset">Offset of the cursor to the drag image</param>
		public void DoDragDrop(DataObject data, DragEffects allowedAction, Image image, PointF cursorOffset)
		{
			Control.DoDragDrop(data, allowedAction, image, cursorOffset);
		}

		#region Events

		/// <summary>
		/// Attaches the specified event to the platform-specific control
		/// </summary>
		/// <remarks>Implementors should override this method to handle any events that the widget
		/// supports. Ensure to call the base class' implementation if the event is not
		/// one the specific widget supports, so the base class' events can be handled as well.</remarks>
		/// <param name="id">Identifier of the event</param>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.KeyDownEvent:
					KeyboardControl.KeyDown += (s, e) => Callback.OnKeyDown(Widget, e);
					break;
				case Eto.Forms.Control.KeyUpEvent:
					KeyboardControl.KeyUp += (s, e) => Callback.OnKeyUp(Widget, e);
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					Control.SizeChanged += (s, e) => Callback.OnSizeChanged(Widget, e);
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
					MouseControl.MouseDoubleClick += (s, e) => Callback.OnMouseDoubleClick(Widget, e);
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					MouseControl.MouseEnter += (s, e) => Callback.OnMouseEnter(Widget, e);
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					MouseControl.MouseLeave += (s, e) => Callback.OnMouseLeave(Widget, e);
					break;
				case Eto.Forms.Control.MouseDownEvent:
					MouseControl.MouseDown += (s, e) => Callback.OnMouseDown(Widget, e);
					break;
				case Eto.Forms.Control.MouseUpEvent:
					MouseControl.MouseUp += (s, e) => Callback.OnMouseUp(Widget, e);
					break;
				case Eto.Forms.Control.MouseMoveEvent:
					MouseControl.MouseMove += (s, e) => Callback.OnMouseMove(Widget, e);
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					MouseControl.MouseWheel += (s, e) => Callback.OnMouseWheel(Widget, e);
					break;
				case Eto.Forms.Control.GotFocusEvent:
					FocusControl.GotFocus += (s, e) => Callback.OnGotFocus(Widget, e);
					break;
				case Eto.Forms.Control.LostFocusEvent:
					FocusControl.LostFocus += (s, e) => Callback.OnLostFocus(Widget, e);
					break;
				case Eto.Forms.Control.TextInputEvent:
					KeyboardControl.TextInput += (s, e) => Callback.OnTextInput(Widget, e);
					break;
				case Eto.Forms.Control.ShownEvent:
					Control.Shown += (s, e) => Callback.OnShown(Widget, e);
					break;
				case Eto.Forms.Control.EnabledChangedEvent:
					Control.EnabledChanged += (s, e) => Callback.OnEnabledChanged(Widget, e);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		/// <summary>
		/// Gets the window this control is contained in
		/// </summary>
		/// <returns>The parent window, or null if it is not currently on a window</returns>
		public Window GetNativeParentWindow() => Control.ParentWindow;

		#endregion

	}
}
