﻿using System;
using System.Collections.Generic;
using System.Text;

namespace System.Text.Patterns {
	/// <summary>
	/// Represents the optor pattern
	/// </summary>
	public sealed class Optor : Pattern, IEquatable<Optor> {
		private readonly Pattern Pattern;

		internal Optor(Pattern Pattern) => this.Pattern = Pattern;

		/// <summary>
		/// Attempt to consume the <see cref="Pattern"/> from the <paramref name="Source"/>, adjusting the position in the <paramref name="Source"/> as appropriate
		/// </summary>
		/// <param name="Source">The <see cref="Source"/> to consume</param>
		/// <returns>A <see cref="Result"/> containing whether a match occured and the captured string</returns>
		public override Result Consume(ref Source Source) {
			return Pattern.Consume(ref Source);
		}

		public override Boolean Equals(Object obj) {
			switch (obj) {
			case Optor Other:
				return Equals(Other);
			case String Other:
				return Equals(Other);
			default:
				return false;
			}
		}

		public override Boolean Equals(String other) => Pattern.Equals(other);

		public Boolean Equals(Optor other) => Pattern.Equals(other.Pattern);

		public override Int32 GetHashCode() => ~Pattern.GetHashCode();

		public override String ToString() => $"~{Pattern}";
	}
}