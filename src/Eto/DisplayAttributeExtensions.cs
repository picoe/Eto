using System;
using System.Reflection;
using System.Linq;
#if NETSTANDARD2_0
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#endif

namespace Eto
{
#if NETSTANDARD1_0
	class BaseAttributeWrapper<T>
		where T: BaseAttributeWrapper<T>.AttributeWrapper, new()
	{
		public abstract class AttributeWrapper
		{
			public object Instance { get; internal set; }
		}

		protected static T Get(Type attributeType, Type type)
		{
			if (attributeType == null)
				return null;
			var instance = type.GetTypeInfo().GetCustomAttribute(attributeType);
			if (instance == null)
				return null;
			var attr = new T();
			attr.Instance = instance;
			return attr;
		}

		protected static T Get(Type attributeType, IPropertyDescriptor descriptor)
		{
			if (attributeType == null)
				return null;
			var instance = descriptor.GetCustomAttribute(attributeType);
			if (instance == null)
				return null;
			var attr = new T();
			attr.Instance = instance;
			return attr;
		}
	}

	class EditorAttributeWrapper : BaseAttributeWrapper<EditorAttributeWrapper.EditorAttribute>
	{
		static Type s_EditorAttributeType =
				Type.GetType("System.ComponentModel.EditorAttribute, System")
				?? Type.GetType("System.ComponentModel.EditorAttribute, System.ComponentModel.TypeConverter")
				?? Type.GetType("System.ComponentModel.EditorAttribute, netstandard");
		static PropertyInfo s_EditorBaseTypeNameProperty = s_EditorAttributeType?.GetRuntimeProperty("EditorBaseTypeName");
		static PropertyInfo s_EditorTypeNameProperty = s_EditorAttributeType?.GetRuntimeProperty("EditorTypeName");

		public class EditorAttribute : AttributeWrapper
		{
			public string EditorBaseTypeName => s_EditorBaseTypeNameProperty?.GetValue(Instance) as string;
			public string EditorTypeName => s_EditorTypeNameProperty?.GetValue(Instance) as string;
		}

		public static EditorAttribute Get(Type type) => Get(s_EditorAttributeType, type);

		public static EditorAttribute Get(IPropertyDescriptor descriptor) => Get(s_EditorAttributeType, descriptor);
	}

	class DisplayAttributeWrapper : BaseAttributeWrapper<DisplayAttributeWrapper.DisplayAttribute>
	{
		static Type s_DisplayAttributeType =
			Type.GetType("System.ComponentModel.DataAnnotations.DisplayAttribute, System.ComponentModel.DataAnnotations")
			?? Type.GetType("System.ComponentModel.DataAnnotations.DisplayAttribute, System.ComponentModel.Annotations");
		static MethodInfo getNameMethod = s_DisplayAttributeType?.GetRuntimeMethod("GetName", new Type[0]);
		static MethodInfo getGroupNameMethod = s_DisplayAttributeType?.GetRuntimeMethod("GetGroupName", new Type[0]);
		static MethodInfo getDescriptionMethod = s_DisplayAttributeType?.GetRuntimeMethod("GetDescription", new Type[0]);

		public class DisplayAttribute : AttributeWrapper
		{
			public string GetName() => getNameMethod?.Invoke(Instance, null) as string;
			public string GetGroupName() => getGroupNameMethod?.Invoke(Instance, null) as string;
			public string GetDescription() => getDescriptionMethod?.Invoke(Instance, null) as string;
		}

		public static DisplayAttribute Get(Type type) => Get(s_DisplayAttributeType, type);

		public static DisplayAttribute Get(IPropertyDescriptor descriptor) => Get(s_DisplayAttributeType, descriptor);
	}

#elif NETSTANDARD2_0
	class BaseAttributeWrapper<T>
		where T : Attribute
	{
		public static T Get(Type type) => type.GetTypeInfo().GetCustomAttribute<T>();

		public static T Get(PropertyDescriptor descriptor) => descriptor.Attributes.OfType<T>().FirstOrDefault();

		public static T Get(IPropertyDescriptor descriptor) => descriptor.GetCustomAttribute<T>();	}

	class EditorAttributeWrapper : BaseAttributeWrapper<EditorAttribute> { }

	class DisplayAttributeWrapper : BaseAttributeWrapper<DisplayAttribute> { }
#else
	Not Implemented.
#endif
}
