using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eto.Platform.Mac.Forms
{
    interface IMacFileDialog
    {
    	List<string> MacFilters { get; }
        IEnumerable<IFileDialogFilter> Filters { get; }
		string GetDefaultExtension();
		int CurrentFilterIndex { get; }
    }
 
    class SavePanelDelegate : NSOpenSavePanelDelegate
    {
		WeakReference handler;
		public IMacFileDialog Handler { get { return (IMacFileDialog)handler.Target; } set { handler = new WeakReference(value); } }
     
        public override bool ShouldEnableUrl (NSSavePanel panel, NSUrl url)
		{
			if (Directory.Exists(url.Path))
				return true;

			var extension = Path.GetExtension(url.Path).ToLowerInvariant().TrimStart('.');
			if (Handler.MacFilters == null || Handler.MacFilters.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
				return true;
			return false;
		}
		
    }
	
    public abstract class MacFileDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IFileDialog, IMacFileDialog
     where TControl: NSSavePanel
     where TWidget: FileDialog
    {
        IFileDialogFilter[] filters;
		List<string> macfilters;
		readonly NSPopUpButton fileTypes;
		List<string> titles;

        protected MacFileDialog ()
        {
			fileTypes = new NSPopUpButton();
        }
		
		void Create()
		{
			if (Control.AccessoryView != null) return;

			var fileTypeView = new NSView();
			fileTypeView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			
			const int padding = 15;
			
			if (macfilters != null && macfilters.Count > 0) {
				var label = new NSTextField();
				label.StringValue = "Format";
				label.DrawsBackground = false;
				label.Bordered = false;
				label.Bezeled = false;
				label.Editable = false;
				label.Selectable = false;
				label.SizeToFit();
				fileTypeView.AddSubview(label);
				
				fileTypes.SizeToFit();
				fileTypes.Activated += (sender, e) => {
					SetFilters ();
					Control.ValidateVisibleColumns ();// SetFilters ();
					Control.Update ();
				};
				fileTypeView.AddSubview(fileTypes);
				fileTypes.SetFrameOrigin(new System.Drawing.PointF(label.Frame.Width + 10, padding));
	
				label.SetFrameOrigin(new System.Drawing.PointF(0, padding + (fileTypes.Frame.Height - label.Frame.Height) / 2));
				
				fileTypeView.Frame = new System.Drawing.RectangleF(0, 0, fileTypes.Frame.Width + label.Frame.Width + 10, fileTypes.Frame.Height + padding*2);
				
				Control.AccessoryView = fileTypeView;
			}
			else
				Control.AccessoryView = null;
		}
     
        public virtual string FileName {
            get { 
                return Control.Url.Path;
            }
            set { }
        }
		
		public Uri Directory {
			get {
				return new Uri(Control.DirectoryUrl.AbsoluteString);
			}
			set {
				Control.DirectoryUrl = new NSUrl(value.AbsoluteUri);
			}
		}
		
		public string GetDefaultExtension ()
		{
			if (CurrentFilterIndex >= 0)
			{
				var filter = filters[CurrentFilterIndex];
				string ext = filter.Extensions.FirstOrDefault();
				if (!string.IsNullOrEmpty(ext))
				{
					return ext.TrimStart('*', '.');
				}
			}
			return null;
		}
		
		void SetFilters()
		{
            macfilters = new List<string> ();
			var filter = filters[CurrentFilterIndex];
            //foreach (var filter in filters) {
                foreach (var filterext in filter.Extensions) {
                    macfilters.Add (filterext.TrimStart ('*', '.'));
                }
            //}
            Control.AllowedFileTypes = macfilters.Distinct ().ToArray ();
		}
		
		public List<string> MacFilters
		{
			get { return macfilters; }
		}

        public IEnumerable<IFileDialogFilter> Filters {
            get { return filters; }
            set { 
                filters = value.ToArray ();
             	titles = new List<string>();
				fileTypes.RemoveAllItems();
                foreach (var filter in filters) {
					titles.Add(filter.Name);
                }
				fileTypes.AddItems(titles.ToArray());
				
				SetFilters ();
            }
        }

		public IFileDialogFilter CurrentFilter
		{
			get
			{
				if (CurrentFilterIndex == -1 || filters == null) return null;
				return filters[CurrentFilterIndex];
			}
			set
			{
				CurrentFilterIndex = Array.IndexOf (filters, value);
			}
		}

        public int CurrentFilterIndex {
            get { 
				var title = fileTypes.TitleOfSelectedItem;
                return titles.IndexOf(title);
			}
            set { 
				fileTypes.SelectItem (filters[value].Name);
			}
        }

        public bool CheckFileExists {
            get { return false; }
            set {  }
        }

        public string Title {
            get { return Control.Title; }
            set { Control.Title = value; }
        }
     
        public DialogResult ShowDialog (Window parent)
        {
            //Control.AllowsOtherFileTypes = false;
            Control.Delegate = new SavePanelDelegate{ Handler = this };
			Create();

			
            int ret = MacModal.Run(Control, parent);
            
            return ret == 1 ? DialogResult.Ok : DialogResult.Cancel;
        }
		
		protected override void Dispose (bool disposing)
		{
			//base.Dispose (disposing);
		}

    }
}
