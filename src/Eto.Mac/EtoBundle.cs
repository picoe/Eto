namespace Eto.Mac
{
	static class MarshalDelegates
	{
		// delegates used for marshalling in .NET Core, as it doesn't support marshalling Func<> or Action<T>
		public delegate bool Func_IntPtr_IntPtr_IntPtr_IntPtr_bool(IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4);
		public delegate IntPtr Func_IntPtr_IntPtr_IntPtr(IntPtr arg1, IntPtr arg2);
		public delegate NSRange Func_IntPtr_IntPtr_NSRange(IntPtr arg1, IntPtr arg2);
		public delegate IntPtr Func_IntPtr_IntPtr_NSRange_IntPtr_IntPtr(IntPtr arg1, IntPtr arg2, NSRange arg3, IntPtr arg4);
		public delegate nuint Func_IntPtr_IntPtr_CGPoint_nuint(IntPtr arg1, IntPtr arg2, CGPoint arg3);
		public delegate CGRect Func_IntPtr_IntPtr_NSRange_IntPtr_CGRect(IntPtr arg1, IntPtr arg2, NSRange arg3, IntPtr arg4);
		public delegate void Action_IntPtr_IntPtr_IntPtr(IntPtr arg1, IntPtr arg2, IntPtr arg3);
		public delegate bool Func_IntPtr_IntPtr_bool(IntPtr arg1, IntPtr arg2);
		public delegate bool Func_IntPtr_IntPtr_IntPtr_bool(IntPtr arg1, IntPtr arg2, IntPtr arg3);
		public delegate void Action_IntPtr_IntPtr_CGRect(IntPtr arg1, IntPtr arg2, CGRect arg3);
		public delegate void Action_IntPtr_IntPtr(IntPtr arg1, IntPtr arg2);
		public delegate NSDragOperation Func_IntPtr_IntPtr_IntPtr_NSDragOperation(IntPtr arg1, IntPtr arg2, IntPtr arg3);
		public delegate void Action_IntPtr_IntPtr_CGSize(IntPtr arg1, IntPtr arg2, CGSize arg3);
		public delegate void Action_IntPtr_IntPtr_IntPtr_NSRange(IntPtr arg1, IntPtr arg2, IntPtr arg3, NSRange range);
		public delegate void Action_IntPtr_IntPtr_IntPtr_NSRange_NSRange(IntPtr arg1, IntPtr arg2, IntPtr arg3, NSRange range, NSRange range2);
	}

	public static class EtoBundle
	{
		
		static readonly IntPtr selEtoLoadNibNamed = Selector.GetHandle ("etoLoadNibNamed:owner:");
		static readonly IntPtr selLoadNibNamed = Selector.GetHandle ("loadNibNamed:owner:");
		
		public static void Init ()
		{
			var bundleClass = ObjCExtensions.GetMetaClass ("NSBundle");
			bundleClass.AddMethod (selEtoLoadNibNamed, EtoLoadNibNamed_Delegate, "B@:@@");
			bundleClass.ExchangeMethod (selLoadNibNamed, selEtoLoadNibNamed);
		}

		static MarshalDelegates.Func_IntPtr_IntPtr_IntPtr_IntPtr_bool EtoLoadNibNamed_Delegate = EtoLoadNibNamed;
		static bool EtoLoadNibNamed (IntPtr self, IntPtr sel, IntPtr filePath, IntPtr owner)
		{
			var str = Messaging.GetNSObject<NSString>(filePath);
			return str == null || str.Length == 0 || Messaging.bool_objc_msgSend_IntPtr_IntPtr(self, selEtoLoadNibNamed, filePath, owner);
		}
	}
}

