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
        public float[] Elements
        {
            get { throw new NotImplementedException(); }
        }

        public float OffsetX
        {
            get { throw new NotImplementedException(); }
        }

        public float OffsetY
        {
            get { throw new NotImplementedException(); }
        }

        public void Rotate(float angle)
        {
            throw new NotImplementedException();
        }

        public void Translate(float x, float y)
        {
            throw new NotImplementedException();
        }

        public void Scale(float sx, float sy)
        {
            throw new NotImplementedException();
        }

        public void Multiply(Matrix m, MatrixOrder matrixOrder)
        {
            throw new NotImplementedException();
        }

        public void Create(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            throw new NotImplementedException();
        }

        public void Invert()
        {
            throw new NotImplementedException();
        }

        public PointF TransformPoint(Point p)
        {
            throw new NotImplementedException();
        }

        public PointF TransformPoint(PointF p)
        {
            throw new NotImplementedException();
        }
    }
}
