using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using UIKit;
using Eto.Drawing;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using ObjCRuntime;

namespace Eto.iOS.Forms.Controls
{
	public interface IBasePickerHandler
	{
		UIView CreatePicker();

		UIView CreateAccessoryView();

		void UpdatePicker(UIView picker);

		void UpdateValue(UIView picker);
	}

	public class EtoLabel : UILabel
	{
		UIView inputView;
		UIView accessoryView;
		WeakReference handler;
		UIPopoverController popover;

		public IBasePickerHandler Handler { get { return (IBasePickerHandler)handler.Target; } set { handler = new WeakReference(value); } }

		public override bool CanBecomeFirstResponder { get { return true; } }

		public override UIView InputView
		{
			get { return inputView ?? (inputView = Handler.CreatePicker()); }
		}

		public override UIView InputAccessoryView
		{
			get { return accessoryView ?? (accessoryView = Handler.CreateAccessoryView()); }
		}

		static readonly Selector selPreferredContentSize = new Selector("preferredContentSize");

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			if (Platform.IsIpad)
			{
				var picker = Handler.CreatePicker();
				Handler.UpdatePicker(InputView);
				picker.SizeToFit();
				var view = new UIViewController { View = picker };
				if (view.RespondsToSelector(selPreferredContentSize))
					view.PreferredContentSize = picker.SizeThatFits(CoreGraphics.CGSize.Empty);
				else
					view.ContentSizeForViewInPopover = picker.SizeThatFits(CoreGraphics.CGSize.Empty);
				popover = new UIPopoverController(view);
				popover.PresentFromRect(Bounds, this, UIPopoverArrowDirection.Any, true);
				popover.DidDismiss += (sender, e) =>
				{
					Handler.UpdateValue(picker);
					popover.Dispose();
					popover = null;
				};
			}
			else
			{
				Handler.UpdatePicker(InputView);
				BecomeFirstResponder();
			}
		}
	}

	public abstract class BasePickerHandler<TWidget, TCallback, TPicker> : IosView<UILabel, TWidget, TCallback>, IBasePickerHandler
		where TWidget: Control
		where TCallback: Control.ICallback
		where TPicker: UIView
	{
		string emptyText;

		public static string DefaultEmptyText = "<select>";

		public string EmptyText { get { return emptyText ?? DefaultEmptyText; } set { emptyText = value; } }

		static UIColor ButtonTextColor = new UIButton(UIButtonType.RoundedRect).TitleColor(UIControlState.Normal);
		static UIColor DisabledTextColor = new UIButton(UIButtonType.RoundedRect).TitleColor(UIControlState.Disabled);
	
		protected BasePickerHandler()
		{
			Control = new EtoLabel { Handler = this };
			Control.AccessibilityTraits = UIAccessibilityTrait.Button;
			Control.UserInteractionEnabled = true;
			Control.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			Control.TextColor = ButtonTextColor;
		}

		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				base.Enabled = value;
				Control.TextColor = value ? ButtonTextColor : DisabledTextColor;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			UpdateText();
		}

		protected abstract string GetTextValue();

		public abstract TPicker CreatePicker();

		UIView CreateAccessoryView()
		{
			var tools = new UIToolbar();
			tools.BarStyle = UIBarStyle.Default;

			tools.Items = CreateButtons().ToArray();
			tools.SizeToFit();
			return tools;
		}

		IEnumerable<UIBarButtonItem> CreateButtons()
		{
			yield return new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (s, ee) => Control.ResignFirstResponder());
			yield return new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			foreach (var item in CreateCustomButtons())
			{
				yield return item;
			}
			yield return new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, ee) =>
			{
				UpdateValue((TPicker)Control.InputView);
				UpdateText();
				Control.ResignFirstResponder();
			});
		}

		protected virtual IEnumerable<UIBarButtonItem> CreateCustomButtons()
		{
			yield break;
		}

		protected void UpdateText()
		{
			LayoutIfNeeded(() => Control.Text = GetTextValue() ?? EmptyText);
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				LayoutIfNeeded(() =>
				{
					base.Font = value;
					Control.Font = value.ToUI();
				});
			}
		}

		protected abstract void UpdateValue(TPicker picker);

		protected abstract void UpdatePicker(TPicker picker);

		void IBasePickerHandler.UpdatePicker(UIView picker)
		{
			UpdatePicker((TPicker)picker);
		}

		void IBasePickerHandler.UpdateValue(UIView picker)
		{
			UpdateValue((TPicker)picker);
		}

		UIView IBasePickerHandler.CreatePicker()
		{
			return CreatePicker();
		}

		UIView IBasePickerHandler.CreateAccessoryView()
		{
			return CreateAccessoryView();
		}

		public virtual bool ShowBorder 
		{
			get { return true; }
			set { }
		}
	}
}
