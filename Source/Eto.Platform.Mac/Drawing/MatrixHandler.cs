using System;
using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using SD = System.Drawing;

namespace Eto.Platform.Mac.Drawing
{
    public class MatrixHandler : WidgetHandler<CGAffineTransform, Matrix>, IMatrix
    {
        public MatrixHandler()
        {
            this.Control = 
                CGAffineTransform.MakeIdentity();
        }

        public MatrixHandler(CGAffineTransform matrix)
        {
            this.Control = matrix;
        }

        public float[] Elements
        {
            get
            {
                return new float[] 
                { 
                    Control.xx,
                    Control.yx,
                    Control.xy,
                    Control.yy,
                    Control.x0,
                    Control.y0,
                };
            }
        }

        public float OffsetX
        {
            get
            {
                return Control.x0;
            }
        }

        public float OffsetY
        {
            get
            {
                return Control.y0;
            }
        }

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
            var a1 =
                this.Control;

            var a2 =
                (CGAffineTransform)m.ControlObject;

            if (matrixOrder == MatrixOrder.Append)
            {                
                a1.Multiply(a2);
                this.Control = a1;
            }
            else
            {
                a2.Multiply(a1);
                this.Control = a2;
            }
        }

        public void Create(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            this.Control =
                new CGAffineTransform(m11, m12, m21, m22, dx, dy);
        }


        public void Invert()
        {
            this.Control = this.Control.Invert();
        }

        public PointF TransformPoint(Point p)
        {
            return
                Generator.ConvertF(
                    this.Control.TransformPoint(
                        new SD.PointF(p.X, p.Y)));
        }

        public PointF TransformPoint(PointF p)
        {
            return
                Generator.Convert(
                    this.Control.TransformPoint(
                        new SD.PointF(p.X, p.Y)));
        }
    }
}
