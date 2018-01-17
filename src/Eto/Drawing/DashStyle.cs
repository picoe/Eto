using System;
using System.Linq;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Dash style for a <see cref="Pen"/>
	/// </summary>
	/// <seealso cref="Pen.DashStyle"/>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public sealed class DashStyle : IEquatable<DashStyle>
	{
		readonly float[] dashes;
		readonly float offset;

		/// <summary>
		/// Gets the dashes and gaps for this style
		/// </summary>
		/// <remarks>
		/// The values specified are the dash lengths and gap lengths in alternating order.
		/// The lengths are multiplied by the thickness of the pen.
		/// 
		/// For example, values of 2, 1 would have a dash of (2 * thickness) followed by a gap of (1 * thickness).
		/// </remarks>
		/// <value>The dashes to use for a pen</value>
		public float[] Dashes { get { return dashes; } }

		/// <summary>
		/// Gets the offset of the first dash
		/// </summary>
		/// <remarks>
		/// A value of 1 indicates that the first dash should start at the (1*thickness) of the pen.
		/// </remarks>
		/// <value>The offset of the first dash, in multiples of pen thickness</value>
		public float Offset { get { return offset; } }

		/// <summary>
		/// Gets a value indicating whether this dash style is solid
		/// </summary>
		/// <value><c>true</c> if this instance is solid; otherwise, <c>false</c>.</value>
		public bool IsSolid
		{
			get { return Dashes == null || Dashes.Length == 0; }
		}

		/// <summary>
		/// Attempts to parse the specified <paramref name="value"/> into a dash style.  This can be one of the
		/// system styles (solid, dash, dot, dashdot, dashdotdot), or a series of numbers separated by commas 
		/// specifying the solid and gap parts (see <see cref="Dashes"/>)
		/// </summary>
		/// <param name="value">String value to parse</param>
		/// <param name="style">DashStyle representation of the specified value if successful</param>
		/// <returns>True if successful, or false if the value could not be parsed</returns>
		public static bool TryParse(string value, out DashStyle style)
		{
			if (string.IsNullOrEmpty(value))
			{
				style = DashStyles.Solid;
				return true;
			}

			switch (value.ToUpperInvariant())
			{
				case "SOLID":
					style = DashStyles.Solid;
					return true;
				case "DASH":
					style = DashStyles.Dash;
					return true;
				case "DOT":
					style = DashStyles.Dot;
					return true;
				case "DASHDOT":
					style = DashStyles.DashDot;
					return true;
				case "DASHDOTDOT":
					style = DashStyles.DashDotDot;
					return true;
			}

			var values = value.Split(',');
			if (values.Length == 0)
			{
				style = DashStyles.Solid;
				return true;
			}
			float offset;
			if (!float.TryParse(values[0], out offset))
				throw new ArgumentOutOfRangeException("value", value);
			float[] dashes = null;
			if (values.Length > 1)
			{
				dashes = new float [values.Length - 1];
				for (int i = 0; i < dashes.Length; i++)
				{
					float dashValue;
					if (!float.TryParse(values[i + 1], out dashValue))
						throw new ArgumentOutOfRangeException("value", value);
					dashes[i] = dashValue;
				}
			}

			style = new DashStyle(offset, dashes);
			return true;
		}

		/// <summary>
		/// Attempts to parse the specified <paramref name="value"/> into a dash style.  This can be one of the
		/// system styles (solid, dash, dot, dashdot, dashdotdot), or a series of numbers separated by commas 
		/// specifying the solid and gap parts (see <see cref="Dashes"/>). 
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Raised when the value could not be converted</exception>
		/// <param name="value">String value to parse</param>
		/// <returns>DashStyle representation of the specified value if successful</returns>
		public static DashStyle Parse(string value)
		{
			DashStyle style;
			if (TryParse(value, out style))
				return style;
			throw new ArgumentOutOfRangeException("value", value, string.Format(CultureInfo.CurrentCulture, "Cannot convert value to a color"));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.DashStyle"/> class.
		/// </summary>
		public DashStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.DashStyle"/> class.
		/// </summary>
		/// <param name="offset">Offset of the first dash in the style</param>
		/// <param name="dashes">Dashes to use for the style.  See <see cref="Dashes"/></param>
		public DashStyle(float offset, params float[] dashes)
		{
			if (dashes != null && dashes.Any(r => r <= 0))
				throw new ArgumentOutOfRangeException("dashes", dashes, string.Format(CultureInfo.CurrentCulture, "Each dash or gap must have a size greater than zero"));
			this.offset = offset;
			this.dashes = dashes;
		}

		/// <summary>
		/// Compares two DashStyle objects for equality
		/// </summary>
		/// <param name="style1">First style to compare</param>
		/// <param name="style2">Second style to compare</param>
		public static bool operator ==(DashStyle style1, DashStyle style2)
		{
			if (ReferenceEquals(style1, style2))
				return true;
			if (ReferenceEquals(style1, null) || ReferenceEquals(style2, null))
				return false;
			if (Math.Abs(style1.Offset - style2.Offset) > 0.01f)
				return false;
			if (style1.Dashes == null)
				return style2.Dashes == null;
			return style2.Dashes != null && style1.Dashes.SequenceEqual(style2.Dashes);
		}

		/// <summary>
		/// Compares two DashStyle objects for inequality
		/// </summary>
		/// <param name="style1">First style to compare</param>
		/// <param name="style2">Second style to compare</param>
		public static bool operator !=(DashStyle style1, DashStyle style2)
		{
			return !(style1 == style2);
		}

		/// <summary>
		/// Determines whether the specified <see cref="Eto.Drawing.DashStyle"/> is equal to the current <see cref="Eto.Drawing.DashStyle"/>.
		/// </summary>
		/// <param name="other">The <see cref="Eto.Drawing.DashStyle"/> to compare with the current <see cref="Eto.Drawing.DashStyle"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="Eto.Drawing.DashStyle"/> is equal to the current
		/// <see cref="Eto.Drawing.DashStyle"/>; otherwise, <c>false</c>.</returns>
		public bool Equals(DashStyle other)
		{
			return this == other;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.Drawing.DashStyle"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Eto.Drawing.DashStyle"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Eto.Drawing.DashStyle"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return this == obj as DashStyle;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Eto.Drawing.DashStyle"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Eto.Drawing.DashStyle"/>.</returns>
		public override string ToString()
		{
			if (object.ReferenceEquals(this, DashStyles.Dash))
				return "dash";
			if (object.ReferenceEquals(this, DashStyles.Dot))
				return "dot";
			if (object.ReferenceEquals(this, DashStyles.DashDot))
				return "dashdot";
			if (object.ReferenceEquals(this, DashStyles.DashDotDot))
				return "dashdotdot";
			if (object.ReferenceEquals(this, DashStyles.Solid))
				return "solid";
			return string.Format(CultureInfo.InvariantCulture, "{0},{1}", Offset, string.Join(",", Dashes.Select(r => r.ToString(CultureInfo.InvariantCulture))));
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.Drawing.DashStyle"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			int hash = offset.GetHashCode();
			if (dashes != null)
			{
				for (int i = 0; i < dashes.Length; i++)
					hash ^= dashes[i].GetHashCode();
			}
			return hash;
		}
	}
}
