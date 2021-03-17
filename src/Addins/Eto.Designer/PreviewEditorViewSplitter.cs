using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Designer
{
    public class PreviewEditorViewSplitter : Splitter
    {
        static double lastPosition = 0.4;

        public Control Editor { get; }

        public PreviewEditorView Preview { get; }

        public PreviewEditorViewSplitter(Control editor, string mainAssembly, IEnumerable<string> references, Func<string> getCode)
        {
            //Size = new Size (200, 200);
            Preview = new PreviewEditorView(mainAssembly, references, getCode);
            Editor = editor;

            Orientation = Orientation.Vertical;
            FixedPanel = SplitterFixedPanel.None;

            Panel1 = Preview;
            Panel2 = editor;
            RelativePosition = lastPosition;
        }


        protected override void OnPositionChanged(EventArgs e)
        {
            base.OnPositionChanged(e);
            lastPosition = RelativePosition;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Preview?.Invalidate();
        }
    }
}

