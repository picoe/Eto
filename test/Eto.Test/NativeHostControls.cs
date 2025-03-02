using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eto.Test.UnitTests;

namespace Eto.Test
{
	public interface INativeHostControls
	{
		public IEnumerable<NativeHostTest> GetNativeHostTests();
	}
	
	public class NativeHostTest
	{
		Func<object> _createControl;
		
		public NativeHostTest(string name, Func<object> create)
		{
			Name = name;
			_createControl = create;
		}
		public string Name { get; }
		public object CreateControl() => _createControl();

		public override string ToString() => Name;
	}
	
    public static class NativeHostControls
    {
		static NativeHostControls()
		{
			EtoTestSetup.Initialize();
		}
		static INativeHostControls Handler => Platform.Instance.CreateShared<INativeHostControls>();

		public static IEnumerable<NativeHostTest> GetNativeHostTests() => Handler.GetNativeHostTests();
    }
}