using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto
{
    public static class MathUtil
    {
        public const double DegreesToRadians = System.Math.PI / 180;
        public const double RadiansToDegrees = 180 / System.Math.PI;

        public static double SinD(double degrees)
        {
            return System.Math.Sin(degrees * DegreesToRadians);
        }

        public static double CosD(double degrees)
        {
            return System.Math.Cos(degrees * DegreesToRadians);
        }

        public static double TanD(double degrees)
        {
            return System.Math.Tan(degrees * DegreesToRadians);
        }

        public static float Square(float f)
        {
            return f * f;
        }

        public static double Length(double x, double y)
        {
            return System.Math.Sqrt(x * x + y * y);
        }

        public static int Clamp(
            this int value,
            int lower,
            int upperInclusive)
        {
            var result =
                System.Math.Max(
                    value, 
                    lower);

            result =
                System.Math.Min(
                    result, 
                    upperInclusive);

            return result;
        }

        /// <summary>
        /// Returns the amount that
        /// clamping causes the value
        /// to be trimmed by.
        /// The result is positive if
        /// value is greater than upperInclusive,
        /// Negative if value is less than lower,
        /// otherwise zero.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lower"></param>
        /// <param name="upperInclusive"></param>
        /// <returns></returns>
        public static int Clip(
            this int value,
            int lower,
            int upperInclusive)
        {
            return
                value > upperInclusive
                ? value - upperInclusive
                : value < lower
                  ? value - lower
                  : 0;
        }

        public static bool IsInRange(
            this int value,
            int lower,
            int upperInclusive)
        {
            return
                value >= lower &&
                value <= upperInclusive;

        }

        /// <summary>
        /// Error function.
        /// Public domain code from:
        /// http://www.johndcook.com/csharp_erf.html
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Erf(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = System.Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * System.Math.Exp(-x * x);

            return sign * y;
        }

        static void TestErf()
        {
            // Select a few input values
            double[] x = 
            {
                -3, 
                -1, 
                0.0, 
                0.5, 
                2.1 
            };

            // Output computed by Mathematica
            // y = Erf[x]
            double[] y = 
            { 
                -0.999977909503, 
                -0.842700792950, 
                0.0, 
                0.520499877813, 
                0.997020533344 
            };

            double maxError = 0.0;
            for (int i = 0; i < x.Length; ++i)
            {
                double error = System.Math.Abs(y[i] - Erf(x[i]));
                if (error > maxError)
                    maxError = error;
            }

            Console.WriteLine("Maximum error: {0}", maxError);
        }  
    }
}
