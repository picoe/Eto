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

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.AttachableMemberIdentifier"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Eto.AttachableMemberIdentifier"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Eto.AttachableMemberIdentifier"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			var other = obj as AttachableMemberIdentifier;
			return Equals(other);
		}

		/// <summary>
		/// Determines whether the specified <see cref="Eto.AttachableMemberIdentifier"/> is equal to the current <see cref="Eto.AttachableMemberIdentifier"/>.
		/// </summary>
		/// <param name="other">The <see cref="Eto.AttachableMemberIdentifier"/> to compare with the current <see cref="Eto.AttachableMemberIdentifier"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="Eto.AttachableMemberIdentifier"/> is equal to the current
		/// <see cref="Eto.AttachableMemberIdentifier"/>; otherwise, <c>false</c>.</returns>
		public bool Equals(AttachableMemberIdentifier other)
		{
			return !IsNull(other) && DeclaringType == other.DeclaringType && MemberName == other.MemberName;
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.AttachableMemberIdentifier"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return ((DeclaringType == null) ? 0 : DeclaringType.GetHashCode()) << 5 + ((MemberName == null) ? 0 : MemberName.GetHashCode());
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Eto.AttachableMemberIdentifier"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Eto.AttachableMemberIdentifier"/>.</returns>
		public override string ToString()
		{
			return (DeclaringType == null) ? MemberName : (DeclaringType.FullName + "." + MemberName);
		}

		/// <summary>
		/// Compares two AttachableMemberIdentifier objects for equality
		/// </summary>
		/// <param name="left">First member identifier to compare</param>
		/// <param name="right">Second member identifier to compare</param>
		public static bool operator ==(AttachableMemberIdentifier left, AttachableMemberIdentifier right)
		{
			return (!IsNull(left)) ? left.Equals(right) : IsNull(right);
		}

		/// <summary>
		/// Compares two AttachableMemberIdentifier objects for inequality
		/// </summary>
		/// <param name="left">First member identifier to compare</param>
		/// <param name="right">Second member identifier to compare</param>
		public static bool operator !=(AttachableMemberIdentifier left, AttachableMemberIdentifier right)
		{
			return (!IsNull(left)) ? (IsNull(right) || left.DeclaringType != right.DeclaringType || left.MemberName != right.MemberName) : (!IsNull(right));
		}
	}
}
