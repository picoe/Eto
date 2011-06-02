using System;
using System.Collections;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class SeparatorToolBarItemHandler : WidgetHandler<SWF.ToolStripSeparator, SeparatorToolBarItem>, ISeparatorToolBarItem, IToolBarItemHandler
	{
		SeparatorToolBarItemType type;
		public SeparatorToolBarItemHandler()
		{
			Control = new SWF.ToolStripSeparator();
		}
		
		public string ID { get; set; }
	
		public SeparatorToolBarItemType Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public void CreateControl (ToolBarHandler handler)
		{
			
		}

	}
}
