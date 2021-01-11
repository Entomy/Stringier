﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Langly.Linguistics;

namespace Langly {
	public static partial class Text {
		#region ToTitle(Element)
		/// <summary>
		/// Converts the value of a UTF-16 code unit to its titlecase form, invariant of orthography.
		/// </summary>
		/// <param name="char">The UTF-16 code unit to convert.</param>
		/// <returns>The titlecase equivalent of <paramref name="char"/>, or the unchanged value of <paramref name="char"/> if <paramref name="char"/> is already titlecase, has no titlecase equivalent, or is not alphabetic.</returns>
		public static Char ToTitle(this Char @char) => (Char)Glyph.Info[@char].Titlecase;

		/// <summary>
		/// Converts the value of a UNICODE character to its titlecase equivalent, invariant of orthography.
		/// </summary>
		/// <param name="rune">The UNICODE character to convert.</param>
		/// <returns>The titlecase equivalent of <paramref name="rune"/>, or the unchanged value of <paramref name="rune"/> if <paramref name="rune"/> is already titlecase, has no titlecase equivalent, or is not alphabetic.</returns>
		public static Rune ToTitle(this Rune rune) => new Rune(Glyph.Info[unchecked((UInt32)rune.Value)].Titlecase);

		/// <summary>
		/// Converts the value of a UNICODE grapheme cluster to its titlecase equivalent, invariant of orthography.
		/// </summary>
		/// <param name="glyph">The UNICODE grapheme cluster to convert.</param>
		/// <returns>The titlecase equivalent of <paramref name="glyph"/>, or the unchanged value of <paramref name="glyph"/> if <paramref name="glyph"/> is already titlecase, has no titlecase equivalent, or is not alphabetic.</returns>
		public static Glyph ToTitle(this Glyph glyph) => new Glyph(Glyph.Info[glyph.Code].Titlecase);

		/// <summary>
		/// Converts the text to its titlecase equivalent, invariant of orthography.
		/// </summary>
		/// <typeparam name="TText">The type of the text.</typeparam>
		/// <param name="text">The text to convert.</param>
		/// <returns>The titlecase equivalent of <paramref name="text"/>, or the unchanged <paramref name="text"/> if <paramref name="text"/> is already titlecase, has no titlecase equivalent, or is not alphabetic. If <paramref name="text"/> is <see langword="null"/> then this method returns <see langword="null"/>, otherwise there is always an instance.</returns>
		[return: MaybeNull, NotNullIfNotNull("text")]
		public static TText ToTitle<TText>([AllowNull] this ICased<TText> text) where TText : class, ICased<TText> => text?.ToTitle();

		#endregion

		#region ToTitle(Element, Orthography)
		/// <summary>
		/// Converts the value of a UTF-16 code unit to its titlecase equivalent, using the casing rules of the specified orthography.
		/// </summary>
		/// <param name="char">The UTF-16 code unit to convert.</param>
		/// <param name="orthography">The <see cref="Orthography"/> instance describing the rules of the orthography.</param>
		/// <returns>The titlecase equivalent of <paramref name="char"/>, or the unchanged value of <paramref name="char"/> if <paramref name="char"/> is already titlecase, has no titlecase equivalent, or is not alphabetic.</returns>
		/// <remarks>
		/// If <paramref name="orthography"/> is <see langword="null"/>, then the invariant should be used.
		/// </remarks>
		public static Char ToTitle(this Char @char, Orthography orthography) => !(orthography is null) ? orthography.ToTitle(@char) : ToTitle(@char);

		/// <summary>
		/// Converts the value of a UNICODE character to its titlecase equivalent, using the casing rules of the specified orthography.
		/// </summary>
		/// <param name="rune">The UNICODE character to convert.</param>
		/// <param name="orthography">The <see cref="Orthography"/> instance describing the rules of the orthography.</param>
		/// <returns>The titlecase equivalent of <paramref name="rune"/>, or the unchanged value of <paramref name="rune"/> if <paramref name="rune"/> is already titlecase, has no titlecase equivalent, or is not alphabetic.</returns>
		/// <remarks>
		/// If <paramref name="orthography"/> is <see langword="null"/>, then the invariant should be used.
		/// </remarks>
		public static Rune ToTitle(this Rune rune, Orthography orthography) => !(orthography is null) ? orthography.ToTitle(rune) : ToTitle(rune);

		/// <summary>
		/// Converts the value of a UNICODE grapheme cluster to its titlecase equivalent, using the casing rules of the specified orthography.
		/// </summary>
		/// <param name="glyph">The UNICODE grapheme cluster to convert.</param>
		/// <param name="orthography">The <see cref="Orthography"/> instance describing the rules of the orthography.</param>
		/// <returns>The titlecase equivalent of <paramref name="glyph"/>, or the unchanged value of <paramref name="glyph"/> if <paramref name="glyph"/> is already titlecase, has no titlecase equivalent, or is not alphabetic.</returns>
		/// <remarks>
		/// If <paramref name="orthography"/> is <see langword="null"/>, then the invariant should be used.
		/// </remarks>
		public static Glyph ToTitle(this Glyph glyph, Orthography orthography) => !(orthography is null) ? orthography.ToTitle(glyph) : ToTitle(glyph);

		/// <summary>
		/// Converts the text to its titlecase equivalent, using the casing rules of the specified orthography.
		/// </summary>
		/// <typeparam name="TText">The type of the text.</typeparam>
		/// <param name="text">The text to convert.</param>
		/// <param name="orthography">The <see cref="Orthography"/> instance describing the rules of the orthography.</param>
		/// <returns>The titlecase equivalent of <paramref name="text"/>, or the unchanged <paramref name="text"/> if <paramref name="text"/> is already titlecase, has no titlecase equivalent, or is not alphabetic. If <paramref name="text"/> is <see langword="null"/> then this method returns <see langword="null"/>, otherwise there is always an instance.</returns>
		/// <remarks>
		/// If <paramref name="orthography"/> is <see langword="null"/>, then the invariant should be used.
		/// </remarks>
		[return: MaybeNull, NotNullIfNotNull("text")]
		public static TText ToTitle<TText>([AllowNull] this ICased<TText> text, [AllowNull] Orthography orthography) where TText : class, ICased<TText> => text?.ToTitle(orthography);
		#endregion
	}
}