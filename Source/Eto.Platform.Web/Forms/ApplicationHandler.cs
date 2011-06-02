using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;

namespace Eto.Platform.Web.Forms
{
	public class ApplicationHandler : WidgetHandler, IApplication
	{
		Application widget;
		
		public ApplicationHandler(Widget widget)
		{
			this.widget = (Application)widget;
		}
		
		#region IApplication Members
		
		public void Run()
		{
			widget.OnInitialized(EventArgs.Empty);
		}
		
		public void RunIteration()
		{
		}
		
		
		public void Quit()
		{
		}
		
		#endregion
		
		#region IWidgetBase Members
		
		public override void Initialize()
		{
		}
		
		public override Object ControlObject
		{
			get { return null; }
		}
		#endregion
	}
}

