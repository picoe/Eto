using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
    public class MatrixHandler : WidgetHandler<swm.Matrix, Matrix>, IMatrix
    {
        public MatrixHandler()
        {
            this.Control = swm.Matrix.Identity;
        }

        public MatrixHandler(swm.Matrix m)
        {
            this.Control = m;
        }

        public float[] Elements
        {
            get
            {
                return new float[] {
                (float)Control.M11, 
                (float)Control.M12, 
                (float)Control.M21, 
                (float)Control.M22, 
                (float)Control.OffsetX, 
                (float)Control.OffsetY};
            }
        }

        public float OffsetX
        {
            get { return (float)Control.OffsetX; }
        }

        public float OffsetY
        {
            get { return (float)Control.OffsetY; }
        }

        public void Rotate(float angle)
        {
            Control.Rotate(angle);
        }

        public void Translate(float x, float y)
        {
            Control.Translate(x, y);
        }

        public void Scale(float sx, float sy)
        {
            Control.Scale(sx, sy);
        }

        public void Multiply(Matrix m, MatrixOrder matrixOrder)
        {
            var m2 = (swm.Matrix) m.ControlObject;

            if (matrixOrder == MatrixOrder.Prepend)
                Control.Prepend(m2);
            else
                Control.Append(m2);
        }

        public void Create(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            this.Control =
                new swm.Matrix(m11, m12, m21, m22, dx, dy);
        }

        public void Invert()
        {
            this.Control.Invert();
        }

        public PointF TransformPoint(Point p)
        {
            return Control.Transform(p.ToWpf()).ToEto();
        }

        public PointF TransformPoint(PointF p)
        {
            return Control.Transform(p.ToWpf()).ToEto();
        }
    }
}
