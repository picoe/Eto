using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IControl : IInstanceWidget
	{
		Color BackgroundColor { get; set; }

		string Id { get; set; }

		Size Size { get; set; }

		bool Enabled { get; set; }
		
		Cursor Cursor { get; set; }
		
		string ToolTip { get; set; }

		void Invalidate ();

		void Invalidate (Rectangle rect);

		Graphics CreateGraphics ();

		void SuspendLayout ();

		void ResumeLayout ();

		void Focus ();

		bool HasFocus { get; }

		bool Visible { get; set; }

		void OnPreLoad (EventArgs e);

		void OnLoad (EventArgs e);

		void OnLoadComplete (EventArgs e);

		void SetParent (Control parent);

		void SetParentLayout (Layout layout);
	}
	
	public abstract class Control : InstanceWidget, IControl
	{
		IControl inner;
		
		public bool Loaded { get; private set; }
		
		#region Events
		
		public const string SizeChangedEvent = "Control.SizeChanged";

		event EventHandler<EventArgs> sizeChanged;

		public event EventHandler<EventArgs> SizeChanged {
			add {
				HandleEvent (SizeChangedEvent);
				sizeChanged += value;
			}
			remove { sizeChanged -= value; }
		}

		public const string KeyDownEvent = "Control.KeyDown";

		event EventHandler<KeyPressEventArgs> keyDown;

		public event EventHandler<KeyPressEventArgs> KeyDown {
			add {
				HandleEvent (KeyDownEvent);
				keyDown += value;
			}
			remove { keyDown -= value; }
		}

		public const string TextChangedEvent = "Control.TextChanged";

		event EventHandler<EventArgs> textChanged;

		public event EventHandler<EventArgs> TextChanged {
			add {
				HandleEvent (TextChangedEvent);
				textChanged += value;
			}
			remove { textChanged -= value; }
		}

		public const string MouseDownEvent = "Control.MouseDown";

		event EventHandler<MouseEventArgs> mouseDown;

		public event EventHandler<MouseEventArgs> MouseDown {
			add {
				HandleEvent (MouseDownEvent);
				mouseDown += value;
			}
			remove { mouseDown -= value; }
		}

		public const string MouseUpEvent = "Control.MouseUp";

		event EventHandler<MouseEventArgs> mouseUp;

		public event EventHandler<MouseEventArgs> MouseUp {
			add {
				HandleEvent (MouseUpEvent);
				mouseUp += value;
			}
			remove { mouseUp -= value; }
		}

		public const string MouseMoveEvent = "Control.MouseMove";

		event EventHandler<MouseEventArgs> mouseMove;

		public event EventHandler<MouseEventArgs> MouseMove {
			add {
				HandleEvent (MouseMoveEvent);
				mouseMove += value;
			}
			remove { mouseMove -= value; }
		}

		public const string MouseLeaveEvent = "Control.MouseLeave";

		event EventHandler<MouseEventArgs> mouseLeave;

		public event EventHandler<MouseEventArgs> MouseLeave {
			add {
				HandleEvent (MouseLeaveEvent);
				mouseLeave += value;
			}
			remove { mouseLeave -= value; }
		}

		public const string MouseEnterEvent = "Control.MouseEnter";

		event EventHandler<MouseEventArgs> mouseEnter;

		public event EventHandler<MouseEventArgs> MouseEnter {
			add {
				HandleEvent (MouseEnterEvent);
				mouseEnter += value;
			}
			remove { mouseEnter -= value; }
		}
		
		public const string MouseDoubleClickEvent = "Control.MouseDoubleClick";

		event EventHandler<MouseEventArgs> mouseDoubleClick;

		public event EventHandler<MouseEventArgs> MouseDoubleClick {
			add {
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

		public const string GotFocusEvent = "Control.GotFocus";

		event EventHandler<EventArgs> gotFocus;

		public event EventHandler<EventArgs> GotFocus {
			add {
				HandleEvent (GotFocusEvent);
				gotFocus += value;
			}
			remove { gotFocus -= value; }
		}

		public const string LostFocusEvent = "Control.LostFocus";

		event EventHandler<EventArgs> lostFocus;

		public event EventHandler<EventArgs> LostFocus {
			add {
				HandleEvent (LostFocusEvent);
				lostFocus += value;
			}
			remove { lostFocus -= value; }
		}
		
		public event EventHandler<EventArgs> PreLoad;
		public event EventHandler<EventArgs> Load;
		public event EventHandler<EventArgs> LoadComplete;
		
		#endregion

		public virtual void OnPreLoad (EventArgs e)
		{
			if (PreLoad != null)
				PreLoad (this, e);
			inner.OnPreLoad (e);
		}
		
		public virtual void OnLoad (EventArgs e)
		{
			Loaded = true;
			if (Load != null)
				Load (this, e);
			inner.OnLoad (e);
		}

		public virtual void OnLoadComplete (EventArgs e)
		{
			if (LoadComplete != null)
				LoadComplete (this, e);
			inner.OnLoadComplete (e);
		}
		
		protected Control (Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			this.inner = (IControl)base.Handler;
		}

		public virtual void OnTextChanged (EventArgs e)
		{
			if (textChanged != null)
				textChanged (this, e);
		}

		public virtual void OnSizeChanged (EventArgs e)
		{
			if (sizeChanged != null)
				sizeChanged (this, e);
		}

		public virtual void OnKeyDown (KeyPressEventArgs e)
		{
			//Console.WriteLine("{0} ({1})", e.KeyData, this);
			if (keyDown != null)
				keyDown (this, e);
			if (!e.Handled && Parent != null)
				Parent.OnKeyDown (e);
		}
		
		public virtual void OnMouseDown (MouseEventArgs e)
		{
			if (mouseDown != null)
				mouseDown (this, e);
		}

		public virtual void OnMouseUp (MouseEventArgs e)
		{
			if (mouseUp != null)
				mouseUp (this, e);
		}

		public virtual void OnMouseMove (MouseEventArgs e)
		{
			if (mouseMove != null)
				mouseMove (this, e);
		}

		public virtual void OnMouseEnter (MouseEventArgs e)
		{
			if (mouseEnter != null)
				mouseEnter (this, e);
		}
		
		public virtual void OnMouseLeave (MouseEventArgs e)
		{
			if (mouseLeave != null)
				mouseLeave (this, e);
		}
		
		public virtual void OnGotFocus (EventArgs e)
		{
			if (gotFocus != null)
				gotFocus (this, e);
		}

		public virtual void OnLostFocus (EventArgs e)
		{
			if (lostFocus != null)
				lostFocus (this, e);
		}
		
		public void Invalidate ()
		{
			inner.Invalidate ();
		}

		public void Invalidate (Rectangle rect)
		{
			inner.Invalidate (rect);
		}

		public string Id {
			get { return inner.Id; }
			set { inner.Id = value; }
		}
		
		public virtual Size Size {
			get { return inner.Size; }
			set { inner.Size = value; }
		}

		public virtual bool Enabled {
			get { return inner.Enabled; }
			set { inner.Enabled = value; }
		}

		public virtual bool Visible {
			get { return inner.Visible; }
			set { inner.Visible = value; }
		}
		
		public virtual Cursor Cursor {
			get { return inner.Cursor; }
			set { inner.Cursor = value; }
		}
		
		public virtual string ToolTip {
			get { return inner.ToolTip; }
			set { inner.ToolTip = value; }
		}
		
		public Layout ParentLayout { get; private set; }
		
		public Control Parent { get; private set; }
		
		public void SetParent (Control parent)
		{
			this.Parent = parent;
			inner.SetParent (parent);
		}

		public void SetParentLayout (Layout layout)
		{
			this.ParentLayout = layout;
			inner.SetParentLayout (layout);
			this.SetParent (layout != null ? layout.Container : null);
		}

		public Color BackgroundColor {
			get { return inner.BackgroundColor; }
			set { inner.BackgroundColor = value; }
		}
		
		public virtual bool HasFocus {
			get { return inner.HasFocus; }
		}
		
		public Graphics CreateGraphics ()
		{
			return inner.CreateGraphics ();
		}
		
		public virtual void Focus ()
		{
			inner.Focus ();
		}
		
		public virtual void SuspendLayout ()
		{
			inner.SuspendLayout ();
		}

		public virtual void ResumeLayout ()
		{
			inner.ResumeLayout ();
		}

		public Window ParentWindow {
			get {
				Control c = this;
				while (c != null) {
					if (c is Window)
						return (Window)c;
					c = c.Parent;
				}
				return null;
			}
		}

	}
	
	public class ControlCollection : List<Control>
	{
		
	}
}
