using System;
using System.Collections.Generic;
using sc = System.ComponentModel;
using System.Globalization;
using Eto.Drawing;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Base for all visual UI elements
	/// </summary>
	/// <remarks>
	/// All visual user interface elements should inherit from this class to provide common functionality like binding,
	/// load/unload, and common events.
	/// </remarks>
	#if !NETSTANDARD
	[ToolboxItem(true)]
	[DesignTimeVisible(true)]
	[DesignerCategory("Eto.Forms")]
	#endif
	[sc.TypeConverter(typeof(ControlConverter))]
	public partial class Control : BindableWidget, IMouseInputSource, IKeyboardInputSource, ICallbackSource
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Gets a value indicating that the control is loaded onto a form, that is it has been created, added to a parent, and shown
		/// </summary>
		/// <remarks>
		/// The <see cref="OnLoad"/> method sets this value to <c>true</c> after cascading to all children (for a <see cref="Container"/>)
		/// and calling the platform handler's implementation.  It is called after adding to a loaded form, or when showing a new form.
		/// 
		/// The <see cref="OnUnLoad"/> method will set this value to <c>false</c> when the control is removed from its parent
		/// </remarks>
		public bool Loaded
		{
			get => GetState(StateFlag.Loaded);
			private set => SetState(StateFlag.Loaded, value);
		}

		/// <summary>
		/// Gets an enumeration of controls that are in the visual tree.
		/// </summary>
		/// <remarks>
		/// This is used to specify which controls are contained by this instance that are part of the visual tree.
		/// This should include all controls including non-logical Eto controls used for layout. 
		/// </remarks>
		/// <value>The visual controls.</value>
		public virtual IEnumerable<Control> VisualControls => Handler.VisualControls;

		/// <summary>
		/// Gets or sets a user-defined object that contains data about the control
		/// </summary>
		/// <remarks>
		/// A common use of the tag property is to store data that is associated with the control that you can later
		/// retrieve.
		/// </remarks>
		public object Tag
		{
			get { return Properties.Get<object>(TagKey); }
			set { Properties[TagKey] = value; }
		}

		/// <summary>
		/// Gets the logical parent control.
		/// </summary>
		/// <remarks>
		/// When the control is part of the visual tree (<see cref="IsVisualControl"/> is true), this returns the logical parent that contains this control.
		/// Otherwise this is the same as <see cref="Parent"/>.
		/// </remarks>
		/// <value>The logical parent.</value>
		public Container LogicalParent
		{
			get
			{
				if (IsVisualControl)
				{
					var foundVisual = false;
					foreach (var parent in Parents.OfType<Container>())
					{
						if (!foundVisual && parent.GetState(StateFlag.IsVisualControl))
							foundVisual = true;
						else
							return parent;
					}
				}
				return Parent;
			}
		}

		/// <summary>
		/// Gets a value indicating this <see cref="T:Eto.Forms.Control"/> is part of the visual tree.
		/// </summary>
		/// <value><c>true</c> if is visual control; otherwise, <c>false</c>.</value>
		public bool IsVisualControl
		{
			get => GetState(StateFlag.IsVisualControl, StateFlag.IsVisualControlHasValue) ?? Parent?.IsVisualControl ?? false; // traverse up logical tree
			internal set => SetState(StateFlag.IsVisualControl, StateFlag.IsVisualControlHasValue, value);
		}

		static readonly object TagKey = new object();

		#region Events

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.SizeChanged"/> event
		/// </summary>
		public const string SizeChangedEvent = "Control.SizeChanged";

		/// <summary>
		/// Occurs when the size of the control is changed.
		/// </summary>
		public event EventHandler<EventArgs> SizeChanged
		{
			add { Properties.AddHandlerEvent(SizeChangedEvent, value); }
			remove { Properties.RemoveEvent(SizeChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Control.SizeChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnSizeChanged(EventArgs e)
		{
			Properties.TriggerEvent(SizeChangedEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.KeyDown"/> event.
		/// </summary>
		public const string KeyDownEvent = "Control.KeyDown";

		/// <summary>
		/// Occurs when a key has been pressed and is down
		/// </summary>
		/// <seealso cref="KeyUp"/>
		public event EventHandler<KeyEventArgs> KeyDown
		{
			add { Properties.AddHandlerEvent(KeyDownEvent, value); }
			remove { Properties.RemoveEvent(KeyDownEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Control.KeyDown"/> event.
		/// </summary>
		/// <param name="e">Key event arguments</param>
		protected virtual void OnKeyDown(KeyEventArgs e)
		{
			Properties.TriggerEvent(KeyDownEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.KeyUp"/> event
		/// </summary>
		public const string KeyUpEvent = "Control.KeyUp";

		/// <summary>
		/// Occurs when a key was released
		/// </summary>
		/// <seealso cref="KeyDown"/>
		public event EventHandler<KeyEventArgs> KeyUp
		{
			add { Properties.AddHandlerEvent(KeyUpEvent, value); }
			remove { Properties.RemoveEvent(KeyUpEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Control.KeyUp"/> event.
		/// </summary>
		/// <param name="e">Key event arguments</param>
		protected virtual void OnKeyUp(KeyEventArgs e)
		{
			Properties.TriggerEvent(KeyUpEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.TextInput"/> event
		/// </summary>
		public const string TextInputEvent = "Control.TextInput";

		/// <summary>
		/// Occurs when text is input for the control. Currently only partially supported on iOS.
		/// </summary>
		public event EventHandler<TextInputEventArgs> TextInput
		{
			add { Properties.AddHandlerEvent(TextInputEvent, value); }
			remove { Properties.RemoveEvent(TextInputEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="TextInput"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnTextInput(TextInputEventArgs e)
		{
			Properties.TriggerEvent(TextInputEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.MouseDown"/> event
		/// </summary>
		public const string MouseDownEvent = "Control.MouseDown";

		/// <summary>
		/// Occurs when a mouse button has been pressed
		/// </summary>
		/// <remarks>
		/// Controls will typically capture the mouse after a mouse button is pressed and will be released
		/// only after the <see cref="MouseUp"/> event.
		/// </remarks>
		/// <seealso cref="Control.MouseUp"/>
		public event EventHandler<MouseEventArgs> MouseDown
		{
			add { Properties.AddHandlerEvent(MouseDownEvent, value); }
			remove { Properties.RemoveEvent(MouseDownEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Control.MouseDown"/> event.
		/// </summary>
		/// <remarks>
		/// To override default behaviour of the control, set <see cref="MouseEventArgs.Handled"/> property to <c>true</c>.
		/// </remarks>
		/// <param name="e">Event arguments</param>
		protected virtual void OnMouseDown(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseDownEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.MouseUp"/> event
		/// </summary>
		public const string MouseUpEvent = "Control.MouseUp";

		/// <summary>
		/// Occurs when a mouse button is released
		/// </summary>
		/// <seealso cref="MouseDown"/>
		public event EventHandler<MouseEventArgs> MouseUp
		{
			add { Properties.AddHandlerEvent(MouseUpEvent, value); }
			remove { Properties.RemoveEvent(MouseUpEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Control.MouseUp"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnMouseUp(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseUpEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.MouseMove"/> event
		/// </summary>
		public const string MouseMoveEvent = "Control.MouseMove";

		/// <summary>
		/// Occurs when mouse moves within the bounds of the control, or when the mouse is captured
		/// </summary>
		/// <remarks>
		/// The mouse is captured after a <see cref="MouseDown"/> event within the control, 
		/// and is released when the mouse button is released
		/// </remarks>
		/// <seealso cref="MouseDown"/>
		/// <seealso cref="MouseUp"/>
		public event EventHandler<MouseEventArgs> MouseMove
		{
			add { Properties.AddHandlerEvent(MouseMoveEvent, value); }
			remove { Properties.RemoveEvent(MouseMoveEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="MouseMove"/> event.
		/// </summary>
		/// <param name="e">Mouse event args</param>
		protected virtual void OnMouseMove(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseMoveEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.MouseLeave"/> event
		/// </summary>
		public const string MouseLeaveEvent = "Control.MouseLeave";

		/// <summary>
		/// Occurs when mouse leaves the bounds of the control
		/// </summary>
		public event EventHandler<MouseEventArgs> MouseLeave
		{
			add { Properties.AddHandlerEvent(MouseLeaveEvent, value); }
			remove { Properties.RemoveEvent(MouseLeaveEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="MouseLeave"/> event.
		/// </summary>
		/// <param name="e">Mouse event arguments</param>
		/// <seealso cref="MouseEnter"/>
		protected virtual void OnMouseLeave(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseLeaveEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.MouseEnter"/> event
		/// </summary>
		public const string MouseEnterEvent = "Control.MouseEnter";

		/// <summary>
		/// Occurs when the mouse enters the bounds of the control
		/// </summary>
		/// <seealso cref="MouseLeave"/>
		public event EventHandler<MouseEventArgs> MouseEnter
		{
			add { Properties.AddHandlerEvent(MouseEnterEvent, value); }
			remove { Properties.RemoveEvent(MouseEnterEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="MouseEnter"/> event.
		/// </summary>
		/// <param name="e">Mouse event arguments</param>
		protected virtual void OnMouseEnter(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseEnterEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.MouseDoubleClick"/> event
		/// </summary>
		public const string MouseDoubleClickEvent = "Control.MouseDoubleClick";

		/// <summary>
		/// Occurs when a mouse button is double clicked within the bounds of the control
		/// </summary>
		/// <remarks>
		/// If you do not set the <see cref="MouseEventArgs.Handled"/> property to true, and the default behaviour of
		/// the control does not accept double clicks, the <see cref="MouseDown"/> event will be called for each click of
		/// the mouse button. 
		/// 
		/// For example, if the user clicks twice in succession, the following will be called:
		/// 1. MouseDown for the first click
		/// 2. MouseDoubleClick for the second click
		/// 3. If Handled has not been set in #2, MouseDown will be called a 2nd time
		/// </remarks>
		/// <seealso cref="MouseDown"/>
		public event EventHandler<MouseEventArgs> MouseDoubleClick
		{
			add { Properties.AddHandlerEvent(MouseDoubleClickEvent, value); }
			remove { Properties.RemoveEvent(MouseDoubleClickEvent, value); }
		}

		/// <summary>
		/// Raises the mouse <see cref="MouseDoubleClick"/> event.
		/// </summary>
		/// <param name="e">Mouse event arguments</param>
		protected virtual void OnMouseDoubleClick(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseDoubleClickEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.MouseWheel"/> event
		/// </summary>
		public const string MouseWheelEvent = "Control.MouseWheel";

		/// <summary>
		/// Occurs when mouse wheel has been changed
		/// </summary>
		public event EventHandler<MouseEventArgs> MouseWheel
		{
			add { Properties.AddHandlerEvent(MouseWheelEvent, value); }
			remove { Properties.RemoveEvent(MouseWheelEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="MouseWheel"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnMouseWheel(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseWheelEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.GotFocus"/> event
		/// </summary>
		public const string GotFocusEvent = "Control.GotFocus";

		/// <summary>
		/// Occurs when the control receives keyboard focus.
		/// </summary>
		/// <remarks>
		/// Note that not all controls can recieve keyboard focus.
		/// </remarks>
		/// <seealso cref="LostFocus"/>
		public event EventHandler<EventArgs> GotFocus
		{
			add { Properties.AddHandlerEvent(GotFocusEvent, value); }
			remove { Properties.RemoveEvent(GotFocusEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="GotFocus"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnGotFocus(EventArgs e)
		{
			Properties.TriggerEvent(GotFocusEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.LostFocus"/> event
		/// </summary>
		public const string LostFocusEvent = "Control.LostFocus";

		/// <summary>
		/// Occurs when control loses keyboard focus
		/// </summary>
		/// <remarks>
		/// Note that not all controls can recieve keyboard focus
		/// </remarks>
		/// <seealso cref="GotFocus"/>
		public event EventHandler<EventArgs> LostFocus
		{
			add { Properties.AddHandlerEvent(LostFocusEvent, value); }
			remove { Properties.RemoveEvent(LostFocusEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="LostFocus"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnLostFocus(EventArgs e)
		{
			Properties.TriggerEvent(LostFocusEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Control.Shown"/> event
		/// </summary>
		public const string ShownEvent = "Control.Shown";

		/// <summary>
		/// Occurs when the control is shown on the screen
		/// </summary>
		/// <remarks>
		/// This event fires when the <see cref="Visible"/> property changes, or when initially showing a control
		/// on a form.
		/// </remarks>
		public event EventHandler<EventArgs> Shown
		{
			add { Properties.AddHandlerEvent(ShownEvent, value); }
			remove { Properties.RemoveEvent(ShownEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Shown"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnShown(EventArgs e)
		{
			Properties.TriggerEvent(ShownEvent, this, e);
		}

		static readonly object PreLoadKey = new object();

		/// <summary>
		/// Occurs before the control is loaded. See the <see cref="Load"/> event for more detail.
		/// </summary>
		/// <seealso cref="Load"/>
		/// <seealso cref="LoadComplete"/>
		/// <seealso cref="UnLoad"/>
		public event EventHandler<EventArgs> PreLoad
		{
			add { Properties.AddEvent(PreLoadKey, value); }
			remove { Properties.RemoveEvent(PreLoadKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="PreLoad"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnPreLoad(EventArgs e)
		{
			Properties.TriggerEvent(PreLoadKey, this, e);
			Handler.OnPreLoad(e);

			OnApplyCascadingStyles();
		}

		static readonly object LoadKey = new object();

		/// <summary>
		/// Occurs when the control is displayed on a visible window
		/// </summary>
		/// <remarks>
		/// A control is loaded when it is part of the control hierarchy and is shown on a window.
		/// When the control is removed from the hierarchy, or the window is closed, the <see cref="UnLoad"/> event
		/// will be called.
		/// </remarks>
		/// <seealso cref="PreLoad"/>
		/// <seealso cref="LoadComplete"/>
		/// <seealso cref="UnLoad"/>
		public event EventHandler<EventArgs> Load
		{
			add { Properties.AddEvent(LoadKey, value); }
			remove { Properties.RemoveEvent(LoadKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="Load"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnLoad(EventArgs e)
		{
#if DEBUG
			if (Loaded)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Control was loaded more than once"));
#endif
			Properties.TriggerEvent(LoadKey, this, e);
			Handler.OnLoad(e);
			Loaded = true;
		}

		static readonly object LoadCompleteKey = new object();

		/// <summary>
		/// Occurs when the load is complete, which happens after the <see cref="Load"/> event
		/// </summary>
		/// <seealso cref="Load"/>
		/// <seealso cref="PreLoad"/>
		/// <seealso cref="UnLoad"/>
		public event EventHandler<EventArgs> LoadComplete
		{
			add { Properties.AddEvent(LoadCompleteKey, value); }
			remove { Properties.RemoveEvent(LoadCompleteKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="LoadComplete"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnLoadComplete(EventArgs e)
		{
			Properties.TriggerEvent(LoadCompleteKey, this, e);
			Handler.OnLoadComplete(e);
		}

		static readonly object UnLoadKey = new object();

		/// <summary>
		/// Occurs when the control is unloaded, which happens when removed from the control hierarchy or the window is closed.
		/// </summary>
		/// <seealso cref="Load"/>
		/// <seealso cref="LoadComplete"/>
		/// <seealso cref="PreLoad"/>
		public event EventHandler<EventArgs> UnLoad
		{
			add { Properties.AddEvent(UnLoadKey, value); }
			remove { Properties.RemoveEvent(UnLoadKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="UnLoad"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnUnLoad(EventArgs e)
		{
#if DEBUG
			if (!Loaded)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Control was unloaded more than once"));
#endif
			Loaded = false;
			Properties.TriggerEvent(UnLoadKey, this, e);
			Handler.OnUnLoad(e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="DragDrop"/> event
		/// </summary>
		public const string DragDropEvent = "Control.DragDrop";

		/// <summary>
		/// Occurs when a drag operation is dropped onto the control.
		/// </summary>
		/// <remarks>
		/// This should perform any of the actual drop logic and update the control state to reflect the dropped data.
		/// Any cleanup should be performed in the <see cref="DragLeave"/> event, which is called immediately before this event.
		/// </remarks>
		public event EventHandler<DragEventArgs> DragDrop
		{
			add { Properties.AddHandlerEvent(DragDropEvent, value); }
			remove { Properties.RemoveEvent(DragDropEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="DragDrop"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDragDrop(DragEventArgs e)
		{
			Properties.TriggerEvent(DragDropEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="DragOver"/> event
		/// </summary>
		public const string DragOverEvent = "Control.DragOver";

		/// <summary>
		/// Occurs when a drag operation is over the control and needs updating based on position or keyboard state changes.
		/// </summary>
		public event EventHandler<DragEventArgs> DragOver
		{
			add { Properties.AddHandlerEvent(DragOverEvent, value); }
			remove { Properties.RemoveEvent(DragOverEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="DragOver"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDragOver(DragEventArgs e) => Properties.TriggerEvent(DragOverEvent, this, e);

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="DragEnter"/> event
		/// </summary>
		public const string DragEnterEvent = "Control.DragEnter";

		/// <summary>
		/// Occurs when a drag operation enters the bounds of the control.
		/// </summary>
		public event EventHandler<DragEventArgs> DragEnter
		{
			add { Properties.AddHandlerEvent(DragEnterEvent, value); }
			remove { Properties.RemoveEvent(DragEnterEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="DragEnter"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDragEnter(DragEventArgs e) => Properties.TriggerEvent(DragEnterEvent, this, e);

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="DragLeave"/> event
		/// </summary>
		public const string DragLeaveEvent = "Control.DragLeave";

		/// <summary>
		/// Occurs when a drag operation leaves the bounds of the control or the drag operation was completed inside the control.
		/// </summary>
		/// <remarks>
		/// Use this event to 'clean up' any state of the control for the current drag operation.
		/// This will be called before the <see cref="DragDrop"/> event.
		/// </remarks>
		public event EventHandler<DragEventArgs> DragLeave
		{
			add { Properties.AddHandlerEvent(DragLeaveEvent, value); }
			remove { Properties.RemoveEvent(DragLeaveEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="DragLeave"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDragLeave(DragEventArgs e) => Properties.TriggerEvent(DragLeaveEvent, this, e);

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="EnabledChanged"/> event
		/// </summary>
		public const string EnabledChangedEvent = "Control.EnabledChanged";

		/// <summary>
		/// Occurs when the <see cref="Enabled"/> value is changed.
		/// </summary>
		public event EventHandler<EventArgs> EnabledChanged
		{
			add { Properties.AddHandlerEvent(EnabledChangedEvent, value); }
			remove { Properties.RemoveEvent(EnabledChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="EnabledChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnEnabledChanged(EventArgs e) => Properties.TriggerEvent(EnabledChangedEvent, this, e);

		#endregion

		static Control()
		{
			EventLookup.Register<Control>(c => c.OnGotFocus(null), Control.GotFocusEvent);
			EventLookup.Register<Control>(c => c.OnKeyDown(null), Control.KeyDownEvent);
			EventLookup.Register<Control>(c => c.OnKeyUp(null), Control.KeyUpEvent);
			EventLookup.Register<Control>(c => c.OnLostFocus(null), Control.LostFocusEvent);
			EventLookup.Register<Control>(c => c.OnMouseDoubleClick(null), Control.MouseDoubleClickEvent);
			EventLookup.Register<Control>(c => c.OnMouseDown(null), Control.MouseDownEvent);
			EventLookup.Register<Control>(c => c.OnMouseEnter(null), Control.MouseEnterEvent);
			EventLookup.Register<Control>(c => c.OnMouseLeave(null), Control.MouseLeaveEvent);
			EventLookup.Register<Control>(c => c.OnMouseMove(null), Control.MouseMoveEvent);
			EventLookup.Register<Control>(c => c.OnMouseUp(null), Control.MouseUpEvent);
			EventLookup.Register<Control>(c => c.OnMouseWheel(null), Control.MouseWheelEvent);
			EventLookup.Register<Control>(c => c.OnShown(null), Control.ShownEvent);
			EventLookup.Register<Control>(c => c.OnSizeChanged(null), Control.SizeChangedEvent);
			EventLookup.Register<Control>(c => c.OnTextInput(null), Control.TextInputEvent);
			EventLookup.Register<Control>(c => c.OnDragDrop(null), Control.DragDropEvent);
			EventLookup.Register<Control>(c => c.OnDragOver(null), Control.DragOverEvent);
			EventLookup.Register<Control>(c => c.OnDragEnter(null), Control.DragEnterEvent);
			EventLookup.Register<Control>(c => c.OnDragLeave(null), Control.DragLeaveEvent);
			EventLookup.Register<Control>(c => c.OnEnabledChanged(null), Control.EnabledChangedEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Control"/> class.
		/// </summary>
		protected Control()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		public Control(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Queues a repaint of the entire control on the screen and any of its children.
		/// </summary>
		/// <remarks>
		/// This is only useful when the control is visible.
		/// </remarks>
		public void Invalidate()
		{
			Handler.Invalidate(true);
		}

		/// <summary>
		/// Queues a repaint of the entire control on the screen
		/// </summary>
		/// <remarks>
		/// This is only useful when the control is visible.
		/// </remarks>
		/// <param name="invalidateChildren"><c>True</c> to invalidate all children, <c>false</c> to only invalidate the container</param>
		public void Invalidate(bool invalidateChildren)
		{
			Handler.Invalidate(invalidateChildren);
		}

		/// <summary>
		/// Queues a repaint of the specified <paramref name="rect"/> of the control and any children.
		/// </summary>
		/// <remarks>
		/// This is only useful when the control is visible.
		/// </remarks>
		/// <param name="rect">Rectangle to repaint</param>
		public void Invalidate(Rectangle rect)
		{
			Handler.Invalidate(rect, true);
		}

		/// <summary>
		/// Queues a repaint of the specified <paramref name="rect"/> of the control
		/// </summary>
		/// <remarks>
		/// This is only useful when the control is visible.
		/// </remarks>
		/// <param name="rect">Rectangle to repaint</param>
		/// <param name="invalidateChildren"><c>True</c> to invalidate all children, <c>false</c> to only invalidate the container</param>
		public void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			Handler.Invalidate(rect, invalidateChildren);
		}

		/// <summary>
		/// Gets or sets the size of the control. Use -1 to specify auto sizing for either the width and/or height.
		/// </summary>
		/// <remarks>
		/// Setting the size of controls is entirely optional as most controls will size themselves appropriately.
		/// When specifying a size, it will be used as the desired size of the control.  The container will reposition
		/// and resize the control depending on the available size.
		/// 
		/// For a <see cref="Window"/>, it is preferred to set the <see cref="Container.ClientSize"/> instead, as various
		/// platforms have different sizes of window decorations, toolbars, etc.
		/// </remarks>
		/// <value>The current size of the control</value>
		public virtual Size Size
		{
			get { return Handler.Size; }
			set { Handler.Size = value; }
		}

		/// <summary>
		/// Gets or sets the width of the control size.
		/// </summary>
		public virtual int Width
		{
			get => Handler.Width;
			set => Handler.Width = value;
		}

		/// <summary>
		/// Gets or sets the height of the control size.
		/// </summary>
		public virtual int Height
		{
			get => Handler.Height;
			set => Handler.Height = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Control"/> (or its children) are enabled and accept user input.
		/// </summary>
		/// <remarks>
		/// Typically when a control is disabled, the user cannot do anything with the control or any of its children.
		/// Including for example, selecting text in a text control.
		/// Certain controls can have a 'Read Only' mode, such as <see cref="TextBox.ReadOnly"/> which allow the user to 
		/// select text, but not change its contents.
		/// </remarks>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		[sc.DefaultValue(true)]
		public virtual bool Enabled
		{
			get => Handler.Enabled;
			set => Handler.Enabled = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Control"/> is visible to the user.
		/// </summary>
		/// <remarks>
		/// When the visibility of a control is set to false, it will still occupy space in the layout, but not be shown.
		/// The only exception is for controls like the <see cref="Splitter"/>, which will hide a pane if the visibility
		/// of one of the panels is changed.
		/// </remarks>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		[sc.DefaultValue(true)]
		public virtual bool Visible
		{
			get { return Handler.Visible; }
			set { Handler.Visible = value; }
		}

		/// <summary>
		/// Gets the container which this control has been added to, if any
		/// </summary>
		/// <value>The parent control, or null if there is no parent</value>
		public new Container Parent
		{
			get { return base.Parent as Container; }
		}

		/// <summary>
		/// Gets or sets the logical parent, which excludes any visual structure of custom containers.
		/// </summary>
		/// <value>The logical parent.</value>
		internal Container InternalLogicalParent
		{
			get { return base.Parent as Container; }
			set { base.Parent = value; }
		}

		static readonly object VisualParent_Key = new object();

		/// <summary>
		/// Gets the visual container of this control, if any.
		/// </summary>
		/// <remarks>
		/// Some containers may use other Eto controls to layout its children, such as the <see cref="StackLayout"/>.
		/// This will return the parent control that visually contains this control as opposed to <see cref="Parent"/>
		/// which will return the logical parent.
		/// </remarks>
		/// <value>The visual parent of this control.</value>
		public Container VisualParent
		{
			get { return Properties.Get<Container>(VisualParent_Key); }
			internal set
			{
				var old = VisualParent;
				Properties.Set(VisualParent_Key, value);
				Handler.SetParent(old, value);
			}
		}

		/// <summary>
		/// Finds a control in the parent hierarchy with the specified type and <see cref="Widget.ID"/> if specified
		/// </summary>
		/// <returns>The parent if found, or null if not found.</returns>
		/// <param name="type">The type of control to find.</param>
		/// <param name="id">Identifier of the parent control to find, or null to find by type only.</param>
		public new Container FindParent(Type type, string id = null)
		{
			var control = Parent;
			while (control != null)
			{
				if ((type == null || type.IsInstanceOfType(control)) && (string.IsNullOrEmpty(id) || control.ID == id))
				{
					return control;
				}
				control = control.Parent;
			}
			return null;
		}

		/// <summary>
		/// Finds a control in the parent hierarchy with the specified <paramref name="id"/>
		/// </summary>
		/// <returns>The parent if found, or null if not found.</returns>
		/// <param name="id">Identifier of the parent control to find.</param>
		public new Container FindParent(string id)
		{
			return FindParent(null, id);
		}

		/// <summary>
		/// Detaches the control by removing it from its parent
		/// </summary>
		/// <remarks>
		/// This is essentially a shortcut to myControl.Parent.Remove(myControl);
		/// </remarks>
		public void Detach()
		{
			if (VisualParent != null)
				VisualParent.Remove(this);
		}

		static readonly object IsAttached_Key = new object();

		/// <summary>
		/// Gets or sets a value indicating this control has been attached to a native container
		/// </summary>
		/// <seealso cref="AttachNative"/>
		bool IsAttached
		{
			get => Properties.Get<bool>(IsAttached_Key);
			set => Properties.Set(IsAttached_Key, value);
		}

		/// <summary>
		/// Attaches the control for direct use in a native application
		/// </summary>
		/// <remarks>
		/// Use this to use a control directly in a native application.  Note that the native application must be running
		/// the same framework as the current platform.  E.g. a WinForms application can use an Eto.Forms control
		/// when using the Eto.WinForms platform.
		/// 
		/// This prepares the control by firing the <see cref="PreLoad"/>, <see cref="Load"/>, etc. events.
		/// </remarks>
		public void AttachNative()
		{
			if (VisualParent != null)
				throw new InvalidOperationException("You can only attach a parentless control");
			
			if (IsAttached)
				return;

			IsAttached = true;
			using (Platform.Context)
			{
				OnPreLoad(EventArgs.Empty);
				OnLoad(EventArgs.Empty);
				Application.Instance.AsyncInvoke(PostAttach);
			}
		}

		void PostAttach()
		{
			// if the control is disposed before we get here Handler will be null, so omit calling OnLoadComplete
			if (!IsDisposed && Handler != null)
				OnLoadComplete(EventArgs.Empty);
		}

		/// <summary>
		/// Detaches the control when it is used in a native application, when you want to reuse the control.
		/// </summary>
		/// <remarks>
		/// This should only be called after <see cref="AttachNative"/> has been called, which is usually done by calling
		/// to <c>ToNative(true)</c>.
		/// </remarks>
		public void DetachNative()
		{
			if (!IsAttached)
				return;

			IsAttached = false;
			using (Platform.Context)
			{
				OnUnLoad(EventArgs.Empty);
			}
		}

		internal void TriggerPreLoad(EventArgs e)
		{
			using (Platform.Context)
				OnPreLoad(e);
		}

		internal void TriggerLoad(EventArgs e)
		{
			using (Platform.Context)
				OnLoad(e);
		}

		internal void TriggerLoadComplete(EventArgs e)
		{
			using (Platform.Context)
				OnLoadComplete(e);
		}

		internal void TriggerUnLoad(EventArgs e)
		{
			using (Platform.Context)
				OnUnLoad(e);
		}

		internal void TriggerStyleChanged(EventArgs e)
		{
			using (Platform.Context)
				OnStyleChanged(e);
		}

		/// <summary>
		/// Gets or sets the color for the background of the control
		/// </summary>
		/// <remarks>
		/// Note that on some platforms (e.g. Mac), setting the background color of a control can change the performance
		/// characteristics of the control and its children, since it must enable layers to do so.
		/// </remarks>
		/// <value>The color of the background.</value>
		public Color BackgroundColor
		{
			get { return Handler.BackgroundColor; }
			set { Handler.BackgroundColor = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance has the keyboard input focus.
		/// </summary>
		/// <value><c>true</c> if this instance has focus; otherwise, <c>false</c>.</value>
		public virtual bool HasFocus
		{
			get { return Handler.HasFocus; }
		}

		/// <summary>
		/// Attempts to set the keyboard input focus to this control, or the first child that accepts focus
		/// </summary>
		public virtual void Focus()
		{
			Handler.Focus();
		}


		static readonly object SuspendCount_Key = new object();

		int SuspendCount
		{
			get { return Properties.Get<int>(SuspendCount_Key); }
			set { Properties.Set(SuspendCount_Key, value); }
		}

		/// <summary>
		/// Gets a value indicating whether the layout of child controls is suspended.
		/// </summary>
		/// <seealso cref="SuspendLayout"/>
		/// <seealso cref="ResumeLayout"/>
		/// <value><c>true</c> if this instance is suspended; otherwise, <c>false</c>.</value>
		public bool IsSuspended { get { return SuspendCount > 0; } }

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
			SuspendCount++;
			Handler.SuspendLayout();
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
			var count = SuspendCount;
			if (count == 0)
				throw new InvalidOperationException("Control is not suspended. You must balance calls to Resume() with Suspend()");
			SuspendCount = --count;

			Handler.ResumeLayout();
		}

		/// <summary>
		/// Gets the window this control is contained in
		/// </summary>
		/// <value>The parent window, or null if it is not currently on a window</value>
		public Window ParentWindow
		{
			get
			{
				Control c = this;
				while (c != null)
				{
					var window = c as Window;
					if (window != null)
						return window;
					c = c.VisualParent;
				}
				return Handler.GetNativeParentWindow();
			}
		}

		/// <summary>
		/// Gets the supported platform commands that can be used to hook up system functions to user defined logic
		/// </summary>
		/// <remarks>
		/// This lists all available commands that can be mapped using the <see cref="MapPlatformCommand"/> method
		/// of the control.
		/// </remarks>
		/// <value>The supported platform commands.</value>
		/// <seealso cref="MapPlatformCommand"/>
		public IEnumerable<string> SupportedPlatformCommands
		{
			get { return Handler.SupportedPlatformCommands; }
		}

		/// <summary>
		/// Specifies a command to execute for a platform-specific command
		/// </summary>
		/// <remarks>
		/// Some platforms have specific system-defined commands that can be associated with a control.
		/// For example, the Mac platform's cut/copy/paste functionality is defined by the system, and if you want to
		/// hook into it, you can use this to map it to your own defined logic.
		/// The valid values of the <paramref name="systemCommand"/> parameter are defined by each platform, and a list can be
		/// retrieved using <see cref="Control.SupportedPlatformCommands"/>
		/// </remarks>
		/// <example>
		/// This example shows how to extend a control with cut/copy/paste for the mac platform:
		/// <code>
		/// var drawable = new Drawable();
		/// if (drawable.Generator.IsMac)
		/// {
		/// 	drawable.MapPlatformCommand("cut", new MyCutCommand());
		/// 	drawable.MapPlatformCommand("copy", new MyCopyCommand());
		/// 	drawable.MapPlatformCommand("paste", new MyPasteCommand());
		/// }
		/// </code>
		/// </example>
		/// <param name="systemCommand">System command</param>
		/// <param name="command">Command to execute, or null to restore to the default behavior</param>
		/// <seealso cref="SupportedPlatformCommands"/>
		public void MapPlatformCommand(string systemCommand, Command command)
		{
			Handler.MapPlatformCommand(systemCommand, command);
		}

		/// <summary>
		/// Converts a point from screen space to control space.
		/// </summary>
		/// <returns>The point in control space</returns>
		/// <param name="point">Point in screen space</param>
		public PointF PointFromScreen(PointF point)
		{
			return Handler.PointFromScreen(point);
		}

		/// <summary>
		/// Converts a point from control space to screen space
		/// </summary>
		/// <returns>The point in screen space</returns>
		/// <param name="point">Point in control space</param>
		public PointF PointToScreen(PointF point)
		{
			return Handler.PointToScreen(point);
		}

		/// <summary>
		/// Converts a rectangle from screen space to control space.
		/// </summary>
		/// <returns>The rectangle in control space</returns>
		/// <param name="rect">Rectangle in screen space</param>
		public RectangleF RectangleToScreen(RectangleF rect)
		{
			return new RectangleF(PointToScreen(rect.Location), PointToScreen(rect.EndLocation));
		}

		/// <summary>
		/// Converts a rectangle from control space to screen space
		/// </summary>
		/// <returns>The rectangle in screen space</returns>
		/// <param name="rect">Rectangle in control space</param>
		public RectangleF RectangleFromScreen(RectangleF rect)
		{
			return new RectangleF(PointFromScreen(rect.Location), PointFromScreen(rect.EndLocation));
		}

		/// <summary>
		/// Gets the bounding rectangle of this control relative to its container
		/// </summary>
		/// <value>The bounding rectangle of the control</value>
		public Rectangle Bounds
		{
			get { return new Rectangle(Location, Size); }
		}

		/// <summary>
		/// Gets the location of the control as positioned by the container
		/// </summary>
		/// <remarks>
		/// A control's location is set by the container.
		/// This can be used to determine where the control is for overlaying floating windows, menus, etc.
		/// </remarks>
		/// <value>The current location of the control</value>
		public Point Location
		{
			get { return Handler.Location; }
		}

		/// <summary>
		/// Gets or sets the type of cursor to use when the mouse is hovering over the control
		/// </summary>
		/// <value>The mouse cursor</value>
		public virtual Cursor Cursor
		{
			get { return Handler.Cursor; }
			set { Handler.Cursor = value; }
		}

		/// <summary>
		/// Gets or sets the tool tip to show when the mouse is hovered over the control
		/// </summary>
		/// <value>The tool tip.</value>
		public virtual string ToolTip
		{
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}

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
		[sc.DefaultValue(int.MaxValue)]
		public virtual int TabIndex
		{
			get { return Handler.TabIndex; }
			set { Handler.TabIndex = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this control can serve as drop target.
		/// </summary>
		public virtual bool AllowDrop
		{
			get { return Handler.AllowDrop; }
			set { Handler.AllowDrop = value; }
		}

		/// <summary>
		/// Starts drag operation using this control as drag source.
		/// </summary>
		/// <param name="data">Drag data.</param>
		/// <param name="allowedEffects">Allowed action.</param>
		public virtual void DoDragDrop(DataObject data, DragEffects allowedEffects)
		{
			Handler.DoDragDrop(data, allowedEffects, null, PointF.Empty);
		}

		/// <summary>
		/// Starts drag operation using this control as drag source.
		/// </summary>
		/// <param name="data">Drag data.</param>
		/// <param name="allowedEffects">Allowed effects.</param>
		/// <param name="image">Custom drag image</param>
		/// <param name="cursorOffset">Offset of the cursor to the drag image</param>
		public virtual void DoDragDrop(DataObject data, DragEffects allowedEffects, Image image, PointF cursorOffset)
		{
			Handler.DoDragDrop(data, allowedEffects, image, cursorOffset);
		}

		/// <summary>
		/// Handles when the <see cref="Style"/> is changed.
		/// </summary>
		/// <remarks>
		/// This applies the cascading styles to the control and any of its children.
		/// </remarks>
		protected override void OnStyleChanged(EventArgs e)
		{
			base.OnStyleChanged(e);

			// already loaded, re-apply styles as they have changed
			if (Loaded)
				OnApplyCascadingStyles();
		}

		/// <summary>
		/// Called when cascading styles should be applied to this control.
		/// </summary>
		/// <remarks>
		/// You don't typically have to call this directly, but override it to apply styles to any child item(s)
		/// that may need styling at the same time.
		/// 
		/// This is automatically done for any Container based control and its child controls.
		/// </remarks>
		protected virtual void OnApplyCascadingStyles() => ApplyStyles(this, Style);

		/// <summary>
		/// Applies the styles to the specified <paramref name="widget"/> up the parent chain.
		/// </summary>
		/// <remarks>
		/// This traverses up the parent chain to apply any cascading styles defined in parent container objects.
		/// 
		/// Call this method on any child widget of a control.
		/// </remarks>
		/// <param name="widget">Widget to style.</param>
		/// <param name="style">Style of the widget to apply.</param>
		protected virtual void ApplyStyles(object widget, string style) => Parent?.ApplyStyles(widget, Style);


		/// <summary>
		/// Handles the disposal of this control
		/// </summary>
		/// <param name="disposing">True if the caller called <see cref="Widget.Dispose()"/> manually, false if being called from a finalizer</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Unbind();
				Detach();
				DetachNative();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Converts a string to a label control implicitly.
		/// </summary>
		/// <remarks>
		/// This provides an easy way to add labels to your layout through code, without having to create <see cref="Label"/> instances.
		/// </remarks>
		/// <param name="labelText">Text to convert to a Label control.</param>
		public static implicit operator Control(string labelText)
		{
			return new Label { Text = labelText };
		}

		/// <summary>
		/// Converts an <see cref="Image"/> to a control implicitly.
		/// </summary>
		/// <remarks>
		/// This provides an easy way to add images to your layout through code, without having to create <see cref="ImageView"/> instances manually.
		/// </remarks>
		/// <param name="image">Image to convert to an ImageView control.</param>
		public static implicit operator Control(Image image)
		{
			return new ImageView { Image = image };
		}

		#region Callback

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for instances of <see cref="Control"/>
		/// </summary>
		public new interface ICallback : Widget.ICallback
		{
			/// <summary>
			/// Raises the key down event.
			/// </summary>
			void OnKeyDown(Control widget, KeyEventArgs e);
			/// <summary>
			/// Raises the key up event.
			/// </summary>
			void OnKeyUp(Control widget, KeyEventArgs e);
			/// <summary>
			/// Raises the mouse down event.
			/// </summary>
			void OnMouseDown(Control widget, MouseEventArgs e);
			/// <summary>
			/// Raises the mouse up event.
			/// </summary>
			void OnMouseUp(Control widget, MouseEventArgs e);
			/// <summary>
			/// Raises the mouse move event.
			/// </summary>
			void OnMouseMove(Control widget, MouseEventArgs e);
			/// <summary>
			/// Raises the mouse leave event.
			/// </summary>
			void OnMouseLeave(Control widget, MouseEventArgs e);
			/// <summary>
			/// Raises the mouse enter event.
			/// </summary>
			void OnMouseEnter(Control widget, MouseEventArgs e);
			/// <summary>
			/// Raises the text input event.
			/// </summary>
			void OnTextInput(Control widget, TextInputEventArgs e);
			/// <summary>
			/// Raises the size changed event.
			/// </summary>
			void OnSizeChanged(Control widget, EventArgs e);
			/// <summary>
			/// Raises the mouse double click event.
			/// </summary>
			void OnMouseDoubleClick(Control widget, MouseEventArgs e);
			/// <summary>
			/// Raises the mouse wheel event.
			/// </summary>
			void OnMouseWheel(Control widget, MouseEventArgs e);
			/// <summary>
			/// Raises the got focus event.
			/// </summary>
			void OnGotFocus(Control widget, EventArgs e);
			/// <summary>
			/// Raises the lost focus event.
			/// </summary>
			void OnLostFocus(Control widget, EventArgs e);
			/// <summary>
			/// Raises the shown event.
			/// </summary>
			void OnShown(Control widget, EventArgs e);
			/// <summary>
			/// Raises the DragDrop event.
			/// </summary>
			void OnDragDrop(Control widget, DragEventArgs e);
			/// <summary>
			/// Raises the DragOver event.
			/// </summary>
			void OnDragOver(Control widget, DragEventArgs e);
			/// <summary>
			/// Raises the DragEnter event.
			/// </summary>
			void OnDragEnter(Control widget, DragEventArgs e);
			/// <summary>
			/// Raises the DragLeave event.
			/// </summary>
			void OnDragLeave(Control widget, DragEventArgs e);
			/// <summary>
			/// Raises the EnabledChanged event.
			/// </summary>
			void OnEnabledChanged(Control widget, EventArgs e);
		}

		/// <summary>
		/// Callback methods for handlers of <see cref="Control"/>
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the key down event.
			/// </summary>
			public void OnKeyDown(Control widget, KeyEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnKeyDown(e);
			}
			/// <summary>
			/// Raises the key up event.
			/// </summary>
			public void OnKeyUp(Control widget, KeyEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnKeyUp(e);
			}
			/// <summary>
			/// Raises the mouse down event.
			/// </summary>
			public void OnMouseDown(Control widget, MouseEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnMouseDown(e);
			}
			/// <summary>
			/// Raises the mouse up event.
			/// </summary>
			public void OnMouseUp(Control widget, MouseEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnMouseUp(e);
			}
			/// <summary>
			/// Raises the mouse move event.
			/// </summary>
			public void OnMouseMove(Control widget, MouseEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnMouseMove(e);
			}
			/// <summary>
			/// Raises the mouse leave event.
			/// </summary>
			public void OnMouseLeave(Control widget, MouseEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnMouseLeave(e);
			}
			/// <summary>
			/// Raises the mouse enter event.
			/// </summary>
			public void OnMouseEnter(Control widget, MouseEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnMouseEnter(e);
			}
			/// <summary>
			/// Raises the text input event.
			/// </summary>
			public void OnTextInput(Control widget, TextInputEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnTextInput(e);
			}
			/// <summary>
			/// Raises the size changed event.
			/// </summary>
			public void OnSizeChanged(Control widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnSizeChanged(e);
			}
			/// <summary>
			/// Raises the mouse double click event.
			/// </summary>
			public void OnMouseDoubleClick(Control widget, MouseEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnMouseDoubleClick(e);
			}
			/// <summary>
			/// Raises the mouse wheel event.
			/// </summary>
			public void OnMouseWheel(Control widget, MouseEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnMouseWheel(e);
			}
			/// <summary>
			/// Raises the got focus event.
			/// </summary>
			public void OnGotFocus(Control widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnGotFocus(e);
			}
			/// <summary>
			/// Raises the lost focus event.
			/// </summary>
			public void OnLostFocus(Control widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnLostFocus(e);
			}
			/// <summary>
			/// Raises the shown event.
			/// </summary>
			public void OnShown(Control widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnShown(e);
			}

			/// <summary>
			/// Raises the DragDrop event.
			/// </summary>
			public void OnDragDrop(Control widget, DragEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnDragDrop(e);
			}

			/// <summary>
			/// Raises the DragOver event.
			/// </summary>
			public void OnDragOver(Control widget, DragEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnDragOver(e);
			}

			/// <summary>
			/// Raises the DragEnter event.
			/// </summary>
			public void OnDragEnter(Control widget, DragEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnDragEnter(e);
			}

			/// <summary>
			/// Raises the DragLeave event.
			/// </summary>
			public void OnDragLeave(Control widget, DragEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnDragLeave(e);
			}

			/// <summary>
			/// Raises the EnabledChanged event.
			/// </summary>
			public void OnEnabledChanged(Control widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnEnabledChanged(e);
			}
		}

		#endregion

		#region Handler
		/// <summary>
		/// Handler interface for <see cref="Control"/>
		/// </summary>
		/// <copyright>(c) 2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Gets or sets the color for the background of the control
			/// </summary>
			/// <remarks>
			/// Note that on some platforms (e.g. Mac), setting the background color of a control can change the performance
			/// characteristics of the control and its children, since it must enable layers to do so.
			/// </remarks>
			/// <value>The color of the background.</value>
			Color BackgroundColor { get; set; }

			/// <summary>
			/// Gets or sets the size of the control. Use -1 to specify auto sizing for either the width and/or height.
			/// </summary>
			/// <remarks>
			/// Setting the size of controls is entirely optional as most controls will size themselves appropriately.
			/// When specifying a size, it will be used as the desired size of the control.  The container will reposition
			/// and resize the control depending on the available size.
			/// 
			/// For a <see cref="Window"/>, it is preferred to set the <see cref="Container.ClientSize"/> instead, as various
			/// platforms have different sizes of window decorations, toolbars, etc.
			/// </remarks>
			/// <value>The current size of the control</value>
			Size Size { get; set; }

			/// <summary>
			/// Gets or sets the width of the control size.
			/// </summary>
			int Width { get; set; }

			/// <summary>
			/// Gets or sets the height of the control size.
			/// </summary>
			int Height { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Control"/> is enabled and accepts user input.
			/// </summary>
			/// <remarks>
			/// Typically when a control is disabled, the user cannot do anything with the control (including for example, selecting
			/// text in a text control).  Certain controls can have a 'Read Only' mode, such as <see cref="TextBox.ReadOnly"/> which
			/// allows the user to select text, but not change its contents.
			/// </remarks>
			/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
			bool Enabled { get; set; }

			/// <summary>
			/// Queues a repaint of the entire control on the screen
			/// </summary>
			/// <remarks>
			/// This is only useful when the control is visible.
			/// </remarks>
			/// <param name="invalidateChildren"><c>True</c> to invalidate all children, <c>false</c> to only invalidate the container</param>
			void Invalidate(bool invalidateChildren);

			/// <summary>
			/// Queues a repaint of the specified <paramref name="rect"/> of the control
			/// </summary>
			/// <remarks>
			/// This is only useful when the control is visible.
			/// </remarks>
			/// <param name="rect">Rectangle to repaint</param>
			/// <param name="invalidateChildren"><c>True</c> to invalidate all children, <c>false</c> to only invalidate the container</param>
			void Invalidate(Rectangle rect, bool invalidateChildren);

			/// <summary>
			/// Suspends the layout of child controls
			/// </summary>
			/// <remarks>
			/// This can be used to optimize some platforms while adding, removing, or changing many child controls at once.
			/// It disables the calculation of control positioning until <see cref="ResumeLayout"/> is called.
			/// Each call to SuspendLayout() must be balanced with a call to <see cref="ResumeLayout"/>.
			/// </remarks>
			void SuspendLayout();

			/// <summary>
			/// Resumes the layout after it has been suspended, and performs a layout
			/// </summary>
			/// <remarks>
			/// This can be used to optimize some platforms while adding, removing, or changing many child controls at once.
			/// Each call to ResumeLayout() must be balanced with a call to <see cref="SuspendLayout"/> before it.
			/// </remarks>
			void ResumeLayout();

			/// <summary>
			/// Attempts to set the keyboard input focus to this control, or the first child that accepts focus
			/// </summary>
			void Focus();

			/// <summary>
			/// Gets a value indicating whether this instance has the keyboard input focus.
			/// </summary>
			/// <value><c>true</c> if this instance has focus; otherwise, <c>false</c>.</value>
			bool HasFocus { get; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Control"/> is visible to the user.
			/// </summary>
			/// <remarks>
			/// When the visibility of a control is set to false, it will still occupy space in the layout, but not be shown.
			/// The only exception is for controls like the <see cref="Splitter"/>, which will hide a pane if the visibility
			/// of one of the panels is changed.
			/// </remarks>
			/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
			bool Visible { get; set; }

			/// <summary>
			/// Called before the control is loaded on a form
			/// </summary>
			/// <param name="e">Event arguments</param>
			/// <seealso cref="OnLoadComplete"/>
			/// <seealso cref="OnLoad"/>
			/// <seealso cref="OnUnLoad"/>
			void OnPreLoad(EventArgs e);

			/// <summary>
			/// Called when the control is loaded on a form
			/// </summary>
			/// <param name="e">Event arguments</param>
			/// <seealso cref="OnPreLoad"/>
			/// <seealso cref="OnLoadComplete"/>
			/// <seealso cref="OnUnLoad"/>
			void OnLoad(EventArgs e);

			/// <summary>
			/// Called after all other controls have been loaded
			/// </summary>
			/// <param name="e">Event arguments</param>
			/// <seealso cref="OnPreLoad"/>
			/// <seealso cref="OnLoad"/>
			/// <seealso cref="OnUnLoad"/>
			void OnLoadComplete(EventArgs e);

			/// <summary>
			/// Called when the control is unloaded, which is when it is not currently on a displayed window
			/// </summary>
			/// <param name="e">Event arguments</param>
			/// <seealso cref="OnPreLoad"/>
			/// <seealso cref="OnLoad"/>
			/// <seealso cref="OnLoadComplete"/>
			void OnUnLoad(EventArgs e);

			/// <summary>
			/// Called when the parent of the control has been set
			/// </summary>
			/// <param name="oldParent">Old parent for the control, or null if the control is added</param>
			/// <param name="newParent">New parent for the control, or null if the parent was removed</param>
			void SetParent(Container oldParent, Container newParent);

			/// <summary>
			/// Gets the supported platform commands that can be used to hook up system functions to user defined logic
			/// </summary>
			/// <remarks>
			/// This lists all available commands that can be mapped using the <see cref="MapPlatformCommand"/> method
			/// of the control.
			/// </remarks>
			/// <value>The supported platform commands.</value>
			/// <seealso cref="MapPlatformCommand"/>
			IEnumerable<string> SupportedPlatformCommands { get; }

			/// <summary>
			/// Specifies a command to execute for a platform-specific command
			/// </summary>
			/// <remarks>
			/// Some platforms have specific system-defined commands that can be associated with a control.
			/// For example, the Mac platform's cut/copy/paste functionality is defined by the system, and if you want to
			/// hook into it, you can use this to map it to your own defined logic.
			/// The valid values of the <paramref name="systemCommand"/> parameter are defined by each platform, and a list can be
			/// retrieved using <see cref="Control.SupportedPlatformCommands"/>
			/// </remarks>
			/// <example>
			/// This example shows how to extend a control with cut/copy/paste for the mac platform:
			/// <code>
			/// var drawable = new Drawable();
			/// if (drawable.Generator.IsMac)
			/// {
			/// 	drawable.MapPlatformCommand("cut", new MyCutCommand());
			/// 	drawable.MapPlatformCommand("copy", new MyCopyCommand());
			/// 	drawable.MapPlatformCommand("paste", new MyPasteCommand());
			/// }
			/// </code>
			/// </example>
			/// <param name="systemCommand">System command.</param>
			/// <param name="command">Command to execute, or null to restore to the default behavior</param>
			/// <seealso cref="SupportedPlatformCommands"/>
			void MapPlatformCommand(string systemCommand, Command command);

			/// <summary>
			/// Converts a point from screen space to control space.
			/// </summary>
			/// <returns>The point in control space</returns>
			/// <param name="point">Point in screen space</param>
			PointF PointFromScreen(PointF point);

			/// <summary>
			/// Converts a point from control space to screen space
			/// </summary>
			/// <returns>The point in screen space</returns>
			/// <param name="point">Point in control space</param>
			PointF PointToScreen(PointF point);

			/// <summary>
			/// Gets the location of the control as positioned by the container
			/// </summary>
			/// <remarks>
			/// A control's location is set by the container.
			/// This can be used to determine where the control is for overlaying floating windows, menus, etc.
			/// </remarks>
			/// <value>The current location of the control</value>
			Point Location { get; }

			/// <summary>
			/// Gets or sets the tool tip to show when the mouse is hovered over the control
			/// </summary>
			/// <value>The tool tip.</value>
			string ToolTip { get; set; }

			/// <summary>
			/// Gets or sets the type of cursor to use when the mouse is hovering over the control
			/// </summary>
			/// <value>The mouse cursor</value>
			Cursor Cursor { get; set; }

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
			int TabIndex { get; set; }

			/// <summary>
			/// Gets an enumeration of controls that are in the visual tree.
			/// </summary>
			/// <remarks>
			/// This is used to specify which controls are contained by this instance that are part of the visual tree.
			/// This should include all controls including non-logical Eto controls used for layout. 
			/// </remarks>
			/// <value>The visual controls.</value>
			IEnumerable<Control> VisualControls { get; }

			/// <summary>
			/// Gets or sets a value indicating whether this control can serve as drop target.
			/// </summary>
			bool AllowDrop { get; set; }

			/// <summary>
			/// Starts drag operation using this control as drag source.
			/// </summary>
			/// <param name="data">Drag data.</param>
			/// <param name="allowedEffects">Allowed effects.</param>
			/// <param name="image">Custom drag image</param>
			/// <param name="cursorOffset">Offset of the cursor to the drag image</param>
			void DoDragDrop(DataObject data, DragEffects allowedEffects, Image image, PointF cursorOffset);

			/// <summary>
			/// Gets a parent window wrapper around the native window
			/// </summary>
			/// <returns>The parent window.</returns>
			Window GetNativeParentWindow();
		}
		#endregion
	}
}
