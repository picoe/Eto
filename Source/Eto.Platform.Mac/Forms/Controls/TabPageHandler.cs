using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class TabPageHandler : WidgetHandler<NSTabViewItem, TabPage>, ITabPage
	{
		NSTabViewItem control;

		public TabPageHandler()
		{
			control = new NSTabViewItem();
			Control = control;
			//control.Click += control_Click;
		}


		
		public virtual object ContainerObject {
			get {
				return control;
			}
		}

		/*
		private void control_Click(object sender, EventArgs e)
		{
			//base.OnClick(e);
		}
		 */
		
		public virtual void SetLayout (Layout layout)
		{
		}
		
		public virtual void SetParentLayout (Layout layout)
		{
		}
		
		public virtual void SetParent (Control parent)
		{
		}

		public virtual void OnLoad (EventArgs e)
		{
		}

		public virtual void OnLoadComplete (EventArgs e)
		{
		}
		
		#region ITabPage implementation

		#endregion

		#region IContainer implementation

		#endregion

		#region IControl implementation
		public void Invalidate ()
		{
			throw new NotImplementedException ();
		}

		void IControl.Invalidate (Eto.Drawing.Rectangle rect)
		{
			throw new NotImplementedException ();
		}

		public Eto.Drawing.Graphics CreateGraphics ()
		{
			throw new NotImplementedException ();
		}

		public void SuspendLayout ()
		{
			throw new NotImplementedException ();
		}

		public void ResumeLayout ()
		{
			throw new NotImplementedException ();
		}

		public void Focus ()
		{
			throw new NotImplementedException ();
		}

		public Eto.Drawing.Color BackgroundColor {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public string Id {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public Eto.Drawing.Size Size {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public Eto.Drawing.Size ClientSize {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool Enabled {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool HasFocus {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool Visible {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}
		#endregion

		#region ISynchronizeInvoke implementation
		public IAsyncResult BeginInvoke (Delegate method, object[] args)
		{
			throw new NotImplementedException ();
		}

		public object EndInvoke (IAsyncResult result)
		{
			throw new NotImplementedException ();
		}

		public object Invoke (Delegate method, object[] args)
		{
			throw new NotImplementedException ();
		}

		public bool InvokeRequired {
			get {
				throw new NotImplementedException ();
			}
		}
		#endregion

	}
}
