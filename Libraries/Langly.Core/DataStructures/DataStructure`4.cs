﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Langly.DataStructures.Filters;

namespace Langly.DataStructures {
	/// <summary>
	/// Represents any possible associative data structure.
	/// </summary>
	/// <typeparam name="TIndex">The type of the indicies of the data structure.</typeparam>
	/// <typeparam name="TElement">The type of the elements in the data structure.</typeparam>
	/// <typeparam name="TSelf">The implementing type; itself.</typeparam>
	/// <typeparam name="TEnumerator">The type of the enumerator for this data structure.</typeparam>
	/// <remarks>
	/// This is extremeley anemic in order to avoid the false assumption and leaky abstraction problems most collection libraries get themselves into. This class only sets up things that are truly common for all data structures.
	/// </remarks>
	[DebuggerDisplay("{ToString(5),nq}")]
	public abstract class DataStructure<TIndex, TElement, TSelf, TEnumerator> : Record<TSelf>,
		ISequence<(TIndex Index, TElement Element), TEnumerator>
		where TSelf : DataStructure<TIndex, TElement, TSelf, TEnumerator>
		where TEnumerator : IEnumerator<(TIndex Index, TElement Element)> {
		/// <summary>
		/// The <see cref="Filter{TIndex, TElement}"/> being used.
		/// </summary>
		/// <remarks>
		/// This is never <see langword="null"/>; a sentinel is used by default.
		/// </remarks>
		[NotNull, DisallowNull]
		protected readonly Filter<TIndex, TElement> Filter;

		/// <summary>
		/// Initializes a new <see cref="DataStructure{TIndex, TElement, TSelf, TEnumerator}"/>.
		/// </summary>
		/// <param name="filter">Flags designating which filters to set up.</param>
		protected DataStructure(Filter filter) {
			if ((filter & DataStructures.Filter.Sparse) != 0) {
				Filter = Sparse<TIndex, TElement>.Instance;
			} else {
				Filter = Null<TIndex, TElement>.Instance;
			}
		}

		/// <inheritdoc/>
		public virtual nint Count { get; protected set; }

		/// <summary>
		/// Determines if the two sequences aren't equal.
		/// </summary>
		/// <param name="left">The lefthand sequence.</param>
		/// <param name="right">The righthand sequence.</param>
		/// <returns><see langword="true"/> if the two sequences aren't equal; otherwise, <see langword="false"/>.</returns>
		public static Boolean operator !=([AllowNull] DataStructure<TIndex, TElement, TSelf, TEnumerator> left, [AllowNull] DataStructure<TIndex, TElement, TSelf, TEnumerator> right) {
			if (left is null && right is null) {
				return false;
			} else if (left is null || right is null) {
				return true;
			} else {
				return !left.Equals(right);
			}
		}

		/// <summary>
		/// Determines if the two sequences are equal.
		/// </summary>
		/// <param name="left">The lefthand sequence.</param>
		/// <param name="right">The righthand sequence.</param>
		/// <returns><see langword="true"/> if the two sequences are equal; otherwise, <see langword="false"/>.</returns>
		public static Boolean operator ==([AllowNull] DataStructure<TIndex, TElement, TSelf, TEnumerator> left, [AllowNull] DataStructure<TIndex, TElement, TSelf, TEnumerator> right) {
			if (left is null && right is null) {
				return true;
			} else if (left is null || right is null) {
				return false;
			} else {
				return left.Equals(right);
			}
		}

		/// <inheritdoc/>
		Boolean IContains<(TIndex Index, TElement Element)>.Contains((TIndex Index, TElement Element) member) {
			TEnumerator ths = GetEnumerator();
			while (ths.MoveNext()) {
				if (ths.Current.Equals(member)) {
					return true;
				}
			}
			return false;
		}

		/// <inheritdoc/>
		public override Boolean Equals([AllowNull] TSelf other) => ReferenceEquals(this, other) || Equals<TEnumerator>(other);

		/// <summary>
		/// Determines if the two sequences are equal.
		/// </summary>
		/// <typeparam name="TOtherEnumerator">The type of the enumerator for the <paramref name="other"/> sequence.</typeparam>
		/// <param name="other">The other sequence.</param>
		/// <returns><see langword="true"/> if the two sequences are equal; otherwise, <see langword="false"/>.</returns>
		public Boolean Equals<TOtherEnumerator>([AllowNull] ISequence<(TIndex Index, TElement Element), TOtherEnumerator> other) where TOtherEnumerator : IEnumerator<(TIndex Index, TElement Element)> {
			// We're calling this off an instance, so if the other is null
			if (other is null) {
				// They aren't equal
				return false;
			}
			// Get enumerators for each
			using TEnumerator ths = GetEnumerator();
			using TOtherEnumerator oth = other.GetEnumerator();
			// Now iterate through both
			while (ths.MoveNext() && oth.MoveNext()) {
				// If the current elements are not equal to each other
				if (!oth.Current.Equals(ths.Current)) {
					// The sequences aren't equal
					return false;
				}
			}
			// If any enumerator can still advance
			if (ths.MoveNext() || oth.MoveNext()) {
				// They aren't the same length and therefore aren't equal
				return false;
			}
			// We've validated that the sequences are the same length, and contain the same elements in the same order, so are therefore equal.
			return true;
		}

		/// <summary>
		/// Determines if the two sequences are equal.
		/// </summary>
		/// <typeparam name="TOtherEnumerator">The type of the enumerator for the <paramref name="other"/> sequence.</typeparam>
		/// <param name="other">The other sequence.</param>
		/// <returns><see langword="true"/> if the two sequences are equal; otherwise, <see langword="false"/>.</returns>
		public Boolean Equals<TOtherEnumerator>([AllowNull] ISequence<IEquals<(TIndex Index, TElement Element)>, TOtherEnumerator> other) where TOtherEnumerator : IEnumerator<IEquals<(TIndex Index, TElement Element)>> {
			// We're calling this off an instance, so if the other is null
			if (other is null) {
				// They aren't equal
				return false;
			}
			// Get enumerators for each
			using TEnumerator ths = GetEnumerator();
			using TOtherEnumerator oth = other.GetEnumerator();
			// Now iterate through both
			while (ths.MoveNext() && oth.MoveNext()) {
				// If the current elements are not equal to each other
				if (!oth.Current.Equals(ths.Current)) {
					// The sequences aren't equal
					return false;
				}
			}
			// If any enumerator can still advance
			if (ths.MoveNext() || oth.MoveNext()) {
				// They aren't the same length and therefore aren't equal
				return false;
			}
			// We've validated that the sequences are the same length, and contain the same elements in the same order, so are therefore equal.
			return true;
		}

		/// <inheritdoc/>
		[return: NotNull]
		public abstract TEnumerator GetEnumerator();

		/// <inheritdoc/>
		public sealed override String ToString() => ToString(5);

		/// <summary>
		/// Returns a string that represents the current object, with no more than <paramref name="amount"/> elements.
		/// </summary>
		/// <param name="amount">The maximum amount of elements to display.</param>
		public String ToString(nint amount) {
			StringBuilder builder = new StringBuilder();
			nint i = 0;
			foreach ((TIndex, TElement) member in this) {
				if (++i == Count) {
					_ = builder.Append(member);
					break;
				} else if (i == amount) {
					_ = builder.Append(member).Append("...");
					break;
				} else {
					_ = builder.Append(member).Append(", ");
				}
			}
			return $"[{builder}]";
		}
	}
}