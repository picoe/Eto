
using System;
namespace Eto.Drawing
{
    /// <summary>
    /// Used for vertical alignment
    /// </summary>
    public class VerticalMetrics
    {
        private VerticalMetrics BaseFontMetrics { get; set; }

        public float Leading { get; set; }
        public float Top { get; set; }

        private float Ascent { get; set; }
        private float Descent { get; set; }
        public float Height { get; set; }
        public float LineHeight { get; private set; }

        /// <summary>
        /// Windows GDI+ includes internal leading
        /// as part of the ascent.
        /// </summary>
        public float InternalLeading { get; set; }

        public float Middle
        {
            get
            {
                return
                    (Bottom
                     - Top) / 2f;
            }
        }

        public float Bottom
        {
            get
            {
                return
                    Top
                    + Leading // Leading / 2 on the top and bottom
                    + Height;
            }
        }

        private float PositiveLeading
        {
            get
            {
                return
                    Math.Max(Leading, 0f);
            }
        }

        public float Baseline
        {
            get
            {
                return
                    Top
                    /// Is this correct? we don't consider negative leading here.
                    /// Test case: Search for "the platonic" in
                    /// http://www.joelonsoftware.com/articles/Unicode.html
                    + PositiveLeading / 2f
                    + Ascent;
            }
        }

        public float TextTop
        {
            get
            {
                return
                    Top
                    + Leading / 2f;
            }
        }

        /// <summary>
        /// Where the text should actually
        /// be drawn.
        /// 
        /// Differs from TextTop due to 
        /// internal leading.
        /// 
        /// We don't want any internal leading
        /// but GDI+ always adds it. Therefore,
        /// this compensates for that.
        /// </summary>
        public float ContentTop
        {
            get
            {
                return
                    TextTop
                    - InternalLeading;
            }
        }

        public float TextBottom
        {
            get
            {
                return
                    Top
                    + Leading / 2f
                    + Height;
            }
        }

        public float SuperBaseline
        {
            get
            {
                return
                    BaseFontMetrics != null
                    ? BaseFontMetrics.SuperBaseline
                    : Baseline
                      - SuperOrSubBaselineOffset;
            }
        }

        public float SubBaseline
        {
            get
            {
                return
                    BaseFontMetrics != null
                    ? BaseFontMetrics.SubBaseline
                    : Baseline
                      + SuperOrSubBaselineOffset;
            }
        }

        public float SelfOrBaseAscent
        {
            get
            {
                return BaseFontMetrics != null
                    ? BaseFontMetrics.SelfOrBaseAscent
                    : Ascent;
            }
        }

        public float SuperOrSubBaselineOffset
        {
            get
            {
                // See http://en.wikipedia.org/wiki/Subscript_and_superscript
                return Height * 0.333f;
            }
        }

        /// <summary>
        /// The distance from the Top of the baseline of 
        /// interest, such as Super or Sub.
        /// </summary>
        public float ActualBaselineDelta { get; set; }


        public VerticalMetrics(
            Font font,
            float lineHeight,
            bool isReplaced,
            Font baseFont = null)
        {
            if (baseFont != null)
                BaseFontMetrics =
                    new VerticalMetrics(
                        baseFont,
                        lineHeight,
                        isReplaced: false);

            /// ActualLineHeightPixels computes differently
            /// (and correctly) for replaced and non-replaced
            /// elements.
            this.LineHeight = 
                lineHeight;

            if (!isReplaced &&
                font != null)
            {
                this.Ascent = font.AscentInPixels;
                this.Descent = font.DescentInPixels;

                // GetAscent includes internal leading on Windows,
                // so we correct here.
                this.InternalLeading =
                    Ascent
                    + Descent
                    - font.HeightInPixels;

                this.Ascent -= this.InternalLeading;

                this.Height = font.HeightInPixels;

                this.Leading =
                    this.LineHeight
                    - this.Height;
            }
            else if(isReplaced)
            {
                this.Ascent = this.LineHeight;
                this.Descent = 0;
                this.Height = this.LineHeight;
                this.Leading = 0f;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "Top:{0} Bottom:{1}",
                this.Top,
                this.Bottom);
        }
    }
}
