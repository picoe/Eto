using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Eto.Forms;

[DataContract(Name = nameof(ObjectData))]
class ObjectDataSurrogate
{
	[DataMember]
	public string TypeName { get; set; }
}

[DataContract]
class ObjectData
{
	internal static bool CanSerialize(Type type) => type == null || type.IsSerializable || type.GetCustomAttribute<DataContractAttribute>() != null;
	internal static byte[] Serialize(object value, Type baseType)
	{
		var data = new ObjectData
		{
			TypeName = baseType.AssemblyQualifiedName,
			Value = value
		};

		using (var ms = new MemoryStream())
		{
			var serializer = new DataContractSerializer(typeof(ObjectData), new [] { baseType });
			serializer.WriteObject(ms, data);
			
			// for debugging
			// var str = Encoding.UTF8.GetString(ms.ToArray());
			
			return ms.ToArray();
		}
	}

	internal static object Deserialize(Stream stream, Type objectType)
	{
		MemoryStream ms = null;

		// for debugging
		// var str = new StreamReader(stream).ReadToEnd();
		// stream.Position = 0;

		if (objectType == null)
		{
			// we need to be able to seek so we can read twice
			if (!stream.CanSeek)
			{
				ms = new MemoryStream();
				stream.CopyTo(ms);
				stream = ms;
			}
			
			// we don't know the type, read it from the stream first
			var serializer = new DataContractSerializer(typeof(ObjectDataSurrogate));
			var dataType = serializer.ReadObject(stream) as ObjectDataSurrogate;
			if (dataType == null)
			{
				ms?.Dispose();
				return null;
			}
			objectType = Type.GetType(dataType.TypeName, false);
			if (objectType == null)
			{
				ms?.Dispose();
				return null;
			}
		}

		{
			// read again, but with known type populated
			stream.Position = 0;
			var serializer = new DataContractSerializer(typeof(ObjectData), new[] { objectType });
			var data = serializer.ReadObject(stream) as ObjectData;

			ms?.Dispose();

			if (data == null)
				return null;

			return data.Value;
		}
	}

	[DataMember]
	public string TypeName { get; set; }
	[DataMember]
	public object Value { get; set; }

}
