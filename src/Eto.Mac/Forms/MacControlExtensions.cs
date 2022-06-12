using Eto.Drawing;
using Eto.Forms;
using System;
using System.Text.RegularExpressions;

namespace Eto.Mac.Forms
{
	public static class MacControlExtensions
	{
		[Obsolete("Use Control.GetPreferredSize")]
		public static SizeF GetPreferredSize(Control control, SizeF availableSize)
		{
			if (control == null)
				return Size.Empty;
			return control.GetPreferredSize(availableSize);
		}

		public static IMacViewHandler GetMacViewHandler(this Control control)
		{
			if (control == null)
				return null;
			var container = control.Handler as IMacViewHandler;
			if (container != null)
				return container;
			var child = control.ControlObject as Control;
			return child == null ? null : child.GetMacViewHandler();
		}

		public static IMacControlHandler GetMacControl(this Control control)
		{
			if (control == null)
				return null;
			var container = control.Handler as IMacControlHandler;
			if (container != null)
				return container;
			var child = control.ControlObject as Control;
			return child == null ? null : child.GetMacControl();
		}

		public static NSView GetContainerView(this Widget control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IMacControlHandler;
			if (containerHandler != null)
				return containerHandler.ContainerControl;
			var childControl = control.ControlObject as Control;
			if (childControl != null)
				return childControl.GetContainerView();
			return control.ControlObject as NSView;
		}

		public static NSAttributedString ToAttributedStringWithMnemonic(this string value, NSDictionary attributes = null)
		{
			if (value == null)
				return null;
			var match = Regex.Match(value, @"(?<=([^&](?:[&]{2})*)|^)[&](?![&])");
			if (match.Success)
			{
				value = value.Remove(match.Index, 1);
				value = value.Replace("&&", "&");
				var str = attributes != null ? new NSMutableAttributedString(value, attributes) : new NSMutableAttributedString(value);
				var attr = new CTStringAttributes();
				attr.UnderlineStyle = CTUnderlineStyle.Single;
				str.AddAttributes(attr, new NSRange(match.Index, 1));
				return str;
			}
			else
			{
				value = value.Replace("&&", "&");
				return attributes != null ? new NSAttributedString(value, attributes) : new NSAttributedString(value);
			}
		}

		public static void CenterInParent(this NSView view)
		{
			var super = view.Superview;
			if (super != null)
			{
				var superFrame = super.Frame;
				var size = view.Frame.Size;
				view.SetFrameOrigin(new CGPoint((nfloat)(superFrame.Width - size.Width) / 2, (nfloat)(superFrame.Height - size.Height) / 2));
			}
		}
		
		public static bool HasDarkTheme(this NSView view)
		{
			if (!MacVersion.IsAtLeast(10, 14))
				return false;
				
			var appearance = view?.EffectiveAppearance ?? NSAppearance.CurrentAppearance;
			
			var name = appearance.Name;
			
			if (name == NSAppearance.NameDarkAqua || name == NSAppearance.NameAccessibilityHighContrastDarkAqua)
				return true;
				
			return false;
		}
	}
}

