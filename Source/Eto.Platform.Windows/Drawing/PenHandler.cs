using System;
using System.Collections.Generic;
using System.Text;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public class PenHandler : WidgetHandler<System.Drawing.Pen, Pen>, IPen
	{
        public PenHandler()
        {
        }

        public PenHandler(SD.Pen pen)
        {
            this.Control = pen;
        }

        public float Width
        {
            get
            {
                return this.Control.Width;
            }
            set
            {
                this.Control.Width = value;
            }
        }

        public Color Color
        {
            get
            {
                return 
                    Generator.Convert(
                        this.Control.Color);
            }
            set
            {
                this.Control.Color =
                    Generator.Convert(
                        value);
            }
        }

        public PenAlignment Alignment
        {
            get
            {
                return (PenAlignment)this.Control.Alignment;
            }
            set
            {
                this.Control.Alignment = 
                    (SD.Drawing2D.PenAlignment)value;
            }
        }

        public void Create(Color color, float width, PenAlignment alignment, DashStyle dashStyle)
        {
            this.Control =
                new SD.Pen(
                    Generator.Convert(color),
                    width);

            this.Control.Alignment = (SD.Drawing2D.PenAlignment)alignment;
            this.Control.DashStyle = (SD.Drawing2D.DashStyle)dashStyle;
        }

        public void Create(Brush brush)
        {
            this.Control =
                new SD.Pen(
                    (SD.Brush)brush.ControlObject);
        }
    }
}
