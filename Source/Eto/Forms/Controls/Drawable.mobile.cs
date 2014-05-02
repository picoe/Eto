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
			: this(null, largeCanvas)
		{
		}

		public Drawable (Generator generator, bool largeCanvas) : base(generator, typeof(IDrawable))
		{
			Handler.Create (largeCanvas);
		}
	}
}
#endif
