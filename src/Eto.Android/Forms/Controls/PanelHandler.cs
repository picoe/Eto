using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	/// <summary>
	/// Handler for <see cref="Panel"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PanelHandler : AndroidPanel<av.View, Panel, Panel.ICallback>, Panel.IHandler
	{
		public override av.View ContainerControl { get { return InnerFrame; } }

		protected override void SetContent(av.View content)
		{
		}
	}
}