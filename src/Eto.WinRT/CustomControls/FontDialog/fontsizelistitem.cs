#if TODO_XAML
using System;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
using System.Globalization;

namespace Eto.WinRT.CustomControls.FontDialog
{
    class FontSizeListItem : TextBlock, IComparable
    {
        readonly double _sizeInPoints;

        public FontSizeListItem(double sizeInPoints)
        {
            _sizeInPoints = sizeInPoints;
            Text = sizeInPoints.ToString();
        }

        public override string ToString()
        {
            return _sizeInPoints.ToString();
        }

        public double SizeInPoints
        {
            get { return _sizeInPoints; }
        }

        public double SizeInPixels
        {
            get { return PointsToPixels(_sizeInPoints); }
        }

        public static bool FuzzyEqual(double a, double b)
        {
            return Math.Abs(a - b) < 0.01;
        }

        int IComparable.CompareTo(object obj)
        {
            double value;

            if (obj is double)
            {
                value = (double)obj;
            }
            else
            {
                if (!double.TryParse(obj.ToString(), out value))
                {
                    return 1;
                }
            }

            return 
                FuzzyEqual(_sizeInPoints, value) ? 0 :
                (_sizeInPoints < value) ? -1 : 1;
        }

        public static double PointsToPixels(double value)
        {
            return value * (96.0 / 72.0);
        }

        public static double PixelsToPoints(double value)
        {
            return value * (72.0 / 96.0);
        }
    }
}
#endif