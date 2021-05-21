﻿using System.Diagnostics.CodeAnalysis;

namespace Stringier {
	/// <summary>
	/// Represents the concatenation of two <see cref="Pattern"/>. That is, one <see cref="Pattern"/> directly after another.
	/// </summary>
	internal sealed class Concatenator : Combinator {
		/// <summary>
		/// The lefthand <see cref="Pattern"/>; the first.
		/// </summary>
		[DisallowNull, NotNull]
		private readonly Pattern Left;

		/// <summary>
		/// The righthand <see cref="Pattern"/>; the last.
		/// </summary>
		[DisallowNull, NotNull]
		private readonly Pattern Right;

		/// <summary>
		/// Initialize a new <see cref="Concatenator"/> with the <paramref name="left"/> and <paramref name="right"/> <see cref="Pattern"/>.
		/// </summary>
		/// <param name="left">The lefthand <see cref="Pattern"/>; the first.</param>
		/// <param name="right">The righthand <see cref="Pattern"/>; the last.</param>
		internal Concatenator([DisallowNull] Pattern left, [DisallowNull] Pattern right) {
			Left = left;
			Right = right;
		}
	}
}