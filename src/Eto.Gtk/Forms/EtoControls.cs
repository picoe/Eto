
using System;
using Eto;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms
{
    public partial class EtoFixed : Gtk.Fixed
    {
        WeakReference _handler;
        public IGtkControl Handler
        {
            get => _handler?.Target as IGtkControl;
            set => _handler = new WeakReference(value);
        }
        
#if GTK3        
        protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
        {
            base.OnGetPreferredWidth(out minimum_width, out natural_width);
            var h = Handler;
            if (h != null)
            {
                var userPreferredSize = h.UserPreferredSize;
                if (userPreferredSize.Width > 0)
                    natural_width = userPreferredSize.Width;

                minimum_width = Math.Min(natural_width, minimum_width);
            }
        }
        
        protected override void OnGetPreferredWidthForHeight(int height, out int minimum_width, out int natural_width)
        {
            base.OnGetPreferredWidthForHeight(height, out minimum_width, out natural_width);
            var h = Handler;
            if (h != null)
            {
                var userPreferredSize = h.UserPreferredSize;
                if (userPreferredSize.Width > 0)
                    natural_width = userPreferredSize.Width;

                minimum_width = Math.Min(natural_width, minimum_width);
            }
        }

        protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
        {
            base.OnGetPreferredHeight(out minimum_height, out natural_height);
            var h = Handler;
            if (h != null)
            {
                var userPreferredSize = h.UserPreferredSize;
                if (userPreferredSize.Height > 0)
                    natural_height = userPreferredSize.Height;

                minimum_height = Math.Min(natural_height, minimum_height);
            }
        }
        
        protected override void OnAdjustSizeAllocation(Gtk.Orientation orientation, out int minimum_size, out int natural_size, out int allocated_pos, out int allocated_size)
        {
            base.OnAdjustSizeAllocation(orientation, out minimum_size, out natural_size, out allocated_pos, out allocated_size);
            var h = Handler;
            if (h != null)
            {
                var preferredSize = orientation == Gtk.Orientation.Horizontal ? h.UserPreferredSize.Width : h.UserPreferredSize.Height;

                if (preferredSize > 0)
                    natural_size = preferredSize;

                minimum_size = Math.Min(natural_size, minimum_size);
            }
        }

        protected override void OnAdjustSizeRequest(Gtk.Orientation orientation, out int minimum_size, out int natural_size)
        {
            base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
            var h = Handler;
            if (h != null)
            {
                var preferredSize = orientation == Gtk.Orientation.Horizontal ? h.UserPreferredSize.Width : h.UserPreferredSize.Height;

                if (preferredSize > 0)
                    natural_size = preferredSize;

                minimum_size = Math.Min(natural_size, minimum_size);
            }
        }
#endif

    }
    
    public partial class EtoEventBox : Gtk.EventBox
    {
        WeakReference _handler;
        public IGtkControl Handler
        {
            get => _handler?.Target as IGtkControl;
            set => _handler = new WeakReference(value);
        }
        
#if GTK3        
        protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
        {
            base.OnGetPreferredWidth(out minimum_width, out natural_width);
            var h = Handler;
            if (h != null)
            {
                var userPreferredSize = h.UserPreferredSize;
                if (userPreferredSize.Width > 0)
                    natural_width = userPreferredSize.Width;

                minimum_width = Math.Min(natural_width, minimum_width);
            }
        }
        
        protected override void OnGetPreferredWidthForHeight(int height, out int minimum_width, out int natural_width)
        {
            base.OnGetPreferredWidthForHeight(height, out minimum_width, out natural_width);
            var h = Handler;
            if (h != null)
            {
                var userPreferredSize = h.UserPreferredSize;
                if (userPreferredSize.Width > 0)
                    natural_width = userPreferredSize.Width;

                minimum_width = Math.Min(natural_width, minimum_width);
            }
        }

        protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
        {
            base.OnGetPreferredHeight(out minimum_height, out natural_height);
            var h = Handler;
            if (h != null)
            {
                var userPreferredSize = h.UserPreferredSize;
                if (userPreferredSize.Height > 0)
                    natural_height = userPreferredSize.Height;

                minimum_height = Math.Min(natural_height, minimum_height);
            }
        }
        
        protected override void OnAdjustSizeAllocation(Gtk.Orientation orientation, out int minimum_size, out int natural_size, out int allocated_pos, out int allocated_size)
        {
            base.OnAdjustSizeAllocation(orientation, out minimum_size, out natural_size, out allocated_pos, out allocated_size);
            var h = Handler;
            if (h != null)
            {
                var preferredSize = orientation == Gtk.Orientation.Horizontal ? h.UserPreferredSize.Width : h.UserPreferredSize.Height;

                if (preferredSize > 0)
                    natural_size = preferredSize;

                minimum_size = Math.Min(natural_size, minimum_size);
            }
        }

        protected override void OnAdjustSizeRequest(Gtk.Orientation orientation, out int minimum_size, out int natural_size)
        {
            base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
            var h = Handler;
            if (h != null)
            {
                var preferredSize = orientation == Gtk.Orientation.Horizontal ? h.UserPreferredSize.Width : h.UserPreferredSize.Height;

                if (preferredSize > 0)
                    natural_size = preferredSize;

                minimum_size = Math.Min(natural_size, minimum_size);
            }
        }
#endif

    }
    
}