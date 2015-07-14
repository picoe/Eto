using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Collection of bindings
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BindingCollection : Collection<IBinding>
	{
		/// <summary>
		/// Unbinds all bindings in the collection
		/// </summary>
		/// <remarks>
		/// Unbinding is used to remove all event handlers on objects so they can be garbage collected.
		/// </remarks>
		public void Unbind()
		{
			foreach (var binding in this)
			{
				binding.Unbind();
			}
		}

		/// <summary>
		/// Updates all bindings manually
		/// </summary>
		/// <remarks>
		/// Bindings can automatically update if enabled and there are sufficient property changed event(s),
		/// However in some cases you will want to update the bindings manually, for example if you want to save
		/// the data on the form, it would validate first, then update the bound object(s) with the updated values.
		/// </remarks>
		public void Update(BindingUpdateMode mode = BindingUpdateMode.Source)
		{
			foreach (var binding in this)
			{
				binding.Update(mode);
			}
		}
	}

}
