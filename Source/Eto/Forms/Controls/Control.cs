using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Base for all visual UI elements
	/// </summary>
	/// <remarks>
	/// All visual user interface elements should inherit from this class to provide common functionality like binding,
	/// load/unload, and common events.
	/// </remarks>
	#if !PCL
	[ToolboxItem(true)]
	[DesignTimeVisible(true)]
	[DesignerCategory("Eto.Forms")]
	#endif
	public partial class Control : Widget, IMouseInputSource, IKeyboardInputSource, ICallbackSource
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets a value indicating that the control is loaded onto a form, that is it has been created, added to a parent, and shown
		/// </summary>
		/// <remarks>
		/// The <see cref="OnLoad"/> method sets this value to <c>true</c> after cascading to all children (for a <see cref="Container"/>)
		/// and calling the platform handler's implementation.  It is called after adding to a loaded form, or when showing a new form.
		/// 
		/// The <see cref="OnUnLoad"/> method will set this value to <c>false</c> when the control is removed from its parent
		/// </remarks>
		public bool Loaded { get; private set; }

		/// <summary>
		/// Gets the collection of bindings that are attached to this widget
		/// </summary>
		public BindingCollection Bindings
		{
			get { return Properties.Create<BindingCollection>(BindingsKey); }
		}

		static readonly object BindingsKey = new object();

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
		/// Event to handle when the <see cref="Control.DataContext"/> has changed
		/// </summary>
		/// <remarks>
		/// This may be fired in the event of a parent in the hierarchy setting the data context.
		/// For example, the <see cref="Forms.Container"/> widget fires this event when it's event is fired.
		/// </remarks>
		public event EventHandler<EventArgs> DataContextChanged
		{
			add { Properties.AddEvent(DataContextChangedKey, value); }
			remove { Properties.RemoveEvent(DataContextChangedKey, value); }
		}

		static readonly object DataContextChangedKey = new object();

		/// <summary>
		/// Raises the <see cref="DataContextChanged"/> event
		/// </summary>
		/// <remarks>
		/// Implementors may override this to fire this event on child widgets in a heirarchy. 
		/// This allows a control to be bound to its own <see cref="DataContext"/>, which would be set
		/// on one of the parent control(s).
		/// </remarks>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDataContextChanged(EventArgs e)
		{
			Properties.TriggerEvent(DataContextChangedKey, this, e);
		}

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
		/// Queues a repaint of the entire control on the screen
		/// </summary>
		/// <remarks>
		/// This is only useful when the control is visible.
		/// </remarks>
		public void Invalidate()
		{
			Handler.Invalidate();
		}

		/// <summary>
		/// Queues a repaint of the specified <paramref name="rect"/> of the control
		/// </summary>
		/// <remarks>
		/// This is only useful when the control is visible.
		/// </remarks>
		/// <param name="rect">Rectangle to repaint</param>
		public void Invalidate(Rectangle rect)
		{
			Handler.Invalidate(rect);
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
			get { return Handler.Size.Width; }
			set { Size = new Size(value, Size.Height); }
		}

		/// <summary>
		/// Gets or sets the height of the control size.
		/// </summary>
		public virtual int Height
		{
			get { return Handler.Size.Height; }
			set { Size = new Size(Size.Width, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Control"/> is enabled and accepts user input.
		/// </summary>
		/// <remarks>
		/// Typically when a control is disabled, the user cannot do anything with the control (including for example, selecting
		/// text in a text control).  Certain controls can have a 'Read Only' mode, such as <see cref="TextBox.ReadOnly"/> which
		/// allows the user to select text, but not change its contents.
		/// </remarks>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public virtual bool Enabled
		{
			get { return Handler.Enabled; }
			set { Handler.Enabled = value; }
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
		[DefaultValue(true)]
		public virtual bool Visible
		{
			get { return Handler.Visible; }
			set { Handler.Visible = value; }
		}

		/// <summary>
		/// Gets or sets the data context for this widget for binding
		/// </summary>
		/// <remarks>
		/// Subclasses may override the standard behaviour so that hierarchy of widgets can be taken into account.
		/// 
		/// For example, a Control may return the data context of a parent, if it is not set explicitly.
		/// </remarks>
		public virtual object DataContext
		{
			get { return Properties.Get<object>(DataContextKey) ?? (Parent == null ? null : Parent.DataContext); }
			set
			{
				if (!ReferenceEquals(value, Properties.Get<object>(DataContextKey)))
				{
					Properties[DataContextKey] = value;
					OnDataContextChanged(EventArgs.Empty);
				}
			}
		}

		static readonly object DataContextKey = new object();
		Container parent;

		/// <summary>
		/// Gets the container which this control has been added to, if any
		/// </summary>
		/// <value>The parent control, or null if there is no parent</value>
		public Container Parent
		{
			get { return parent; }
			internal set
			{
				parent = value;
				Handler.SetParent(value);
			}
		}

		/// <summary>
		/// Finds a control in the parent hierarchy with the specified type and <see cref="Widget.ID"/> if specified
		/// </summary>
		/// <returns>The parent if found, or null if not found</returns>
		/// <param name="id">Identifier of the parent control to find, or null to ignore</param>
		/// <typeparam name="T">The type of control to find</typeparam>
		public T FindParent<T>(string id = null)
			where T : Container
		{
			var control = Parent;
			while (control != null)
			{
				var ctl = control as T;
				if (ctl != null && (string.IsNullOrEmpty(id) || control.ID == id))
				{
					return ctl;
				}
				control = control.Parent;
			}
			return default(T);
		}

		/// <summary>
		/// Finds a control in the parent hierarchy with the specified type and <see cref="Widget.ID"/> if specified
		/// </summary>
		/// <returns>The parent if found, or null if not found.</returns>
		/// <param name="type">The type of control to find.</param>
		/// <param name="id">Identifier of the parent control to find, or null to find by type only.</param>
		public Container FindParent(Type type, string id = null)
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
		public Container FindParent(string id)
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
			if (Parent != null)
				Parent.Remove(this);
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
			if (Parent != null)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "You can only attach a parentless control"));

			using (Platform.Context)
			{
				OnPreLoad(EventArgs.Empty);
				OnLoad(EventArgs.Empty);
				OnDataContextChanged(EventArgs.Empty);
				OnLoadComplete(EventArgs.Empty);
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

		internal void TriggerDataContextChanged(EventArgs e)
		{
			using (Platform.Context)
				OnDataContextChanged(e);
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
					c = c.Parent;
				}
				return null;
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
		/// Unbinds any bindings in the <see cref="Bindings"/> collection and removes the bindings
		/// </summary>
		public virtual void Unbind()
		{
			var bindings = Properties.Get<BindingCollection>(BindingsKey);
			if (bindings != null)
			{
				bindings.Unbind();
				Properties[BindingsKey] = null;
			}
		}

		/// <summary>
		/// Updates all bindings in this widget
		/// </summary>
		public virtual void UpdateBindings(BindingUpdateMode mode = BindingUpdateMode.Source)
		{
			var bindings = Properties.Get<BindingCollection>(BindingsKey);
			if (bindings != null)
			{
				bindings.Update(mode);
			}
		}

		/// <summary>
		/// Handles the disposal of this control
		/// </summary>
		/// <param name="disposing">True if the caller called <see cref="Widget.Dispose()"/> manually, false if being called from a finalizer</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Unbind();
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
				widget.Platform.Invoke(() => widget.OnKeyDown(e));
			}
			/// <summary>
			/// Raises the key up event.
			/// </summary>
			public void OnKeyUp(Control widget, KeyEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnKeyUp(e));
			}
			/// <summary>
			/// Raises the mouse down event.
			/// </summary>
			public void OnMouseDown(Control widget, MouseEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnMouseDown(e));
			}
			/// <summary>
			/// Raises the mouse up event.
			/// </summary>
			public void OnMouseUp(Control widget, MouseEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnMouseUp(e));
			}
			/// <summary>
			/// Raises the mouse move event.
			/// </summary>
			public void OnMouseMove(Control widget, MouseEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnMouseMove(e));
			}
			/// <summary>
			/// Raises the mouse leave event.
			/// </summary>
			public void OnMouseLeave(Control widget, MouseEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnMouseLeave(e));
			}
			/// <summary>
			/// Raises the mouse enter event.
			/// </summary>
			public void OnMouseEnter(Control widget, MouseEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnMouseEnter(e));
			}
			/// <summary>
			/// Raises the text input event.
			/// </summary>
			public void OnTextInput(Control widget, TextInputEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnTextInput(e));
			}
			/// <summary>
			/// Raises the size changed event.
			/// </summary>
			public void OnSizeChanged(Control widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnSizeChanged(e));
			}
			/// <summary>
			/// Raises the mouse double click event.
			/// </summary>
			public void OnMouseDoubleClick(Control widget, MouseEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnMouseDoubleClick(e));
			}
			/// <summary>
			/// Raises the mouse wheel event.
			/// </summary>
			public void OnMouseWheel(Control widget, MouseEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnMouseWheel(e));
			}
			/// <summary>
			/// Raises the got focus event.
			/// </summary>
			public void OnGotFocus(Control widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnGotFocus(e));
			}
			/// <summary>
			/// Raises the lost focus event.
			/// </summary>
			public void OnLostFocus(Control widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnLostFocus(e));
			}
			/// <summary>
			/// Raises the shown event.
			/// </summary>
			public void OnShown(Control widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnShown(e));
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
			void Invalidate();

			/// <summary>
			/// Queues a repaint of the specified <paramref name="rect"/> of the control
			/// </summary>
			/// <remarks>
			/// This is only useful when the control is visible.
			/// </remarks>
			/// <param name="rect">Rectangle to repaint</param>
			void Invalidate(Rectangle rect);

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
			/// <param name="parent">New parent for the control, or null if the parent was removed</param>
			void SetParent(Container parent);

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
		}
		#endregion
	}
}
