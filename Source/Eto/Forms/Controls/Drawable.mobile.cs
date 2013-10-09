#if MOBILE
using System;

namespace Eto.Forms
{
	public partial interface IDrawable
	{
		void Create (bool largeCanvas);
	}

	public partial class Drawable
	{
		public Drawable (bool largeCanvas)
			: this(Generator.Current, largeCanvas)
		{
		}

		public Drawable (Generator g, bool largeCanvas) : base(g, typeof(IDrawable))
		{
			Handler.Create (largeCanvas);
		}
	}
}
#endif
