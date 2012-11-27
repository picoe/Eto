using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
    /// <summary>
    /// Manages transformations in a retained
    /// mode graphics system.
    /// </summary>
    public class TransformStack
    {
        Matrix current;
        public Matrix Current
        {
            get 
            {
                if (current == null)
                {
                    current =
                        new Matrix(
                            this.generator);
                    
                    if (current.Handler == null)
                        throw new EtoException();
                }

                if (current.Handler == null)
                    throw new EtoException();

                return current; 
            }
        }

        Eto.Generator generator;
        Action<Matrix> push;
        Action<Matrix> pop;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="push">A callback that should prepend the specified value to the current matrix </param>
        /// <param name="pop">A callback that should either pop the matrix stack or set the current matrix to the specified value</param>
        public TransformStack(
            Eto.Generator generator,
            Action<Matrix> push,
            Action<Matrix> pop)
        {
            this.generator = generator;
            this.push = push;
            this.pop = pop;
        }

        public Matrix Transform
        {
            get
            {
                return Current.Clone();
            }
            set
            {
                MultiplyTransform(value); // BUGBUG: this this correct?
            }
        }

        public void TranslateTransform(float dx, float dy)
        {
            var m = new Matrix();
            m.Translate(dx, dy);
            Push(m);
        }

        public void RotateTransform(float angle)
        {
            var m = new Matrix();
            m.Rotate(angle);
            Push(m);
        }

        public void ScaleTransform(float sx, float sy)
        {
            var m = new Matrix();
            m.Scale(sx, sy);
            Push(m);
        }

        public void MultiplyTransform(Matrix matrix)
        {
            Push(matrix);
        }

        /// <summary>
        /// Each entry is created during Save
        /// and pushed during the next Save
        /// </summary>
        private Stack<StackEntry> stack;

        class StackEntry
        {
            public int popCount;
            public Matrix matrix;
        }

        StackEntry s = null;

        private void Push(Matrix m)
        {
            // If we're in a SaveTransform block,
            // increment the pop count.
            if (s != null)
                s.popCount++;

            // compute the new matrix by prepending
            Current.Multiply(m, MatrixOrder.Prepend);

            // push the transform
            push(m);
        }

        public void SaveTransform()
        {
            // Create a stack the first time.
            if (stack == null)
                stack = new Stack<StackEntry>();

            // If there is an existing
            // entry, push it
            if (s != null)
                stack.Push(s);

            // start a new entry
            s = new StackEntry
            {
                popCount = 0,
                matrix = Current.Clone()
            };
        }

        public void RestoreTransform()
        {
            // If there is a current entry, use it.
            var t = s;

            if (t == null)
                throw new EtoException("RestoreTransform called without SaveTransform");

            // restore the transform
            current = t.matrix;

            // Pop the drawing context
            // popCount times
            while (
                t != null &&
                t.popCount-- > 0)
                // return a cloned matrix
                // since the caller may dispose it.
                pop(Current.Clone());

            // reset the current entry always
            s = null;

            // otherwise if the stack is nonempty
            // pop the value.
            if (stack != null &&
                stack.Count > 0)
                s = stack.Pop();
        }
    }
}
