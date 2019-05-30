using System;
using Eto.Drawing;
using Eto.Forms;

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

namespace Eto.Mac.Forms.Controls
{
	/// <summary>
	/// TODO: Try to eliminate code duplication between this class
	/// and TextBoxHandler. 
	/// </summary>
	public class SearchBoxHandler : MacText<NSSearchField, SearchBox, SearchBox.ICallback>, SearchBox.IHandler, ITextBoxWithMaxLength
	{
		class EtoTextField : NSSearchField, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public SearchBoxHandler Handler
			{ 
				get { return (SearchBoxHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoTextField()
			{
				Bezeled = true;
				Editable = true;
				Selectable = true;
				Cell.Scrollable = true;
				Cell.Wraps = false;
				Cell.UsesSingleLineMode = true;
			}
		}

		public override bool HasFocus
		{
			get
			{
				if (Widget.ParentWindow == null)
					return false;
				return ((IMacWindow)Widget.ParentWindow.Handler).FieldEditorClient == Control;
			}
		}

		protected override NSSearchField CreateControl()
		{
			return new EtoTextField();
		}

		protected override void Initialize()
		{
			var control = Control;

			control.Formatter = new EtoFormatter { Handler = this };

			MaxLength = -1;

			base.Initialize();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);
			size.Width = Math.Max(100, size.Height);
			return size;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Changed += HandleTextChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static void HandleTextChanged(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as SearchBoxHandler;
			if (handler != null)
			{
				handler.Callback.OnTextChanged(handler.Widget, EventArgs.Empty);
			}
		}

		public bool ReadOnly
		{
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}

		public int MaxLength
		{
			get;
			set;
		}

		public string PlaceholderText
		{
			get { return Control.Cell.PlaceholderString; }
			set { Control.Cell.PlaceholderString = value ?? string.Empty; }
		}
	}
}
