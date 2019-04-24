﻿namespace System.Text.Patterns {
	/// <summary>
	/// Represents a spanner pattern
	/// </summary>
	public sealed class Spanner : Pattern, IEquatable<Spanner> {
		private readonly Pattern Pattern;

		internal Spanner(Pattern Pattern) => this.Pattern = Pattern;

		public override Result Consume(Result Candidate) => Consume(Candidate, out _);

		public override Result Consume(Result Candidate, out String Capture) {
			StringBuilder CaptureBuilder = new StringBuilder();
			String capture;
			Result Result = Candidate;
			while (Result) {
				Result = Pattern.Consume(Result, out capture);
				CaptureBuilder.Append(capture);
			}
			Capture = CaptureBuilder.ToString();
			return Result;
		}

		public override Boolean Equals(Object obj) {
			switch (obj) {
			case Spanner Other:
				return Equals(Other);
			case String Other:
				return Equals(Other);
			default:
				return false;
			}
		}

		public override Boolean Equals(String other) => Pattern.Equals(other);

		public Boolean Equals(Spanner other) => Pattern.Equals(other.Pattern);

		public override Int32 GetHashCode() => Pattern.GetHashCode();

		public override String ToString() => Pattern.ToString();
	}
}