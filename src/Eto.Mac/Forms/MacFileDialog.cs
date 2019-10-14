using System;
using Eto.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms
{
	interface IMacFileDialog
	{
		List<string> MacFilters { get; }

		string GetDefaultExtension();

		int CurrentFilterIndex { get; }
	}

	class SavePanelDelegate : NSOpenSavePanelDelegate
	{
		WeakReference handler;

		public IMacFileDialog Handler { get { return (IMacFileDialog)handler.Target; } set { handler = new WeakReference(value); } }

		public override bool ShouldEnableUrl(NSSavePanel panel, NSUrl url)
		{
			if (Directory.Exists(url.Path))
				return true;

			// Xamarin.Mac's version of mono has string.TrimStart(char), which is not in the .NET Framework!
			var extension = Path.GetExtension(url.Path).TrimStart(new[] { '.' });
			if (Handler.MacFilters == null || Handler.MacFilters.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
				return true;
			return false;
		}
		
	}

	public abstract class MacFileDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, FileDialog.IHandler, IMacFileDialog
     where TControl: NSSavePanel
     where TWidget: FileDialog
	{
		List<string> macfilters;
		readonly NSPopUpButton fileTypes;

		protected MacFileDialog()
		{
			fileTypes = new NSPopUpButton();
		}

		void Create()
		{
			if (Control.AccessoryView != null)
				return;

			var fileTypeView = new NSView();
			fileTypeView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			
			const int padding = 15;
			
			if (Widget.Filters.Count > 0)
			{
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
				fileTypes.Activated += (sender, e) =>
				{
					SetCurrentItem();
					Control.ValidateVisibleColumns();
					Control.Update();
				};
				fileTypeView.AddSubview(fileTypes);
				fileTypes.SetFrameOrigin(new CGPoint((nfloat)label.Frame.Width + 10, padding));
	
				label.SetFrameOrigin(new CGPoint(0, (nfloat)(padding + (fileTypes.Frame.Height - label.Frame.Height) / 2)));
				
				fileTypeView.Frame = new CGRect(0, 0, (nfloat)(fileTypes.Frame.Width + label.Frame.Width + 10), (nfloat)(fileTypes.Frame.Height + padding * 2));
				
				Control.AccessoryView = fileTypeView;
				SetCurrentItem();
			}
			else
				Control.AccessoryView = null;
		}

		public virtual string FileName
		{
			get
			{ 
				return Control.Url.Path;
			}
			set { }
		}

		public Uri Directory
		{
			get
			{
				return new Uri(Control.DirectoryUrl.AbsoluteString);
			}
			set
			{
				Control.DirectoryUrl = new NSUrl(value.AbsoluteUri);
			}
		}

		public string GetDefaultExtension()
		{
			var filter = Widget.CurrentFilter;
			if (filter != null)
			{
				string ext = filter.Extensions.FirstOrDefault();
				if (!string.IsNullOrEmpty(ext))
				{
					return ext.TrimStart('*', '.');
				}
			}
			return null;
		}

		public List<string> MacFilters
		{
			get { return macfilters; }
		}

		public void SetCurrentItem()
		{
			macfilters = Widget.CurrentFilter.Extensions?.Select(r => r.TrimStart('*', '.')).ToList();

			if (macfilters == null || macfilters.Count == 0 || macfilters.Contains(""))
			{
				macfilters = null;
				// Xamarin.Mac throws exception when setting to null (ugh)
				Messaging.void_objc_msgSend_IntPtr(Control.Handle, Selector.GetHandle("setAllowedFileTypes:"), IntPtr.Zero);
			}
			else
				Control.AllowedFileTypes = macfilters.Distinct().ToArray();
		}

		public int CurrentFilterIndex
		{
			get
			{ 
				var title = fileTypes.TitleOfSelectedItem;
				var item = Widget.Filters.FirstOrDefault(r => r.Name == title);
				if (item == null)
					return -1;
				return Widget.Filters.IndexOf(item);
			}
			set
			{ 
				fileTypes.SelectItem(Widget.Filters[value].Name);
			}
		}

		public bool CheckFileExists
		{
			get { return false; }
			set { }
		}

		public string Title
		{
			get { return Control.Message; }
			set { Control.Message = value ?? string.Empty; }
		}

		public DialogResult ShowDialog(Window parent)
		{
			//Control.AllowsOtherFileTypes = false;
			Control.Delegate = new SavePanelDelegate{ Handler = this };
			Create();

			int ret = MacModal.Run(Control, parent);
            
			return ret == 1 ? DialogResult.Ok : DialogResult.Cancel;
		}

		protected override void Dispose(bool disposing)
		{
			//base.Dispose (disposing);
		}

		public void InsertFilter(int index, FileFilter filter)
		{
			fileTypes.InsertItem(filter.Name, index);
		}

		public void RemoveFilter(int index)
		{
			fileTypes.RemoveItem(index);
		}

		public void ClearFilters()
		{
			fileTypes.RemoveAllItems();
		}
	}
}
