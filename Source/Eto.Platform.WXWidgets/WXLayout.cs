using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	public abstract class WXLayout : ILayout
	{
		Widget widget;
		WXContainer container;

		public WXLayout(Widget widget)
		{
			this.widget = widget;
		}

		public Widget Widget
		{
			get { return widget; }
		}

		public WXContainer Container
		{
			get { return container; }
		}

		public void Initialize(WXContainer container)
		{
			this.Initialize();
			this.container = container;
		}
		
		#region ILayout Members

		public abstract void AddChild(Control child);

		public abstract void RemoveChild(Control child);

		#endregion

		#region IWidget Members

		public abstract object ControlObject { get; }

		public virtual void Initialize()
		{
		}

		#endregion
	}
}
