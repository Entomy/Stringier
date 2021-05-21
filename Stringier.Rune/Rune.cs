﻿//!! Do NOT add IComparable to this, even though that's correct. It was mistakenly forgotten from the earlier Rune implementations

#if NETSTANDARD1_3
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Unicode;

namespace System.Text {
	/// <summary>
	/// Represents a Unicode scalar value ([ U+0000..U+D7FF ], inclusive; or [ U+E000..U+10FFFF ], inclusive).
	/// </summary>
	/// <remarks>
	/// This type's constructors and conversion operators validate the input, so consumers can call the APIs
	/// assuming that the underlying <see cref="Rune"/> instance is well-formed.
	/// </remarks>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly struct Rune : IComparable<Rune>, IEquatable<Rune> {
		internal const int MaxUtf16CharsPerRune = 2; // supplementary plane code points are encoded as 2 UTF-16 code units
		internal const int MaxUtf8BytesPerRune = 4; // supplementary plane code points are encoded as 4 UTF-8 code units

		private const char HighSurrogateStart = '\ud800';
		private const char LowSurrogateStart = '\udc00';
		private const int HighSurrogateRange = 0x3FF;

		private const byte IsWhiteSpaceFlag = 0x80;
		private const byte IsLetterOrDigitFlag = 0x40;
		private const byte UnicodeCategoryMask = 0x1F;

		// Contains information about the ASCII character range [ U+0000..U+007F ], with:
		// - 0x80 bit if set means 'is whitespace'
		// - 0x40 bit if set means 'is letter or digit'
		// - 0x20 bit is reserved for future use
		// - bottom 5 bits are the UnicodeCategory of the character
		private static ReadOnlySpan<byte> AsciiCharInfo => new byte[]
		{
			0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x8E, 0x8E, 0x8E, 0x8E, 0x8E, 0x0E, 0x0E, // U+0000..U+000F
            0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, // U+0010..U+001F
            0x8B, 0x18, 0x18, 0x18, 0x1A, 0x18, 0x18, 0x18, 0x14, 0x15, 0x18, 0x19, 0x18, 0x13, 0x18, 0x18, // U+0020..U+002F
            0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x18, 0x18, 0x19, 0x19, 0x19, 0x18, // U+0030..U+003F
            0x18, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, // U+0040..U+004F
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x14, 0x18, 0x15, 0x1B, 0x12, // U+0050..U+005F
            0x1B, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, // U+0060..U+006F
            0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x14, 0x19, 0x15, 0x19, 0x0E, // U+0070..U+007F
        };

		private readonly uint _value;

		/// <summary>
		/// Creates a <see cref="Rune"/> from the provided UTF-16 code unit.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If <paramref name="ch"/> represents a UTF-16 surrogate code point
		/// U+D800..U+DFFF, inclusive.
		/// </exception>
		public Rune(char ch) {
			uint expanded = ch;
			if (UnicodeUtility.IsSurrogateCodePoint(expanded)) {
				throw new ArgumentOutOfRangeException(nameof(ch));
			}
			_value = expanded;
		}

		/// <summary>
		/// Creates a <see cref="Rune"/> from the provided UTF-16 surrogate pair.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If <paramref name="highSurrogate"/> does not represent a UTF-16 high surrogate code point
		/// or <paramref name="lowSurrogate"/> does not represent a UTF-16 low surrogate code point.
		/// </exception>
		public Rune(char highSurrogate, char lowSurrogate)
			: this((uint)char.ConvertToUtf32(highSurrogate, lowSurrogate), false) {
		}

		/// <summary>
		/// Creates a <see cref="Rune"/> from the provided Unicode scalar value.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If <paramref name="value"/> does not represent a value Unicode scalar value.
		/// </exception>
		public Rune(int value)
			: this((uint)value) {
		}

		/// <summary>
		/// Creates a <see cref="Rune"/> from the provided Unicode scalar value.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If <paramref name="value"/> does not represent a value Unicode scalar value.
		/// </exception>
		[CLSCompliant(false)]
		public Rune(uint value) {
			if (!UnicodeUtility.IsValidUnicodeScalar(value)) {
				throw new ArgumentOutOfRangeException(nameof(value));
			}
			_value = value;
		}

		// non-validating ctor
		private Rune(uint scalarValue, bool unused) {
			UnicodeDebug.AssertIsValidScalar(scalarValue);
			_value = scalarValue;
		}

		public static bool operator ==(Rune left, Rune right) => left._value == right._value;

		public static bool operator !=(Rune left, Rune right) => left._value != right._value;

		public static bool operator <(Rune left, Rune right) => left._value < right._value;

		public static bool operator <=(Rune left, Rune right) => left._value <= right._value;

		public static bool operator >(Rune left, Rune right) => left._value > right._value;

		public static bool operator >=(Rune left, Rune right) => left._value >= right._value;

		// Operators below are explicit because they may throw.

		public static explicit operator Rune(char ch) => new Rune(ch);

		[CLSCompliant(false)]
		public static explicit operator Rune(uint value) => new Rune(value);

		public static explicit operator Rune(int value) => new Rune(value);

		// Displayed as "'<char>' (U+XXXX)"; e.g., "'e' (U+0065)"
		private string DebuggerDisplay => FormattableString.Invariant($"U+{_value:X4} '{(IsValid(_value) ? ToString() : "\uFFFD")}'");

		/// <summary>
		/// Returns true if and only if this scalar value is ASCII ([ U+0000..U+007F ])
		/// and therefore representable by a single UTF-8 code unit.
		/// </summary>
		public bool IsAscii => UnicodeUtility.IsAsciiCodePoint(_value);

		/// <summary>
		/// Returns true if and only if this scalar value is within the BMP ([ U+0000..U+FFFF ])
		/// and therefore representable by a single UTF-16 code unit.
		/// </summary>
		public bool IsBmp => UnicodeUtility.IsBmpCodePoint(_value);

		/// <summary>
		/// Returns the Unicode plane (0 to 16, inclusive) which contains this scalar.
		/// </summary>
		public int Plane => UnicodeUtility.GetPlane(_value);

		/// <summary>
		/// A <see cref="Rune"/> instance that represents the Unicode replacement character U+FFFD.
		/// </summary>
		public static Rune ReplacementChar => UnsafeCreate(UnicodeUtility.ReplacementChar);

		/// <summary>
		/// Returns the length in code units (<see cref="char"/>) of the
		/// UTF-16 sequence required to represent this scalar value.
		/// </summary>
		/// <remarks>
		/// The return value will be 1 or 2.
		/// </remarks>
		public int Utf16SequenceLength {
			get {
				int codeUnitCount = UnicodeUtility.GetUtf16SequenceLength(_value);
				Debug.Assert(codeUnitCount > 0 && codeUnitCount <= MaxUtf16CharsPerRune);
				return codeUnitCount;
			}
		}

		/// <summary>
		/// Returns the length in code units of the
		/// UTF-8 sequence required to represent this scalar value.
		/// </summary>
		/// <remarks>
		/// The return value will be 1 through 4, inclusive.
		/// </remarks>
		public int Utf8SequenceLength {
			get {
				int codeUnitCount = UnicodeUtility.GetUtf8SequenceLength(_value);
				Debug.Assert(codeUnitCount > 0 && codeUnitCount <= MaxUtf8BytesPerRune);
				return codeUnitCount;
			}
		}

		/// <summary>
		/// Returns the Unicode scalar value as an integer.
		/// </summary>
		public int Value => (int)_value;

		public int CompareTo(Rune other) => this.Value - other.Value; // values don't span entire 32-bit domain; won't integer overflow

		/// <summary>
		/// Decodes the <see cref="Rune"/> at the beginning of the provided UTF-16 source buffer.
		/// </summary>
		/// <returns>
		/// <para>
		/// If the source buffer begins with a valid UTF-16 encoded scalar value, returns <see cref="OperationStatus.Done"/>,
		/// and outs via <paramref name="result"/> the decoded <see cref="Rune"/> and via <paramref name="charsConsumed"/> the
		/// number of <see langword="char"/>s used in the input buffer to encode the <see cref="Rune"/>.
		/// </para>
		/// <para>
		/// If the source buffer is empty or contains only a standalone UTF-16 high surrogate character, returns <see cref="OperationStatus.NeedMoreData"/>,
		/// and outs via <paramref name="result"/> <see cref="ReplacementChar"/> and via <paramref name="charsConsumed"/> the length of the input buffer.
		/// </para>
		/// <para>
		/// If the source buffer begins with an ill-formed UTF-16 encoded scalar value, returns <see cref="OperationStatus.InvalidData"/>,
		/// and outs via <paramref name="result"/> <see cref="ReplacementChar"/> and via <paramref name="charsConsumed"/> the number of
		/// <see langword="char"/>s used in the input buffer to encode the ill-formed sequence.
		/// </para>
		/// </returns>
		/// <remarks>
		/// The general calling convention is to call this method in a loop, slicing the <paramref name="source"/> buffer by
		/// <paramref name="charsConsumed"/> elements on each iteration of the loop. On each iteration of the loop <paramref name="result"/>
		/// will contain the real scalar value if successfully decoded, or it will contain <see cref="ReplacementChar"/> if
		/// the data could not be successfully decoded. This pattern provides convenient automatic U+FFFD substitution of
		/// invalid sequences while iterating through the loop.
		/// </remarks>
		public static OperationStatus DecodeFromUtf16(ReadOnlySpan<char> source, out Rune result, out int charsConsumed) {
			if (!source.IsEmpty) {
				// First, check for the common case of a BMP scalar value.
				// If this is correct, return immediately.

				char firstChar = source[0];
				if (TryCreate(firstChar, out result)) {
					charsConsumed = 1;
					return OperationStatus.Done;
				}

				// First thing we saw was a UTF-16 surrogate code point.
				// Let's optimistically assume for now it's a high surrogate and hope
				// that combining it with the next char yields useful results.

				if (1 < (uint)source.Length) {
					char secondChar = source[1];
					if (TryCreate(firstChar, secondChar, out result)) {
						// Success! Formed a supplementary scalar value.
						charsConsumed = 2;
						return OperationStatus.Done;
					} else {
						// Either the first character was a low surrogate, or the second
						// character was not a low surrogate. This is an error.
						goto InvalidData;
					}
				} else if (!char.IsHighSurrogate(firstChar)) {
					// Quick check to make sure we're not going to report NeedMoreData for
					// a single-element buffer where the data is a standalone low surrogate
					// character. Since no additional data will ever make this valid, we'll
					// report an error immediately.
					goto InvalidData;
				}
			}

			// If we got to this point, the input buffer was empty, or the buffer
			// was a single element in length and that element was a high surrogate char.

			charsConsumed = source.Length;
			result = ReplacementChar;
			return OperationStatus.NeedMoreData;

		InvalidData:

			charsConsumed = 1; // maximal invalid subsequence for UTF-16 is always a single code unit in length
			result = ReplacementChar;
			return OperationStatus.InvalidData;
		}

		/// <summary>
		/// Decodes the <see cref="Rune"/> at the beginning of the provided UTF-8 source buffer.
		/// </summary>
		/// <returns>
		/// <para>
		/// If the source buffer begins with a valid UTF-8 encoded scalar value, returns <see cref="OperationStatus.Done"/>,
		/// and outs via <paramref name="result"/> the decoded <see cref="Rune"/> and via <paramref name="bytesConsumed"/> the
		/// number of <see langword="byte"/>s used in the input buffer to encode the <see cref="Rune"/>.
		/// </para>
		/// <para>
		/// If the source buffer is empty or contains only a partial UTF-8 subsequence, returns <see cref="OperationStatus.NeedMoreData"/>,
		/// and outs via <paramref name="result"/> <see cref="ReplacementChar"/> and via <paramref name="bytesConsumed"/> the length of the input buffer.
		/// </para>
		/// <para>
		/// If the source buffer begins with an ill-formed UTF-8 encoded scalar value, returns <see cref="OperationStatus.InvalidData"/>,
		/// and outs via <paramref name="result"/> <see cref="ReplacementChar"/> and via <paramref name="bytesConsumed"/> the number of
		/// <see langword="char"/>s used in the input buffer to encode the ill-formed sequence.
		/// </para>
		/// </returns>
		/// <remarks>
		/// The general calling convention is to call this method in a loop, slicing the <paramref name="source"/> buffer by
		/// <paramref name="bytesConsumed"/> elements on each iteration of the loop. On each iteration of the loop <paramref name="result"/>
		/// will contain the real scalar value if successfully decoded, or it will contain <see cref="ReplacementChar"/> if
		/// the data could not be successfully decoded. This pattern provides convenient automatic U+FFFD substitution of
		/// invalid sequences while iterating through the loop.
		/// </remarks>
		public static OperationStatus DecodeFromUtf8(ReadOnlySpan<byte> source, out Rune result, out int bytesConsumed) {
			// This method follows the Unicode Standard's recommendation for detecting
			// the maximal subpart of an ill-formed subsequence. See The Unicode Standard,
			// Ch. 3.9 for more details. In summary, when reporting an invalid subsequence,
			// it tries to consume as many code units as possible as long as those code
			// units constitute the beginning of a longer well-formed subsequence per Table 3-7.

			int index = 0;

			// Try reading input[0].

			if ((uint)index >= (uint)source.Length) {
				goto NeedsMoreData;
			}

			uint tempValue = source[index];
			if (!UnicodeUtility.IsAsciiCodePoint(tempValue)) {
				goto NotAscii;
			}

		Finish:

			bytesConsumed = index + 1;
			Debug.Assert(1 <= bytesConsumed && bytesConsumed <= 4); // Valid subsequences are always length [1..4]
			result = UnsafeCreate(tempValue);
			return OperationStatus.Done;

		NotAscii:

			// Per Table 3-7, the beginning of a multibyte sequence must be a code unit in
			// the range [C2..F4]. If it's outside of that range, it's either a standalone
			// continuation byte, or it's an overlong two-byte sequence, or it's an out-of-range
			// four-byte sequence.

			if (!UnicodeUtility.IsInRangeInclusive(tempValue, 0xC2, 0xF4)) {
				goto FirstByteInvalid;
			}

			tempValue = (tempValue - 0xC2) << 6;

			// Try reading input[1].

			index++;
			if ((uint)index >= (uint)source.Length) {
				goto NeedsMoreData;
			}

			// Continuation bytes are of the form [10xxxxxx], which means that their two's
			// complement representation is in the range [-65..-128]. This allows us to
			// perform a single comparison to see if a byte is a continuation byte.

			int thisByteSignExtended = (sbyte)source[index];
			if (thisByteSignExtended >= -64) {
				goto Invalid;
			}

			tempValue += (uint)thisByteSignExtended;
			tempValue += 0x80; // remove the continuation byte marker
			tempValue += (0xC2 - 0xC0) << 6; // remove the leading byte marker

			if (tempValue < 0x0800) {
				Debug.Assert(UnicodeUtility.IsInRangeInclusive(tempValue, 0x0080, 0x07FF));
				goto Finish; // this is a valid 2-byte sequence
			}

			// This appears to be a 3- or 4-byte sequence. Since per Table 3-7 we now have
			// enough information (from just two code units) to detect overlong or surrogate
			// sequences, we need to perform these checks now.

			if (!UnicodeUtility.IsInRangeInclusive(tempValue, ((0xE0 - 0xC0) << 6) + (0xA0 - 0x80), ((0xF4 - 0xC0) << 6) + (0x8F - 0x80))) {
				// The first two bytes were not in the range [[E0 A0]..[F4 8F]].
				// This is an overlong 3-byte sequence or an out-of-range 4-byte sequence.
				goto Invalid;
			}

			if (UnicodeUtility.IsInRangeInclusive(tempValue, ((0xED - 0xC0) << 6) + (0xA0 - 0x80), ((0xED - 0xC0) << 6) + (0xBF - 0x80))) {
				// This is a UTF-16 surrogate code point, which is invalid in UTF-8.
				goto Invalid;
			}

			if (UnicodeUtility.IsInRangeInclusive(tempValue, ((0xF0 - 0xC0) << 6) + (0x80 - 0x80), ((0xF0 - 0xC0) << 6) + (0x8F - 0x80))) {
				// This is an overlong 4-byte sequence.
				goto Invalid;
			}

			// The first two bytes were just fine. We don't need to perform any other checks
			// on the remaining bytes other than to see that they're valid continuation bytes.

			// Try reading input[2].

			index++;
			if ((uint)index >= (uint)source.Length) {
				goto NeedsMoreData;
			}

			thisByteSignExtended = (sbyte)source[index];
			if (thisByteSignExtended >= -64) {
				goto Invalid; // this byte is not a UTF-8 continuation byte
			}

			tempValue <<= 6;
			tempValue += (uint)thisByteSignExtended;
			tempValue += 0x80; // remove the continuation byte marker
			tempValue -= (0xE0 - 0xC0) << 12; // remove the leading byte marker

			if (tempValue <= 0xFFFF) {
				Debug.Assert(UnicodeUtility.IsInRangeInclusive(tempValue, 0x0800, 0xFFFF));
				goto Finish; // this is a valid 3-byte sequence
			}

			// Try reading input[3].

			index++;
			if ((uint)index >= (uint)source.Length) {
				goto NeedsMoreData;
			}

			thisByteSignExtended = (sbyte)source[index];
			if (thisByteSignExtended >= -64) {
				goto Invalid; // this byte is not a UTF-8 continuation byte
			}

			tempValue <<= 6;
			tempValue += (uint)thisByteSignExtended;
			tempValue += 0x80; // remove the continuation byte marker
			tempValue -= (0xF0 - 0xE0) << 18; // remove the leading byte marker

			UnicodeDebug.AssertIsValidSupplementaryPlaneScalar(tempValue);
			goto Finish; // this is a valid 4-byte sequence

		FirstByteInvalid:

			index = 1; // Invalid subsequences are always at least length 1.

		Invalid:

			Debug.Assert(1 <= index && index <= 3); // Invalid subsequences are always length 1..3
			bytesConsumed = index;
			result = ReplacementChar;
			return OperationStatus.InvalidData;

		NeedsMoreData:

			Debug.Assert(0 <= index && index <= 3); // Incomplete subsequences are always length 0..3
			bytesConsumed = index;
			result = ReplacementChar;
			return OperationStatus.NeedMoreData;
		}

		/// <summary>
		/// Decodes the <see cref="Rune"/> at the end of the provided UTF-16 source buffer.
		/// </summary>
		/// <remarks>
		/// This method is very similar to <see cref="DecodeFromUtf16(ReadOnlySpan{char}, out Rune, out int)"/>, but it allows
		/// the caller to loop backward instead of forward. The typical calling convention is that on each iteration
		/// of the loop, the caller should slice off the final <paramref name="charsConsumed"/> elements of
		/// the <paramref name="source"/> buffer.
		/// </remarks>
		public static OperationStatus DecodeLastFromUtf16(ReadOnlySpan<char> source, out Rune result, out int charsConsumed) {
			int index = source.Length - 1;
			if ((uint)index < (uint)source.Length) {
				// First, check for the common case of a BMP scalar value.
				// If this is correct, return immediately.

				char finalChar = source[index];
				if (TryCreate(finalChar, out result)) {
					charsConsumed = 1;
					return OperationStatus.Done;
				}

				if (char.IsLowSurrogate(finalChar)) {
					// The final character was a UTF-16 low surrogate code point.
					// This must be preceded by a UTF-16 high surrogate code point, otherwise
					// we have a standalone low surrogate, which is always invalid.

					index--;
					if ((uint)index < (uint)source.Length) {
						char penultimateChar = source[index];
						if (TryCreate(penultimateChar, finalChar, out result)) {
							// Success! Formed a supplementary scalar value.
							charsConsumed = 2;
							return OperationStatus.Done;
						}
					}

					// If we got to this point, we saw a standalone low surrogate
					// and must report an error.

					charsConsumed = 1; // standalone surrogate
					result = ReplacementChar;
					return OperationStatus.InvalidData;
				}
			}

			// If we got this far, the source buffer was empty, or the source buffer ended
			// with a UTF-16 high surrogate code point. These aren't errors since they could
			// be valid given more input data.

			charsConsumed = (int)((uint)(-source.Length) >> 31); // 0 -> 0, all other lengths -> 1
			result = ReplacementChar;
			return OperationStatus.NeedMoreData;
		}

		/// <summary>
		/// Decodes the <see cref="Rune"/> at the end of the provided UTF-8 source buffer.
		/// </summary>
		/// <remarks>
		/// This method is very similar to <see cref="DecodeFromUtf8(ReadOnlySpan{byte}, out Rune, out int)"/>, but it allows
		/// the caller to loop backward instead of forward. The typical calling convention is that on each iteration
		/// of the loop, the caller should slice off the final <paramref name="bytesConsumed"/> elements of
		/// the <paramref name="source"/> buffer.
		/// </remarks>
		public static OperationStatus DecodeLastFromUtf8(ReadOnlySpan<byte> source, out Rune value, out int bytesConsumed) {
			int index = source.Length - 1;
			if ((uint)index < (uint)source.Length) {
				// The buffer contains at least one byte. Let's check the fast case where the
				// buffer ends with an ASCII byte.

				uint tempValue = source[index];
				if (UnicodeUtility.IsAsciiCodePoint(tempValue)) {
					bytesConsumed = 1;
					value = UnsafeCreate(tempValue);
					return OperationStatus.Done;
				}

				// If the final byte is not an ASCII byte, we may be beginning or in the middle of
				// a UTF-8 multi-code unit sequence. We need to back up until we see the start of
				// the multi-code unit sequence; we can detect the leading byte because all multi-byte
				// sequences begin with a byte whose 0x40 bit is set. Since all multi-byte sequences
				// are no greater than 4 code units in length, we only need to search back a maximum
				// of four bytes.

				if (((byte)tempValue & 0x40) != 0) {
					// This is a UTF-8 leading byte. We'll do a forward read from here.
					// It'll return invalid (if given C0, F5, etc.) or incomplete. Both are fine.

					return DecodeFromUtf8(source.Slice(index), out value, out bytesConsumed);
				}

				// If we got to this point, the final byte was a UTF-8 continuation byte.
				// Let's check the three bytes immediately preceding this, looking for the starting byte.

				for (int i = 3; i > 0; i--) {
					index--;
					if ((uint)index >= (uint)source.Length) {
						goto Invalid; // out of data
					}

					// The check below will get hit for ASCII (values 00..7F) and for UTF-8 starting bytes
					// (bits 0xC0 set, values C0..FF). In two's complement this is the range [-64..127].
					// It's just a fast way for us to terminate the search.

					if ((sbyte)source[index] >= -64) {
						goto ForwardDecode;
					}
				}

			Invalid:

				// If we got to this point, either:
				// - the last 4 bytes of the input buffer are continuation bytes;
				// - the entire input buffer (if fewer than 4 bytes) consists only of continuation bytes; or
				// - there's no UTF-8 leading byte between the final continuation byte of the buffer and
				//   the previous well-formed subsequence or maximal invalid subsequence.
				//
				// In all of these cases, the final byte must be a maximal invalid subsequence of length 1.
				// See comment near the end of this method for more information.

				value = ReplacementChar;
				bytesConsumed = 1;
				return OperationStatus.InvalidData;

			ForwardDecode:

				// If we got to this point, we found an ASCII byte or a UTF-8 starting byte at position source[index].
				// Technically this could also mean we found an invalid byte like C0 or F5 at this position, but that's
				// fine since it'll be handled by the forward read. From this position, we'll perform a forward read
				// and see if we consumed the entirety of the buffer.

				source = source.Slice(index);
				Debug.Assert(!source.IsEmpty, "Shouldn't reach this for empty inputs.");

				OperationStatus operationStatus = DecodeFromUtf8(source, out Rune tempRune, out int tempBytesConsumed);
				if (tempBytesConsumed == source.Length) {
					// If this forward read consumed the entirety of the end of the input buffer, we can return it
					// as the result of this function. It could be well-formed, incomplete, or invalid. If it's
					// invalid and we consumed the remainder of the buffer, we know we've found the maximal invalid
					// subsequence, which is what we wanted anyway.

					bytesConsumed = tempBytesConsumed;
					value = tempRune;
					return operationStatus;
				}

				// If we got to this point, we know that the final continuation byte wasn't consumed by the forward
				// read that we just performed above. This means that the continuation byte has to be part of an
				// invalid subsequence since there's no UTF-8 leading byte between what we just consumed and the
				// continuation byte at the end of the input. Furthermore, since any maximal invalid subsequence
				// of length > 1 must have a UTF-8 leading byte as its first code unit, this implies that the
				// continuation byte at the end of the buffer is itself a maximal invalid subsequence of length 1.

				goto Invalid;
			} else {
				// Source buffer was empty.
				value = ReplacementChar;
				bytesConsumed = 0;
				return OperationStatus.NeedMoreData;
			}
		}

		/// <summary>
		/// Encodes this <see cref="Rune"/> to a UTF-16 destination buffer.
		/// </summary>
		/// <param name="destination">The buffer to which to write this value as UTF-16.</param>
		/// <returns>The number of <see cref="char"/>s written to <paramref name="destination"/>.</returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="destination"/> is not large enough to hold the output.
		/// </exception>
		public int EncodeToUtf16(Span<char> destination) {
			if (!TryEncodeToUtf16(destination, out int charsWritten)) {
				throw new ArgumentException("Destination is too short.", nameof(destination));
			}

			return charsWritten;
		}

		/// <summary>
		/// Encodes this <see cref="Rune"/> to a UTF-8 destination buffer.
		/// </summary>
		/// <param name="destination">The buffer to which to write this value as UTF-8.</param>
		/// <returns>The number of <see cref="byte"/>s written to <paramref name="destination"/>.</returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="destination"/> is not large enough to hold the output.
		/// </exception>
		public int EncodeToUtf8(Span<byte> destination) {
			if (!TryEncodeToUtf8(destination, out int bytesWritten)) {
				throw new ArgumentException("Destination is too short.", nameof(destination));
			}

			return bytesWritten;
		}

		public override bool Equals(object? obj) => (obj is Rune other) && Equals(other);

		public bool Equals(Rune other) => this == other;

		public override int GetHashCode() => Value;

		/// <summary>
		/// Gets the <see cref="Rune"/> which begins at index <paramref name="index"/> in
		/// string <paramref name="input"/>.
		/// </summary>
		/// <remarks>
		/// Throws if <paramref name="input"/> is null, if <paramref name="index"/> is out of range, or
		/// if <paramref name="index"/> does not reference the start of a valid scalar value within <paramref name="input"/>.
		/// </remarks>
		public static Rune GetRuneAt(string input, int index) {
			int runeValue = ReadRuneFromString(input, index);
			if (runeValue < 0) {
				throw new ArgumentException("Cannot extract a Unicode scalar value from the specified index in the input.", nameof(index));
			}

			return UnsafeCreate((uint)runeValue);
		}

		/// <summary>
		/// Returns <see langword="true"/> iff <paramref name="value"/> is a valid Unicode scalar
		/// value, i.e., is in [ U+0000..U+D7FF ], inclusive; or [ U+E000..U+10FFFF ], inclusive.
		/// </summary>
		public static bool IsValid(int value) => IsValid((uint)value);

		/// <summary>
		/// Returns <see langword="true"/> iff <paramref name="value"/> is a valid Unicode scalar
		/// value, i.e., is in [ U+0000..U+D7FF ], inclusive; or [ U+E000..U+10FFFF ], inclusive.
		/// </summary>
		[CLSCompliant(false)]
		public static bool IsValid(uint value) => UnicodeUtility.IsValidUnicodeScalar(value);

		// returns a negative number on failure
		internal static int ReadFirstRuneFromUtf16Buffer(ReadOnlySpan<char> input) {
			if (input.IsEmpty) {
				return -1;
			}

			// Optimistically assume input is within BMP.

			uint returnValue = input[0];
			if (UnicodeUtility.IsSurrogateCodePoint(returnValue)) {
				if (!UnicodeUtility.IsHighSurrogateCodePoint(returnValue)) {
					return -1;
				}

				// Treat 'returnValue' as the high surrogate.

				if (1 >= (uint)input.Length) {
					return -1; // not an argument exception - just a "bad data" failure
				}

				uint potentialLowSurrogate = input[1];
				if (!UnicodeUtility.IsLowSurrogateCodePoint(potentialLowSurrogate)) {
					return -1;
				}

				returnValue = UnicodeUtility.GetScalarFromUtf16SurrogatePair(returnValue, potentialLowSurrogate);
			}

			return (int)returnValue;
		}

		// returns a negative number on failure
		private static int ReadRuneFromString(string input, int index) {
			if (input is null) {
				throw new ArgumentNullException(nameof(input));
			}

			if ((uint)index >= (uint)input!.Length) {
				throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
			}

			// Optimistically assume input is within BMP.

			uint returnValue = input[index];
			if (UnicodeUtility.IsSurrogateCodePoint(returnValue)) {
				if (!UnicodeUtility.IsHighSurrogateCodePoint(returnValue)) {
					return -1;
				}

				// Treat 'returnValue' as the high surrogate.
				//
				// If this becomes a hot code path, we can skip the below bounds check by reading
				// off the end of the string using unsafe code. Since strings are null-terminated,
				// we're guaranteed not to read a valid low surrogate, so we'll fail correctly if
				// the string terminates unexpectedly.

				index++;
				if ((uint)index >= (uint)input.Length) {
					return -1; // not an argument exception - just a "bad data" failure
				}

				uint potentialLowSurrogate = input[index];
				if (!UnicodeUtility.IsLowSurrogateCodePoint(potentialLowSurrogate)) {
					return -1;
				}

				returnValue = UnicodeUtility.GetScalarFromUtf16SurrogatePair(returnValue, potentialLowSurrogate);
			}

			return (int)returnValue;
		}

		/// <summary>
		/// Returns a <see cref="string"/> representation of this <see cref="Rune"/> instance.
		/// </summary>
		public override string ToString() {
#if SYSTEM_PRIVATE_CORELIB
            if (IsBmp)
            {
                return string.CreateFromChar((char)_value);
            }
            else
            {
                UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar(_value, out char high, out char low);
                return string.CreateFromChar(high, low);
            }
#else
			if (IsBmp) {
				return ((char)_value).ToString();
			} else {
				Span<char> buffer = stackalloc char[MaxUtf16CharsPerRune];
				UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar(_value, out buffer[0], out buffer[1]);
				return buffer.ToString();
			}
#endif
		}

		/// <summary>
		/// Attempts to create a <see cref="Rune"/> from the provided input value.
		/// </summary>
		public static bool TryCreate(char ch, out Rune result) {
			uint extendedValue = ch;
			if (!UnicodeUtility.IsSurrogateCodePoint(extendedValue)) {
				result = UnsafeCreate(extendedValue);
				return true;
			} else {
				result = default;
				return false;
			}
		}

		/// <summary>
		/// Attempts to create a <see cref="Rune"/> from the provided UTF-16 surrogate pair.
		/// Returns <see langword="false"/> if the input values don't represent a well-formed UTF-16surrogate pair.
		/// </summary>
		public static bool TryCreate(char highSurrogate, char lowSurrogate, out Rune result) {
			// First, extend both to 32 bits, then calculate the offset of
			// each candidate surrogate char from the start of its range.

			uint highSurrogateOffset = (uint)highSurrogate - HighSurrogateStart;
			uint lowSurrogateOffset = (uint)lowSurrogate - LowSurrogateStart;

			// This is a single comparison which allows us to check both for validity at once since
			// both the high surrogate range and the low surrogate range are the same length.
			// If the comparison fails, we call to a helper method to throw the correct exception message.

			if ((highSurrogateOffset | lowSurrogateOffset) <= HighSurrogateRange) {
				// The 0x40u << 10 below is to account for uuuuu = wwww + 1 in the surrogate encoding.
				result = UnsafeCreate((highSurrogateOffset << 10) + ((uint)lowSurrogate - LowSurrogateStart) + (0x40u << 10));
				return true;
			} else {
				// Didn't have a high surrogate followed by a low surrogate.
				result = default;
				return false;
			}
		}

		/// <summary>
		/// Attempts to create a <see cref="Rune"/> from the provided input value.
		/// </summary>
		public static bool TryCreate(int value, out Rune result) => TryCreate((uint)value, out result);

		/// <summary>
		/// Attempts to create a <see cref="Rune"/> from the provided input value.
		/// </summary>
		[CLSCompliant(false)]
		public static bool TryCreate(uint value, out Rune result) {
			if (UnicodeUtility.IsValidUnicodeScalar(value)) {
				result = UnsafeCreate(value);
				return true;
			} else {
				result = default;
				return false;
			}
		}

		/// <summary>
		/// Encodes this <see cref="Rune"/> to a UTF-16 destination buffer.
		/// </summary>
		/// <param name="destination">The buffer to which to write this value as UTF-16.</param>
		/// <param name="charsWritten">
		/// The number of <see cref="char"/>s written to <paramref name="destination"/>,
		/// or 0 if the destination buffer is not large enough to contain the output.</param>
		/// <returns>True if the value was written to the buffer; otherwise, false.</returns>
		/// <remarks>
		/// The <see cref="Utf16SequenceLength"/> property can be queried ahead of time to determine
		/// the required size of the <paramref name="destination"/> buffer.
		/// </remarks>
		public bool TryEncodeToUtf16(Span<char> destination, out int charsWritten) {
			if (destination.Length >= 1) {
				if (IsBmp) {
					destination[0] = (char)_value;
					charsWritten = 1;
					return true;
				} else if (destination.Length >= 2) {
					UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar(_value, out destination[0], out destination[1]);
					charsWritten = 2;
					return true;
				}
			}

			// Destination buffer not large enough

			charsWritten = default;
			return false;
		}

		/// <summary>
		/// Encodes this <see cref="Rune"/> to a destination buffer as UTF-8 bytes.
		/// </summary>
		/// <param name="destination">The buffer to which to write this value as UTF-8.</param>
		/// <param name="bytesWritten">
		/// The number of <see cref="byte"/>s written to <paramref name="destination"/>,
		/// or 0 if the destination buffer is not large enough to contain the output.</param>
		/// <returns>True if the value was written to the buffer; otherwise, false.</returns>
		/// <remarks>
		/// The <see cref="Utf8SequenceLength"/> property can be queried ahead of time to determine
		/// the required size of the <paramref name="destination"/> buffer.
		/// </remarks>
		public bool TryEncodeToUtf8(Span<byte> destination, out int bytesWritten) {
			// The bit patterns below come from the Unicode Standard, Table 3-6.

			if (destination.Length >= 1) {
				if (IsAscii) {
					destination[0] = (byte)_value;
					bytesWritten = 1;
					return true;
				}

				if (destination.Length >= 2) {
					if (_value <= 0x7FFu) {
						// Scalar 00000yyy yyxxxxxx -> bytes [ 110yyyyy 10xxxxxx ]
						destination[0] = (byte)((_value + (0b110u << 11)) >> 6);
						destination[1] = (byte)((_value & 0x3Fu) + 0x80u);
						bytesWritten = 2;
						return true;
					}

					if (destination.Length >= 3) {
						if (_value <= 0xFFFFu) {
							// Scalar zzzzyyyy yyxxxxxx -> bytes [ 1110zzzz 10yyyyyy 10xxxxxx ]
							destination[0] = (byte)((_value + (0b1110 << 16)) >> 12);
							destination[1] = (byte)(((_value & (0x3Fu << 6)) >> 6) + 0x80u);
							destination[2] = (byte)((_value & 0x3Fu) + 0x80u);
							bytesWritten = 3;
							return true;
						}

						if (destination.Length >= 4) {
							// Scalar 000uuuuu zzzzyyyy yyxxxxxx -> bytes [ 11110uuu 10uuzzzz 10yyyyyy 10xxxxxx ]
							destination[0] = (byte)((_value + (0b11110 << 21)) >> 18);
							destination[1] = (byte)(((_value & (0x3Fu << 12)) >> 12) + 0x80u);
							destination[2] = (byte)(((_value & (0x3Fu << 6)) >> 6) + 0x80u);
							destination[3] = (byte)((_value & 0x3Fu) + 0x80u);
							bytesWritten = 4;
							return true;
						}
					}
				}
			}

			// Destination buffer not large enough

			bytesWritten = default;
			return false;
		}

		/// <summary>
		/// Attempts to get the <see cref="Rune"/> which begins at index <paramref name="index"/> in
		/// string <paramref name="input"/>.
		/// </summary>
		/// <returns><see langword="true"/> if a scalar value was successfully extracted from the specified index,
		/// <see langword="false"/> if a value could not be extracted due to invalid data.</returns>
		/// <remarks>
		/// Throws only if <paramref name="input"/> is null or <paramref name="index"/> is out of range.
		/// </remarks>
		public static bool TryGetRuneAt(string input, int index, out Rune value) {
			int runeValue = ReadRuneFromString(input, index);
			if (runeValue >= 0) {
				value = UnsafeCreate((uint)runeValue);
				return true;
			} else {
				value = default;
				return false;
			}
		}

		// Allows constructing a Unicode scalar value from an arbitrary 32-bit integer without
		// validation. It is the caller's responsibility to have performed manual validation
		// before calling this method. If a Rune instance is forcibly constructed
		// from invalid input, the APIs on this type have undefined behavior, potentially including
		// introducing a security hole in the consuming application.
		//
		// An example of a security hole resulting from an invalid Rune value, which could result
		// in a stack overflow.
		//
		// public int GetMarvin32HashCode(Rune r) {
		//   Span<char> buffer = stackalloc char[r.Utf16SequenceLength];
		//   r.TryEncode(buffer, ...);
		//   return Marvin32.ComputeHash(buffer.AsBytes());
		// }

		/// <summary>
		/// Creates a <see cref="Rune"/> without performing validation on the input.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Rune UnsafeCreate(uint scalarValue) => new Rune(scalarValue, false);

		// These are analogs of APIs on System.Char

		public static double GetNumericValue(Rune value) {
			if (value.IsAscii) {
				uint baseNum = value._value - '0';
				return (baseNum <= 9) ? (double)baseNum : -1;
			} else {
				// not an ASCII char; fall back to globalization table
#if SYSTEM_PRIVATE_CORELIB
                return CharUnicodeInfo.GetNumericValue(value.Value);
#else
				if (value.IsBmp) {
					return CharUnicodeInfo.GetNumericValue((char)value._value);
				}
				return CharUnicodeInfo.GetNumericValue(value.ToString(), 0);
#endif
			}
		}

		public static UnicodeCategory GetUnicodeCategory(Rune value) {
			if (value.IsAscii) {
				return (UnicodeCategory)(AsciiCharInfo[value.Value] & UnicodeCategoryMask);
			} else {
				return GetUnicodeCategoryNonAscii(value);
			}
		}

		private static UnicodeCategory GetUnicodeCategoryNonAscii(Rune value) {
			Debug.Assert(!value.IsAscii, "Shouldn't use this non-optimized code path for ASCII characters.");
			return CharUnicodeInfo.GetUnicodeCategory(value.ToString(), 0);
		}

		// Returns true iff this Unicode category represents a letter
		private static bool IsCategoryLetter(UnicodeCategory category) {
			return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.UppercaseLetter, (uint)UnicodeCategory.OtherLetter);
		}

		// Returns true iff this Unicode category represents a letter or a decimal digit
		private static bool IsCategoryLetterOrDecimalDigit(UnicodeCategory category) {
			return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.UppercaseLetter, (uint)UnicodeCategory.OtherLetter)
				|| (category == UnicodeCategory.DecimalDigitNumber);
		}

		// Returns true iff this Unicode category represents a number
		private static bool IsCategoryNumber(UnicodeCategory category) {
			return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.DecimalDigitNumber, (uint)UnicodeCategory.OtherNumber);
		}

		// Returns true iff this Unicode category represents a punctuation mark
		private static bool IsCategoryPunctuation(UnicodeCategory category) {
			return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.ConnectorPunctuation, (uint)UnicodeCategory.OtherPunctuation);
		}

		// Returns true iff this Unicode category represents a separator
		private static bool IsCategorySeparator(UnicodeCategory category) {
			return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.SpaceSeparator, (uint)UnicodeCategory.ParagraphSeparator);
		}

		// Returns true iff this Unicode category represents a symbol
		private static bool IsCategorySymbol(UnicodeCategory category) {
			return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.MathSymbol, (uint)UnicodeCategory.OtherSymbol);
		}

		public static bool IsControl(Rune value) {
			// Per the Unicode stability policy, the set of control characters
			// is forever fixed at [ U+0000..U+001F ], [ U+007F..U+009F ]. No
			// characters will ever be added to or removed from the "control characters"
			// group. See https://www.unicode.org/policies/stability_policy.html.

			// Logic below depends on Rune.Value never being -1 (since Rune is a validating type)
			// 00..1F (+1) => 01..20 (&~80) => 01..20
			// 7F..9F (+1) => 80..A0 (&~80) => 00..20

			return ((value._value + 1) & ~0x80u) <= 0x20u;
		}

		public static bool IsDigit(Rune value) {
			if (value.IsAscii) {
				return UnicodeUtility.IsInRangeInclusive(value._value, '0', '9');
			} else {
				return GetUnicodeCategoryNonAscii(value) == UnicodeCategory.DecimalDigitNumber;
			}
		}

		public static bool IsLetter(Rune value) {
			if (value.IsAscii) {
				return ((value._value - 'A') & ~0x20u) <= (uint)('Z' - 'A'); // [A-Za-z]
			} else {
				return IsCategoryLetter(GetUnicodeCategoryNonAscii(value));
			}
		}

		public static bool IsLetterOrDigit(Rune value) {
			if (value.IsAscii) {
				return (AsciiCharInfo[value.Value] & IsLetterOrDigitFlag) != 0;
			} else {
				return IsCategoryLetterOrDecimalDigit(GetUnicodeCategoryNonAscii(value));
			}
		}

		public static bool IsLower(Rune value) {
			if (value.IsAscii) {
				return UnicodeUtility.IsInRangeInclusive(value._value, 'a', 'z');
			} else {
				return GetUnicodeCategoryNonAscii(value) == UnicodeCategory.LowercaseLetter;
			}
		}

		public static bool IsNumber(Rune value) {
			if (value.IsAscii) {
				return UnicodeUtility.IsInRangeInclusive(value._value, '0', '9');
			} else {
				return IsCategoryNumber(GetUnicodeCategoryNonAscii(value));
			}
		}

		public static bool IsPunctuation(Rune value) {
			return IsCategoryPunctuation(GetUnicodeCategory(value));
		}

		public static bool IsSeparator(Rune value) {
			return IsCategorySeparator(GetUnicodeCategory(value));
		}

		public static bool IsSymbol(Rune value) {
			return IsCategorySymbol(GetUnicodeCategory(value));
		}

		public static bool IsUpper(Rune value) {
			if (value.IsAscii) {
				return UnicodeUtility.IsInRangeInclusive(value._value, 'A', 'Z');
			} else {
				return GetUnicodeCategoryNonAscii(value) == UnicodeCategory.UppercaseLetter;
			}
		}

		public static bool IsWhiteSpace(Rune value) {
			if (value.IsAscii) {
				return (AsciiCharInfo[value.Value] & IsWhiteSpaceFlag) != 0;
			}

			// Only BMP code points can be white space, so only call into CharUnicodeInfo
			// if the incoming value is within the BMP.

			return value.IsBmp &&
#if SYSTEM_PRIVATE_CORELIB
                CharUnicodeInfo.GetIsWhiteSpace((char)value._value);
#else
				char.IsWhiteSpace((char)value._value);
#endif
		}

		public static Rune ToLower(Rune value, CultureInfo culture) => GetRuneAt(culture.TextInfo.ToLower(value.ToString()), 0);

		public static Rune ToLowerInvariant(Rune value) => GetRuneAt(value.ToString().ToLowerInvariant(), 0);

		public static Rune ToUpper(Rune value, CultureInfo culture) => GetRuneAt(culture.TextInfo.ToUpper(value.ToString()), 0);

		public static Rune ToUpperInvariant(Rune value) => GetRuneAt(value.ToString().ToUpperInvariant(), 0);
	}
}
#else
using System.Text;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(Rune))]
#endif