
namespace Eto.Forms
{
	public abstract class DynamicItem
	{
		public bool? XScale { get; set; }

		public bool? YScale { get; set; }

		public abstract Control Generate (DynamicLayout layout);

		public virtual void Generate (DynamicLayout layout, TableLayout parent, int x, int y)
		{
			var c = Generate (layout);
			if (c != null)
				parent.Add (c, x, y);
			if (XScale != null)
				parent.SetColumnScale (x, XScale.Value);
			if (YScale != null)
				parent.SetRowScale (y, YScale.Value);
		}
	}
}
