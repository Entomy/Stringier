﻿using Defender;
using System;
using System.Globalization;
using System.Text;
using Stringier.Categories;

namespace Stringier {
	public static partial class Text {
		#region Trim(Text)
		/// <summary>
		/// Removes all the leading and trailing white-space characters from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <returns>The string that remains after all white-space characters are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static String Trim(this String text) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan()).ToString();
		}

		/// <summary>
		/// Removes all the leading and trailing white-space characters from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <returns>The string that remains after all white-space characters are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this Char[] text) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan());
		}

		/// <summary>
		/// Removes all the leading and trailing white-space characters from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <returns>The string that remains after all white-space characters are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this Span<Char> text) => Trim((ReadOnlySpan<Char>)text);

		/// <summary>
		/// Removes all the leading and trailing white-space characters from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <returns>The string that remains after all white-space characters are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this ReadOnlySpan<Char> text) {
			Int32 start = 0;
			while (WhiteSpace.Contains(text[start])) {
				start++;
			}
			return text.Slice(start);
		}

		/// <summary>
		/// Removes all the leading and trailing white-space characters from the current string.
		/// </summary>
		/// <param name="text">Pointer to the characters.</param>
		/// <param name="length">The length of the <paramref name="text"/>.</param>
		/// <returns>The string that remains after all white-space characters are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		[CLSCompliant(false)]
		public static unsafe ReadOnlySpan<Char> Trim(Char* text, Int32 length) {
			Guard.NotNull(text, nameof(text));
			return Trim(new ReadOnlySpan<Char>(text, length));
		}
		#endregion

		#region Trim(Text, Char)
		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified character from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimChar">The Unicode character to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="trimChar"/> character are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static String Trim(this String text, Char trimChar) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), trimChar).ToString();
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified character from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimChar">The Unicode character to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="trimChar"/> character are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this Char[] text, Char trimChar) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), trimChar);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified character from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimChar">The Unicode character to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="trimChar"/> character are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this Span<Char> text, Char trimChar) => Trim((ReadOnlySpan<Char>)text, trimChar);

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified character from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimChar">The Unicode character to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="trimChar"/> character are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this ReadOnlySpan<Char> text, Char trimChar) {
			Int32 start = 0;
			while (text[start] == trimChar) {
				start++;
			}
			return text.Slice(start);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified character from the current string.
		/// </summary>
		/// <param name="text">Pointer to the characters.</param>
		/// <param name="length">The length of the <paramref name="text"/>.</param>
		/// <param name="trimChar">The Unicode character to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="trimChar"/> character are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		[CLSCompliant(false)]
		public static unsafe ReadOnlySpan<Char> Trim(Char* text, Int32 length, Char trimChar) => MemoryExtensions.Trim(new ReadOnlySpan<Char>(text, length), trimChar);
		#endregion

		#region Trim(Text, Char[])
		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of characters specified in an array from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null"/>.</param>
		/// <returns>The string that remains after all occurrences of characters in the <paramref name="trimChars"/> parameter are removed from the start and end of the current string. If <paramref name="trimChars"/> is <see langword="null"/> or an empty array, white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static String Trim(this String text, params Char[] trimChars) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), trimChars).ToString();
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of characters specified in an array from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null"/>.</param>
		/// <returns>The string that remains after all occurrences of characters in the <paramref name="trimChars"/> parameter are removed from the start and end of the current string. If <paramref name="trimChars"/> is <see langword="null"/> or an empty array, white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this Char[] text, params Char[] trimChars) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), trimChars);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of characters specified in an array from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null"/>.</param>
		/// <returns>The string that remains after all occurrences of characters in the <paramref name="trimChars"/> parameter are removed from the start and end of the current string. If <paramref name="trimChars"/> is <see langword="null"/> or an empty array, white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this Span<Char> text, params Char[] trimChars) => Trim((ReadOnlySpan<Char>)text, trimChars);

		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of characters specified in an array from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null"/>.</param>
		/// <returns>The string that remains after all occurrences of characters in the <paramref name="trimChars"/> parameter are removed from the start and end of the current string. If <paramref name="trimChars"/> is <see langword="null"/> or an empty array, white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this ReadOnlySpan<Char> text, params Char[] trimChars) {
			if (trimChars is null || trimChars.Length == 0) {
				return text.Trim();
			}
			Int32 start = 0;
			foreach (Char t in text) {
				if (!trimChars.Contains(t)) {
					goto Done;
				}
				start++;
			}
		Done:
			return text.Slice(start);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of characters specified in an array from the current string.
		/// </summary>
		/// <param name="text">Pointer to the characters.</param>
		/// <param name="length">The length of the <paramref name="text"/>.</param>
		/// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null"/>.</param>
		/// <returns>The string that remains after all occurrences of characters in the <paramref name="trimChars"/> parameter are removed from the start and end of the current string. If <paramref name="trimChars"/> is <see langword="null"/> or an empty array, white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		[CLSCompliant(false)]
		public static unsafe ReadOnlySpan<Char> Trim(Char* text, Int32 length, params Char[] trimChars) {
			Guard.NotNull(text, nameof(text));
			return Trim(new ReadOnlySpan<Char>(text, length), trimChars);
		}
		#endregion

		#region Trim(Text, Rune)
		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified rune from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimRune">The <see cref="Rune"/> to remove.</param>
		/// <returns>
		/// The string that remains after all occurrences of the <paramref name="trimRune"/> rune are removed from the start and end of the current string. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		public static String Trim(this String text, Rune trimRune) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), trimRune).ToString();
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified rune from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimRune">The <see cref="Rune"/> to remove.</param>
		/// <returns>
		/// The string that remains after all occurrences of the <paramref name="trimRune"/> rune are removed from the start and end of the current string. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		public static ReadOnlySpan<Char> Trim(this Char[] text, Rune trimRune) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), trimRune);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified rune from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimRune">The <see cref="Rune"/> to remove.</param>
		/// <returns>
		/// The string that remains after all occurrences of the <paramref name="trimRune"/> rune are removed from the start and end of the current string. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		public static ReadOnlySpan<Char> Trim(this Span<Char> text, Rune trimRune) => Trim((ReadOnlySpan<Char>)text, trimRune);

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified rune from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimRune">The <see cref="Rune"/> to remove.</param>
		/// <returns>
		/// The string that remains after all occurrences of the <paramref name="trimRune"/> rune are removed from the start and end of the current string. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		public static ReadOnlySpan<Char> Trim(this ReadOnlySpan<Char> text, Rune trimRune) {
			Int32 start = 0;
			Rune rune;
			while ((rune = text.GetRuneAt(start)) == trimRune) {
				start += rune.Utf16SequenceLength;
			}
			Int32 end = start;
			while ((rune = text.GetRuneAt(end)) != trimRune) {
				end += rune.Utf16SequenceLength;
			}
			return text.Slice(start, end - start);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified rune from the current string.
		/// </summary>
		/// <param name="text">Pointer to the characters.</param>
		/// <param name="length">The length of the <paramref name="text"/>.</param>
		/// <param name="trimRune">The <see cref="Rune"/> to remove.</param>
		/// <returns>
		/// The string that remains after all occurrences of the <paramref name="trimRune"/> rune are removed from the start and end of the current string. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		[CLSCompliant(false)]
		public static unsafe ReadOnlySpan<Char> Trim(Char* text, Int32 length, Rune trimRune) => Trim(new ReadOnlySpan<Char>(text, length), trimRune);
		#endregion

		#region Trim(Text, Rune[])
		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of runes specified in an array from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimRunes">An array of <see cref="Rune"/> to remove, or <see langword="null"/>.</param>
		/// <returns>
		/// The string that remains after all occurrences of the runes in the <paramref name="trimRunes"/> parameter are removed from the start and end of the current string. If <paramref name="trimRunes"/> is <see langword="null"/> or an empty array, Unicode white-space runes are removed instead. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		public static String Trim(this String text, params Rune[] trimRunes) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), trimRunes).ToString();
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of runes specified in an array from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimRunes">An array of <see cref="Rune"/> to remove, or <see langword="null"/>.</param>
		/// <returns>
		/// The string that remains after all occurrences of the runes in the <paramref name="trimRunes"/> parameter are removed from the start and end of the current string. If <paramref name="trimRunes"/> is <see langword="null"/> or an empty array, Unicode white-space runes are removed instead. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		public static ReadOnlySpan<Char> Trim(this Char[] text, params Rune[] trimRunes) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), trimRunes);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of runes specified in an array from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimRunes">An array of <see cref="Rune"/> to remove, or <see langword="null"/>.</param>
		/// <returns>
		/// The string that remains after all occurrences of the runes in the <paramref name="trimRunes"/> parameter are removed from the start and end of the current string. If <paramref name="trimRunes"/> is <see langword="null"/> or an empty array, Unicode white-space runes are removed instead. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		public static ReadOnlySpan<Char> Trim(this Span<Char> text, params Rune[] trimRunes) => Trim((ReadOnlySpan<Char>)text, trimRunes);

		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of runes specified in an array from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="trimRunes">An array of <see cref="Rune"/> to remove, or <see langword="null"/>.</param>
		/// <returns>
		/// The string that remains after all occurrences of the runes in the <paramref name="trimRunes"/> parameter are removed from the start and end of the current string. If <paramref name="trimRunes"/> is <see langword="null"/> or an empty array, Unicode white-space runes are removed instead. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		public static ReadOnlySpan<Char> Trim(this ReadOnlySpan<Char> text, params Rune[] trimRunes) {
			if (trimRunes is null || trimRunes.Length == 0) {
				return text.Trim();
			}
			Int32 start = 0;
			foreach (Char t in text) {
				if (!trimRunes.Contains(t)) {
					goto Done;
				}
				start++;
			}
		Done:
			return text.Slice(start);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a set of runes specified in an array from the current string.
		/// </summary>
		/// <param name="text">Pointer to the characters.</param>
		/// <param name="length">The length of the <paramref name="text"/>.</param>
		/// <param name="trimRunes">An array of <see cref="Rune"/> to remove, or <see langword="null"/>.</param>
		/// <returns>
		/// The string that remains after all occurrences of the runes in the <paramref name="trimRunes"/> parameter are removed from the start and end of the current string. If <paramref name="trimRunes"/> is <see langword="null"/> or an empty array, Unicode white-space runes are removed instead. If no runes can be trimmed from the current instance, the method returns the current instance unchanged.
		/// </returns>
		[CLSCompliant(false)]
		public static unsafe ReadOnlySpan<Char> Trim(Char* text, Int32 length, params Rune[] trimRunes) {
			Guard.NotNull(text, nameof(text));
			return Trim(new ReadOnlySpan<Char>(text, length), trimRunes);
		}
		#endregion

		#region Trim(Text, Category)
		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified UNICODE category from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="category">The UNICODE category to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="category"/> are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static String Trim(this String text, Category category) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), category).ToString();
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified UNICODE category from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="category">The UNICODE category to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="category"/> are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this Char[] text, Category category) {
			Guard.NotNull(text, nameof(text));
			return Trim(text.AsSpan(), category);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified UNICODE category from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="category">The UNICODE category to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="category"/> are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this Span<Char> text, Category category) => Trim((ReadOnlySpan<Char>)text, category);

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified UNICODE category from the current string.
		/// </summary>
		/// <param name="text">The current string.</param>
		/// <param name="category">The UNICODE category to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="category"/> are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		public static ReadOnlySpan<Char> Trim(this ReadOnlySpan<Char> text, Category category) {
			Guard.NotNull(category, nameof(category));
			Int32 start = 0;
			if (category == Surrogate) {
				while (Surrogate.Contains(text[start])) {
					start++;
				}
			} else {
				Rune rune;
				while (category.Contains(rune = text.GetRuneAt(start))) {
					start += rune.Utf16SequenceLength;
				}
			}
			return text.Slice(start);
		}

		/// <summary>
		/// Removes all the leading and trailing occurrences of a specified UNICODE category from the current string.
		/// </summary>
		/// <param name="text">Pointer to the characters.</param>
		/// <param name="length">The length of the <paramref name="text"/>.</param>
		/// <param name="category">The UNICODE category to remove.</param>
		/// <returns>The string that remains after all occurrences of the <paramref name="category"/> are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
		[CLSCompliant(false)]
		public static unsafe ReadOnlySpan<Char> Trim(Char* text, Int32 length, Category category) {
			Guard.NotNull(text, nameof(text));
			return Trim(new ReadOnlySpan<Char>(text, length), category);
		}
		#endregion
	}
}
