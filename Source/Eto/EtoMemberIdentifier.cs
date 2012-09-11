using System;
#if XAML
using System.Xaml;
#endif

namespace Eto
{
	public class EtoMemberIdentifier : AttachableMemberIdentifier
	{
		public EtoMemberIdentifier (Type declaringType, string memberName)
			: base (declaringType, memberName)
		{
		}
	}

#if !XAML
	public class AttachableMemberIdentifier : IEquatable<AttachableMemberIdentifier>
	{
		public Type DeclaringType { get; private set; }
			
		public string MemberName { get; private set; }

		public AttachableMemberIdentifier (Type declaringType, string memberName)
		{
			this.DeclaringType = declaringType;
			this.MemberName = memberName;
		}
		
		private static bool IsNull (AttachableMemberIdentifier a)
		{
			return object.ReferenceEquals (a, null);
		}
		
		public override bool Equals (object obj)
		{
			AttachableMemberIdentifier other = obj as AttachableMemberIdentifier;
			return this.Equals (other);
		}
		
		public bool Equals (AttachableMemberIdentifier other)
		{
			return !AttachableMemberIdentifier.IsNull (other) && this.DeclaringType == other.DeclaringType && this.MemberName == other.MemberName;
		}
		
		public override int GetHashCode ()
		{
			return ((!(this.DeclaringType != null)) ? 0 : this.DeclaringType.GetHashCode ()) << 5 + ((this.MemberName == null) ? 0 : this.MemberName.GetHashCode ());
		}
		
		public override string ToString ()
		{
			return (!(this.DeclaringType != null)) ? this.MemberName : (this.DeclaringType.FullName + "." + this.MemberName);
		}
		
		public static bool operator == (AttachableMemberIdentifier left, AttachableMemberIdentifier right)
		{
			return (!AttachableMemberIdentifier.IsNull (left)) ? left.Equals (right) : AttachableMemberIdentifier.IsNull (right);
		}
		
		public static bool operator != (AttachableMemberIdentifier left, AttachableMemberIdentifier right)
		{
			return (!AttachableMemberIdentifier.IsNull (left)) ? (AttachableMemberIdentifier.IsNull (right) || left.DeclaringType != right.DeclaringType || left.MemberName != right.MemberName) : (!AttachableMemberIdentifier.IsNull (right));
		}
	}
#endif

}
