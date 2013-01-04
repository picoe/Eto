using System;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Windows.Drawing;
using System.Diagnostics;

namespace Eto.Platform.Windows
{
	public interface IWindowsControl
	{
		bool InternalVisible { get; }

		SWF.DockStyle DockStyle { get; }

		SWF.Control ContainerControl { get; }

		Size DesiredSize { get; }

		void SetScale (bool xscale, bool yscale);
	}

	public static class WindowsControlExtensions
	{
        public static IWindowsControl GetWindowsControl(this Control control)
        {
            IWindowsControl result = null;

            if (control == null)
                return result;

            result = control.Handler as IWindowsControl;

            // recurse
            if (result == null &&
                control.ControlObject is Control)
                result = (control.ControlObject as Control).GetWindowsControl();

            return result;
        }

        public static SWF.Control GetSwfControl(this Control control)
        {
            var result = control.ControlObject as SWF.Control;

            if (result == null)
            {
                var c = control.ControlObject as Control;

                // recurse
                if (c != null)
                    result = c.GetSwfControl();
            }

            return result;
        }

        public static SWF.Control GetContainerControl(this Control control)
        {
            var wc = control.GetWindowsControl();
            if (wc != null)
                return wc.ContainerControl;
            else
                return control.GetSwfControl();
        }
    }

	public static class ControlExtensions
	{
        public static void SetScale(this Control control, bool xscale, bool yscale)
        {
            var handler = control.GetWindowsControl();

            if (handler != null)
                handler.SetScale(xscale, yscale);
        }
    }

    /*public class MouseEventSourceHandler : Eto.Interface.IMouseInputSource
    {
    }*/

	public abstract class WindowsControl<T, W> : WidgetHandler<T, W>, IControl, IWindowsControl
		where T: System.Windows.Forms.Control
		where W: Control
	{
		bool internalVisible = true;
		Font font;
		Cursor cursor;
		string tooltip;
		Size desiredSize = new Size(-1, -1);
		protected bool XScale { get; set; }
		protected bool YScale { get; set; }

		public virtual Size? DefaultSize { get { return null; } }

		public virtual Size DesiredSize
		{
			get
			{
				var size = desiredSize;
				var defSize = DefaultSize;
				if (defSize != null) {
					if (size.Width == -1) size.Width = defSize.Value.Width;
					if (size.Height == -1) size.Height = defSize.Value.Height;
				}
				return size;
			}
		}

		public virtual SWF.Control ContainerControl
		{
			get { return this.Control; }
		}

		public override void Initialize ()
		{
			Control.TabIndex = 100;
			XScale = true;
			YScale = true;
			this.Control.Margin = SWF.Padding.Empty;
		}
		
		public virtual SWF.DockStyle DockStyle {
			get { return SWF.DockStyle.Fill; }
		}

		protected virtual void CalculateMinimumSize ()
		{
			var size = this.DesiredSize;
			if (XScale) size.Width = 0;
			if (YScale) size.Height = 0;
			Control.MinimumSize = size.ToSD ();
		}

		public virtual void SetScale (bool xscale, bool yscale)
		{
			this.XScale = xscale;
			this.YScale = yscale;
			CalculateMinimumSize ();
		}

		public void SetScale ()
		{
			SetScale (XScale, YScale);
		}

		public override void AttachEvent (string handler)
		{
            Action<Action<DragEventArgs>, SWF.DragEventArgs>
                handleDragEvent = (f, e) =>
                {
                    var e1 =
                        e.ToEto();

                    // call the function
                    f(e1);

                    e.Effect =
                        e1.Effect.ToSWF();
                };


			switch (handler) {
			case Eto.Forms.Control.KeyDownEvent: 
				Control.KeyDown += new SWF.KeyEventHandler (Control_KeyDown);
				Control.KeyUp += new SWF.KeyEventHandler (Control_KeyUp);
				Control.KeyPress += new System.Windows.Forms.KeyPressEventHandler (Control_KeyPress);
				break;
				
			case Eto.Forms.Control.TextChangedEvent:
				Control.TextChanged += Control_TextChanged;
				break;
			case Eto.Forms.Control.SizeChangedEvent:
				Control.SizeChanged += Control_SizeChanged;
				break;
            case Eto.Forms.Control.MouseClickEvent:
                Control.MouseClick += Control_Click;
                break;
            case Eto.Forms.Control.MouseDoubleClickEvent:
				Control.MouseDoubleClick += Control_DoubleClick;
				break;
			case Eto.Forms.Control.MouseEnterEvent:
				Control.MouseEnter += HandleControlMouseEnter;
				break;
			case Eto.Forms.Control.MouseLeaveEvent:
				Control.MouseLeave += HandleControlMouseLeave;
				break;
			case Eto.Forms.Control.MouseDownEvent:
				Control.MouseDown += Control_MouseDown;
				break;
			case Eto.Forms.Control.MouseUpEvent:
				Control.MouseUp += Control_MouseUp;
				break;
			case Eto.Forms.Control.MouseMoveEvent:
				Control.MouseMove += Control_MouseMove;
				break;
            case Eto.Forms.Control.MouseHoverEvent:
                Control.MouseHover += Control_MouseHover;
                break;
            case Eto.Forms.Control.MouseWheelEvent:
                Control.MouseWheel += Control_MouseWheel;
                break;
            case Eto.Forms.Control.GotFocusEvent:
				Control.GotFocus += delegate {
					Widget.OnGotFocus (EventArgs.Empty); 
				};
				break;
			case Eto.Forms.Control.LostFocusEvent:
				Control.LostFocus += delegate {
					Widget.OnLostFocus (EventArgs.Empty);
				};
				break;
            case Eto.Forms.DragDropInputSource.DragDropEvent:
                Control.DragDrop += (s, e) =>
                    handleDragEvent(
                        Widget.DragDropInputSource.OnDragDrop,
                        e);
                break;
            case Eto.Forms.DragDropInputSource.DragEnterEvent:
                Control.DragEnter += (s, e) =>
                    handleDragEvent(
                        Widget.DragDropInputSource.OnDragEnter,
                        e);
                break;
            case Eto.Forms.DragDropInputSource.DragOverEvent:
                Control.DragOver += (s, e) =>
                    handleDragEvent(
                        Widget.DragDropInputSource.OnDragOver,
                        e);
                break;
            case Eto.Forms.DragDropInputSource.GiveFeedbackEvent:
                Control.GiveFeedback += (s, e) =>
                    Widget.DragDropInputSource.OnGiveFeedback(
                        e.ToEto());
                break;
            case Eto.Forms.DragDropInputSource.QueryContinueDragEvent:
                Control.QueryContinueDrag += (s, e) =>
                    // TODO: convert the result back to SWF
                    Widget.DragDropInputSource.OnQueryContinueDrag(
                        e.ToEto());
                break;
            }
		}

        void Control_MouseWheel(object sender, SWF.MouseEventArgs e)
        {
            Widget.OnMouseWheel(e.ToEto());
        }

        void Control_MouseHover(object sender, EventArgs e)
        {
            Widget.OnMouseHover(new MouseEventArgs(MouseButtons.None, KeyMap.Convert(SWF.Control.ModifierKeys), SWF.Control.MousePosition.ToEto()));
        }

		void HandleControlMouseLeave (object sender, EventArgs e)
		{
            Widget.OnMouseLeave(new MouseEventArgs(MouseButtons.None, KeyMap.Convert(SWF.Control.ModifierKeys), SWF.Control.MousePosition.ToEto()));
		}

		void HandleControlMouseEnter (object sender, EventArgs e)
		{
            Widget.OnMouseEnter(new MouseEventArgs(MouseButtons.None, KeyMap.Convert(SWF.Control.ModifierKeys), SWF.Control.MousePosition.ToEto()));
		}

        void Control_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Widget.OnMouseClick(e.ToEto());
        }
        
        void Control_DoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            Widget.OnMouseDoubleClick(e.ToEto());
		}

		void Control_MouseUp (Object sender, SWF.MouseEventArgs e)
		{
            Widget.OnMouseUp(e.ToEto());
		}

		void Control_MouseMove (Object sender, SWF.MouseEventArgs e)
		{
            Widget.OnMouseMove(e.ToEto());
		}

		void Control_MouseDown (object sender, SWF.MouseEventArgs e)
		{
            Widget.OnMouseDown(e.ToEto());
		}

		public virtual string Text {
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public virtual Size Size {
			get { return ContainerControl.Size.ToEto (); }
			set {
				this.ContainerControl.AutoSize = value.Width == -1 || value.Height == -1;
				ContainerControl.Size = value.ToSD ();
				desiredSize = value;
				CalculateMinimumSize ();
			}
		}

		public virtual Size ClientSize {
			get { return new Size (ContainerControl.ClientSize.Width, ContainerControl.ClientSize.Height); }
			set {
				this.ContainerControl.AutoSize = value.Width == -1 || value.Height == -1;
				ContainerControl.ClientSize = value.ToSD ();
			}
		}

		public bool Enabled {
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		public Cursor Cursor {
			get { return cursor; }
			set {
				cursor = value;
				if (cursor != null)
					this.Control.Cursor = cursor.ControlObject as SWF.Cursor;
				else
					this.Control.Cursor = null;
			}
		}
		
		public string ToolTip {
			get { return tooltip; }
			set {
				tooltip = value;
				SetToolTip ();
			}
		}

		public virtual void Invalidate ()
		{
			Control.Invalidate (true);
		}

		public virtual void Invalidate (Rectangle rect)
		{
			Control.Invalidate (rect.ToSD (), true);
		}

		public virtual Color BackgroundColor {
			get { return Control.BackColor.ToEto (); }
			set { Control.BackColor = value.ToSD (); }
		}

		public Graphics CreateGraphics ()
		{
			return new Graphics (Widget.Generator, new GraphicsHandler (Control.CreateGraphics ()));
		}

		public virtual void SuspendLayout ()
		{
			Control.SuspendLayout ();
		}

		public virtual void ResumeLayout ()
		{
			Control.ResumeLayout ();
		}

		public void Focus ()
		{
			if (!Control.Visible)
				Control.TabIndex = 0;
			Control.Focus ();
		}

		public bool HasFocus {
			get { return Control.Focused; }
		}

		bool IWindowsControl.InternalVisible {
			get { return internalVisible; }
		}

		public bool Visible {
			get { return ContainerControl.Visible; }
			set {
				internalVisible = value;
				ContainerControl.Visible = value;
			}
		}

		public virtual void SetLayout (Layout layout)
		{
		}

		public virtual void SetParentLayout (Layout layout)
		{
		}

		public virtual void SetParent (Control parent)
		{
            // This is needed to
            // detach docking windows.
            if (parent == null)
                Control.Parent = null;            
		}

		void Control_SizeChanged (object sender, EventArgs e)
		{
			Widget.OnSizeChanged (e);
		}

		public virtual void OnPreLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoad (EventArgs e)
		{
		}

		public virtual void OnLoadComplete (EventArgs e)
		{
			SetToolTip ();
		}
		
		void SetToolTip ()
		{
			if (this.Widget.ParentWindow != null) {
				var parent = this.Widget.ParentWindow.Handler as IWindowHandler;
				if (parent != null)
					parent.ToolTips.SetToolTip (Control, tooltip);
			}
		}
		
		Key key;
		bool handled;
		char keyChar;
		bool charPressed;

		void Control_KeyDown (object sender, System.Windows.Forms.KeyEventArgs e)
		{
			charPressed = false;
			handled = true;
			key = KeyMap.Convert (e.KeyData);

			if (key != Key.None) {
				KeyPressEventArgs kpea = new KeyPressEventArgs (key, KeyType.KeyDown);
				Widget.OnKeyDown (kpea);
                e.SuppressKeyPress = kpea.SuppressKeyPress;
				e.Handled = kpea.Handled;
				handled = kpea.Handled;
			} else
				handled = false;
			if (!handled && charPressed) {
				// this is when something in the event causes messages to be processed for some reason (e.g. show dialog box)
				// we want the char event to come after the dialog is closed, and handled is set to true!
                KeyPressEventArgs kpea = new KeyPressEventArgs(key, KeyType.KeyDown, keyChar);
				Widget.OnKeyDown (kpea);
                e.SuppressKeyPress = kpea.SuppressKeyPress;
				e.Handled = kpea.Handled;
			}
		}

        void Control_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            charPressed = false;
            handled = true;
            key = KeyMap.Convert(e.KeyData);

            if (key != Key.None)
            {
                KeyPressEventArgs kpea = new KeyPressEventArgs(key, KeyType.KeyUp);
                Widget.OnKeyUp(kpea);
                e.Handled = kpea.Handled;
                handled = kpea.Handled;
            }
            else
                handled = false;
            if (!handled && charPressed)
            {
                // this is when something in the event causes messages to be processed for some reason (e.g. show dialog box)
                // we want the char event to come after the dialog is closed, and handled is set to true!
                KeyPressEventArgs kpea = new KeyPressEventArgs(key, KeyType.KeyUp, keyChar);
                Widget.OnKeyUp(kpea);
                e.Handled = kpea.Handled;
            }
        }
        
        void Control_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			charPressed = true;
			keyChar = e.KeyChar;
			if (!handled) {
                KeyPressEventArgs kpea = new KeyPressEventArgs(key, KeyType.KeyDown, keyChar);
				Widget.OnKeyDown (kpea);
				e.Handled = kpea.Handled;
			} else
				e.Handled = true;
		}

		void Control_TextChanged (object sender, EventArgs e)
		{
			Widget.OnTextChanged (e);
		}
		
		public Font Font {
			get
			{
				if (font == null)
					font = new Font (Widget.Generator, new FontHandler (Control.Font));
				return font;
			}
			set
			{
				font = value;
				if (font != null)
					this.Control.Font = font.ControlObject as System.Drawing.Font;
				else
					this.Control.Font = null;
			}
		}
		
		public virtual void MapPlatformAction (string systemAction, BaseAction action)
		{
		}

        public virtual Point ScreenToWorld(Point p)
        {
            return 
                this.Control.PointToClient(
                    p.ToSD()).ToEto();
        }

        public virtual Point WorldToScreen(Point p)
        {
            return 
                this.Control.PointToScreen(
                    p.ToSD()).ToEto();
        }

        public DragDropEffects DoDragDrop(
            object data, 
            DragDropEffects allowedEffects)
        {
            return                
                this.Control.DoDragDrop(
                data,
                allowedEffects.ToSWF()).ToEto();
        }


        public bool Capture
        {
            get
            {
                return this.Control.Capture;
            }
            set
            {
                this.Control.Capture = value;
            }
        }

        public Point MousePosition
        {
            get { return SWF.Control.MousePosition.ToEto(); }
        }

        public Point Location
        {
            get { return this.Control.Location.ToEto(); }
        }

        public void SetControl(object control)
        {
            this.Control = control as T;
        }
    }
}
