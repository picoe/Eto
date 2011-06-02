using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Eto.Web
{
	public class NamedControl : WebControl, INamingContainer
	{
		public NamedControl(System.Web.UI.HtmlTextWriterTag tag)
		: base(tag)
		{
		}
	}
}
