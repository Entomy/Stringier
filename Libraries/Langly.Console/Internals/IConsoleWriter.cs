﻿using System;
using Langly.Traits;

namespace Langly.Internals {
	/// <summary>
	/// Indicates the type provides write behavior for the <see cref="Console"/>.
	/// </summary>
	[CLSCompliant(false)]
	public interface IConsoleWriter : IWriteSpan<Char, IConsoleWriter>, IWriteUnsafe<Char, IConsoleWriter> { }
}
