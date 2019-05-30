using System;
using Windows.UI.Xaml;

namespace Eto.Test.WinRT.Data
{
	/// <summary>
	/// Visual representation of a Test
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TestItem : TestItemBase
	{
		public TestItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, TestGroup group,
			Func<FrameworkElement> getControl)
			: base(uniqueId, title, subtitle, imagePath, description)
		{
			this.getControl = getControl;
			this._content = content;
			this._group = group;
		}

		Func<FrameworkElement> getControl;

		private string _content = string.Empty;
		public string Content
		{
			get { return this._content; }
			set { this.SetProperty(ref this._content, value); }
		}

		public FrameworkElement ContentControl
		{
			get { return getControl != null ? getControl() : null; } // we don't cache this so that a test section can be run again during debugging
		}

		private TestGroup _group;
		public TestGroup Group
		{
			get { return this._group; }
			set { this.SetProperty(ref this._group, value); }
		}
	}
}
