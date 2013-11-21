using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Platform.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IFontFamily"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontFamilyHandler : WidgetHandler<FontFamily>, IFontFamily
    {
		public string Name { get; private set; }

		public IEnumerable<FontTypeface> Typefaces
		{
			get { return new List<FontTypeface>(); } // TODO
		}

		public void Create(string familyName)
		{
			this.Name = familyName;
		}

		public string ID { get; set; }

		public object ControlObject
		{
			get { return Name; }
		}

		public void HandleEvent(string id, bool defaultEvent = false)
		{
		}
	}
}
