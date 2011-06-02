using System;
using System.Collections;
using System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Forms.WXWidgets
{
	public abstract class WXContainer : WXControl, IContainer
	{
		private ArrayList controls;
		Layout layout = null;

		public WXContainer(Widget widget) : base(widget)
		{
			this.controls = new ArrayList();
		}

		public void CreateChildren()
		{
			foreach (Control c in controls)
			{
				((WXControl)c.InnerControl).CreateControl((Container)Widget);
			}
		}

		public ArrayList Controls
		{
			get { return controls; }
		}

		
		#region IContainer Members

		public virtual object ContainerObject
		{
			get	{ return ControlObject; }
		}

		public Layout Layout
		{
			get { return layout; }
			set 
			{
				layout = value; 
				SetLayoutControl(layout);
			}
		}

		#endregion

		public virtual void SetLayoutControl(Layout layout)
		{
			((WXLayout)layout.InnerControl).Initialize(this);
			/*
			control.Controls.Clear();
			if (layout.ControlObject != null)
			{
				control.Controls.Add((SWF.Control)layout.ControlObject);
			}
			*/
		}
	}
}
