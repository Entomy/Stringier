﻿using System;
using System.IO;
using System.Text;

namespace Stringier.Streams {
	public static class StreamExtensions {
		/// <summary>
		/// Reads the next, maximum of, <paramref name="count"/> characters from the input stream and advances the byte position by the amount read.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="count">The maximum of the characters to read.</param>
		/// <returns>An <see cref="Array"/> of <see cref="Byte"/> containing the read characters.</returns>
		public static Byte[] Read(this Stream stream, Int32 count) {
			Byte[] buf = new Byte[count];
			Int32 c = stream.Read(buf, 0, count);
			Byte[] res = new Byte[c];
			Array.Copy(buf, 0, res, 0, c);
			return res;
		}

		/// <summary>
		/// Reads a codepoint from the stream and advances the position within the stream by one codepoint. <see cref="Array.Empty{T}"/> signifies an invalid sequence or inability to read from the stream.
		/// </summary>
		/// <returns>The codepoint as a <see cref="Array"/> of zero to two <see cref="Char"/></returns>
		/// <remarks>
		/// <para>This method assumes UTF-8 encoding.</para>
		/// <para>This method requires a bit of explaination as there's a great deal of misunderstandings. <see cref="Char"/> is a 16-bit integer, yet UNICODE requires at least 21 bits of storage. This is an archaic decision from back in the days of UCS-2. Changing things to update with the times wouldn't be feasible. Because of this, .NET strings sometimes require 2 characters to represent a single codepoint. That means, while in most cases, this operation will return a single character in an array, there are cases where the codepoint exceeds 16-bits and has to be encoded. For that reason, if you're on a new enough runtime, you want to use <see cref="ReadRune(Stream)"/>, which does this same operation but with a type capable of representing the codepoint as a single value.</para>
		/// </remarks>
		public static Char[] ReadCodepoint(this Stream stream) {
			Byte[] sequence = new Byte[4] { 0x00, 0x00, 0x00, 0x00 };
			Int32 bytes = 1;
			// The first byte in the sequence encodes the length of the sequence, so determine that, then read in the rest of the sequence.
			Int32 @byte = stream.ReadByte();
			switch (@byte) {
			case var b when (b - 0x00) * (0x7F - b) >= 0:
				sequence[0] = (Byte)@byte;
				break;
			case var b when (b - 0xC2) * (0xDF - b) >= 0:
				bytes = 2;
				sequence[0] = (Byte)@byte;
				if ((@byte = stream.ReadByte()) >= 0) {
					sequence[1] = (Byte)@byte;
				} else {
					goto Error;
				}
				break;
			case var b when (b - 0xE0) * (0xEF - b) >= 0:
				bytes = 3;
				sequence[0] = (Byte)@byte;
				if ((@byte = stream.ReadByte()) >= 0) {
					sequence[1] = (Byte)@byte;
				} else {
					goto Error;
				}
				if ((@byte = stream.ReadByte()) >= 0) {
					sequence[2] = (Byte)@byte;
				} else {
					goto Error;
				}
				break;
			case var b when (b - 0xF0) * (0xF4 - b) >= 0:
				bytes = 4;
				sequence[0] = (Byte)@byte;
				if ((@byte = stream.ReadByte()) >= 0) {
					sequence[1] = (Byte)@byte;
				} else {
					goto Error;
				}
				if ((@byte = stream.ReadByte()) >= 0) {
					sequence[2] = (Byte)@byte;
				} else {
					goto Error;
				}
				if ((@byte = stream.ReadByte()) >= 0) {
					sequence[3] = (Byte)@byte;
				} else {
					goto Error;
				}
				break;
			default:
				goto Error;
			}
			// Now decode the sequence
			Decoder decoder = Encoding.UTF8.GetDecoder();
			Char[] chars = new Char[2];
			switch (decoder.GetChars(sequence, 0, bytes, chars, 0)) {
			case 1:
				return new[] { chars[0] };
			case 2:
				return chars;
			}
		Error:
			return Array.Empty<Char>();
		}

#if NETCOREAPP3_0 || NETCOREAPP3_1
		/// <summary>
		/// Reads a <see cref="Rune"/> from the stream and advances the position within the stream by one rune;
		/// </summary>
		/// <returns>The next <see cref="Rune"/> in the stream.</returns>
		/// <remarks>
		/// This method assumes UTF-8 encoding.
		/// </remarks>
		public static Rune ReadRune(this Stream stream) {
			Span<Char> chars = stream.ReadCodepoint();
			switch (chars.Length) {
			case 1:
				return new Rune(chars[0]);
			case 2:
				return new Rune(chars[0], chars[1]);
			default:
				return new Rune();
			}
		}
#endif
	}
}