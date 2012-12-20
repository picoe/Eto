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
	}

	public static class Matrix
	{
		public static IMatrix Multiply (IMatrix matrix, params IMatrix[] matrices)
		{
			matrix = matrix.Clone ();
			for (int i = 0; i < matrices.Length; i++)
				matrix.Append (matrices [i]);
			return matrix;
		}

		public static IMatrix FromScale (SizeF scale, Generator generator = null)
		{
			return FromScale (scale.Width, scale.Height, generator);
		}
		
		public static IMatrix FromScale (float scaleX, float scaleY, Generator generator = null)
		{
			return Matrix.Create (scaleX, 0, 0, scaleY, 0, 0, generator);
		}
		
		public static IMatrix FromScaleAt (SizeF scale, PointF center, Generator generator = null)
		{
			return FromScaleAt (scale.Width, scale.Height, center.X, center.Y, generator);
		}
		
		public static IMatrix FromScaleAt (float scaleX, float scaleY, float centerX, float centerY, Generator generator = null)
		{
			var matrix = Matrix.Create (generator);
			matrix.ScaleAt (scaleX, scaleY, centerX, centerY);
			return matrix;
		}
		
		public static IMatrix FromTranslation (PointF point, Generator generator = null)
		{
			return FromTranslation (point.X, point.Y, generator);
		}
		
		public static IMatrix FromTranslation (float x, float y, Generator generator = null)
		{
			var matrix = Matrix.Create (generator);
			matrix.Translate (x, y);
			return matrix;
		}
		
		public static IMatrix FromRotation (float angle, Generator generator = null)
		{
			var matrix = Matrix.Create (generator);
			matrix.Rotate (angle);
			return matrix;
		}
		
		public static IMatrix FromRotationAt (float angle, PointF center, Generator generator = null)
		{
			return FromRotationAt (angle, center.X, center.Y, generator);
		}
		
		public static IMatrix FromRotationAt (float angle, float centerX, float centerY, Generator generator = null)
		{
			var matrix = Matrix.Create (generator);
			matrix.RotateAt (angle, centerX, centerY);
			return matrix;
		}

		public static IMatrix FromSkew (float skewX, float skewY, Generator generator = null)
		{
			var matrix = Matrix.Create (generator);
			matrix.Skew (skewX, skewY);
			return matrix;
		}

		public static Func<IMatrix> Instantiator (Generator generator = null)
		{
			var activator = generator.Find<IMatrixHandler> ();
			return () => {
				var matrix = activator ();
				matrix.Create ();
				return matrix;
			};
		}

		public static Func<float, float, float, float, float, float, IMatrix> InstantiatorWithElements (Generator generator = null)
		{
			var activator = generator.Find<IMatrixHandler> ();
			return (xx, yx, xy, yy, x0, y0) => {
				var matrix = activator ();
				matrix.Create (xx, yx, xy, yy, x0, y0);
				return matrix;
			};
		}

		public static IMatrix Create (Generator generator = null)
		{
			var handler = generator.Create<IMatrixHandler> ();
			handler.Create ();
			return handler;
		}

		public static IMatrix Create (float[] elements, Generator generator = null)
		{
			if (elements == null)
				throw new ArgumentNullException ("elements");
			if (elements.Length != 6)
				throw new ArgumentOutOfRangeException ("elements", elements, "Elements must be an array with a length of 6");
			var handler = generator.Create<IMatrixHandler> ();
			handler.Create (elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
			return handler;
		}

		public static IMatrix Create (float xx, float yx, float xy, float yy, float x0, float y0, Generator generator = null)
		{
			var handler = generator.Create<IMatrixHandler> ();
			handler.Create (xx, yx, xy, yy, x0, y0);
			return handler;
		}

		public static void RotateAt (this IMatrix matrix, float angle, PointF center)
		{
			matrix.RotateAt (angle, center.X, center.Y);
		}

		public static void Translate (this IMatrix matrix, PointF translate)
		{
			matrix.Translate (translate.X, translate.Y);
		}
		
		public static void Scale (this IMatrix matrix, SizeF scale)
		{
			matrix.Scale (scale.Width, scale.Height);
		}
		
		public static void Scale (this IMatrix matrix, float scale)
		{
			matrix.Scale (scale, scale);
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

		public static void Append (this IMatrix matrix, params IMatrix[] matrices)
		{
			for (int i = 0; i < matrices.Length; i++)
				matrix.Append (matrices [i]);
		}

		public static void Prepend (this IMatrix matrix, params IMatrix[] matrices)
		{
			for (int i = 0; i < matrices.Length; i++)
				matrix.Prepend (matrices [i]);
		}
	}
}
