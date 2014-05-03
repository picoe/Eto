using System;

namespace Eto.Forms
{
	[AutoInitialize(false)]
	public partial interface IDrawable
	{
		void Create (bool largeCanvas);
	}

	public partial class Drawable
	{
		public Drawable(bool largeCanvas)
		{
			Handler.Create(largeCanvas);
			Initialize();
		}

		#pragma warning disable 612,618

		[Obsolete("Use Drawable(bool) instead")]
		public Drawable (Generator generator, bool largeCanvas) : base(generator, typeof(IDrawable))
		{
			Handler.Create (largeCanvas);
		}

		#pragma warning restore 612,618
	}
}