using System;

namespace Eto
{
	/// <summary>
	/// Member identifier for attached properties
	/// </summary>
	public class EtoMemberIdentifier : AttachableMemberIdentifier
	{
		/// <summary>
		/// Initializes a new instance of the EtoMemberIdentifier class
		/// </summary>
		/// <param name="declaringType">Type that the property is attached to</param>
		/// <param name="memberName">Name of the member/property</param>
		public EtoMemberIdentifier(Type declaringType, string memberName)
			: base(declaringType, memberName)
		{
		}
	}

	/// <summary>
	/// Attachable member identifier for properties, when xaml is not present/available
	/// </summary>
	/// <remarks>
	/// This is used to provide an alternate implementation of the AttachableMemberIdentifier
	/// when compiling without XAML support.
	/// </remarks>
	public class AttachableMemberIdentifier : IEquatable<AttachableMemberIdentifier>
	{
		public Type DeclaringType { get; private set; }

		public string MemberName { get; private set; }

		public AttachableMemberIdentifier(Type declaringType, string memberName)
		{
			this.DeclaringType = declaringType;
			this.MemberName = memberName;
		}

		private static bool IsNull(AttachableMemberIdentifier a)
		{
			return object.ReferenceEquals(a, null);
		}

		public override bool Equals(object obj)
		{
			AttachableMemberIdentifier other = obj as AttachableMemberIdentifier;
			return Equals(other);
		}

		public bool Equals(AttachableMemberIdentifier other)
		{
			return !AttachableMemberIdentifier.IsNull(other) && DeclaringType == other.DeclaringType && MemberName == other.MemberName;
		}

		public override int GetHashCode()
		{
			return ((DeclaringType == null) ? 0 : DeclaringType.GetHashCode()) << 5 + ((MemberName == null) ? 0 : MemberName.GetHashCode());
		}

		public override string ToString()
		{
			return (DeclaringType == null) ? MemberName : (DeclaringType.FullName + "." + MemberName);
		}

		public static bool operator ==(AttachableMemberIdentifier left, AttachableMemberIdentifier right)
		{
			return (!AttachableMemberIdentifier.IsNull(left)) ? left.Equals(right) : AttachableMemberIdentifier.IsNull(right);
		}

		public static bool operator !=(AttachableMemberIdentifier left, AttachableMemberIdentifier right)
		{
			return (!AttachableMemberIdentifier.IsNull(left)) ? (AttachableMemberIdentifier.IsNull(right) || left.DeclaringType != right.DeclaringType || left.MemberName != right.MemberName) : (!AttachableMemberIdentifier.IsNull(right));
		}
	}
}
