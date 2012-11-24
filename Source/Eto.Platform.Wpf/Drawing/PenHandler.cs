using System;
using Eto.Drawing;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
    public class PenHandler : WidgetHandler<swm.Pen, Pen>, IPen
    {
        public float Width
        {
            get
            {
                return (float)Control.Thickness;
            }
            set
            {
                Control.Thickness = value;
            }
        }

        public PenAlignment Alignment
        {
            get
            {
                // Wpf does not support Alignment
                return PenAlignment.Center; 
            }
            set
            {
                // Wpf does not support Alignment
            }
        }

        public Color Color
        {
            get
            {
                var b = Control.Brush as swm.SolidColorBrush;

                if (b != null)
                    return b.Color.ToEto();

                return Colors.Black; // should never come here
            }
            set
            {
                // re-create the pen
                Create(value, Width, PenAlignment.Center, Control.DashStyle.ToEto());
            }
        }

        public void Create(Color color, float width, PenAlignment alignment, DashStyle dashStyle)
        {
            Control =
                new swm.Pen(
                    new swm.SolidColorBrush(color.ToWpf()),
                    width);

            Control.DashStyle = dashStyle.ToWpf();
        }

        public void Create(Brush brush)
        {
            throw new NotImplementedException();
        }
    }
}
