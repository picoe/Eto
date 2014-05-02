using System;
using Eto.Forms;
using swc = System.Windows.Controls;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public class SeparatorToolItemHandler : ToolItemHandler<swc.ContentControl, SeparatorToolItem>, ISeparatorToolItem, IToolItem
	{
		class EtoSpaceSeparator : swc.Control
		{
			public EtoSpaceSeparator()
			{
				this.Width = this.Height = 16;
			}
		}

		public SeparatorToolItemHandler()
		{
			Control = new swc.ContentControl();
			Type = SeparatorToolItemType.Divider;
		}

		public SeparatorToolItemType Type
		{
			get
			{
				var control = Control.Content;
				if (control is swc.Separator)
					return SeparatorToolItemType.Divider;
				if (control is EtoSpaceSeparator)
					return SeparatorToolItemType.Space;
				return SeparatorToolItemType.FlexibleSpace;
			}
			set
			{
				swc.Control control;
				switch (value)
				{
					case SeparatorToolItemType.Divider:
						control = new swc.Separator();
						break;
					case SeparatorToolItemType.FlexibleSpace:
					case SeparatorToolItemType.Space:
						control = new EtoSpaceSeparator();
						break;
					default:
						throw new NotSupportedException();
				}
				Control.Content = control;
			}
		}


		public override string Text
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public override string ToolTip
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public override Image Image
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public override bool Enabled
		{
			get { return false; }
			set { throw new NotSupportedException(); }
		}
	}
}
