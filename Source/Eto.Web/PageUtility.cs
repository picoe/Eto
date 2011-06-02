using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Eto.Web
{
	public static class PageUtility
	{
		
		public static void SetDefaultButton(WebControl control, Control button)
		{
			control.Attributes.Add("onkeypress", string.Format("if (event.keyCode == 13) {0};", button.Page.ClientScript.GetPostBackEventReference(button, string.Empty)));
		}

		public static string GetObjectScript(Control control)
		{
			return string.Format("document.getElementById('{0}')", control.ClientID);
		}
		
		public static void SetInitialFocus(Control control)
		{
			if (control.Page.ClientScript.IsClientScriptBlockRegistered(typeof(PageUtility), "InitialFocus"))
				return;
			
			if (control.Page == null)
			{
				throw new ArgumentException("The Control must be added to a Page before you can set the IntialFocus to it.");
			}
			if (control.Page.Request.Browser.EcmaScriptVersion.Major >= 1)
			{
				// Create JavaScript
				StringBuilder s = new StringBuilder();
				s.Append("\n<script type='text/javascript'>\n");
				s.Append("<!--\n");
				s.Append("function SetInitialFocus()\n");
				s.Append("{\n");
				s.AppendFormat("   {0}.focus();\n", GetObjectScript(control));
				s.Append("}\n");
				s.Append("window.onload = SetInitialFocus;\n");
				s.Append("// -->\n");
				s.Append("</script>");
								
				// Register Client Script
				control.Page.ClientScript.RegisterClientScriptBlock(typeof(PageUtility), "InitialFocus", s.ToString());
			}
		}
	}
}
