using System;
using System.Globalization;
using System.Linq;
using Eto.Forms;

namespace Eto.Test
{
	public class CultureDropDown : DropDown
	{
		public CultureDropDown()
		{
			ItemTextBinding = Binding.Delegate((CultureInfo c) => c.ThreeLetterISOLanguageName == "IVL" ? "Invariant" : c.Name);
			DataStore = CultureInfo.GetCultures(CultureTypes.AllCultures).OrderBy(r => r.Name);
		}

		public new CultureInfo SelectedValue
		{
			get => base.SelectedValue as CultureInfo;
			set => base.SelectedValue = value;
		}
	}
}
