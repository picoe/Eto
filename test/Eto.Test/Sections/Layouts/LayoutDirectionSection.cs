using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eto.Test.Sections.Layouts
{
	[Section("Layouts", "LayoutDirection")]
    public class LayoutDirectionSection : Panel
    {
        public LayoutDirectionSection()
		{
			var dd = new EnumDropDown<LayoutDirection>();
			dd.Bind(c => c.SelectedValue, Application.Instance, m => m.DefaultLayoutDirection);

			Content = new TableLayout
			{
				Rows = {
					"Application.DefaultLayoutDirection:", dd
				}
			};
			
		}
    }
}