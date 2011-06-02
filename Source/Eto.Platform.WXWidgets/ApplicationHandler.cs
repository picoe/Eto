using System;

namespace Eto.Forms.WXWidgets
{
	internal class WXApp : wx.App
	{

		public event EventHandler Init;
		public WXApp()
		{
		}

		public override bool OnInit()
		{
			if (Init != null) Init(this, EventArgs.Empty);

			return true;
		}

		
	}

	internal class ApplicationHandler : IApplication
	{
		WXApp app;
		Application widget;

		public ApplicationHandler(Widget widget)
		{
			this.widget = (Application)widget;
			app = new WXApp();
			app.Init += new EventHandler(app_Init);
		}

		public void Run()
		{
			app.Run();
			app = null;
		}

		public void Quit()
		{
			app.Dispose();
		}

		private void app_Init(object sender, EventArgs e)
		{
			widget.OnInit(EventArgs.Empty);
			if (widget.MainForm != null)
			{
				((FormHandler)widget.MainForm.InnerControl).Closed += new EventHandler(ApplicationHandler_Closed);
			}
		}

		#region IWidget Members

		public object ControlObject
		{
			get { return null; }
		}

		public void Initialize()
		{
		}

		#endregion

		private void ApplicationHandler_Closed(object sender, EventArgs e)
		{
			//app.Dispose();
		}
	}
}
