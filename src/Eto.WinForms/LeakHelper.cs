using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace Eto.WinForms
{
	/// <summary>
	/// Helper to fix leaking windows forms controls (NumericUpDown and DataGridView)
	/// Source: http://hacking-code.blogspot.ca/2010/08/unhooking-leaking-object-from.htmls
	/// </summary>
	public class LeakHelper
	{
		// we'll use a static List to cache a reference to the internal list of UserPreferenceChangedEvent listeners so that 
		// we do not need to search for it every time.
		static IList _UserPreferenceChangedList;

		static public void UnhookObject(object pObjectToUnhook)
		{
			//First check for null and get a ref to the UserPreferenceChangedEvent's internal list of listeners if needed.
			if (_UserPreferenceChangedList == null) GetUserPreferenceChangedList();
			//then, scan that list for any delegates that point to pObjectToUnhook.
			SearchListAndRemoveEventHandlers(pObjectToUnhook);
		}

		static void GetUserPreferenceChangedList()
		{
			Type oSystemEventsType = typeof(SystemEvents);

			//Using reflection, get the FieldInfo object for the internal collection of handlers
			// we will use this collection to find the handler we want to unhook and remove it.
			// as you can guess by the naming convention it is a private member.
			FieldInfo oFieldInfo = oSystemEventsType.GetField("_handlers",
								BindingFlags.Static |
								BindingFlags.GetField |
								BindingFlags.FlattenHierarchy |
								BindingFlags.NonPublic);

			// Not there in .NET Core..
			if (oFieldInfo == null)
				return;

			//now, get a reference to the value of this field so that you can manipulate it.
			//pass null to GetValue() because we are working with a static member.
			object oFieldInfoValue = oFieldInfo.GetValue(null);

			//the returned object is of type Dictionary<object, List<SystemEventInvokeInfo>>
			//each of the Lists<> in the Dictionary<> is used to maintain a different event implementation.
			//It may be more efficient to figure out how the UserPreferenceChanged event is keyed here but a quick-and-dirty
			// method is to just scan them all the first time and then cache the List<> object once it's found.

			var dictFieldInfoValue = oFieldInfoValue as IDictionary;
			foreach (object oEvent in dictFieldInfoValue)
			{
				var deEvent = (DictionaryEntry)oEvent;
				var listEventListeners = deEvent.Value as IList;

				//we need to take the first item in the list, get it's delegate and check the type...
				if (listEventListeners.Count > 0 && listEventListeners[0] != null)
				{
					Delegate oDelegate = GetDelegateFromSystemEventInvokeInfo(listEventListeners[0]);
					if (oDelegate is UserPreferenceChangedEventHandler)
					{ _UserPreferenceChangedList = listEventListeners; }
				}
				//if we've found the list, no need to continue searching
				if (_UserPreferenceChangedList != null) break;
			}
		}

		static void SearchListAndRemoveEventHandlers(object pObjectToUnhook)
		{
			if (_UserPreferenceChangedList == null) return; //Do not run if we somehow haven't found the list.

			//unfortunately, SystemEventInvokeInfo is a private class so we can't declare a reference of that type.
			//we will use object and then use reflection to get what we need...
			var listDelegatesToRemove = new List<UserPreferenceChangedEventHandler>();

			//this is NOT threadsafe. Unfortunately, if the collection is modified an exception will be thrown during iteration.
			// This will happen any time another thread hooks or unhooks the UserPreferenceChanged event while we iterate.
			// Modify this to be threadsafe somehow if that is required.
			foreach (object oSystemEventInvokeInfo in _UserPreferenceChangedList)
			{
				var oDelegate = GetDelegateFromSystemEventInvokeInfo(oSystemEventInvokeInfo) as UserPreferenceChangedEventHandler;

				if (oDelegate != null && oDelegate.Target == pObjectToUnhook)
				{
					//at this point we have found an event handler that must be unhooked.
					listDelegatesToRemove.Add(oDelegate);
				}
			}

			//We should unhook using the public method because the internal implementation of this event is unknown.
			// iterating the private internal list is already shady enough without manipulating it directly...
			foreach (UserPreferenceChangedEventHandler itemToRemove in listDelegatesToRemove)
			{
				SystemEvents.UserPreferenceChanged -= itemToRemove;
			}
		}

		static Delegate GetDelegateFromSystemEventInvokeInfo(object pSystemEventInvokeInfo)
		{
			Type typeSystemEventInvokeInfo = pSystemEventInvokeInfo.GetType();
			FieldInfo oTmpFieldInfo = typeSystemEventInvokeInfo.GetField("_delegate",
								BindingFlags.Instance |
								BindingFlags.GetField |
								BindingFlags.FlattenHierarchy |
								BindingFlags.NonPublic);

			//Here we are NOT working with a static field so we will supply the SystemEventInvokeInfo
			// object that we found in the List<> object to the GetValue() function.
			return oTmpFieldInfo.GetValue(pSystemEventInvokeInfo) as Delegate;
		}
	}
}
