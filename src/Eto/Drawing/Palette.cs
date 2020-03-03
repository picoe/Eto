using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents a collection of <see cref="Color"/> objects
	/// </summary>
	/// <remarks>
	/// Typically used for <see cref="IndexedBitmap"/> or other purposes where a collection of colors is needed.
	/// 
	/// This class keeps a cache of 32-bit ARGB values for each element in the collection for faster retrieval. These
	/// values are generated using <see cref="Color.ToArgb"/>.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class Palette : ObservableCollection<Color>
	#if !NETSTANDARD
	, ICloneable
	#endif
	{
		readonly List<int> argb;
		static readonly int[] egaColors = { 0, 1, 2, 3, 4, 5, 20, 7, 56, 57, 58, 59, 60, 61, 62, 63 };

		/// <summary>
		/// Gets the index of standard EGA colors from a 64-color palette
		/// </summary>
		public static int[] EGAColors
		{
			get { return (int[])egaColors.Clone(); }
		}

		/// <summary>
		/// Initializes a new instance of the Pallette class
		/// </summary>
		public Palette()
		{
			argb = new List<int>();
		}

		/// <summary>
		/// Initializes a new instance of the Palette class with the specified colors
		/// </summary>
		/// <param name="colors">Initial colors to add to the palette</param>
		public Palette(IEnumerable<Color> colors)
			: this()
		{
			foreach (var item in colors)
				Add(item);
		}

		/// <summary>
		/// Gets the standard 64-color EGA palette
		/// </summary>
		/// <remarks>
		/// To get the standard 16-colors of an EGA palette, use <see cref="FromEGA"/>
		/// </remarks>
		/// <returns></returns>
		public static Palette GetEgaPalette()
		{
			const float mid = 168f / 255f;
			const float low = 84f / 255f;
			const float high = 252f / 255f;
			var pal = new Palette();
			pal.Add(new Color(0f, 0f, 0f));
			pal.Add(new Color(0f, 0f, mid));
			pal.Add(new Color(0f, mid, 0f));
			pal.Add(new Color(0f, mid, mid));
			pal.Add(new Color(mid, 0f, 0f));
			pal.Add(new Color(mid, 0f, mid));
			pal.Add(new Color(mid, mid, 0f));
			pal.Add(new Color(mid, mid, mid));
			pal.Add(new Color(0f, 0f, low));
			pal.Add(new Color(0f, 0f, high));
			pal.Add(new Color(0f, mid, low));
			pal.Add(new Color(0f, mid, high));
			pal.Add(new Color(mid, 0f, low));
			pal.Add(new Color(mid, 0f, high));
			pal.Add(new Color(mid, mid, low));
			pal.Add(new Color(mid, mid, high));
			pal.Add(new Color(0f, low, 0f));
			pal.Add(new Color(0f, low, mid));
			pal.Add(new Color(0f, high, 0f));
			pal.Add(new Color(0f, high, mid));
			pal.Add(new Color(mid, low, 0f));
			pal.Add(new Color(mid, low, mid));
			pal.Add(new Color(mid, high, 0f));
			pal.Add(new Color(mid, high, mid));
			pal.Add(new Color(0f, low, low));
			pal.Add(new Color(0f, low, high));
			pal.Add(new Color(0f, high, low));
			pal.Add(new Color(0f, high, high));
			pal.Add(new Color(mid, low, low));
			pal.Add(new Color(mid, low, high));
			pal.Add(new Color(mid, high, low));
			pal.Add(new Color(mid, high, high));
			pal.Add(new Color(low, 0f, 0f));
			pal.Add(new Color(low, 0f, mid));
			pal.Add(new Color(low, mid, 0f));
			pal.Add(new Color(low, mid, mid));
			pal.Add(new Color(high, 0f, 0f));
			pal.Add(new Color(high, 0f, mid));
			pal.Add(new Color(high, mid, 0f));
			pal.Add(new Color(high, mid, mid));
			pal.Add(new Color(low, 0f, low));
			pal.Add(new Color(low, 0f, high));
			pal.Add(new Color(low, mid, low));
			pal.Add(new Color(low, mid, high));
			pal.Add(new Color(high, 0f, low));
			pal.Add(new Color(high, 0f, high));
			pal.Add(new Color(high, mid, low));
			pal.Add(new Color(high, mid, high));
			pal.Add(new Color(low, low, 0f));
			pal.Add(new Color(low, low, mid));
			pal.Add(new Color(low, high, 0f));
			pal.Add(new Color(low, high, mid));
			pal.Add(new Color(high, low, 0f));
			pal.Add(new Color(high, low, mid));
			pal.Add(new Color(high, high, 0f));
			pal.Add(new Color(high, high, mid));
			pal.Add(new Color(low, low, low));
			pal.Add(new Color(low, low, high));
			pal.Add(new Color(low, high, low));
			pal.Add(new Color(low, high, high));
			pal.Add(new Color(high, low, low));
			pal.Add(new Color(high, low, high));
			pal.Add(new Color(high, high, low));
			pal.Add(new Color(high, high, high));
			return pal;
		}

		/// <summary>
		/// Gets the standard 16-color palette used in DOS
		/// </summary>
		/// <returns>A new instance of a Palette with the standard 16 DOS colors</returns>
		public static Palette GetDosPalette()
		{
			const float mid = 171f / 255f;
			const float low = 87f / 255f;
			const float high = 1f;

			var pal = new Palette();
			pal.Add(new Color(0f, 0f, 0f));
			pal.Add(new Color(0f, 0f, mid));
			pal.Add(new Color(0f, mid, 0f));
			pal.Add(new Color(0f, mid, mid));
			pal.Add(new Color(mid, 0f, 0f));
			pal.Add(new Color(mid, 0f, mid));
			pal.Add(new Color(mid, low, 0f));
			pal.Add(new Color(mid, mid, mid));
			pal.Add(new Color(low, low, low));
			pal.Add(new Color(low, low, high));
			pal.Add(new Color(low, high, low));
			pal.Add(new Color(low, high, high));
			pal.Add(new Color(high, low, low));
			pal.Add(new Color(high, low, high));
			pal.Add(new Color(high, high, low));
			pal.Add(new Color(high, high, high));
			return pal;
		}

		/// <summary>
		/// Gets the standard 16 colors of the specified EGA palette, at the indexes specified with <see cref="EGAColors"/>
		/// </summary>
		/// <param name="palEGA">EGA palette to get the standard 16 colors from</param>
		/// <returns>A new instance of a palette with the 16 colors at the indexes specified with <see cref="EGAColors"/></returns>
		public static Palette FromEGA(Palette palEGA)
		{
			if (palEGA.Count != 64)
				throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Source palette is not an EGA palette"));
			var output = new Palette();
			for (int i = 0; i < EGAColors.Length; i++)
			{
				output.Add(palEGA[EGAColors[i]]);
			}
			return output;
		}

		/// <summary>
		/// Saves this palette to the specified binary writer in (A)RGB components
		/// </summary>
		/// <remarks>
		/// Each component is saved as a single byte (regardless of the value of <paramref name="shift"/>).
		/// It is saved in the order of Alpha (if <paramref name="includeAlpha"/> is true), Red, Green, then Blue.
		/// </remarks>
		/// <param name="writer">Writer to write the data to</param>
		/// <param name="shift">Shift amount for each component. 0 = 0-255, 1 = 0-128, 2 = 0-64, etc</param>
		/// <param name="includeAlpha">True to include alpha, false to only include RGB components</param>
		public void Save(BinaryWriter writer, int shift = 0, bool includeAlpha = false)
		{
			for (int i = 0; i < Count; i++)
			{
				var c = this[i];
				if (includeAlpha)
					writer.Write((byte)((int)(c.A * byte.MaxValue) >> shift));
				writer.Write((byte)((int)(c.R * byte.MaxValue) >> shift));
				writer.Write((byte)((int)(c.G * byte.MaxValue) >> shift));
				writer.Write((byte)((int)(c.B * byte.MaxValue) >> shift));
			}
		}

		/// <summary>
		/// Loads the palette from the specified binary reader in (A)RGB components
		/// </summary>
		/// <remarks>
		/// Each component is read as a single byte (regardless of the value of <paramref name="shift"/>).
		/// It is read in the order of Alpha (if <paramref name="includeAlpha"/> is true), Red, Green, then Blue.
		/// </remarks>
		/// <param name="reader">Reader to read the data from</param>
		/// <param name="size">Number of palette entried to load</param>
		/// <param name="shift">Shift amount for each component. 0 = 0-255, 1 = 0-128, 2 = 0-64, etc</param>
		/// <param name="includeAlpha">True to include the alpha component, false to only read RGB components</param>
		public void Load(BinaryReader reader, int size, int shift = 0, bool includeAlpha = false)
		{
			Clear();
			for (int i = 0; i < size; i++)
			{
				int alpha = byte.MaxValue;
				if (includeAlpha)
					alpha = (reader.ReadByte() << shift) & 0xff;
				int red = (reader.ReadByte() << shift) & 0xff;
				int green = (reader.ReadByte() << shift) & 0xff;
				int blue = (reader.ReadByte() << shift) & 0xff;
				Add(Color.FromArgb(red, green, blue, alpha));
			}
		}

		/// <summary>
		/// Gets the cached ARGB value of the color at the specified <paramref name="index"/>
		/// </summary>
		/// <param name="index">Index to get the ARGB color for</param>
		/// <returns>A 32-bit ARGB color value of the color at the specified index</returns>
		public int GetRGBColor(int index)
		{
			return argb[index];
		}

		/// <summary>
		/// Adds the specified <paramref name="colors"/> to this palette collection
		/// </summary>
		/// <param name="colors">Colors to add to this palette collection</param>
		public void AddRange(IEnumerable<Color> colors)
		{
			foreach (var item in colors)
				Add(item);
		}

		/// <summary>
		/// Called when inserting a color, to insert the cached argb value of the color
		/// </summary>
		protected override void InsertItem(int index, Color item)
		{
			argb.Insert(index, item.ToArgb());
			base.InsertItem(index, item);
		}

		/// <summary>
		/// Called when setting a color in the palette, to set the cached argb value of the color
		/// </summary>
		protected override void SetItem(int index, Color item)
		{
			argb[index] = item.ToArgb();
			base.SetItem(index, item);
		}

		/// <summary>
		/// Called when clearing the items, to clear the cached argb values
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			argb.Clear();
		}

		/// <summary>
		/// Called when removing an item, to remove the cached argb value of the color
		/// </summary>
		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
			argb.RemoveAt(index);
		}

		/// <summary>
		/// Finds the closest color in this palette to the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Color to use to find the closest color</param>
		/// <returns>Index of the closest entry of the specified <paramref name="color"/></returns>
		public int FindClosest(Color color)
		{
			int closestIndex = 0;
			var colorHsl = new ColorHSL(color);
			var closestDifference = ColorHSL.Distance(colorHsl, new ColorHSL(this[0]));
			for (int i = 1; i < Count; i++)
			{
				var curDifference = ColorHSL.Distance(colorHsl, new ColorHSL(this[i]));
				if (curDifference < closestDifference)
				{
					closestIndex = i;
					closestDifference = curDifference;
				}
				if (Math.Abs(curDifference) < 0.001f)
					break;
			}
			return closestIndex;
		}

		/// <summary>
		/// Finds the index of the specified color, or adds it if it does not exist
		/// </summary>
		/// <param name="color">Color to find/add</param>
		/// <returns>Index of the existing entry in this palette that matches the specified color, or the index of the newly added entry if not found</returns>
		public int FindAddColour(Color color)
		{
			var index = argb.IndexOf(color.ToArgb());
			if (index != -1)
				return index;
			
			Add(color);
			return Count - 1;
		}

		/// <summary>
		/// Gets the hash code for this palette
		/// </summary>
		/// <returns>Hash code of this palette</returns>
		public override int GetHashCode()
		{
			int code = 0;
			for (int i = 0; i < argb.Count; i++)
			{
				code ^= argb[i].GetHashCode();
			}
			return code;
		}

		/// <summary>
		/// Gets a value indicating that this object is equal to the specified <paramref name="obj"/>
		/// </summary>
		/// <param name="obj">Object to compare for equality</param>
		/// <returns>True if the specified obj is a Palette and contains the same colors as this instance, false otherwise</returns>
		public override bool Equals(object obj)
		{
			var p = obj as Palette;
			if (p == null)
				return false;
			if (p.Count != Count)
				return false;
			for (int i = 0; i < p.Count; i++)
			{
				if (p.argb[i] != argb[i])
					return false;
			}
			return true;
		}

		/// <summary>
		/// Creates a clone of this palette
		/// </summary>
		/// <returns>A new instance of a palette with the same color entries as this instance</returns>
		public Palette Clone()
		{
			return new Palette(this);
		}

		#if !NETSTANDARD

		/// <summary>
		/// Creates a clone of this palette
		/// </summary>
		/// <returns>A new instance of a palette with the same color entries as this instance</returns>
		object ICloneable.Clone()
		{
			return Clone();
		}

		#endif
	}
}
