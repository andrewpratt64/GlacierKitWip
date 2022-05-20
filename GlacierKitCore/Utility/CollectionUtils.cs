using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Utility
{
	/// <summary>
	/// Collection-related utilities
	/// </summary>
	public static class CollectionUtils
	{
		/// <summary>
		/// Generalized description of the size of a single collection
		/// </summary>
		public enum ECollectionSizeType : sbyte
		{
			/// <summary>
			/// The collection is null
			/// </summary>
			Null = ECollectionSizeFlags.Null,
			/// <summary>
			/// The collection has no items
			/// </summary>
			Empty = ECollectionSizeFlags.Empty,
			/// <summary>
			/// The collection has exactly one item
			/// </summary>
			SingleItem = ECollectionSizeFlags.SingleItem,
			/// <summary>
			/// The collection has more than one item
			/// </summary>
			SeveralItems = ECollectionSizeFlags.SeveralItems
		}


		/// <summary>
		/// Bitflags for addressing collections based on their size
		/// </summary>
		[Flags]
		public enum ECollectionSizeFlags : sbyte
		{
			/// <summary>
			/// Excludes everything, including null
			/// </summary>
			None = 0,
			/// <summary>
			/// Includes null
			/// </summary>
			Null = 1,
			/// <summary>
			/// Includes empty collections
			/// </summary>
			Empty = 2,
			/// <summary>
			/// Includes collections with only one item
			/// </summary>
			SingleItem = 4,
			/// <summary>
			/// Includes collections with more than one item
			/// </summary>
			SeveralItems = 8,
			/// <summary>
			/// Includes collections with one or more items
			/// </summary>
			AnyItems = SingleItem | SeveralItems,
			/// <summary>
			/// Includes null and empty collections
			/// </summary>
			NullOrEmpty = Null | Empty,
			/// <summary>
			/// Includes all collections that are not null
			/// </summary>
			NotNull = ~Null
		}


		/// <summary>
		/// Get the ECollectionSizeFlags of a given ECollectionSizeType instance
		/// </summary>
		/// <param name="sizeType">Size type to convert</param>
		/// <returns><paramref name="sizeType"/> as a ECollectionSizeFlags object</returns>
		public static ECollectionSizeFlags SizeFlagsOfType(ECollectionSizeType sizeType)
		{
			return (ECollectionSizeFlags)sizeType;
		}


		/// <summary>
		/// Get information about the size of a given collection
		/// </summary>
		/// <param name="collection">The collection to who's size is to be inspected</param>
		/// <returns>Bitflags representing the details of the size of <paramref name="collection"/></returns>
		public static ECollectionSizeFlags GetCollectionSizeFlagsOf(ICollection? collection)
		{
			if (collection == null)
			{
				return ECollectionSizeFlags.Null;
			}

			int size = collection.Count;
			if (size > 1)
				return ECollectionSizeFlags.SeveralItems;
			else if (size > 0)
				return ECollectionSizeFlags.SingleItem;
			return ECollectionSizeFlags.Empty;
		}
	}
}
