using System.ComponentModel;
using System;

namespace Eto.Forms
{
	[TypeConverter(typeof(DynamicItemConverter))]
	public abstract class DynamicItem
	{
		public bool? XScale { get; set; }

		public bool? YScale { get; set; }

		[Obsolete("Use Create() instead")]
		public Control Generate(DynamicLayout layout)
		{
			return Create(layout);
		}

		[Obsolete("Use Create() instead")]
		public void Generate(DynamicLayout layout, TableLayout parent, int x, int y)
		{
			Create(layout, parent, x, y);
		}

		public abstract Control Create(DynamicLayout layout);

		public virtual void Create(DynamicLayout layout, TableLayout parent, int x, int y)
		{
			var c = Create(layout);
			if (c != null)
				parent.Add(c, x, y);
			if (XScale != null)
				parent.SetColumnScale(x, XScale.Value);
			if (YScale != null)
				parent.SetRowScale(y, YScale.Value);
		}

		public static implicit operator DynamicItem(Control control)
		{
			return new DynamicControl { Control = control };
		}
	}
}
