using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class SearchBoxHandler : iosControl<UISearchBar, SearchBox>, ISearchBox
	{
		public override UISearchBar CreateControl ()
		{
			return new UISearchBar();
		}

		protected override Eto.Drawing.Size GetNaturalSize ()
		{
			return Size.Max (base.GetNaturalSize (), new Size(60, 0));
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			MaxLength = Int32.MaxValue;
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case Eto.Forms.Control.TextChangedEvent:
					Control.TextChanged += (s, e) => Widget.OnTextChanged(e);
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		public string Text {
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly {
			get; set;
		}

		public int MaxLength {
			get;
			set;
		}

		public string PlaceholderText {
			get { return Control.Placeholder; }
			set { Control.Placeholder = value; }
		}

		public void SelectAll ()
		{
			Control.SelectAll ();
		}
	}
}

