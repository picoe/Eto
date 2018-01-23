using System;
using UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class SearchBoxHandler : IosView<UISearchBar, SearchBox, SearchBox.ICallback>, SearchBox.IHandler
	{
		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return SizeF.Max(base.GetNaturalSize(availableSize), new SizeF(60, 0));
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control = new UISearchBar();
			MaxLength = Int32.MaxValue;
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case TextControl.TextChangedEvent:
					Control.TextChanged += (s, e) => Callback.OnTextChanged(Widget, e);
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public int MaxLength
		{
			get;
			set;
		}

		public string PlaceholderText
		{
			get { return Control.Placeholder; }
			set { Control.Placeholder = value; }
		}

		public void SelectAll()
		{
			Control.SelectAll(Control);
		}

		public Color TextColor
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public int CaretIndex
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public Range<int> Selection
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool ShowBorder
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public TextAlignment TextAlignment
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
}

