using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Eto.Drawing
{
	public interface IMatrix
	{
		float[] Elements { get; }

		float Xx { get; set; }

		float Xy { get; set; }

		float Yx { get; set; }

		float Yy { get; set; }

		float X0 { get; set; }

		float Y0 { get; set; }

		void Rotate (float angle);

		void RotateAt (float angle, float centerX, float centerY);

		void Translate (float x, float y);

		void Scale (float scaleX, float scaleY);

		void ScaleAt (float scaleX, float scaleY, float centerX, float centerY);

		void Skew (float skewX, float skewY);

		void Append (IMatrix matrix);

		void Prepend (IMatrix matrix);

		void Invert ();

		PointF TransformPoint (Point point);

		PointF TransformPoint (PointF point);

		object ControlObject { get; }

		IMatrix Clone ();
	}

	public interface IMatrixHandler : IMatrix
	{
		void Create ();

		void Create (float xx, float yx, float xy, float yy, float x0, float y0);

		IMatrixHandler CloneHandler ();
	}

	public static class MatrixExtensions
	{
		public static void Translate (this IMatrix matrix, PointF translate)
		{
			matrix.Scale (translate.X, translate.Y);
		}

		public static void Scale (this IMatrix matrix, SizeF scale)
		{
			matrix.Scale (scale.Width, scale.Height);
		}
		
		public static void Scale (this IMatrix matrix, float scale)
		{
			matrix.Scale (scale, scale);
		}

		public static void RotateAt (this IMatrix matrix, float angle, PointF center)
		{
			matrix.RotateAt (angle, center.X, center.Y);
		}

		public static void ScaleAt (this IMatrix matrix, SizeF scale, PointF center)
		{
			matrix.ScaleAt (scale.Width, scale.Height, center.X, center.Y);
		}
		
		public static void ScaleAt (this IMatrix matrix, float scale, PointF center)
		{
			matrix.ScaleAt (scale, scale, center.X, center.Y);
		}
		
		public static void ScaleAt (this IMatrix matrix, float scale, float centerX, float centerY)
		{
			matrix.ScaleAt (scale, scale, centerX, centerY);
		}

		public static Matrix ToMatrix (this IMatrix matrix)
		{
			if (matrix is IMatrixHandler)
				return new Matrix((IMatrixHandler)matrix);
			else
				return (Matrix)matrix;
		}
	}

	public class Matrix : IMatrix, IEquatable<Matrix>, ICloneable
	{
		IMatrixHandler Handler { get; set; }

		public object ControlObject
		{
			get { return Handler.ControlObject; }
		}

		public static Matrix Multiply (Matrix matrix, params Matrix[] matrices)
		{
			var m = matrix.Clone ();
			for (int i = 0; i < matrices.Length; i++)
				m.Append (matrices [i]);
			return m;
		}

		public static Matrix FromScale (SizeF scale)
		{
			return FromScale (scale.Width, scale.Height);
		}
		
		public static Matrix FromScale (float scaleX, float scaleY)
		{
			var result = new Matrix ();
			result.Scale (scaleX, scaleY);
			return result;
		}
		
		public static Matrix FromScaleAt (SizeF scale, PointF center)
		{
			return FromScaleAt (scale.Width, scale.Height, center.X, center.Y);
		}
		
		public static Matrix FromScaleAt (float scaleX, float scaleY, float centerX, float centerY)
		{
			var result = new Matrix ();
			result.ScaleAt (scaleX, scaleY, centerX, centerY);
			return result;
		}
		
		public static Matrix FromTranslation (PointF point)
		{
			return FromTranslation (point.X, point.Y);
		}
		
		public static Matrix FromTranslation (float x, float y)
		{
			var result = new Matrix ();
			result.Translate (x, y);
			return result;
		}
		
		public static Matrix FromRotation (float angle)
		{
			var result = new Matrix ();
			result.Rotate (angle);
			return result;
		}
		
		public static Matrix FromRotationAt (float angle, PointF center)
		{
			return FromRotationAt (angle, center.X, center.Y);
		}
		
		public static Matrix FromRotationAt (float angle, float centerX, float centerY)
		{
			var result = new Matrix ();
			result.RotateAt (angle, centerX, centerY);
			return result;
		}

		public static Matrix FromSkew (float skewX, float skewY)
		{
			var result = new Matrix ();
			result.Skew (skewX, skewY);
			return result;
		}

		public Matrix (IMatrixHandler handler)
		{
			this.Handler = handler;
		}

		public Matrix (Generator generator = null)
		{
			generator = generator ?? Generator.Current;
			this.Handler = generator.CreateHandler<IMatrixHandler> ();
			this.Handler.Create ();
		}

		public Matrix (float xx, float yx, float xy, float yy, float x0, float y0, Generator generator = null)
		{
			generator = generator ?? Generator.Current;
			this.Handler = generator.CreateHandler<IMatrixHandler> ();
			this.Handler.Create (xx, yx, xy, yy, x0, y0);
		}

		public Matrix (float[] elements, Generator generator = null)
		{
			if (elements == null)
				throw new ArgumentNullException ("elements");
			if (elements.Length != 6)
				throw new ArgumentOutOfRangeException ("elements", elements, "Elements must have exactly 6 components");
			this.Handler = generator.CreateHandler<IMatrixHandler> ();
			this.Handler.Create (elements [0], elements [1], elements [2], elements [3], elements [4], elements [5]);
		}

		public static Func<Matrix> MatrixInstantiator (Generator generator = null)
		{
			var activator = (generator ?? Generator.Current).Find<IMatrixHandler> ();
			return () => {
				var matrix = (IMatrixHandler)activator ();
				matrix.Create ();
				return new Matrix(matrix);
			};
		}

		public static Func<IMatrix> Instantiator (Generator generator = null)
		{
			var activator = (generator ?? Generator.Current).Find<IMatrixHandler> ();
			return () => {
				var matrix = (IMatrixHandler)activator ();
				matrix.Create ();
				return matrix;
			};
		}

		public static Func<float[], IMatrix> ActivatorWithElements (Generator generator = null)
		{
			var activator = (generator ?? Generator.Current).Find<IMatrixHandler> ();
			return (e) => {
				var matrix = (IMatrixHandler)activator ();
				if (e == null)
					throw new ArgumentNullException ("elements");
				if (e.Length != 6)
					throw new ArgumentOutOfRangeException ("elements", e, "Elements must be an array with a length of 6 floats");
				matrix.Create (e[0], e[1], e[2], e[3], e[4], e[5]);
				return matrix;
			};
		}

		public static Func<float, float, float, float, float, float, IMatrix> ActivatorWithComponents (Generator generator = null)
		{
			var activator = (generator ?? Generator.Current).Find<IMatrixHandler> ();
			return (xx, xy, yx, yy, x0, y0) => {
				var matrix = (IMatrixHandler)activator ();
				matrix.Create (xx, xy, yx, yy, x0, y0);
				return matrix;
			};
		}

		/*
		public static IMatrix Create (Generator generator = null)
		{
			generator = generator ?? Generator.Current;
			var handler = generator.CreateHandler<IMatrixHandler> ();
			handler.Create ();
			return handler;
		}

		public static IMatrix Create (float[] elements, Generator generator = null)
		{
			if (elements == null)
				throw new ArgumentNullException ("elements");
			if (elements.Length != 6)
				throw new ArgumentOutOfRangeException ("elements", elements, "Elements must be an array with a length of 6 floats");
			generator = generator ?? Generator.Current;
			var handler = generator.CreateHandler<IMatrixHandler> ();
			handler.Create (elements [0], elements [1], elements [2], elements [3], elements [4], elements [5]);
			return handler;
		}

		public static IMatrix Create (float xx, float yx, float xy, float yy, float x0, float y0, Generator generator = null)
		{
			generator = generator ?? Generator.Current;
			var handler = generator.CreateHandler<IMatrixHandler> ();
			handler.Create (xx, yx, xy, yy, x0, y0);
			return handler;
		}
		 */

		public float Xx { get { return Handler.Xx; } set { Handler.Xx = value; } }

		public float Xy { get { return Handler.Xy; } set { Handler.Xy = value; } }

		public float Yx { get { return Handler.Yx; } set { Handler.Yx = value; } }

		public float Yy { get { return Handler.Yy; } set { Handler.Yy = value; } }

		public float X0 { get { return Handler.X0; } set { Handler.X0 = value; } }

		public float Y0 { get { return Handler.Y0; } set { Handler.Y0 = value; } }

		public float[] Elements { get { return Handler.Elements; } }

		public void Rotate (float angle)
		{
			Handler.Rotate (angle);
		}

		public void RotateAt (float angle, PointF center)
		{
			Handler.RotateAt (angle, center.X, center.Y);
		}

		public void RotateAt (float angle, float centerX, float centerY)
		{
			Handler.RotateAt (angle, centerX, centerY);
		}

		public void Translate (PointF translate)
		{
			Handler.Translate (translate.X, translate.Y);
		}

		public void Translate (float x, float y)
		{
			Handler.Translate (x, y);
		}

		public void Scale (SizeF scale)
		{
			Handler.Scale (scale.Width, scale.Height);
		}

		public void Scale (float scaleX, float scaleY)
		{
			Handler.Scale (scaleX, scaleY);
		}

		public void Scale (float scale)
		{
			Handler.Scale (scale, scale);
		}

		public void ScaleAt (SizeF scale, PointF center)
		{
			Handler.ScaleAt (scale.Width, scale.Height, center.X, center.Y);
		}

		public void ScaleAt (float scaleX, float scaleY, float centerX, float centerY)
		{
			Handler.ScaleAt (scaleX, scaleY, centerX, centerY);
		}

		public void ScaleAt (float scale, float centerX, float centerY)
		{
			Handler.ScaleAt (scale, scale, centerX, centerY);
		}

		public void Skew (float skewX, float skewY)
		{
			Handler.Skew (skewX, skewY);
		}

		public void Append (IMatrix matrix)
		{
			if (matrix == null)
				throw new ArgumentNullException ("matrix");
			Handler.Append (matrix);
		}

		public void Prepend (IMatrix matrix)
		{
			if (matrix == null)
				throw new ArgumentNullException ("matrix");
			Handler.Prepend (matrix);
		}

		public void Invert ()
		{
			Handler.Invert ();
		}

		public void Append (params IMatrix[] matrices)
		{
			for (int i = 0; i < matrices.Length; i++)
				Append (matrices [i]);
		}

		public void Prepend (params IMatrix[] matrices)
		{
			for (int i = 0; i < matrices.Length; i++)
				Prepend (matrices [i]);
		}

		public PointF TransformPoint (Point point)
		{
			return Handler.TransformPoint (point);
		}

		public PointF TransformPoint (PointF point)
		{
			return Handler.TransformPoint (point);
		}

		public override string ToString ()
		{
			return string.Format ("{0} {1} {2} {3} {4} {5}", Xx, Xy, Yx, Yy, X0, Y0);
		}

		public static Matrix operator * (Matrix matrix1, Matrix matrix2)
		{
			var m = matrix1.Clone ();
			m.Append (matrix2);
			return m;
		}

		public static Matrix operator * (Matrix matrix, SizeF scale)
		{
			var m = matrix.Clone ();
			m.Scale (scale);
			return m;
		}

		public static Matrix operator / (Matrix matrix, SizeF scale)
		{
			var m = matrix.Clone ();
			m.Scale (1 / scale.Width, 1 / scale.Height);
			return m;
		}

		public static Matrix operator + (Matrix matrix, PointF point)
		{
			var m = matrix.Clone ();
			m.Translate (point);
			return m;
		}

		public static Matrix operator - (Matrix matrix, PointF point)
		{
			var m = matrix.Clone ();
			m.Translate (-point);
			return m;
		}

		public static bool operator != (Matrix matrix1, Matrix matrix2)
		{
			return !(matrix1 == matrix2);
		}

		public static bool operator == (Matrix matrix1, Matrix matrix2)
		{
			return matrix1.Xx == matrix2.Xx
				&& matrix1.Xy == matrix2.Xy
				&& matrix1.Yx == matrix2.Yx
				&& matrix1.Yy == matrix2.Yy
				&& matrix1.X0 == matrix2.X0
				&& matrix1.Y0 == matrix2.Y0;
		}

		public override bool Equals (object obj)
		{
			return (obj is Matrix && (Matrix)obj == this);
		}

		public override int GetHashCode ()
		{
			return Xx.GetHashCode () ^ Xy.GetHashCode () ^ Yx.GetHashCode () ^ Yy.GetHashCode () ^ X0.GetHashCode () ^ Y0.GetHashCode ();
		}

		public bool Equals (Matrix other)
		{
			return other == this;
		}

		public Matrix Clone ()
		{
			return new Matrix (Handler.CloneHandler ());
		}

		IMatrix IMatrix.Clone ()
		{
			return this.Clone ();
		}

		object ICloneable.Clone ()
		{
			return this.Clone ();
		}
	}
}
