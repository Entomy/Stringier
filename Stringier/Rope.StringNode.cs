﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Langly {
	public partial class Rope {
		/// <summary>
		/// Represents a <see cref="Rope"/> node whos content is a <see cref="String"/>.
		/// </summary>
		protected sealed class StringNode : Node {
			private readonly String Text;

			public StringNode([DisallowNull] String text, [AllowNull] Node next, [AllowNull] Node previous) : base(next, previous) => Text = text;

			/// <inheritdoc/>
			public override Char this[nint index] => Text[(Int32)index];

			/// <inheritdoc/>
			public override nint Length => Text.Length;

			/// <inheritdoc/>
			public override String ToString() => Text;

			/// <inheritdoc/>
			internal override void Insert(nint index, Char element, out Node head, out Node tail) => throw new NotImplementedException();

			/// <inheritdoc/>
			internal override void Insert(nint index, [DisallowNull] String element, out Node head, out Node tail) => throw new NotImplementedException();

			/// <inheritdoc/>
			internal override void Insert(nint index, ReadOnlyMemory<Char> element, out Node head, out Node tail) => throw new NotImplementedException();

			/// <inheritdoc/>
			internal override unsafe void Insert(nint index, [DisallowNull] Char* element, Int32 length, out Node head, out Node tail) => throw new NotImplementedException();
		}
	}
}