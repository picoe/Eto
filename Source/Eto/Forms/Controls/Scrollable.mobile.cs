using System;
namespace Eto.Forms
{
	public partial interface IScrollable
	{
		float MinimumZoom { get; set; }
		float MaximumZoom { get; set; }
	}
	
	public partial class Scrollable
	{
		
		public float MinimumZoom
		{
			get { return inner.MinimumZoom; }
			set { inner.MinimumZoom = value; }
		}

		public float MaximumZoom
		{
			get { return inner.MaximumZoom; }
			set { inner.MaximumZoom = value; }
		}
	}
}

