
namespace Eto.Drawing
{
	/// <summary>
	/// Common dash styles used for <see cref="DashStyle"/>
	/// </summary>
	/// <seealso cref="Pen.DashStyle"/>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class DashStyles
	{
		static DashStyle solid;
		static DashStyle dash;
		static DashStyle dot;
		static DashStyle dashDot;
		static DashStyle dashDotDot;

		/// <summary>
		/// Gets a solid dash style
		/// </summary>
		/// <value>The solid dash style</value>
		public static DashStyle Solid
		{
			get
			{
				if (solid == null) solid = new DashStyle ();
				return solid;
			}
		}

		/// <summary>
		/// Gets a dash style with a single dash
		/// </summary>
		/// <value>The dash style</value>
		public static DashStyle Dash
		{
			get
			{
				if (dash == null) dash = new DashStyle (0f, 2f, 2f);
				return dash;
			}
		}

		/// <summary>
		/// Gets a dot style
		/// </summary>
		/// <value>The dot dash style</value>
		public static DashStyle Dot
		{
			get
			{
				if (dot == null) dot = new DashStyle (0f, 1f, 1f);
				return dot;
			}
		}

		/// <summary>
		/// Gets the dash dot style
		/// </summary>
		/// <value>The dash dot style</value>
		public static DashStyle DashDot
		{
			get
			{
				if (dashDot == null) dashDot = new DashStyle (0f, 3f, 1f, 1f, 1f);
				return dashDot;
			}
		}

		/// <summary>
		/// Gets the dash dot dot style
		/// </summary>
		/// <value>The dash dot dot style</value>
		public static DashStyle DashDotDot
		{
			get
			{
				if (dashDotDot == null) dashDotDot = new DashStyle (0f, 3f, 1f, 1f, 1f, 1f, 1f);
				return dashDotDot;
			}
		}
	}
}
