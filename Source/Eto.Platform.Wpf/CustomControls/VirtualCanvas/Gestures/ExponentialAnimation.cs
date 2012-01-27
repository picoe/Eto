//-----------------------------------------------------------------------
// <copyright file="ExponentialAnimation.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Microsoft.Sample.Controls
{
    /// <summary>
    /// EdgeBehavior defines what type of exponential animation to do.
    /// </summary>
    public enum EdgeBehavior {
        EaseIn, EaseOut, EaseInOut
    }

    /// <summary>
    /// This class provides a non-linear DoubleAnimation.
    /// </summary>
    public class ExponentialDoubleAnimation : DoubleAnimation {

        /// <summary>
        /// The property for defining an EdgeBehavior value.
        /// </summary>
        public static readonly DependencyProperty EdgeBehaviorProperty =
            DependencyProperty.Register("EdgeBehavior", typeof(EdgeBehavior), typeof(ExponentialDoubleAnimation), new PropertyMetadata(EdgeBehavior.EaseIn));

        /// <summary>
        /// Property for defining the exponential power of the animation.
        /// </summary>
        public static readonly DependencyProperty PowerProperty =
            DependencyProperty.Register("Power", typeof(double), typeof(ExponentialDoubleAnimation), new PropertyMetadata(2.0));

        /// <summary>
        /// Construct new empty ExponentialDoubleAnimation object.
        /// </summary>
        public ExponentialDoubleAnimation() {
        }

        /// <summary>
        /// Construct new ExponentialDoubleAnimation with given arguments
        /// </summary>
        /// <param name="from">Animate from this value</param>
        /// <param name="to">To this value</param>
        /// <param name="power">With this exponential power</param>
        /// <param name="behavior">Using this type of behavior</param>
        /// <param name="duration">For this long</param>
        public ExponentialDoubleAnimation(double from, double to, double power, EdgeBehavior behavior, Duration duration) {
            this.EdgeBehavior = behavior;
            this.Duration = duration;
            this.Power = power;
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Get/Set the exponential behavior.  It can either start fast and finish slow (EaseIn), or it can start slow
        /// and finish fast (EaseOut) or it can do both (like a parabola) which is EaseInOut. Default is EaseIn.
        /// </summary>
        public EdgeBehavior EdgeBehavior {
            get {
                return (EdgeBehavior)GetValue(EdgeBehaviorProperty);
            }
            set {
                SetValue(EdgeBehaviorProperty, value);
            }
        }

        /// <summary>
        /// Get/Set the power of the exponential.  The default is 2.
        /// </summary>
        public double Power {
            get {
                return (double)GetValue(PowerProperty);
            }
            set {
                if (value > 0.0) {
                    SetValue(PowerProperty, value);
                } else {
                    throw new ArgumentException("cannot set power to less than 0.0. Value: " + value);
                }
            }
        }

        /// <summary>
        /// This method is called by WPF to implement the actual animation, so this method calculates 
        /// the exponential value based on how long we've been running so far.
        /// </summary>
        /// <param name="defaultOriginValue"></param>
        /// <param name="defaultDestinationValue"></param>
        /// <param name="clock"></param>
        /// <returns></returns>
        protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock) {
            double returnValue;
            double start = (double)From;
            double delta = (double)To - start;
            double timeFraction = animationClock.CurrentProgress.Value;
            if (timeFraction == 1)
            {
                return (double)this.To;
            }
            switch (this.EdgeBehavior) {
                case EdgeBehavior.EaseIn:
                    returnValue = EaseIn(timeFraction, start, delta, Power);
                    break;
                case EdgeBehavior.EaseOut:
                    returnValue = EaseOut(timeFraction, start, delta, Power);
                    break;
                case EdgeBehavior.EaseInOut:
                default:
                    returnValue = EaseInOut(timeFraction, start, delta, Power);
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// All Freesable objects have to implement this method.
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore() {
            return new ExponentialDoubleAnimation();
        }

        /// <summary>
        /// Impelement the EaseIn style of exponential animation which is one of exponential growth.
        /// </summary>
        /// <param name="timeFraction">Time we've been running from 0 to 1.</param>
        /// <param name="start">Start value</param>
        /// <param name="delta">Delta between start value and the end value we want</param>
        /// <param name="power">The rate of exponental growth</param>
        /// <returns></returns>
        private static double EaseIn(double timeFraction, double start, double delta, double power) {
            double returnValue = 0.0;
            // math magic: simple exponential growth
            returnValue = Math.Pow(timeFraction, power);
            returnValue *= delta;
            returnValue = returnValue + start;
            return returnValue;
        }

        /// <summary>
        /// Impelement the EaseOut style of exponential animation which is one of exponential decay.
        /// </summary>
        /// <param name="timeFraction">Time we've been running from 0 to 1.</param>
        /// <param name="start">Start value</param>
        /// <param name="delta">Delta between start value and the end value we want</param>
        /// <param name="power">The rate of exponental decay</param>
        /// <returns></returns>
        private static double EaseOut(double timeFraction, double start, double delta, double power)
        {
            double returnValue = 0.0;

            // math magic: simple exponential decay
            returnValue = Math.Pow(timeFraction, 1 / power);
            returnValue *= delta;
            returnValue = returnValue + start;
            return returnValue;
        }

        /// <summary>
        /// Impelement the EaseInOut style of exponential animation which is one of exponential growth
        /// for the first half of the animation and one of exponential decay for the second half.
        /// </summary>
        /// <param name="timeFraction">Time we've been running from 0 to 1.</param>
        /// <param name="start">Start value</param>
        /// <param name="delta">Delta between start value and the end value we want</param>
        /// <param name="power">The rate of exponental growth/decay</param>
        /// <returns></returns>
        private static double EaseInOut(double timeFraction, double start, double delta, double power)
        {
            double returnValue = 0.0;

            // we cut each effect in half by multiplying the time fraction by two and halving the distance.
            if (timeFraction <= 0.5) {
                returnValue = EaseOut(timeFraction * 2, start, delta / 2, power);
            } else {
                returnValue = EaseIn((timeFraction - 0.5) * 2, start, delta / 2, power);
                returnValue += (delta / 2);
            }
            return returnValue;
        }
    }
}