using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public class SplitterHandler : WebControl, ISplitter
	{
		SWU.Control control;
		private Control panel1 = null;
		private Control panel2 = null;
		
		public SplitterHandler(Widget widget) : base(widget)
		{
			control = new SWUC.PlaceHolder();
		}
		
		public override object ControlObject
		{
			get { return control; }
		}
		
		public int Position
		{
			get { return 0; }
			set {  }
		}
		
		public SplitterOrientation Orientation
		{
			get
			{
				return SplitterOrientation.Horizontal;
			}
			set
			{
			}
		}
		
		public Control Panel1
		{
			get
			{ return panel1; }
			set
			{
				panel1 = value;
				if (panel1 != null)
				{
					
					((SWUC.WebControl)panel1.ControlObject).Style["WIDTH"] = "100%";
					((SWUC.WebControl)panel1.ControlObject).Style["HEIGHT"] = "100%";
				}
			}
		}
		
		public Control Panel2
		{
			get
			{ return panel2; }
			set
			{
				panel2 = value;
				if (panel2 != null)
				{
					((SWUC.WebControl)panel2.ControlObject).Style["WIDTH"] = "100%";
					((SWUC.WebControl)panel2.ControlObject).Style["HEIGHT"] = "100%";
				}
			}
		}
	}
}

