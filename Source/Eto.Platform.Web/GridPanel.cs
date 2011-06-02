using System;
using SWU = System.Web.UI;
using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;

namespace Eto.Platform.Web
{
	public class GridPanel : SWUC.WebControl
	{
		public GridPanel()
		{
			Style["POSITION"] = "relative";
			Style["OVERFLOW"] = "auto";
			Style["WIDTH"] = "100%";
			Style["HEIGHT"] = "100%";
		}

		public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
		{
			writer.WriteBeginTag("div");
			Attributes.Render(writer);
			writer.Write(SWU.HtmlTextWriter.TagRightChar);
			//writer.WriteBeginTag("div");
			//writer.WriteAttribute("style", "overflow:auto;position:relative;width:100%;height:100%;");
			//writer.Write(SWU.HtmlTextWriter.TagRightChar);

			/*writer.WriteBeginTag("div");
			writer.WriteAttribute("style", "overflow:auto;");
			writer.Write(SWU.HtmlTextWriter.TagRightChar);*/

		}

		public override void RenderEndTag(System.Web.UI.HtmlTextWriter writer)
		{
			//writer.WriteEndTag("div");
			writer.WriteEndTag("div");
			//writer.WriteEndTag("span");
		}




	}
}
