using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Eto.Web
{
	public class BasePage : Page
	{
		bool viewStateLoaded = false;
		
		public bool ViewStateLoaded
		{
			get { return viewStateLoaded; }
		}
		
		protected override void OnLoad(EventArgs e)
		{
			Initialize();
			Layout();
			
			base.OnLoad(e);
		}
		
		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			ReadState();
		}
		
		
		protected virtual void Initialize()
		{
			
		}
		
		protected virtual void Layout()
		{
			
		}
		
		protected virtual void ReadState()
		{
			
		}
		
		protected override void LoadViewState(Object savedState)
		{
			base.LoadViewState(savedState);
			viewStateLoaded = true;
		}
		
		
		
	}
}
