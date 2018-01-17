using System;
using Eto.Drawing;
using sd = SharpDX.Direct2D1;

namespace Eto.Direct2D.Drawing
{
	public abstract class BrushData : IDisposable
	{
		public float Alpha { get; set; }

		protected BrushData()
		{
			Alpha = 1f;
		}

		public void Reset()
		{
			if (Brush != null)
				Brush.Dispose();
			Brush = null;
		}

		protected sd.Brush Brush { get; private set; }

		public sd.Brush Get(sd.RenderTarget target)
		{
			if (Brush == null || !ReferenceEquals(Brush.Tag, target))
			{
				Brush = Create(target);
				Brush.Opacity = Alpha;
				Brush.Tag = target;
				OnCreated();
			}
			return Brush;
		}

		protected abstract sd.Brush Create(sd.RenderTarget target);

		protected virtual void OnCreated()
		{
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && Brush != null)
				Brush.Dispose();
		}
	}

	public abstract class TransformBrushData : BrushData
	{
		IMatrix transform;

		public IMatrix Transform
		{
			get { return transform; }
			set
			{
				transform = value;
				if (Brush != null)
					Brush.Transform = transform.ToDx();
			}
		}

		protected override void OnCreated()
		{
			base.OnCreated();
			Brush.Transform = transform.ToDx();
		}
	}
}
