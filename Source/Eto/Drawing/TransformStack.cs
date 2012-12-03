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
        Eto.Generator generator;
        Action<Matrix> push;
        Action pop;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="push">A callback that should prepend the specified value to the current matrix </param>
        /// <param name="pop">A callback that should either pop the matrix stack or set the current matrix to the specified value</param>
        public TransformStack(
            Eto.Generator generator,
            Action<Matrix> push,
            Action pop)
        {
            this.generator = generator;
            this.push = push;
            this.pop = pop;
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
        }

        StackEntry s = null;

        private void Push(Matrix m)
        {
            // If we're in a SaveTransform block,
            // increment the pop count.
            if (s != null)
                s.popCount++;

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
            };
        }

        public void RestoreTransform()
        {
            // If there is a current entry, use it.
            var t = s;

            if (t == null)
                throw new EtoException("RestoreTransform called without SaveTransform");

            // Pop the drawing context
            // popCount times
            while (
                t != null &&
                t.popCount-- > 0)
                // return a cloned matrix
                // since the caller may dispose it.
                pop();

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
