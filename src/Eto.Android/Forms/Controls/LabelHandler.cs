using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Android.Forms.Controls
{
	/// <summary>
	/// Handler for <see cref="Label"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LabelHandler : AndroidCommonControl<aw.TextView, Label, Label.ICallback>, Label.IHandler
	{
		const av.GravityFlags AlignmentMask = av.GravityFlags.HorizontalGravityMask | av.GravityFlags.VerticalGravityMask;

		public LabelHandler()
		{
			Control = new aw.TextView(Platform.AppContextThemed);
		}

		public TextAlignment TextAlignment
		{
			get { return Control.Gravity.ToEtoHorizontal(); }
			set
			{
				var gravity = value.ToAndroidGravity();
				Control.Gravity = (Control.Gravity & ~av.GravityFlags.HorizontalGravityMask & AlignmentMask) | gravity;
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return Control.Gravity.ToEtoVertical(); }
			set
			{
				var gravity = value.ToAndroid();
				Control.Gravity = (Control.Gravity & ~av.GravityFlags.VerticalGravityMask & AlignmentMask) | gravity;
			}
		}

		// TODO
		public WrapMode Wrap
		{
			get;
			set;
		}
	}
}