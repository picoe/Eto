namespace Eto.Forms;

/// <summary>
/// Extension methods for the <see cref="IDataStore{T}"/>
/// </summary>
public static class DataStoreExtensions
{
	/// <summary>
	/// Gets the expanded row count of the specified IDataStore&lt;ITreeItem&gt;, taking into account the expanded state of each child.
	/// </summary>
	/// <remarks>
	/// Note that this has to traverse the entire tree up to the <paramref name="index"/>, so it will get slower for large trees
	/// </remarks>
	/// <typeparam name="T">Type of item</typeparam>
	/// <param name="store">The data store to count the rows</param>
	/// <param name="index">The index in <paramref name="store"/> to count up to, or -1 to count all rows</param>
	/// <returns>The total row count including the count of any expanded nodes</returns>
	/// <exception cref="ArgumentNullException">When store is null</exception>
	public static int GetExpandedRowCount<T>(this IDataStore<T> store, int index = -1)
		where T: ITreeItem<T>
	{
		if (store == null) throw new ArgumentNullException(nameof(store));

		int rows = index == -1 ? store.Count : index;
		int count = rows;

		for (int i = 0; i < rows; i++)
		{
			var child = store[i];
			if (child.Expanded && child is IDataStore<T> childStore)
			{
				count += GetExpandedRowCount<T>(childStore, -1);
			}
		}
		return count;
	}
		
	/// <summary>
	/// Gets the row of a path of indecies for each level in a IDataStore&lt;ITreeItem&gt;
	/// </summary>
	/// <typeparam name="T">Type of item</typeparam>
	/// <param name="store">The data store to get the row.</param>
	/// <param name="indexPath">Array of indexes leading to the item to get the row for.</param>
	/// <returns>Row index where the specified index path points to</returns>
	/// <exception cref="ArgumentNullException">When store is null</exception>
	public static int GetRowOfIndexPath<T>(this IDataStore<T> store, int[] indexPath)
		where T: ITreeItem<T>
	{
		if (indexPath == null || indexPath.Length == 0)
			return -1;
				
		var item = store ?? throw new ArgumentNullException(nameof(store));
		int count = item.GetExpandedRowCount(indexPath[0]);
		for (int i = 0; i < indexPath.Length - 1; i++)
		{
			item = item[indexPath[i]] as IDataStore<T>;
			if (item != null)
				count += item.GetExpandedRowCount(indexPath[i + 1]);
		}
		count += indexPath.Length - 1;
		return count;
	}
}