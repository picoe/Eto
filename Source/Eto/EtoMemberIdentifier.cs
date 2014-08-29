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
		/// <summary>
		/// Gets the type that declared the member.
		/// </summary>
		/// <value>The type of the declaring class.</value>
		public Type DeclaringType { get; private set; }

		/// <summary>
		/// Gets the name of the member.
		/// </summary>
		/// <value>The name of the member.</value>
		public string MemberName { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.AttachableMemberIdentifier"/> class.
		/// </summary>
		/// <param name="declaringType">Declaring type.</param>
		/// <param name="memberName">Name of the member.</param>
		public AttachableMemberIdentifier(Type declaringType, string memberName)
		{
			this.DeclaringType = declaringType;
			this.MemberName = memberName;
		}

		static bool IsNull(AttachableMemberIdentifier a)
		{
			return ReferenceEquals(a, null);
		}

		public override bool Equals(object obj)
		{
			var other = obj as AttachableMemberIdentifier;
			return Equals(other);
		}

		public bool Equals(AttachableMemberIdentifier other)
		{
			return !IsNull(other) && DeclaringType == other.DeclaringType && MemberName == other.MemberName;
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
			return (!IsNull(left)) ? left.Equals(right) : IsNull(right);
		}

		public static bool operator !=(AttachableMemberIdentifier left, AttachableMemberIdentifier right)
		{
			return (!IsNull(left)) ? (IsNull(right) || left.DeclaringType != right.DeclaringType || left.MemberName != right.MemberName) : (!IsNull(right));
		}
	}
}
