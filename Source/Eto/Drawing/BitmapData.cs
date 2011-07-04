using System;

namespace Eto.Drawing
{
	public abstract class BitmapData
	{
		IntPtr data;
		int scanWidth;
		object controlObject;

		public BitmapData(IntPtr data, int scanWidth, object controlObject)
		{
			this.data = data;
			this.scanWidth = scanWidth;
			this.controlObject = controlObject;
		}

		public abstract uint TranslateArgbToData(uint argb);
		public abstract uint TranslateDataToArgb(uint bitmapData);

		public IntPtr Data
		{
			get { return data; }
		}
		
		public virtual bool Flipped
		{
			get { return false; }
		}

		public int ScanWidth
		{
			get { return scanWidth; }
		}

		public object ControlObject
		{
			get { return controlObject; }
		}

	}
}

