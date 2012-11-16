using System;
using System.Collections.Generic;
using System.Text;
using Eto.Drawing;
using SD = System.Drawing;
using SD2D = System.Drawing.Drawing2D;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public class MatrixHandler : WidgetHandler<SD2D.Matrix, Matrix>, IMatrix
	{
        public MatrixHandler()
        {
            this.Control = new SD2D.Matrix();
        }

        public MatrixHandler(SD2D.Matrix matrix)
        {
            this.Control = matrix;
        }

        #region IMatrix Members

        public float[] Elements
        {
            get { return Control.Elements; }
        }

        public float OffsetX
        {
            get
            {
                return Control.OffsetX;
            }
        }

        public float OffsetY
        {
            get
            {
                return Control.OffsetY;
            }
        }

        #endregion

        #region IMatrix Members


        public void Rotate(float angle)
        {
            this.Control.Rotate(angle);
        }

        public void Translate(float x, float y)
        {
            this.Control.Translate(x, y);
        }

        public void Scale(float sx, float sy)
        {
            this.Control.Scale(sx, sy);
        }

        public void Multiply(Matrix m, MatrixOrder matrixOrder)
        {
            this.Control.Multiply(
                Generator.Convert(m),
                (SD2D.MatrixOrder)matrixOrder);
        }

        #endregion

        #region IMatrix Members


        public void Create(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            this.Control =
                new SD2D.Matrix(m11, m12, m21, m22, dx, dy);
        }

        #endregion


        public void Invert()
        {
            this.Control.Invert();
        }

        public PointF TransformPoint(Point p)
        {
            var px = new SD.Point[] {Generator.Convert(p)};
            
            this.Control.TransformPoints(px);

            return Generator.Convert(px[0]);
        }

        public PointF TransformPoint(PointF p)
        {
            var px = new SD.PointF[] { Generator.Convert(p) };

            this.Control.TransformPoints(px);

            return Generator.Convert(px[0]);
        }
    }
}
