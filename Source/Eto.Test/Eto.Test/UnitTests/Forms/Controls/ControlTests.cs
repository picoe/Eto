using System;
using Eto.Forms;
using NUnit.Framework;
using System.Collections.Generic;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ControlTests : TestBase
	{
		public static IEnumerable<Control> Controls()
		{
			yield return new TextBox();
			yield return new Button();
			yield return new Drawable();
			yield return new Label();
		}

		[TestCaseSource("Controls")]
		public void DefaultValuesShouldBeCorrect(Control control)
		{
            TestProperties(f => control,
			               c => c.Enabled,
			               c => c.ToolTip,
			               c => c.TabIndex
			);
			
		}
	}
}
