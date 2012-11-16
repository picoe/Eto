using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Interface;

namespace Eto.Forms
{
    /// <summary>
    /// Exposes drag/drop events on a control
    /// </summary>
    public class DragDropInputSource : IDragDropInputSource
    {
        private Control Control { get; set; }

        public DragDropInputSource(Control control)
        {
            this.Control = control;
        }

        #region QueryContinueDrag
        public const string QueryContinueDragEvent = "Control.QueryContinueDrag";

        EventHandler<QueryContinueDragEventArgs> QueryContinueDrag_;

        public event EventHandler<QueryContinueDragEventArgs> QueryContinueDrag
        {
            add
            {
                Control.HandleEvent(QueryContinueDragEvent);
                QueryContinueDrag_ += value;
            }
            remove { QueryContinueDrag_ -= value; }
        }

        public virtual void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            if (QueryContinueDrag_ != null)
                QueryContinueDrag_(this, e);
        }

        #endregion

        #region DragOver
        public const string DragOverEvent = "Control.DragOver";

        EventHandler<DragEventArgs> DragOver_;

        public event EventHandler<DragEventArgs> DragOver
        {
            add
            {
                Control.HandleEvent(DragOverEvent);
                DragOver_ += value;
            }
            remove { DragOver_ -= value; }
        }

        public virtual void OnDragOver(DragEventArgs e)
        {
            if (DragOver_ != null)
                DragOver_(this, e);
        }

        #endregion

        #region DragDrop
        public const string DragDropEvent = "Control.DragDrop";

        EventHandler<DragEventArgs> DragDrop_;

        public event EventHandler<DragEventArgs> DragDrop
        {
            add
            {
                Control.HandleEvent(DragDropEvent);
                DragDrop_ += value;
            }
            remove { DragDrop_ -= value; }
        }

        public virtual void OnDragDrop(DragEventArgs e)
        {
            if (DragDrop_ != null)
                DragDrop_(this, e);
        }

        #endregion

        public event EventHandler DragLeave;


        #region GiveFeedback
        public const string GiveFeedbackEvent = "Control.GiveFeedback";

        EventHandler<GiveFeedbackEventArgs> GiveFeedback_;

        public event EventHandler<GiveFeedbackEventArgs> GiveFeedback
        {
            add
            {
                Control.HandleEvent(GiveFeedbackEvent);
                GiveFeedback_ += value;
            }
            remove { GiveFeedback_ -= value; }
        }

        public virtual void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            if (GiveFeedback_ != null)
                GiveFeedback_(this, e);
        }

        #endregion

        #region DragEnter
        public const string DragEnterEvent = "Control.DragEnter";

        EventHandler<DragEventArgs> DragEnter_;

        public event EventHandler<DragEventArgs> DragEnter
        {
            add
            {
                Control.HandleEvent(DragEnterEvent);
                DragEnter_ += value;
            }
            remove { DragEnter_ -= value; }
        }

        public virtual void OnDragEnter(DragEventArgs e)
        {
            if (DragEnter_ != null)
                DragEnter_(this, e);
        }

        #endregion

        public Drawing.Point PointToScreen(Drawing.Point p)
        {
            return Control.WorldToScreen(p);
        }
    }
}
