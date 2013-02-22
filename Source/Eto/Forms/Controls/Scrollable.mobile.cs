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
			get { return handler.MinimumZoom; }
			set { handler.MinimumZoom = value; }
		}

		public float MaximumZoom
		{
			get { return handler.MaximumZoom; }
			set { handler.MaximumZoom = value; }
		}
		
		public float Zoom
		{
			get { return handler.Zoom; }
			set { handler.Zoom = value; }
		}
	}
}

#endif
