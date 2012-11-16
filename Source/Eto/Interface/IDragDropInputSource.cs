using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Interface
{
    public interface IDragDropInputSource
    {
        event EventHandler<QueryContinueDragEventArgs> QueryContinueDrag;
        event EventHandler<DragEventArgs> DragOver;
        event EventHandler<DragEventArgs> DragDrop;
        event EventHandler DragLeave;
        event EventHandler<GiveFeedbackEventArgs> GiveFeedback;
        event EventHandler<DragEventArgs> DragEnter;
        Point PointToScreen(Point p);
    }
}
