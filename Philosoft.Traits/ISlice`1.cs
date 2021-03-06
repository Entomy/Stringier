﻿using System.Diagnostics.CodeAnalysis;

namespace System.Traits {
	/// <summary>
	/// Indicates the collection is sliceable.
	/// </summary>
	/// <typeparam name="TResult">The resulting type; often itself.</typeparam>
	public interface ISlice<out TResult> {
#if !NETSTANDARD1_3
		/// <summary>
		/// Gets a slice out of the collection within the specified range.
		/// </summary>
		/// <param name="range">The zero-based range of the elements.</param>
		/// <returns>A slice that consists of all the elements of the current collection within the <paramref name="range"/>.</returns>
		[MaybeNull, AllowNull]
		TResult this[Range range] { get; }
#endif

		/// <summary>
		/// Forms a slice out of the collection.
		/// </summary>
		/// <returns>A slice that consists of all elements of the current collection.</returns>
		[return: MaybeNull]
		TResult Slice();

		/// <summary>
		/// Forms a slice out of the collection that begins at a specified index.
		/// </summary>
		/// <param name="start">The index at which to begin the slice</param>
		/// <returns>A slice that consists of all elements of the current collection from <paramref name="start"/> to the end of the collection.</returns>
		[return: MaybeNull]
		TResult Slice(Int32 start);

		/// <summary>
		/// Forms a slice out of the current span starting at a specified index for a specified length.
		/// </summary>
		/// <param name="start">The index at which to begin the slice.</param>
		/// <param name="length">The desired length for the slice.</param>
		/// <returns>A slice that consists of <paramref name="length"/> elements from the current collection starting at <paramref name="start"/>.</returns>
		[return: MaybeNull]
		TResult Slice(Int32 start, Int32 length);
	}
}
