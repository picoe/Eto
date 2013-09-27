#if MOBILE
using System;
namespace Eto.Forms
{
	public partial interface IScrollable
	{
		float MinimumZoom { get; set; }
		float MaximumZoom { get; set; }
		float Zoom { get; set; }
	}
	
	public partial class Scrollable
	{
		
		public float MinimumZoom
		{
			get { return Handler.MinimumZoom; }
			set { Handler.MinimumZoom = value; }
		}

		public float MaximumZoom
		{
			get { return Handler.MaximumZoom; }
			set { Handler.MaximumZoom = value; }
		}
		
		public float Zoom
		{
			get { return Handler.Zoom; }
			set { Handler.Zoom = value; }
		}
	}
}

#endif
