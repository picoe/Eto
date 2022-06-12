﻿using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;


namespace Eto.Mac.Forms
{
	public class NotificationHandler : WidgetHandler<NSUserNotification, Notification, Notification.ICallback>, Notification.IHandler
	{
		static Dictionary<string, WeakReference> notifications = new Dictionary<string, WeakReference>();

		internal static NSObject Info_Id = new NSString("eto.id");
		internal static NSObject Info_Data = new NSString("eto.data");

		static NSObject[] s_UserInfoKeys = { Info_Id, Info_Data };

		public NotificationHandler()
		{
			Control = new NSUserNotification();
		}

		protected override void Initialize()
		{
			base.Initialize();
			ID = Guid.NewGuid().ToString();
		}

		static NotificationHandler()
		{
			NSUserNotificationCenter.DefaultUserNotificationCenter.ShouldPresentNotification += (center, notification) => true;
			ApplicationHandler.Instance.HandleEvent(Application.NotificationActivatedEvent);
		}

		public string Message
		{
			get { return Control.InformativeText; }
			set { Control.InformativeText = value ?? string.Empty; }
		}

		public bool RequiresTrayIndicator => false;

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value ?? string.Empty; }
		}

		public string UserData { get; set; }

		static readonly object ContentImage_Key = new object();

		static bool? _SupportsContentImage;
		static bool SupportsContentImage(NSUserNotification notification)
		{
			if (_SupportsContentImage != null)
				return _SupportsContentImage.Value;
			_SupportsContentImage = notification.RespondsToSelector(new Selector("contentImage"));
			return _SupportsContentImage.Value;
		}

		public Image ContentImage
		{
			get { return Widget.Properties.Get<Image>(ContentImage_Key); }
			set
			{
				Widget.Properties.Set(ContentImage_Key, value);
				//Control.SetValueForKey(icon.ToNS(), new NSString("_identityImage"));
				//Control.SetValueForKey(NSNumber.FromBoolean(false), new NSString("_identityImageHasBorder"));
				if (SupportsContentImage(Control))
					Control.ContentImage = value.ToNS();
			}
		}

		public void Show(TrayIndicator indicator = null)
		{
			;
			Control.UserInfo = NSDictionary.FromObjectsAndKeys(new NSObject[] { new NSString(ID), (NSString)UserData }, s_UserInfoKeys);
			NSUserNotificationCenter.DefaultUserNotificationCenter.DeliverNotification(Control);
		}
	}
}
