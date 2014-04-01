using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.UnitTest.Handlers
{
	/// <summary>
	/// A mock IGraphics implementation.
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	/// </summary>
	class TestGraphicsHandler : IGraphics
	{
		public Eto.Generator Generator { get; set; }
		public Widget Widget { get; set; }
		public Size Size { get; set; }
		public string ID { get; set; }
		public object ControlObject { get; set; }


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

		public void CreateFromImage(Bitmap image)
		{
			
		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public void RotateTransform(float angle)
		{
			throw new NotImplementedException();
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			throw new NotImplementedException();
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			throw new NotImplementedException();
		}

		public void SaveTransform()
		{
			throw new NotImplementedException();
		}

		public void RestoreTransform()
		{
			throw new NotImplementedException();
		}

		public RectangleF ClipBounds
		{
			get { throw new NotImplementedException(); }
		}

		public void SetClip(RectangleF rectangle)
		{
			throw new NotImplementedException();
		}

		public void SetClip(IGraphicsPath path)
		{
			throw new NotImplementedException();
		}

		public void ResetClip()
		{
			throw new NotImplementedException();
		}

		public void Clear(SolidBrush brush)
		{
			throw new NotImplementedException();
		}

		public void HandleEvent(string id, bool defaultEvent = false)
		{
			throw new NotImplementedException();
		}

		public void Initialize()
		{
		}
	}
}
