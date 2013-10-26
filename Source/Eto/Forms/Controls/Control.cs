using System;
using System.ComponentModel;
using System.Collections.Generic;
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

		void MapPlatformAction(string systemAction, BaseAction action);

		PointF PointFromScreen(PointF point);

		PointF PointToScreen(PointF point);

		Point Location { get; }
	}

	[ToolboxItem(true)]
	[DesignTimeVisible(true)]
	[DesignerCategory("Eto.Forms")]
	public abstract partial class Control : InstanceWidget, IMouseInputSource, IKeyboardInputSource
	{
		new IControl Handler { get { return (IControl)base.Handler; } }

		object dataContext;

		public bool Loaded { get; private set; }

		/// <summary>
		/// Gets the attached properties for this widget
		/// </summary>
		PropertyStore properties;

		public PropertyStore Properties
		{
			get
			{
				if (properties == null)
					properties = new PropertyStore(this);
				return properties;
			}
		}

		/// <summary>
		/// Gets the collection of bindings that are attached to this widget
		/// </summary>
		BindingCollection bindings;

		public BindingCollection Bindings
		{
			get
			{
				if (bindings == null)
					bindings = new BindingCollection();
				return bindings;
			}
		}

		/// <summary>
		/// Gets or sets a user-defined object that contains data about the control
		/// </summary>
		/// <remarks>
		/// A common use of the tag property is to store data that is associated with the control that you can later
		/// retrieve.
		/// </remarks>
		public object Tag { get; set; }

		#region Events

		public const string SizeChangedEvent = "Control.SizeChanged";
		EventHandler<EventArgs> sizeChanged;

		public event EventHandler<EventArgs> SizeChanged
		{
			add
			{
				HandleEvent(SizeChangedEvent);
				sizeChanged += value;
			}
			remove { sizeChanged -= value; }
		}

		public virtual void OnSizeChanged(EventArgs e)
		{
			if (sizeChanged != null)
				sizeChanged(this, e);
		}

		public const string KeyDownEvent = "Control.KeyDown";
		EventHandler<KeyEventArgs> keyDown;

		public event EventHandler<KeyEventArgs> KeyDown
		{
			add
			{
				HandleEvent(KeyDownEvent);
				keyDown += value;
			}
			remove { keyDown -= value; }
		}

		public virtual void OnKeyDown(KeyEventArgs e)
		{
			//Console.WriteLine("{0} ({1})", e.KeyData, this);
			if (keyDown != null)
				keyDown(this, e);
			if (!e.Handled && Parent != null)
				Parent.OnKeyDown(e);
		}

		public const string KeyUpEvent = "Control.KeyUp";
		EventHandler<KeyEventArgs> keyUp;

		public event EventHandler<KeyEventArgs> KeyUp
		{
			add
			{
				HandleEvent(KeyUpEvent);
				keyUp += value;
			}
			remove { keyUp -= value; }
		}

		public virtual void OnKeyUp(KeyEventArgs e)
		{
			//Console.WriteLine("{0} ({1})", e.KeyData, this);
			if (keyUp != null)
				keyUp(this, e);
			if (!e.Handled && Parent != null)
				Parent.OnKeyUp(e);
		}

		public const string TextInputEvent = "Control.TextInput";
		EventHandler<TextInputEventArgs> textInput;

		public event EventHandler<TextInputEventArgs> TextInput
		{
			add
			{
				HandleEvent(TextInputEvent);
				textInput += value;
			}
			remove { textInput -= value; }
		}

		public virtual void OnTextInput(TextInputEventArgs e)
		{
			if (textInput != null)
				textInput(this, e);
		}

		public const string MouseDownEvent = "Control.MouseDown";
		EventHandler<MouseEventArgs> mouseDown;

		public event EventHandler<MouseEventArgs> MouseDown
		{
			add
			{
				HandleEvent(MouseDownEvent);
				mouseDown += value;
			}
			remove { mouseDown -= value; }
		}

		public virtual void OnMouseDown(MouseEventArgs e)
		{
			if (mouseDown != null)
				mouseDown(this, e);
		}

		public const string MouseUpEvent = "Control.MouseUp";
		EventHandler<MouseEventArgs> mouseUp;

		public event EventHandler<MouseEventArgs> MouseUp
		{
			add
			{
				HandleEvent(MouseUpEvent);
				mouseUp += value;
			}
			remove { mouseUp -= value; }
		}

		public virtual void OnMouseUp(MouseEventArgs e)
		{
			if (mouseUp != null)
				mouseUp(this, e);
		}

		public const string MouseMoveEvent = "Control.MouseMove";
		EventHandler<MouseEventArgs> mouseMove;

		public event EventHandler<MouseEventArgs> MouseMove
		{
			add
			{
				HandleEvent(MouseMoveEvent);
				mouseMove += value;
			}
			remove { mouseMove -= value; }
		}

		public virtual void OnMouseMove(MouseEventArgs e)
		{
			if (mouseMove != null)
				mouseMove(this, e);
		}

		public const string MouseLeaveEvent = "Control.MouseLeave";
		EventHandler<MouseEventArgs> mouseLeave;

		public event EventHandler<MouseEventArgs> MouseLeave
		{
			add
			{
				HandleEvent(MouseLeaveEvent);
				mouseLeave += value;
			}
			remove { mouseLeave -= value; }
		}

		public virtual void OnMouseLeave(MouseEventArgs e)
		{
			if (mouseLeave != null)
				mouseLeave(this, e);
		}

		public const string MouseEnterEvent = "Control.MouseEnter";
		EventHandler<MouseEventArgs> mouseEnter;

		public event EventHandler<MouseEventArgs> MouseEnter
		{
			add
			{
				HandleEvent(MouseEnterEvent);
				mouseEnter += value;
			}
			remove { mouseEnter -= value; }
		}

		public virtual void OnMouseEnter(MouseEventArgs e)
		{
			if (mouseEnter != null)
				mouseEnter(this, e);
		}

		public const string MouseDoubleClickEvent = "Control.MouseDoubleClick";
		EventHandler<MouseEventArgs> mouseDoubleClick;

		public event EventHandler<MouseEventArgs> MouseDoubleClick
		{
			add
			{
				HandleEvent(MouseDoubleClickEvent);
				mouseDoubleClick += value;
			}
			remove { mouseDoubleClick -= value; }
		}

		public virtual void OnMouseDoubleClick(MouseEventArgs e)
		{
			if (mouseDoubleClick != null)
				mouseDoubleClick(this, e);
		}

		public const string MouseWheelEvent = "Control.MouseWheel";
		EventHandler<MouseEventArgs> mouseWheel;

		public event EventHandler<MouseEventArgs> MouseWheel
		{
			add
			{
				HandleEvent(MouseWheelEvent);
				mouseWheel += value;
			}
			remove { mouseWheel -= value; }
		}

		public virtual void OnMouseWheel(MouseEventArgs e)
		{
			if (mouseWheel != null)
				mouseWheel(this, e);
		}

		public const string GotFocusEvent = "Control.GotFocus";
		EventHandler<EventArgs> gotFocus;

		public event EventHandler<EventArgs> GotFocus
		{
			add
			{
				HandleEvent(GotFocusEvent);
				gotFocus += value;
			}
			remove { gotFocus -= value; }
		}

		public virtual void OnGotFocus(EventArgs e)
		{
			if (gotFocus != null)
				gotFocus(this, e);
		}

		public const string LostFocusEvent = "Control.LostFocus";
		EventHandler<EventArgs> lostFocus;

		public event EventHandler<EventArgs> LostFocus
		{
			add
			{
				HandleEvent(LostFocusEvent);
				lostFocus += value;
			}
			remove { lostFocus -= value; }
		}

		public virtual void OnLostFocus(EventArgs e)
		{
			if (lostFocus != null)
				lostFocus(this, e);
		}

		public const string ShownEvent = "Control.Shown";
		EventHandler<EventArgs> shown;

		public event EventHandler<EventArgs> Shown
		{
			add
			{
				HandleEvent(ShownEvent);
				shown += value;
			}
			remove { shown -= value; }
		}

		public virtual void OnShown(EventArgs e)
		{
			if (shown != null)
				shown(this, e);
		}

		public event EventHandler<EventArgs> PreLoad;

		public virtual void OnPreLoad(EventArgs e)
		{
			if (PreLoad != null)
				PreLoad(this, e);
			Handler.OnPreLoad(e);
		}

		public event EventHandler<EventArgs> Load;

		public virtual void OnLoad(EventArgs e)
		{
			if (Load != null)
				Load(this, e);
			Handler.OnLoad(e);
			Loaded = true;
		}

		public event EventHandler<EventArgs> LoadComplete;

		public virtual void OnLoadComplete(EventArgs e)
		{
			if (LoadComplete != null)
				LoadComplete(this, e);
			Handler.OnLoadComplete(e);
		}

		public event EventHandler<EventArgs> UnLoad;

		public virtual void OnUnLoad(EventArgs e)
		{
			Loaded = false;
			if (UnLoad != null)
				UnLoad(this, e);
			Handler.OnUnLoad(e);
		}

		/// <summary>
		/// Event to handle when the <see cref="Control.DataContext"/> has changed
		/// </summary>
		/// <remarks>
		/// This may be fired in the event of a parent in the hierarchy setting the data context.
		/// For example, the <see cref="Forms.Container"/> widget fires this event when it's event is fired.
		/// </remarks>
		public event EventHandler<EventArgs> DataContextChanged;

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
			if (DataContextChanged != null)
				DataContextChanged(this, e);
		}

		#endregion

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
			get { return dataContext ?? (Parent != null ? Parent.DataContext : null); }
			set
			{
				dataContext = value;
				OnDataContextChanged(EventArgs.Empty);
			}
		}

		[Obsolete("Use Parent instead")]
		public Container ParentLayout { get { return Parent; } }

		public Container Parent { get; private set; }

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

		internal void SetParent(Container parent, bool changeContext = true)
		{
			if (Parent != parent)
			{
				var loaded = Loaded;
				Handler.SetParent(parent);
				Parent = parent;
				if (changeContext)
					OnDataContextChanged(EventArgs.Empty);
				if (parent == null && loaded)
				{
					OnUnLoad(EventArgs.Empty);
				}
			}
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

		public void MapPlatformAction(string systemAction, BaseAction action)
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

		#region Obsolete

		[Obsolete("This event is deprecated")]
		public const string HiddenEvent = "Control.Hidden";
		EventHandler<EventArgs> hidden;

		[Obsolete("This event is deprecated")]
		public event EventHandler<EventArgs> Hidden
		{
			add
			{
				HandleEvent(HiddenEvent);
				hidden += value;
			}
			remove { hidden -= value; }
		}

		[Obsolete("This event is deprecated")]
		public virtual void OnHidden(EventArgs e)
		{
			if (hidden != null)
				hidden(this, e);
		}

		#endregion

		/// <summary>
		/// Unbinds any bindings in the <see cref="Bindings"/> collection and removes the bindings
		/// </summary>
		public virtual void Unbind()
		{
			if (bindings != null)
			{
				bindings.Unbind();
				bindings = null;
			}
		}

		/// <summary>
		/// Updates all bindings in this widget
		/// </summary>
		public virtual void UpdateBindings()
		{
			if (bindings != null)
			{
				bindings.Update();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Unbind();
			}

			base.Dispose(disposing);
		}
	}

	public class ControlCollection : List<Control>
	{
	}
}
