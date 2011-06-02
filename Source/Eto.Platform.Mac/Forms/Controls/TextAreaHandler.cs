using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class TextAreaHandler : MacText<NSTextField, TextArea>, ITextArea
	{
		public TextAreaHandler()
		{
			Control = new NSTextField();
			Control.Editable = true;
			Control.Selectable = true;
		}
		
		#region ITextArea Members
		
		public bool ReadOnly
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		#endregion
	}
}
