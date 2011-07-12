using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IControl : IInstanceWidget, ISynchronizeInvoke
	{
		Color BackgroundColor { get; set; }
		string Id { get; set; }
		Size Size { get; set; }
		bool Enabled { get; set; }
		void Invalidate();
		void Invalidate(Rectangle rect);
		Graphics CreateGraphics();
		void SuspendLayout();
		void ResumeLayout();
		void Focus();
		bool HasFocus { get; }
		bool Visible { get; set; }

		void OnLoad(EventArgs e);
		void SetParent(Control parent);
		void SetParentLayout(Layout layout);
	}
	
	
	public abstract class Control : InstanceWidget, ISynchronizeInvoke, IControl
	{
		IControl inner;
		
		#region Events
		
		public const string SizeChangedEvent = "Control.SizeChanged";
		event EventHandler<EventArgs> sizeChanged;
		public event EventHandler<EventArgs> SizeChanged
		{
			add { HandleEvent(SizeChangedEvent); sizeChanged += value; }
			remove { sizeChanged -= value; }
		}

		public const string KeyDownEvent = "Control.KeyDown";
		event EventHandler<KeyPressEventArgs> keyDown;
		public event EventHandler<KeyPressEventArgs> KeyDown
		{
			add { HandleEvent(KeyDownEvent); keyDown += value; }
			remove { keyDown -= value; }
		}

		public const string TextChangedEvent = "Control.TextChanged";
		event EventHandler<EventArgs> textChanged;
		public event EventHandler<EventArgs> TextChanged
		{
			add { HandleEvent(TextChangedEvent); textChanged += value; }
			remove { textChanged -= value; }
		}

		public const string MouseDownEvent = "Control.MouseDown";
		event EventHandler<MouseEventArgs> mouseDown;
		public event EventHandler<MouseEventArgs> MouseDown
		{
			add { HandleEvent(MouseDownEvent); mouseDown += value; }
			remove { mouseDown -= value; }
		}

		public const string MouseUpEvent = "Control.MouseUp";
		event EventHandler<MouseEventArgs> mouseUp;
		public event EventHandler<MouseEventArgs> MouseUp
		{
			add { HandleEvent(MouseUpEvent); mouseUp += value; }
			remove { mouseUp -= value; }
		}

		public const string MouseMoveEvent = "Control.MouseMove";
		event EventHandler<MouseEventArgs> mouseMove;
		public event EventHandler<MouseEventArgs> MouseMove
		{
			add { HandleEvent(MouseMoveEvent); mouseMove += value; }
			remove { mouseMove -= value; }
		}
		
		public const string MouseDoubleClickEvent = "Control.MouseDoubleClick";
		event EventHandler<MouseEventArgs> mouseDoubleClick;
		public event EventHandler<MouseEventArgs> MouseDoubleClick
		{
			add { HandleEvent(MouseDoubleClickEvent); mouseDoubleClick += value; }
			remove { mouseDoubleClick -= value; }
		}
		public virtual void OnMouseDoubleClick(MouseEventArgs e)
		{
			if (mouseDoubleClick != null) mouseDoubleClick(this, e);
		}
		

		public const string GotFocusEvent = "Control.GotFocus";
		event EventHandler<EventArgs> gotFocus;
		public event EventHandler<EventArgs> GotFocus
		{
			add { HandleEvent(GotFocusEvent); gotFocus += value; }
			remove { gotFocus -= value; }
		}

		public const string LostFocusEvent = "Control.LostFocus";
		event EventHandler<EventArgs> lostFocus;
		public event EventHandler<EventArgs> LostFocus
		{
			add { HandleEvent(LostFocusEvent); lostFocus += value; }
			remove { lostFocus -= value; }
		}
		
		public event EventHandler<EventArgs> Load;
		
		#endregion
		
		public virtual void OnLoad(EventArgs e)
		{
			if (Load != null) Load(this, e);
			inner.OnLoad(e);
		}

		public Control() : this(Generator.Current)
		{
		}
		
		public Control(Generator generator) : this(generator, typeof(IControl))
		{
		}

		protected Control(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			this.inner = (IControl)base.Handler;
		}

		public virtual void OnTextChanged(EventArgs e)
		{
			if (textChanged != null) textChanged(this, e);
		}

		public virtual void OnSizeChanged(EventArgs e)
		{
			if (sizeChanged != null) sizeChanged(this, e);
		}

		public virtual void OnKeyDown(KeyPressEventArgs e)
		{
			//MessageBox.Show(this.Generator, (Control)this, String.Format("{0} ({1})", e.KeyData.ToString(), this.ToString()));
			if (keyDown != null) keyDown(this, e);
			if (!e.Handled && Parent != null) Parent.OnKeyDown(e);
		}
		
		public virtual void OnMouseDown(MouseEventArgs e)
		{
			if (mouseDown != null) mouseDown(this, e);
		}

		public virtual void OnMouseUp(MouseEventArgs e)
		{
			if (mouseUp != null) mouseUp(this, e);
		}

		public virtual void OnMouseMove(MouseEventArgs e)
		{
			if (mouseMove != null) mouseMove(this, e);
		}

		public virtual void OnGotFocus(EventArgs e)
		{
			if (gotFocus != null) gotFocus(this, e);
		}

		public virtual void OnLostFocus(EventArgs e)
		{
			if (lostFocus != null) lostFocus(this, e);
		}
		
		public void Invalidate()
		{
			inner.Invalidate();
		}

		public void Invalidate(Rectangle rect)
		{
			inner.Invalidate(rect);
		}

		public string Id
		{
			get { return inner.Id; }
			set { inner.Id = value; }
		}
		
		public Size Size
		{
			get { return inner.Size; }
			set { inner.Size = value; }
		}

		public bool Enabled
		{
			get { return inner.Enabled; }
			set { inner.Enabled = value; }
		}

		public bool Visible
		{
			get { return inner.Visible; }
			set { inner.Visible = value; }
		}
		
		public Layout ParentLayout { get; private set; }
		
		public Control Parent { get; private set; }
		
		public void SetParent(Control parent)
		{
			this.Parent = parent;
			inner.SetParent(parent);
		}

		public void SetParentLayout(Layout layout)
		{
			this.ParentLayout = layout;
			inner.SetParentLayout(layout);
			this.SetParent(layout != null ? layout.Container : null);
		}

		public Color BackgroundColor
		{
			get { return inner.BackgroundColor; }
			set { inner.BackgroundColor = value; }
		}
		
		public bool HasFocus
		{
			get { return inner.HasFocus; }
		}
		
		public Graphics CreateGraphics()
		{
			return inner.CreateGraphics();
		}
		
		public void Focus()
		{
			inner.Focus();
		}
		
		public void SuspendLayout()
		{
			inner.SuspendLayout();
		}

		public void ResumeLayout()
		{
			inner.ResumeLayout();
		}

		public Window ParentWindow
		{
			get
			{
				Control c = this;
				while (c != null)
				{
					if (c is Window) return (Window)c;
					c = c.Parent;
				}
				return null;
			}
		}


		#region Positional methods
/*
		public int Top
		{
			get { return Location.Y; }
			set { Location = new Point(Location.X, value); }
		}

		public int Left
		{
			get { return Location.X; }
			set { Location = new Point(value, Location.Y); }
		}

		public int Bottom
		{
			get { return Location.Y + Size.Height; }
			set { Location = new Point(Location.X, value - Size.Height); }
		}

		public int Right
		{
			get { return Location.X + Size.Width; }
			set { Location = new Point(value - Size.Width, Location.Y); }
		}
		public int Width
		{
			get { return Size.Width; }
			set { Size = new Size(value, Size.Height); }
		}

		public int Height
		{
			get { return Size.Height; }
			set { Size = new Size(Size.Width, value); }
		}
		public int MiddleX
		{
			get { return Location.X + (Size.Width/2); }
			set { Location = new Point(value - (Size.Width/2), Location.Y); }
		}
		public int MiddleY
		{
			get { return Location.Y + (Size.Height/2); }
			set { Location = new Point(Location.X, value - (Size.Height/2)); }
		}
		 */
		#endregion

#if false
		public override void PushData()
		{
			/*
			if (controls != null)
			{
				foreach (Control c in controls)
				{
					c.PushData();
				}
			}  */
			base.PushData();
		}

		public override void PullData()
		{
			/*if (controls != null)
			{
				foreach (Control c in controls)
				{
					c.PullData();
				}
			}
			*/
			base.PullData();
		}
#endif

		public object Invoke(Delegate method)
		{
			return Invoke(method, null);
		}

		#region ISynchronizeInvoke Members

		public object EndInvoke(IAsyncResult result)
		{
			return inner.EndInvoke(result);
		}

		public object Invoke(Delegate method, params object[] args)
		{
			return inner.Invoke(method, args);
		}

		public bool InvokeRequired
		{
			get { return inner.InvokeRequired; }
		}

		public IAsyncResult BeginInvoke(Delegate method, object[] args)
		{
			return inner.BeginInvoke(method, args);
		}

		#endregion
	}
	
	public class ControlCollection : List<Control>
	{
		
	}
}
