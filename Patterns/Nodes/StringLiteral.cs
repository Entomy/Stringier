﻿using System;

namespace Stringier.Patterns.Nodes {
	/// <summary>	
	/// Represents a string literal pattern, a pattern matching this exact string.
	/// </summary>
	/// <remarks>
	/// This exists to box <see cref="System.String"/> into something that we can treat as a part of a pattern.
	/// </remarks>
	internal sealed class StringLiteral : Literal, IEquatable<StringLiteral> {
		/// <summary>
		/// The actual <see cref="System.String"/> being matched.
		/// </summary>
		internal readonly String String = "";

		/// <summary>
		/// Initialize a new <see cref="StringLiteral"/> with the given <paramref name="string"/>.
		/// </summary>
		/// <param name="string">The <see cref="System.String"/> to parse.</param>
		internal StringLiteral(String @string) : base(StringComparison.Ordinal) => String = @string;

		/// <summary>
		/// Intialize a new <see cref="StringLiteral"/> with the given <paramref name="string"/>.
		/// </summary>
		/// <param name="string">The <see cref="System.String"/> to parse.</param>
		/// <param name="comparisonType">The <see cref="StringComparison"/> to use when parsing.</param>
		internal StringLiteral(String @string, StringComparison comparisonType) : base(comparisonType) => String = @string;

		/// <summary>
		/// Checks the first character in the <paramref name="source"/> against the header of this node.
		/// </summary>
		/// <remarks>
		/// This is primarily used to check whether a pattern may exist at the current position.
		/// </remarks>
		/// <param name="source">The <see cref="Source"/> to check against.</param>
		/// <returns><c>true</c> if this <see cref="Pattern"/> may be present, <c>false</c> if definately not.</returns>
		internal override Boolean CheckHeader(ref Source source) => String.CheckHeader(ref source);

		/// <summary>
		/// Call the Consume parser of this <see cref="Node"/> on the <paramref name="source"/> with the <paramref name="result"/>.
		/// </summary>
		/// <param name="source">The <see cref="Source"/> to consume.</param>
		/// <param name="result">A <see cref="Result"/> containing whether a match occured and the captured <see cref="String"/>.</param
		internal override void Consume(ref Source source, ref Result result) => String.Consume(ref source, ref result, ComparisonType);

		/// <summary>
		/// Call the Neglect parser of this <see cref="Node"/> on the <paramref name="source"/> with the <paramref name="result"/>.
		/// </summary>
		/// <param name="source">The <see cref="Source"/> to consume.</param>
		/// <param name="result">A <see cref="Result"/> containing whether a match occured and the captured <see cref="String"/>.</param
		internal override void Neglect(ref Source source, ref Result result) => String.Neglect(ref source, ref result, ComparisonType);

		/// <summary>
		/// Determines whether this instance and a specified object have the same value.
		/// </summary>
		/// <param name="node">The <see cref="Node"/> to compare with the current <see cref="Node"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="Node"/> is equal to the current <see cref="Node"/>; otherwise, <c>false</c>.</returns>
		public override Boolean Equals(Node node) {
			switch (node) {
			case StringLiteral other:
				return Equals(other);
			default:
				return false;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/> can be represented by this <see cref="Node"/>.
		/// </summary>
		/// <param name="other">The <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/> to check against this <see cref="Node"/>.</param>.
		/// <returns><c>true</c> if representable; otherwise, <c>false</c>.</returns>
		public override Boolean Equals(ReadOnlySpan<Char> other) => String.Equals(other, ComparisonType);

		/// <summary>
		/// Determines whether the specified <see cref="String"/> can be represented by this <see cref="Node"/>.
		/// </summary>
		/// <param name="other">The <see cref="String"/> to check against this <see cref="Node"/>.</param>
		/// <returns><c>true</c> if representable; otherwise, <c>false</c>.</returns>
		public override Boolean Equals(String other) => other is null ? false : String.Equals(other, ComparisonType);

		/// <summary>
		/// Determines whether this instance and a specified object have the same value.
		/// </summary>
		/// <param name="other">The <see cref="CharLiteral"/> to compare with the current <see cref="Node"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="Node"/> is equal to the current <see cref="Node"/>; otherwise, <c>false</c>.</returns>
		public Boolean Equals(StringLiteral other) => ComparisonType.Equals(other.ComparisonType) && String.Equals(other.String, ComparisonType);

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override Int32 GetHashCode() => String.GetHashCode();

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() => $"{String}";

		//#region Concatenator

		//internal override Node Concatenate(Node Right) {
		//	if (Right is null) {
		//		throw new ArgumentNullException(nameof(Right));
		//	}
		//	switch (Right) {
		//	case StringLiteral right:
		//		if (ComparisonType.Equals(right.ComparisonType)) {
		//			return new StringLiteral(String + right.String);
		//		} else {
		//			goto default;
		//		}
		//	case CharLiteral right:
		//		if (ComparisonType.Equals(right.ComparisonType)) {
		//			return new StringLiteral(String + right.Char);
		//		} else {
		//			goto default;
		//		}
		//	default:
		//		return base.Concatenate(Right);
		//	}
		//}

		//internal override Node Concatenate(String Right) {
		//	if (Right is null) {
		//		throw new ArgumentNullException(nameof(Right));
		//	}
		//	return new StringLiteral(String + Right);
		//}

		//internal override Node Concatenate(Char Right) => new StringLiteral(String + Right);

		//#endregion

		//#region Repeater

		//internal override Node Repeat(Int32 Count) => new StringLiteral(String.Repeat(Count));

		//#endregion
	}
}