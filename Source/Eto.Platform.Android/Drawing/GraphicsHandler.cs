using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Platform.Android.Drawing
{
	public class GraphicsHandler : WidgetHandler<ag.Canvas, Graphics>, IGraphics
	{
		public GraphicsHandler()
		{
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
			throw new NotImplementedException();
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

		public bool Antialias
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
			get { return false; }
		}

		public void TranslateTransform(float offsetX, float offsetY)
		{
			Control.Translate(offsetX, offsetY);
		}

		public void RotateTransform(float angle)
		{
			throw new NotImplementedException();
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			Control.Scale(scaleX, scaleY);
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
	}
}