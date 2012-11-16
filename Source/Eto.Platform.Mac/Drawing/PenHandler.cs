using System;
using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Drawing
{
	public class PenHandler : IPen
	{
        public float Width { get; set; }

        public PenAlignment Alignment { get; set; }

        private DashStyle DashStyle { get; set; }

        public Color Color { get; set; }

        public PenHandler()
		{
		}

        public void Create(
            Color color, 
            float width, 
            PenAlignment alignment, 
            DashStyle dashStyle)
        {
            this.Color = color;
            this.Width = width;
            this.Alignment = alignment;
            this.DashStyle = dashStyle;
        }

        public void Create(Brush brush)
        {
            var brushHandler = 
                brush.ControlObject 
                as BrushHandler;

            if (brushHandler != null)
                this.Color =
                    brushHandler.Color; // TODO
        }

        public string ID { get; set; }

        public object ControlObject
        {
            get { return null; }
        }

        public void HandleEvent(string handler)
        {
            
        }

        public Widget Widget { get; set; }

        public void Initialize()
        {            
        }
    }
}
