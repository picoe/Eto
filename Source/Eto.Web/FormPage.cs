using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Eto.Web
{
	public class FormPage : BasePage
	{
		WebControl html;
		HtmlHead head;
		WebControl body;
		HtmlForm form;
		
		public override ControlCollection Controls
		{
			get { return form.Controls; }
		}
		
		public FormPage()
		{
			html = new WebControl(HtmlTextWriterTag.Html);
			head = new HtmlHead();
			head.Controls.Add(new HtmlTitle());
			
			body = new WebControl(HtmlTextWriterTag.Body);
			
			form = new CustomHtmlForm();
			form.ID = "form";
		}
		
		private class CustomHtmlForm : HtmlForm
		{
			protected override void RenderAttributes(HtmlTextWriter writer)
			{
				if (base.ID != null)
					writer.WriteAttribute("id", base.ClientID);

				writer.WriteAttribute("method", this.Method);
				base.Attributes.Remove("method");

				writer.WriteAttribute("action", HttpContext.Current.Request.Url.AbsolutePath);
				
				this.Attributes.Render(writer);
			}

			protected override void RenderBeginTag(HtmlTextWriter writer)
			{
				base.RenderBeginTag(writer);
				writer.WriteFullBeginTag("div");
			}
			
			protected override void RenderEndTag(HtmlTextWriter writer)
			{
				writer.WriteEndTag("div");
				base.RenderEndTag(writer);
			}
			
			
		}

		protected override void OnLoad(EventArgs e)
		{
			// html 4.01 strict
			//base.Controls.Add(new LiteralControl("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">"));
			// xhtml 1.0 strict
			base.Controls.Add(new LiteralControl("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\n"));
			// xhtml 1.1
			//base.Controls.Add(new LiteralControl("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">\n"));

			body.Controls.Add(form);
			
			html.Controls.Add(head);
			html.Controls.Add(body);
			
			base.Controls.Add(html);

			base.OnLoad(e);
		}
		
		
	}
}
