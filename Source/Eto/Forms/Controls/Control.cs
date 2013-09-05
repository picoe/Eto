using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IControl : IInstanceWidget
	{
		Color BackgroundColor { get; set; }

		Size Size { get; set; }

		bool Enabled { get; set; }

		void Invalidate ();

		void Invalidate (Rectangle rect);

		void SuspendLayout ();

		void ResumeLayout ();

		void Focus ();

		bool HasFocus { get; }

		bool Visible { get; set; }

		void OnPreLoad (EventArgs e);

		void OnLoad (EventArgs e);

		void OnLoadComplete (EventArgs e);

		void OnUnLoad (EventArgs e);

		void SetParent (Control parent);

		void SetParentLayout (Layout layout);

		void MapPlatformAction (string systemAction, BaseAction action);

		PointF PointFromScreen (PointF point);
		
		PointF PointToScreen (PointF point);

		Point Location { get; }
	}
	
	[ToolboxItem(true)]
	[DesignTimeVisible(true)]
	[DesignerCategory("Eto.Forms")]
	public abstract partial class Control : 
        InstanceWidget, 
        IMouseInputSource,
        IKeyboardInputSource
	{
		new IControl Handler { get { return (IControl)base.Handler; } }
		
		public bool Loaded { get; private set; }

		#region Events
		
		public const string SizeChangedEvent = "Control.SizeChanged";
		EventHandler<EventArgs> sizeChanged;

		public event EventHandler<EventArgs> SizeChanged
		{
			add
			{
				HandleEvent (SizeChangedEvent);
				sizeChanged += value;
			}
			remove { sizeChanged -= value; }
		}

		public virtual void OnSizeChanged (EventArgs e)
		{
			if (sizeChanged != null)
				sizeChanged (this, e);
		}

		public const string KeyDownEvent = "Control.KeyDown";
		EventHandler<KeyEventArgs> keyDown;

		public event EventHandler<KeyEventArgs> KeyDown
		{
			add
			{
				HandleEvent (KeyDownEvent);
				keyDown += value;
			}
			remove { keyDown -= value; }
		}
		
		public virtual void OnKeyDown (KeyEventArgs e)
		{
			//Console.WriteLine("{0} ({1})", e.KeyData, this);
			if (keyDown != null)
				keyDown (this, e);
			if (!e.Handled && Parent != null)
				Parent.OnKeyDown (e);
		}

		public const string KeyUpEvent = "Control.KeyUp";
		EventHandler<KeyEventArgs> keyUp;

		public event EventHandler<KeyEventArgs> KeyUp
		{
			add
			{
				HandleEvent (KeyUpEvent);
				keyUp += value;
			}
			remove { keyUp -= value; }
		}

		public virtual void OnKeyUp (KeyEventArgs e)
		{
			//Console.WriteLine("{0} ({1})", e.KeyData, this);
			if (keyUp != null)
				keyUp (this, e);
			if (!e.Handled && Parent != null)
				Parent.OnKeyUp (e);
		}

		public const string TextChangedEvent = "Control.TextChanged";
		EventHandler<EventArgs> textChanged;

		public event EventHandler<EventArgs> TextChanged
		{
			add
			{
				HandleEvent (TextChangedEvent);
				textChanged += value;
			}
			remove { textChanged -= value; }
		}

		public virtual void OnTextChanged (EventArgs e)
		{
			if (textChanged != null)
				textChanged (this, e);
		}

		public const string MouseDownEvent = "Control.MouseDown";
		EventHandler<MouseEventArgs> mouseDown;

		public event EventHandler<MouseEventArgs> MouseDown
		{
			add
			{
				HandleEvent (MouseDownEvent);
				mouseDown += value;
			}
			remove { mouseDown -= value; }
		}

		public virtual void OnMouseDown (MouseEventArgs e)
		{
			if (mouseDown != null)
				mouseDown (this, e);
		}

		public const string MouseUpEvent = "Control.MouseUp";
		EventHandler<MouseEventArgs> mouseUp;

		public event EventHandler<MouseEventArgs> MouseUp
		{
			add
			{
				HandleEvent (MouseUpEvent);
				mouseUp += value;
			}
			remove { mouseUp -= value; }
		}

		public virtual void OnMouseUp (MouseEventArgs e)
		{
			if (mouseUp != null)
				mouseUp (this, e);
		}

		public const string MouseMoveEvent = "Control.MouseMove";
		EventHandler<MouseEventArgs> mouseMove;

		public event EventHandler<MouseEventArgs> MouseMove
		{
			add
			{
				HandleEvent (MouseMoveEvent);
				mouseMove += value;
			}
			remove { mouseMove -= value; }
		}

		public virtual void OnMouseMove (MouseEventArgs e)
		{
			if (mouseMove != null)
				mouseMove (this, e);
		}

		public const string MouseLeaveEvent = "Control.MouseLeave";
		EventHandler<MouseEventArgs> mouseLeave;

		public event EventHandler<MouseEventArgs> MouseLeave
		{
			add
			{
				HandleEvent (MouseLeaveEvent);
				mouseLeave += value;
			}
			remove { mouseLeave -= value; }
		}

		public virtual void OnMouseLeave (MouseEventArgs e)
		{
			if (mouseLeave != null)
				mouseLeave (this, e);
		}

		public const string MouseEnterEvent = "Control.MouseEnter";
		EventHandler<MouseEventArgs> mouseEnter;

		public event EventHandler<MouseEventArgs> MouseEnter
		{
			add
			{
				HandleEvent (MouseEnterEvent);
				mouseEnter += value;
			}
			remove { mouseEnter -= value; }
		}

		public virtual void OnMouseEnter (MouseEventArgs e)
		{
			if (mouseEnter != null)
				mouseEnter (this, e);
		}
		
		public const string MouseDoubleClickEvent = "Control.MouseDoubleClick";
		EventHandler<MouseEventArgs> mouseDoubleClick;

		public event EventHandler<MouseEventArgs> MouseDoubleClick
		{
			add
			{
				HandleEvent (MouseDoubleClickEvent);
				mouseDoubleClick += value;
			}
			remove { mouseDoubleClick -= value; }
		}

		public virtual void OnMouseDoubleClick (MouseEventArgs e)
		{
			if (mouseDoubleClick != null)
				mouseDoubleClick (this, e);
		}

		public const string MouseWheelEvent = "Control.MouseWheel";
		EventHandler<MouseEventArgs> mouseWheel;
		
		public event EventHandler<MouseEventArgs> MouseWheel
		{
			add
			{
				HandleEvent (MouseWheelEvent);
				mouseWheel += value;
			}
			remove { mouseWheel -= value; }
		}
		
		public virtual void OnMouseWheel (MouseEventArgs e)
		{
			if (mouseWheel != null)
				mouseWheel (this, e);
		}
		
		public const string GotFocusEvent = "Control.GotFocus";
		EventHandler<EventArgs> gotFocus;

		public event EventHandler<EventArgs> GotFocus
		{
			add
			{
				HandleEvent (GotFocusEvent);
				gotFocus += value;
			}
			remove { gotFocus -= value; }
		}

		public virtual void OnGotFocus (EventArgs e)
		{
			if (gotFocus != null)
				gotFocus (this, e);
		}

		public const string LostFocusEvent = "Control.LostFocus";
		EventHandler<EventArgs> lostFocus;

		public event EventHandler<EventArgs> LostFocus
		{
			add
			{
				HandleEvent (LostFocusEvent);
				lostFocus += value;
			}
			remove { lostFocus -= value; }
		}

		public virtual void OnLostFocus (EventArgs e)
		{
			if (lostFocus != null)
				lostFocus (this, e);
		}
		
		public const string ShownEvent = "Control.Shown";
		EventHandler<EventArgs> shown;

		public event EventHandler<EventArgs> Shown
		{
			add
			{
				HandleEvent (ShownEvent);
				shown += value;
			}
			remove { shown -= value; }
		}

		public virtual void OnShown (EventArgs e)
		{
			if (shown != null)
				shown (this, e);
		}

		public event EventHandler<EventArgs> PreLoad;

		public virtual void OnPreLoad (EventArgs e)
		{
			if (PreLoad != null)
				PreLoad (this, e);
			Handler.OnPreLoad (e);
		}

		public event EventHandler<EventArgs> Load;

		public virtual void OnLoad (EventArgs e)
		{
			Loaded = true;
			if (Load != null)
				Load (this, e);
			Handler.OnLoad (e);
		}

		public event EventHandler<EventArgs> LoadComplete;

		public virtual void OnLoadComplete (EventArgs e)
		{
			if (LoadComplete != null)
				LoadComplete (this, e);
			Handler.OnLoadComplete (e);
		}

		public event EventHandler<EventArgs> UnLoad;
		
		public virtual void OnUnLoad (EventArgs e)
		{
			Loaded = false;
			if (UnLoad != null)
				UnLoad (this, e);
			Handler.OnUnLoad (e);
		}

		#endregion

		protected Control (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		protected Control (Generator generator, IControl handler, bool initialize = true)
			: base (generator, handler, initialize)
		{
		}

		public void Invalidate ()
		{
			Handler.Invalidate ();
		}

		public void Invalidate (Rectangle rect)
		{
			Handler.Invalidate (rect);
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
		
		public override object DataContext
		{
			get { return base.DataContext ?? (Parent != null ? Parent.DataContext : null); }
			set { base.DataContext = value; }
		}
		
		public Layout ParentLayout { get; private set; }
		
		public Control Parent { get; private set; }

		public T FindParent<T> (string id)
			where T: class
		{
			var control = this.Parent;
			while (control != null) {
				if (control is T && (string.IsNullOrEmpty (id) || control.ID == id)) {
					return control as T;
				}
				control = control.Parent;
			}
			return default(T);
		}

		public T FindParent<T> ()
			where T : class
		{
			var control = this.Parent;
			while (control != null) {
				if (control is T)
					return control as T;
				control = control.Parent;
			}
			return default (T);
		}
		
		public void SetParent (Control parent)
		{
			var loaded = this.Loaded;
			Handler.SetParent (parent);
			this.Parent = parent;
			OnDataContextChanged (EventArgs.Empty);
			if (parent == null && loaded) {
				this.OnUnLoad (EventArgs.Empty);
			}
		}

		public void SetParentLayout (Layout layout)
		{
			Handler.SetParentLayout (layout);
			this.ParentLayout = layout;
			this.SetParent (layout != null ? layout.Container : null);
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
		
		public virtual void Focus ()
		{
			Handler.Focus ();
		}
		
		public virtual void SuspendLayout ()
		{
			Handler.SuspendLayout ();
		}

		public virtual void ResumeLayout ()
		{
			Handler.ResumeLayout ();
		}

		public Window ParentWindow
		{
			get
			{
				Control c = this;
				while (c != null) {
					if (c is Window)
						return (Window)c;
					c = c.Parent;
				}
				return null;
			}
		}
		
		public void MapPlatformAction (string systemAction, BaseAction action)
		{
			Handler.MapPlatformAction (systemAction, action);
		}

		public PointF PointFromScreen (PointF point)
		{
			return Handler.PointFromScreen (point);
		}

		public PointF PointToScreen (PointF point)
		{
			return Handler.PointToScreen (point);
		}

		public Point Location
		{
			get { return Handler.Location; }
		}

		#region Obsolete

		[Obsolete ("This event is deprecated")]
		public const string HiddenEvent = "Control.Hidden";

		EventHandler<EventArgs> hidden;

		[Obsolete ("This event is deprecated")]
		public event EventHandler<EventArgs> Hidden
		{
			add
			{
				HandleEvent (HiddenEvent);
				hidden += value;
			}
			remove { hidden -= value; }
		}

		[Obsolete ("This event is deprecated")]
		public virtual void OnHidden (EventArgs e)
		{
			if (hidden != null)
				hidden (this, e);
		}

		#endregion
	}
	
	public class ControlCollection : List<Control>
	{
		
	}
}
