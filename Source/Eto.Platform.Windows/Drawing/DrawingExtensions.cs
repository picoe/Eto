using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Platform.Windows.Drawing
{
    /// <summary>
    /// TODO: remove all uses of these and replace them
    /// with Generator.Convert().
    /// </summary>
    public static partial class PointExtensions
    {
        public static System.Drawing.Point ToPoint(
            this Eto.Drawing.Point point)
        {
            return new System.Drawing.Point(
                point.X,
                point.Y);
        }
        public static Eto.Drawing.Point ToPoint(
            this System.Drawing.Point point)
        {
            return new Eto.Drawing.Point(
                point.X,
                point.Y);
        }
    }
    public static partial class PointFExtensions
    {
        public static System.Drawing.PointF ToPointF(
            this Eto.Drawing.PointF point)
        {
            return new System.Drawing.PointF(
                point.X,
                point.Y);
        }
        public static Eto.Drawing.PointF ToPointF(
            this System.Drawing.PointF point)
        {
            return new Eto.Drawing.PointF(
                point.X,
                point.Y);
        }
    }
    public static partial class RectangleExtensions
    {
        public static System.Drawing.Rectangle ToRectangle(
            this Eto.Drawing.Rectangle rect)
        {
            return new System.Drawing.Rectangle(
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height);
        }
        public static Eto.Drawing.Rectangle ToRectangle(
            this System.Drawing.Rectangle rect)
        {
            return new Eto.Drawing.Rectangle(
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height);
        }
    }
    public static partial class RectangleFExtensions
    {
        public static System.Drawing.RectangleF ToRectangleF(
            this Eto.Drawing.RectangleF rect)
        {
            return new System.Drawing.RectangleF(
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height);
        }
        public static Eto.Drawing.RectangleF ToRectangleF(
            this System.Drawing.RectangleF rect)
        {
            return new Eto.Drawing.RectangleF(
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height);
        }
    }
    public static partial class SizeExtensions
    {
        public static System.Drawing.Size ToSize(
            this Eto.Drawing.Size size)
        {
            return new System.Drawing.Size(
                size.Width,
                size.Height);
        }
        public static Eto.Drawing.Size ToSize(
            this System.Drawing.Size size)
        {
            return new Eto.Drawing.Size(
                size.Width,
                size.Height);
        }
    }
    public static partial class SizeFExtensions
    {
        public static System.Drawing.SizeF ToSizeF(
            this Eto.Drawing.SizeF size)
        {
            return new System.Drawing.SizeF(
                size.Width,
                size.Height);
        }
        public static Eto.Drawing.SizeF ToSizeF(
            this System.Drawing.SizeF size)
        {
            return new Eto.Drawing.SizeF(
                size.Width,
                size.Height);
        }
    }
}
