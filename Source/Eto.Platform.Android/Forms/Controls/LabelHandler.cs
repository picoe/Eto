using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android.Forms.Controls
{
	/// <summary>
	/// Handler for <see cref="ILabel"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LabelHandler : AndroidCommonControl<aw.TextView, Label>, ILabel
	{
		const av.GravityFlags AlignmentMask = av.GravityFlags.HorizontalGravityMask | av.GravityFlags.VerticalGravityMask;

		public LabelHandler()
		{
			Control = new aw.TextView(a.App.Application.Context);
		}

		public HorizontalAlign HorizontalAlign
		{
			get { return Control.Gravity.ToEtoHorizontal(); }
			set
			{
				var gravity = value.ToAndroid();
				Control.Gravity = (Control.Gravity & ~av.GravityFlags.HorizontalGravityMask & AlignmentMask) | gravity;
			}
		}

		public VerticalAlign VerticalAlign
		{
			get { return Control.Gravity.ToEtoVertical(); }
			set
			{
				var gravity = value.ToAndroid();
				Control.Gravity = (Control.Gravity & ~av.GravityFlags.VerticalGravityMask & AlignmentMask) | gravity;
			}
		}

		public WrapMode Wrap
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Color TextColor
		{
			get { return Color.FromArgb((uint)Control.CurrentTextColor); }
			set { Control.SetTextColor(value.ToAndroid()); }
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
	}
}