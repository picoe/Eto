using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IControl : IInstanceWidget
	{
		Color BackgroundColor { get; set; }

		Size Size { get; set; }

		bool Enabled { get; set; }

		void Invalidate();

		void Invalidate(Rectangle rect);

		void SuspendLayout();

		void ResumeLayout();

		void Focus();

		bool HasFocus { get; }

		bool Visible { get; set; }

		void OnPreLoad(EventArgs e);

		void OnLoad(EventArgs e);

		void OnLoadComplete(EventArgs e);

		void OnUnLoad(EventArgs e);

		void SetParent(Container parent);

		void MapPlatformAction(string systemAction, Command action);

		PointF PointFromScreen(PointF point);

		PointF PointToScreen(PointF point);

		Point Location { get; }
	}

	/// <summary>
	/// Base for all visual UI elements
	/// </summary>
	/// <remarks>
	/// All visual user interface elements should inherit from this class to provide common functionality like binding,
	/// load/unload, and common events.
	/// </remarks>
	[ToolboxItem(true)]
	[DesignTimeVisible(true)]
	[DesignerCategory("Eto.Forms")]
	public abstract partial class Control : InstanceWidget, IMouseInputSource, IKeyboardInputSource
	{
		new IControl Handler { get { return (IControl)base.Handler; } }

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

		public const string SizeChangedEvent = "Control.SizeChanged";

		public event EventHandler<EventArgs> SizeChanged
		{
			add { Properties.AddHandlerEvent(SizeChangedEvent, value); }
			remove { Properties.RemoveEvent(SizeChangedEvent, value); }
		}

		public virtual void OnSizeChanged(EventArgs e)
		{
			Properties.TriggerEvent(SizeChangedEvent, this, e);
		}

		public const string KeyDownEvent = "Control.KeyDown";

		public event EventHandler<KeyEventArgs> KeyDown
		{
			add { Properties.AddHandlerEvent(KeyDownEvent, value); }
			remove { Properties.RemoveEvent(KeyDownEvent, value); }
		}

		public virtual void OnKeyDown(KeyEventArgs e)
		{
			Properties.TriggerEvent(KeyDownEvent, this, e);
		}

		public const string KeyUpEvent = "Control.KeyUp";

		public event EventHandler<KeyEventArgs> KeyUp
		{
			add { Properties.AddHandlerEvent(KeyUpEvent, value); }
			remove { Properties.RemoveEvent(KeyUpEvent, value); }
		}

		public virtual void OnKeyUp(KeyEventArgs e)
		{
			Properties.TriggerEvent(KeyUpEvent, this, e);
		}

		public const string TextInputEvent = "Control.TextInput";

		public event EventHandler<TextInputEventArgs> TextInput
		{
			add { Properties.AddHandlerEvent(TextInputEvent, value); }
			remove { Properties.RemoveEvent(TextInputEvent, value); }
		}

		public virtual void OnTextInput(TextInputEventArgs e)
		{
			Properties.TriggerEvent(TextInputEvent, this, e);
		}

		public const string MouseDownEvent = "Control.MouseDown";

		public event EventHandler<MouseEventArgs> MouseDown
		{
			add { Properties.AddHandlerEvent(MouseDownEvent, value); }
			remove { Properties.RemoveEvent(MouseDownEvent, value); }
		}

		public virtual void OnMouseDown(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseDownEvent, this, e);
		}

		public const string MouseUpEvent = "Control.MouseUp";

		public event EventHandler<MouseEventArgs> MouseUp
		{
			add { Properties.AddHandlerEvent(MouseUpEvent, value); }
			remove { Properties.RemoveEvent(MouseUpEvent, value); }
		}

		public virtual void OnMouseUp(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseUpEvent, this, e);
		}

		public const string MouseMoveEvent = "Control.MouseMove";

		public event EventHandler<MouseEventArgs> MouseMove
		{
			add { Properties.AddHandlerEvent(MouseMoveEvent, value); }
			remove { Properties.RemoveEvent(MouseMoveEvent, value); }
		}

		public virtual void OnMouseMove(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseMoveEvent, this, e);
		}

		public const string MouseLeaveEvent = "Control.MouseLeave";

		public event EventHandler<MouseEventArgs> MouseLeave
		{
			add { Properties.AddHandlerEvent(MouseLeaveEvent, value); }
			remove { Properties.RemoveEvent(MouseLeaveEvent, value); }
		}

		public virtual void OnMouseLeave(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseLeaveEvent, this, e);
		}

		public const string MouseEnterEvent = "Control.MouseEnter";

		public event EventHandler<MouseEventArgs> MouseEnter
		{
			add { Properties.AddHandlerEvent(MouseEnterEvent, value); }
			remove { Properties.RemoveEvent(MouseEnterEvent, value); }
		}

		public virtual void OnMouseEnter(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseEnterEvent, this, e);
		}

		public const string MouseDoubleClickEvent = "Control.MouseDoubleClick";

		public event EventHandler<MouseEventArgs> MouseDoubleClick
		{
			add { Properties.AddHandlerEvent(MouseDoubleClickEvent, value); }
			remove { Properties.RemoveEvent(MouseDoubleClickEvent, value); }
		}

		public virtual void OnMouseDoubleClick(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseDoubleClickEvent, this, e);
		}

		public const string MouseWheelEvent = "Control.MouseWheel";

		public event EventHandler<MouseEventArgs> MouseWheel
		{
			add { Properties.AddHandlerEvent(MouseWheelEvent, value); }
			remove { Properties.RemoveEvent(MouseWheelEvent, value); }
		}

		public virtual void OnMouseWheel(MouseEventArgs e)
		{
			Properties.TriggerEvent(MouseWheelEvent, this, e);
		}

		public const string GotFocusEvent = "Control.GotFocus";

		public event EventHandler<EventArgs> GotFocus
		{
			add { Properties.AddHandlerEvent(GotFocusEvent, value); }
			remove { Properties.RemoveEvent(GotFocusEvent, value); }
		}

		public virtual void OnGotFocus(EventArgs e)
		{
			Properties.TriggerEvent(GotFocusEvent, this, e);
		}

		public const string LostFocusEvent = "Control.LostFocus";

		public event EventHandler<EventArgs> LostFocus
		{
			add { Properties.AddHandlerEvent(LostFocusEvent, value); }
			remove { Properties.RemoveEvent(LostFocusEvent, value); }
		}

		public virtual void OnLostFocus(EventArgs e)
		{
			Properties.TriggerEvent(LostFocusEvent, this, e);
		}

		public const string ShownEvent = "Control.Shown";

		public event EventHandler<EventArgs> Shown
		{
			add { Properties.AddHandlerEvent(ShownEvent, value); }
			remove { Properties.RemoveEvent(ShownEvent, value); }
		}

		public virtual void OnShown(EventArgs e)
		{
			Properties.TriggerEvent(ShownEvent, this, e);
		}

		static readonly object PreLoadKey = new object();

		public event EventHandler<EventArgs> PreLoad
		{
			add { Properties.AddEvent(PreLoadKey, value); }
			remove { Properties.RemoveEvent(PreLoadKey, value); }
		}

		public virtual void OnPreLoad(EventArgs e)
		{
			Properties.TriggerEvent(PreLoadKey, this, e);
			Handler.OnPreLoad(e);
		}

		static readonly object LoadKey = new object();

		public event EventHandler<EventArgs> Load
		{
			add { Properties.AddEvent(LoadKey, value); }
			remove { Properties.RemoveEvent(LoadKey, value); }
		}

		public virtual void OnLoad(EventArgs e)
		{
#if DEBUG
			if (Loaded)
				throw new EtoException("Control was loaded more than once");
#endif
			Properties.TriggerEvent(LoadKey, this, e);
			Handler.OnLoad(e);
			Loaded = true;
		}

		static readonly object LoadCompleteKey = new object();

		public event EventHandler<EventArgs> LoadComplete
		{
			add { Properties.AddEvent(LoadCompleteKey, value); }
			remove { Properties.RemoveEvent(LoadCompleteKey, value); }
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
			Properties.TriggerEvent(LoadCompleteKey, this, e);
			Handler.OnLoadComplete(e);
		}

		static readonly object UnLoadKey = new object();

		public event EventHandler<EventArgs> UnLoad
		{
			add { Properties.AddEvent(UnLoadKey, value); }
			remove { Properties.RemoveEvent(UnLoadKey, value); }
		}

		public virtual void OnUnLoad(EventArgs e)
		{
#if DEBUG
			if (!Loaded)
				throw new EtoException("Control was unloaded more than once");
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
		/// Called to fire the <see cref="DataContextChanged"/> event
		/// </summary>
		/// <remarks>
		/// Implementors may override this to fire this event on child widgets in a heirarchy. 
		/// This allows a control to be bound to its own <see cref="DataContext"/>, which would be set
		/// on one of the parent control(s).
		/// </remarks>
		/// <param name="e">Event arguments</param>
		protected internal virtual void OnDataContextChanged(EventArgs e)
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

		protected Control(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		protected Control(Generator generator, IControl handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
		}

		public void Invalidate()
		{
			Handler.Invalidate();
		}

		public void Invalidate(Rectangle rect)
		{
			Handler.Invalidate(rect);
		}

		public virtual Size Size
		{
			get { return Handler.Size; }
			set { Handler.Size = value; }
		}

		public virtual bool Enabled
		{
			get { return Handler.Enabled; }
			set { Handler.Enabled = value; }
		}

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
				Properties[DataContextKey] = value;
				OnDataContextChanged(EventArgs.Empty);
			}
		}

		static readonly object DataContextKey = new object();

		[Obsolete("Use Parent instead")]
		public Container ParentLayout { get { return Parent; } }

		Container parent;
		public Container Parent
		{
			get { return parent; }
			internal set
			{
				parent = value;
				Handler.SetParent(value);
			}
		}

		public T FindParent<T>(string id)
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

		public T FindParent<T>()
			where T : Container
		{
			var control = Parent;
			while (control != null)
			{
				var ctl = control as T;
				if (ctl != null)
					return ctl;
				control = control.Parent;
			}
			return default(T);
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

		public Color BackgroundColor
		{
			get { return Handler.BackgroundColor; }
			set { Handler.BackgroundColor = value; }
		}

		public virtual bool HasFocus
		{
			get { return Handler.HasFocus; }
		}

		public virtual void Focus()
		{
			Handler.Focus();
		}

		public virtual void SuspendLayout()
		{
			Handler.SuspendLayout();
		}

		public virtual void ResumeLayout()
		{
			Handler.ResumeLayout();
		}

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

		public void MapPlatformAction(string systemAction, Command action)
		{
			Handler.MapPlatformAction(systemAction, action);
		}

		public PointF PointFromScreen(PointF point)
		{
			return Handler.PointFromScreen(point);
		}

		public PointF PointToScreen(PointF point)
		{
			return Handler.PointToScreen(point);
		}

		public Point Location
		{
			get { return Handler.Location; }
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
		public virtual void UpdateBindings()
		{
			var bindings = Properties.Get<BindingCollection>(BindingsKey);
			if (bindings != null)
			{
				bindings.Update();
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
	}
}
