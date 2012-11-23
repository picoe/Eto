using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using MonoMac.AppKit;
using Eto.Platform.Mac.Drawing;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Eto.Platform.Mac.IO;
using System.Threading;
using SD = System.Drawing;

namespace Eto.Platform.Mac
{
	public class Generator : Eto.Generator
	{ 	
		public override string ID {
			get { return Generators.Mac; }
		}

		public Generator ()
		{
			AddAssembly (typeof (Generator).Assembly);
		}
		
		
		public override IDisposable ThreadStart ()
		{
			return new NSAutoreleasePool ();
		}
        public static RectangleF Convert(System.Drawing.RectangleF rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

		
        public static System.Drawing.RectangleF Convert(RectangleF rect)
        {
            return new System.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
		
        public static System.Drawing.PointF Convert(PointF point)
        {
            return new System.Drawing.PointF(point.X, point.Y);
        }

        public static PointF Convert(System.Drawing.PointF point)
        {
            return new PointF(point.X, point.Y);
        }
		

        internal static Matrix Convert(
            CGAffineTransform t)
        {
            return new Matrix(
                t.xx,
                t.yx,
                t.xy,
                t.yy,
                t.x0,
                t.y0);
        }

        internal static SD.PointF[] Convert(PointF[] points)
        {
            var result =
                new SD.PointF[points.Length];

            for (var i = 0;
                i < points.Length;
                ++i)
            {
                var p = points[i];
                result[i] =
                    new SD.PointF(p.X, p.Y);
            }

            return result;
        }

        internal static CGAffineTransform Convert(
            Matrix m)
        {
            var e = m.Elements;

            return new CGAffineTransform(
                e[0],
                e[1],
                e[2],
                e[3],
                e[4],
                e[5]);
        }
    }
}
