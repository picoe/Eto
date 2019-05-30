using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

namespace Eto.Serialization.Xaml
{
	class EtoNameScope : INameScope
	{
		public object Instance { get; set; }

		Dictionary<string, object> registeredNames = new Dictionary<string, object>();

		public object FindName (string name)
		{
			object result;
			if (registeredNames.TryGetValue(name, out result))
				return result;
			return null;
		}

		public void RegisterName (string name, object scopedElement)
		{
			if (scopedElement != null)
				registeredNames.Add(name, scopedElement);

			if (Instance == null)
				return;
			var instanceType = Instance.GetType();
			var obj = scopedElement as Widget;
			if (obj != null && !string.IsNullOrEmpty(name))
			{
				#if !NET40
				var property = instanceType.GetRuntimeProperties().FirstOrDefault(r => r.Name == name && r.SetMethod != null &&!r.SetMethod.IsStatic);
				if (property != null)
					property.SetValue(Instance, obj, null);
				else
				{
					var field = instanceType.GetTypeInfo().GetDeclaredField(name);
					if (field != null && !field.IsStatic)
						field.SetValue(Instance, obj);
				}
				#else
				var property = instanceType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property != null)
				property.SetValue(Instance, obj, null);
				else
				{
				var field = instanceType.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				if (field != null)
				field.SetValue(Instance, obj);
				}
				#endif
			}
		}

		public void UnregisterName (string name)
		{
		}
	}
	
}