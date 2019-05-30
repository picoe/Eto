using System;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// Filter definition for <see cref="Eto.Forms.FilePicker"/> and <see cref="Eto.Forms.FileDialog"/>
	/// </summary>
	/// <remarks>
	/// Each filter defines an option for the user to limit the selection of files in the dialog.
	/// </remarks>
	public class FileFilter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileFilter"/> class.
		/// </summary>
		public FileFilter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileFilter"/> class.
		/// </summary>
		/// <param name="name">Name of the filter to display to the user</param>
		/// <param name="extensions">Extensions of the files to filter by, including the dot for each</param>
		public FileFilter(string name, params string[] extensions)
		{
			this.Name = name;
			this.Extensions = extensions;
		}

		/// <summary>
		/// Gets or sets the name of the filter.
		/// </summary>
		/// <value>The name of the filter.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the extensions to filter the file list
		/// </summary>
		/// <remarks>Each extension should include the period. e.g. ".jpeg", ".png", etc.</remarks>
		/// <value>The extensions.</value>
		public string[] Extensions { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="FileFilter"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="FileFilter"/>.</returns>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="FileFilter"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			int hash;
			hash = Name != null ? Name.GetHashCode() : string.Empty.GetHashCode();
			if (Extensions != null)
				hash ^= Extensions.GetHashCode();
			return hash;
		}

		/// <summary>
		/// Converts a string representation of a filter in the form of "[name]|[ext];[ext];[ext]" to a FileDialogFilter.
		/// </summary>
		/// <param name="filter">String representation of the file dialog filter</param>
		/// <returns>A new file dialog filter with the name and extensions specified in the <paramref name="filter"/> argument</returns>
		public static implicit operator FileFilter(string filter)
		{
			var parts = filter.Split('|');
			if (parts.Length != 2)
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Filter must be in the form of '<name>|<ext>;<ext>;<ext>;"), nameof(filter));

			return new FileFilter
			{
				Name = parts[0],
				Extensions = parts[1].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
			};
		}
	}
}
