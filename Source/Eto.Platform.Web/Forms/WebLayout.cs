using System;

using Eto.Forms;

namespace Eto.Platform.Web.Forms
{
	public abstract class WebLayout : ILayout
	{
		Widget widget;
		WebContainer container;
		
		public WebLayout(Widget widget)
		{
			this.widget = widget;
		}
		
		public Widget Widget
		{
			get { return widget; }
		}
		
		public WebContainer Container
		{
			get { return container; }
		}
		
		
		#region IWidgetBase Members
		
		public abstract object ControlObject
		{ get; }
		
		public virtual void Initialize()
		{
		}

		public virtual void SetWidget(Widget widget)
		{
			// TODO: Implement this method
		}
		
		public virtual bool HandleEvent(String handler)
		{
			return false;
		}
		
		#endregion
		
		public virtual void Initialize(WebContainer container)
		{
			this.container = container;
		}
	}
}

