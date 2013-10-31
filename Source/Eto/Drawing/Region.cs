
namespace Eto.Drawing
{
	/// <summary>
	/// Handler interface for the <see cref="Region"/> class
	/// </summary>
	public interface IRegion : IInstanceWidget
	{
		/// <summary>
		/// Excludes the specified <paramref name="rectangle"/> from the region
		/// </summary>
		/// <param name="rectangle">Rectangle to exclude</param>
		void Exclude (Rectangle rectangle);

		/// <summary>
		/// Resets the region
		/// </summary>
		void Reset ();

		/// <summary>
		/// Sets the specified <paramref name="rectangle"/> in the region
		/// </summary>
		/// <param name="rectangle">Rectangle to set the region to</param>
		void Set (Rectangle rectangle);
	}

	/// <summary>
	/// Defines a region to use for clipping
	/// </summary>
	public class Region : InstanceWidget
	{
		new IRegion Handler { get { return (IRegion)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the Region class
		/// </summary>
		public Region ()
			: this (Generator.Current)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Region class using the specified generator to instantiate the handler
		/// </summary>
		/// <param name="generator">Generator to use for instantiating the handler</param>
		public Region (Generator generator)
			: base (generator, typeof(IRegion))
		{
		}

		/// <summary>
		/// Excludes the specified <paramref name="rectangle"/> from the region
		/// </summary>
		/// <param name="rectangle">Rectangle to exclude</param>
		public void Exclude (Rectangle rectangle)
		{
			Handler.Exclude (rectangle);
		}

		/// <summary>
		/// Resets the region
		/// </summary>
		public void Reset ()
		{
			Handler.Reset ();
		}

		/// <summary>
		/// Sets the specified <paramref name="rectangle"/> in the region
		/// </summary>
		/// <param name="rectangle">Rectangle to set the region to</param>
		public void Set (Rectangle rectangle)
		{
			Handler.Set (rectangle);
		}
	}

}
