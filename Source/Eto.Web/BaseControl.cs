using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;

namespace Eto.Web
{
	public class BaseControl : WebControl, INamingContainer
	{
		bool viewStateLoaded;
		
		public BaseControl() : base(HtmlTextWriterTag.Div)
		{
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
			Initialize();
			Layout();
			ReadState();
		}
		
		public bool ViewStateLoaded
		{
			get { return viewStateLoaded; }
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
		
		protected override void LoadViewState(object savedState)
		{
			viewStateLoaded = true;
			base.LoadViewState(savedState);
		}
		
	}
}
