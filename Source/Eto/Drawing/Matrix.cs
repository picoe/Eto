using System;
using System.IO;
using System.Reflection;

namespace Eto.Drawing
{
    public interface IMatrix : IInstanceWidget
	{
        float[] Elements { get; }

        float OffsetX { get; }

        float OffsetY { get; }

        void Rotate(float angle);

        void Translate(float x, float y);

        void Scale(float sx, float sy);

        void Multiply(Matrix m, MatrixOrder matrixOrder);

        void Create(float m11, float m12, float m21, float m22, float dx, float dy);

        void Invert();

        PointF TransformPoint(Point p);

        PointF TransformPoint(PointF p);
    }

    public class Matrix : InstanceWidget
	{
		IMatrix inner;
		
        public Matrix(): this(Generator.Current)
        {
        }

        public Matrix(Generator g, IMatrix inner) : base(g, inner)
        {
            this.inner = inner;
        }

        public Matrix(Generator g)
            : base(g, typeof(IMatrix))
        {
            inner = (IMatrix)this.Handler;
        }

        public Matrix(float m11,
	                  float m12,
	                  float m21,
	                  float m22,
	                  float dx,
	                  float dy)
            :this(Generator.Current)
        {
            inner.Create(m11, m12, m21, m22, dx, dy);
        }

        public float OffsetX { 
            get { return inner.OffsetX; }
        }

        public float OffsetY
        {
            get { return inner.OffsetY; }
        }

        public float[] Elements { get { return inner.Elements; } }

        public void Rotate(float angle)
        {
            inner.Rotate(angle);
        }

        public void Translate(PointF p)
        {
            inner.Translate(p.X, p.Y);
        }

        public void Translate(float x, float y)
        {
            inner.Translate(x, y);
        }

        public void Scale(float sx, float sy)
        {
            inner.Scale(sx, sy);
        }

        public void Multiply(Matrix m, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            inner.Multiply(m, matrixOrder);
        }

        public void Invert()
        {
            inner.Invert();
        }

        public Matrix Append(params Matrix[] matrices)
        {
            foreach (
                var m 
                in matrices)
                Multiply(m, MatrixOrder.Append);

            return this; // to allow chaining
        }

        public PointF TransformPoint(Point p)
        {
            return inner.TransformPoint(p);
        }

        public PointF TransformPoint(PointF p)
        {
            return inner.TransformPoint(p);
        }

        public static Matrix FromScale(float sx, float sy)
        {
            var result = new Matrix();

            result.Scale(sx, sy);

            return result;
        }


        public static Matrix FromTranslation(PointF p)
        {
            var result = new Matrix();

            result.Translate(p);

            return result;
        }


        public override string ToString()
        {
            var e =
                this.Elements;


            return 
                e != null &&
                e.Length == 6
                ? string.Format(
                    "{{ {0} {1} {2} {3} {4} {5} }}",
                    e[0],
                    e[1],
                    e[2],
                    e[3],
                    e[4],
                    e[5])
                : "empty matrix";
        }
    }

    // Summary:
    //     Specifies the order for matrix transform operations.
    public enum MatrixOrder
    {
        // Summary:
        //     The new operation is applied before the old operation.
        Prepend = 0,
        //
        // Summary:
        //     The new operation is applied after the old operation.
        Append = 1,
    }
}
