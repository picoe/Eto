namespace Eto.GirCore.Drawing
{
	public class IconFrameHandler : IconFrame.IHandler
	{
		public object Create(IconFrame frame, Stream stream) => new Bitmap(stream);

		public object Create(IconFrame frame, Func<Stream> load) => new Bitmap(load());

		public object Create(IconFrame frame, Bitmap bitmap) => bitmap;

		public Bitmap GetBitmap(IconFrame frame) => (Bitmap)frame.ControlObject;

		public Size GetPixelSize(IconFrame frame) => GetBitmap(frame).Size;
	}
}
