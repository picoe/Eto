using System;
using System.Collections.Generic;
using System.Globalization;
using Eto.Drawing;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Manages transformations in a retained mode graphics system.
	/// </summary>
	public class TransformStack
	{
		readonly Action<IMatrix> push;
		readonly Action pop;
		Stack<IMatrix> stack;

		public IMatrix Current { get; private set; }

		/// <summary>
		/// Initializes a new instance of the TransformStack class
		/// </summary>
		/// <param name="push">A callback that should prepend the specified value to the current matrix </param>
		/// <param name="pop">A callback that should either pop the matrix stack or set the current matrix to the specified value</param>
		public TransformStack(Action<IMatrix> push, Action pop)
		{
			this.push = push;
			this.pop = pop;
		}

		public void TranslateTransform(float dx, float dy)
		{
			Prepend(Matrix.FromTranslation(dx, dy));
		}

		public void RotateTransform(float angle)
		{
			Prepend(Matrix.FromRotation(angle));
		}

		public void ScaleTransform(float sx, float sy)
		{
			Prepend(Matrix.FromScale(sx, sy));
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			Prepend(matrix);
		}

		public void PopAll()
		{
			if (Current != null)
				pop();
		}

		public void PushAll()
		{
			if (Current != null)
			{
				push(Current);
			}
		}

		void Prepend(IMatrix matrix)
		{
			if (Current != null)
			{
				pop();
				Current.Prepend(matrix);
			}
			else
			{
				Current = matrix.Clone();
			}
			push(Current);
		}

		public void SaveTransform()
		{
			if (stack == null)
				stack = new Stack<IMatrix>();

			if (Current != null)
			{
				stack.Push(Current);
				Current = Current.Clone();
			}
			else
				stack.Push(null);
		}

		public void RestoreTransform()
		{
			// If there is a current entry, use it.
			if (stack == null || stack.Count == 0)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "RestoreTransform called without SaveTransform"));

			if (Current != null)
				pop();
			Current = stack.Pop();
			if (Current != null)
				push(Current);
		}
	}
}
