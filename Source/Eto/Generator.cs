using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Eto
{
	/// <summary>
	/// Extensions for the <see cref="Platform"/> class
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Obsolete("Use Platform instead")]
	public static class GeneratorExtensions
	{
		/// <summary>
		/// Creates a new instance of the handler of the specified type of <typeparamref name="T"/>
		/// </summary>
		/// <remarks>
		/// This extension should be used when creating instances of a fixed type.
		/// This is a helper so that you can use a null platform variable to create instances with the current
		/// platform without having to do the extra check
		/// </remarks>
		/// <typeparam name="T">Type of handler to create</typeparam>
		/// <param name="generator">Platform to create the instance, or null to use the current platform</param>
		/// <returns>A new instance of a handler</returns>
		public static T Create<T>(this Generator generator)
		{
			return (T)((Platform)generator ?? Platform.Instance).Create(typeof(T));
		}

		/// <summary>
		/// Creates a shared singleton instance of the specified type of <typeparamref name="T"/>
		/// </summary>
		/// <remarks>
		/// This extension should be used when creating shared instances of a fixed type.
		/// This is a helper so that you can use a null platform variable to create instances with the current
		/// platform without having to do the extra check
		/// </remarks>
		/// <param name="generator">Platform to create or get the shared instance, or null to use the current platform</param>
		/// <typeparam name="T">The type of handler to get a shared instance for</typeparam>
		/// <returns>The shared instance of a handler of the given type, or a new instance if not already created</returns>
		public static T CreateShared<T>(this Generator generator)
		{
			return (T)((Platform)generator ?? Platform.Instance).CreateShared(typeof(T));
		}

		/// <summary>
		/// Finds the delegate to create instances of the specified type
		/// </summary>
		/// <typeparam name="T">Type of the handler interface (usually derived from <see cref="Widget.IHandler"/> or another type)</typeparam>
		/// <returns>The delegate to use to create instances of the specified type</returns>
		public static Func<T> Find<T>(this Generator generator)
			where T: class
		{
			return (Func<T>)((Platform)generator ?? Platform.Instance).Find(typeof(T));
		}

		/// <summary>
		/// Gets a shared cache dictionary
		/// </summary>
		/// <remarks>
		/// This is used to cache things like brushes and pens, but can also be used to cache other things for your
		/// application's use.
		/// </remarks>
		/// <param name="generator">Generator to get the cache for</param>
		/// <param name="cacheKey">Unique cache key to load the cache instance</param>
		/// <typeparam name="TKey">The type of the lookup key</typeparam>
		/// <typeparam name="TValue">The type of the lookup value</typeparam>
		public static Dictionary<TKey, TValue> Cache<TKey, TValue>(this Generator generator, object cacheKey)
		{
			return ((Platform)generator ?? Platform.Instance).GetSharedProperty <Dictionary<TKey, TValue>>(cacheKey, () => new Dictionary<TKey, TValue>());
		}
	}

	/// <summary>
	/// Compatibility for transition to <see cref="Platform"/> instead of Generator as the name of this class
	/// </summary>
	[Obsolete("Use Platform instead")]
	public abstract class Generator
	{
		/// <summary>
		/// Obsolete
		/// </summary>
		[Obsolete("Use Platform.Instance instead")]
		public static Platform Current
		{
			get { return Platform.Instance; }
		}

		/// <summary>
		/// Obsolete
		/// </summary>
		[Obsolete("No longer available. Use Platform.WidgetCreated event to do validation if required")]
		public static Platform ValidateGenerator { get; set; }

		/// <summary>
		/// Obsolete
		/// </summary>
		[Obsolete("Use Platform.Initialize instead")]
		public static void Initialize(string generatorType)
		{
			Platform.Initialize(generatorType);
		}

		/// <summary>
		/// Obsolete
		/// </summary>
		[Obsolete("Use Platform.Initialize instead")]
		public static void Initialize(Generator generator)
		{
			Platform.Initialize((Platform)generator);
		}

		/// <summary>
		/// Called by handlers to make sure they use the generator specified by ValidateGenerator
		/// </summary>
		/// <param name="generator"></param>
		[Conditional("DEBUG")]
		[Obsolete("No longer available. Use Platform.WidgetCreated event to do validation if required")]
		public static void Validate(Generator generator)
		{
			if (ValidateGenerator != null && !object.ReferenceEquals(generator, ValidateGenerator))
			{
				throw new EtoException(string.Format(CultureInfo.InvariantCulture, "Expected to use generator {0}", ValidateGenerator));
			}
		}

		#region Move to Platform when removing Generator

		/// <summary>
		/// Gets the ID of this platform
		/// </summary>
		/// <remarks>
		/// The platform ID can be used to determine which platform is currently in use.  The platform
		/// does not necessarily correspond to the OS that it is running on, as for example the GTK platform
		/// can run on OS X and Windows.
		/// </remarks>
		public abstract string ID { get; }

		/// <summary>
		/// Gets a value indicating whether this platform is a mac based platform (MonoMac/XamMac)
		/// </summary>
		/// <value><c>true</c> if this platform is mac; otherwise, <c>false</c>.</value>
		public virtual bool IsMac { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this platform is based on Windows Forms
		/// </summary>
		/// <value><c>true</c> if this platform is window forms; otherwise, <c>false</c>.</value>
		public virtual bool IsWinForms { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this platform is based on WPF
		/// </summary>
		/// <value><c>true</c> if this platform is wpf; otherwise, <c>false</c>.</value>
		public virtual bool IsWpf { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this platform is based on GTK# (2 or 3)
		/// </summary>
		/// <value><c>true</c> if this platform is gtk; otherwise, <c>false</c>.</value>
		public virtual bool IsGtk { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this platform is based on Xamarin.iOS
		/// </summary>
		/// <value><c>true</c> if this platform is ios; otherwise, <c>false</c>.</value>
		public virtual bool IsIos { get { return false; } }

		#endregion

		/// <summary>
		/// Obsolete
		/// </summary>
		[Obsolete("Use Platform.Detect")]
		public static Platform Detect
		{
			get { return Platform.Detect; }
		}

		/// <summary>
		/// Returns true if the current generator has been set.
		/// </summary>
		[Obsolete("Use Platform.Instance and check if it is null instead")]
		public static bool HasCurrent { get { return Platform.Instance != null; } }

		/// <summary>
		/// Obsolete
		/// </summary>
		[Obsolete("Use Platform.Get() instead and check for null instead of catching an exception if failed")]
		public static Generator GetGenerator(string generatorType)
		{
			return Platform.Get(generatorType);
		}
	}
}

