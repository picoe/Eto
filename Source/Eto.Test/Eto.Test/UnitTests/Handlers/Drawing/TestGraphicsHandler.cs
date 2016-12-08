using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.UnitTests.Handlers.Drawing
{
	/// <summary>
	/// A mock IGraphics implementation.
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class TestGraphicsHandler : TestWidgetHandler, Graphics.IHandler
	{
		Drawable drawable;
		IMatrix transform = Matrix.Create();
		Stack<IMatrix> transforms = new Stack<IMatrix>();

		public float PointsPerPixel
		{
			get { throw new NotImplementedException(); }
		}

		public PixelOffsetMode PixelOffsetMode
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public TestGraphicsHandler()
		{
		}

		public TestGraphicsHandler(Drawable drawable)
		{
			this.drawable = drawable;
			ResetClip();
		}

		public void CreateFromImage(Bitmap image)
		{

		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			throw new NotImplementedException();
		}

		public void DrawLines(Pen pen, IEnumerable<PointF> points)
		{
			throw new NotImplementedException();
		}

		public void DrawPolygon(Pen pen, IEnumerable<PointF> points)
		{
			throw new NotImplementedException();
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			throw new NotImplementedException();
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			throw new NotImplementedException();
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			throw new NotImplementedException();
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			throw new NotImplementedException();
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw new NotImplementedException();
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw new NotImplementedException();
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			throw new NotImplementedException();
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			throw new NotImplementedException();
		}

		public void DrawImage(Image image, float x, float y)
		{
			throw new NotImplementedException();
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			throw new NotImplementedException();
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			throw new NotImplementedException();
		}

		public void DrawText(Font font, SolidBrush brush, float x, float y, string text)
		{
			throw new NotImplementedException();
		}

		public SizeF MeasureString(Font font, string text)
		{
			// A fixed-width implementation that returns 10 * the length of the string.
			return new SizeF(text.Length * 10f, font.LineHeight);
		}

		public void Flush()
		{
			throw new NotImplementedException();
		}

		public bool AntiAlias
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public ImageInterpolation ImageInterpolation
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool IsRetained
		{
			get { throw new NotImplementedException(); }
		}

		public void TranslateTransform(float offsetX, float offsetY)
		{
			transform.Translate(offsetX, offsetY);
		}

		public void RotateTransform(float angle)
		{
			transform.Rotate(angle);
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			transform.Scale(scaleX, scaleY);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			transform.Append(matrix);
		}

		public void SaveTransform()
		{
			transforms.Push(transform);
		}

		public void RestoreTransform()
		{
			transform = transforms.Pop();
		}

		public IMatrix CurrentTransform
		{
			get { return transform; }
		}

		RectangleF clipBounds;
		public RectangleF ClipBounds
		{
			get
			{
				var t = transform.Clone();
				t.Invert();
				return t.TransformRectangle(clipBounds);
			}
		}

		public void SetClip(RectangleF rectangle)
		{
			clipBounds = rectangle;
		}

		public void SetClip(IGraphicsPath path)
		{
			throw new NotImplementedException();
		}

		public void ResetClip()
		{
			if (drawable != null)
				clipBounds = new RectangleF(drawable.Size);
		}

		public void Clear(SolidBrush brush)
		{
			throw new NotImplementedException();
		}
	}
}
