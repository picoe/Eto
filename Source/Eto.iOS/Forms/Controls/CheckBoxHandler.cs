using System;
using System.Reflection;
using Eto.Forms;
using UIKit;
using Eto.Drawing;
using sd = System.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class CheckBoxHandler : IosView<CheckBoxHandler.CheckBox, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		public override UIView ContainerControl { get { return Control; } }

		public class CheckBox : UIView
		{
			bool threeState;
			UILabel labelControl;
			UISwitch switchControl;
			UISegmentedControl segmented;

			public string Text { get { return labelControl.Text; } set { labelControl.Text = value; } }

			public UILabel Label { get { return labelControl; } }

			public UISwitch Switch { get { return switchControl; } }

			public UISegmentedControl Segmented { get { return segmented; } }

			public bool? Checked
			{ 
				get
				{
					if (ThreeState)
						return segmented.SelectedSegment == 2 ? (bool?)true : (segmented.SelectedSegment == 1 ? (bool?)false : null);
					else
						return switchControl.On;
				}
				set
				{ 
					switchControl.On = value ?? false;
					segmented.SelectedSegment = value == null ? 0 : (value.Value ? 2 : 1);
				}
			}

			public bool ThreeState
			{
				get { return threeState; }
				set
				{
					threeState = value;
					switchControl.Hidden = threeState;
					segmented.Hidden = !threeState;
					this.SetNeedsLayout();
				}
			}

			public event EventHandler<EventArgs> ValueChanged;

			protected virtual void OnValueChanged(EventArgs e)
			{
				if (ValueChanged != null)
					ValueChanged(this, e);
			}

			public CheckBox()
			{
				labelControl = new UILabel();
				switchControl = new UISwitch();
				// todo: localize
				segmented = new UISegmentedControl(new object[] { "", "OFF", "ON" });
				segmented.SelectedSegment = 1;
				segmented.Hidden = true;
				switchControl.ValueChanged += (sender, e) =>
				{
					OnValueChanged(EventArgs.Empty);
				};
				segmented.ValueChanged += (sender, e) =>
				{
					OnValueChanged(EventArgs.Empty);
				};
				AddSubview(labelControl);
				AddSubview(switchControl);
				AddSubview(segmented);
			}

			public override CoreGraphics.CGSize SizeThatFits(CoreGraphics.CGSize size)
			{
				var labelSize = labelControl.SizeThatFits(size);
				var toggle = threeState ? (UIControl)segmented : (UIControl)switchControl;

				var switchSize = toggle.SizeThatFits(size);
				return new CoreGraphics.CGSize(labelSize.Width + switchSize.Width, (nfloat)Math.Max(labelSize.Height, switchSize.Height));
			}

			public override void LayoutSubviews()
			{
				base.LayoutSubviews();
				var toggle = threeState ? (UIControl)segmented : (UIControl)switchControl;

				var switchSize = toggle.SizeThatFits(UIView.UILayoutFittingCompressedSize);
				var size = this.Frame.Size;
				var pos = size.Width - switchSize.Width;
				labelControl.Frame = new CoreGraphics.CGRect(new CoreGraphics.CGPoint(0, 0), new CoreGraphics.CGSize(pos, size.Height));
				toggle.Frame = new CoreGraphics.CGRect(new CoreGraphics.CGPoint(pos, 0), switchSize);
			}
		}

		public CheckBoxHandler()
		{
			Control = new CheckBox();
			Control.ValueChanged += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ThreeState
		{
			get { return Control.ThreeState; }
			set { Control.ThreeState = value; }
		}

		public bool? Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				Control.Label.Font = value.ToUI();
			}
		}

		public Color TextColor
		{
			get { return Control.TintColor.ToEto(); }
			set { Control.TintColor = value.ToNSUI(); }
		}
	}
}
