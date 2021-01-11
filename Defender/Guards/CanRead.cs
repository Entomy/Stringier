﻿using System;
using System.Runtime.CompilerServices;

namespace Langly {
	public static partial class Guard {
		/// <summary>
		/// Guard against the stream being unreadable.
		/// </summary>
		/// <param name="stream">The <see cref="System.IO.Stream"/> object.</param>
		/// <param name="name">The name of the argument.</param>
		/// <exception cref="ArgumentNotReadableException">Thrown if the guard clause fails.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CanRead(System.IO.Stream stream, String name) {
			if (stream?.CanRead != true) {
				throw ArgumentNotReadableException.With(stream, name);
			}
		}

		/// <summary>
		/// Guard against the stream being unreadable.
		/// </summary>
		/// <typeparam name="TElement">The type of element being streamed.</typeparam>
		/// <typeparam name="TError">The type of the error object.</typeparam>
		/// <param name="stream">The <see cref="IReadable{TElement, TError}"/> object.</param>
		/// <param name="name">The name of the argument.</param>
		/// <exception cref="ArgumentNotReadableException">Thrown if the guard clause fails.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CanRead<TElement, TError>(IReadable<TElement, TError> stream, String name) {
			if (stream?.Readable != true) {
				throw ArgumentNotReadableException.With(stream, name);
			}
		}
	}
}