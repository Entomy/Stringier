﻿namespace System.Text.Patterns {
	/// <summary>
	/// Represents a negator pattern
	/// </summary>
	internal sealed class Negator : Node, IEquatable<Negator> {
		private readonly Node Pattern;

		internal Negator(Node Pattern) => this.Pattern = Pattern;

		internal Negator(Pattern Pattern) : this(Pattern.Head) { }

		/// <summary>
		/// Attempt to consume the <see cref="Pattern"/> from the <paramref name="Source"/>, adjusting the position in the <paramref name="Source"/> as appropriate
		/// </summary>
		/// <param name="Source">The <see cref="Source"/> to consume</param>
		/// <returns>A <see cref="Result"/> containing whether a match occured and the captured string</returns>
		public override Result Consume(ref Source Source) => Pattern.Neglect(ref Source);

		public override Boolean Equals(Object obj) {
			switch (obj) {
			case Negator Other:
				return Equals(Other);
			case String Other:
				return Equals(Other);
			default:
				return false;
			}
		}

		public override Boolean Equals(String other) => Pattern.Equals(other);

		public Boolean Equals(Negator other) => Pattern.Equals(other.Pattern);

		public override Int32 GetHashCode() => Pattern.GetHashCode();

		public override Result Neglect(ref Source Source) => Pattern.Consume(ref Source);

		public override String ToString() => $"!{Pattern}";
	}
}