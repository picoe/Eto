namespace Eto.Android.Forms
{
	internal class ScreenHandler : WidgetHandler<Object, Screen>, Screen.IHandler
	{
		public ScreenHandler(float scale, float density, RectangleF bounds, RectangleF workingArea, int bitsPerPixel, bool isPrimary)
		{
			Bounds = bounds;
			Scale = scale;
			RealScale = scale * density;
			WorkingArea = workingArea;
			BitsPerPixel = bitsPerPixel;
			IsPrimary = isPrimary;
		}

		public float RealScale
		{
			private set;
			get;
		}

		public float Scale
		{
			private set;
			get;
		}

		public RectangleF Bounds
		{
			get;
			private set;
		}

		public RectangleF WorkingArea
		{
			get;
			private set;
		}

		public int BitsPerPixel
		{
			get;
			private set;
		}

		public bool IsPrimary
		{
			get;
			private set;
		}

		public Image GetImage(RectangleF rect)
		{
			throw new NotImplementedException();
		}
	}
}
