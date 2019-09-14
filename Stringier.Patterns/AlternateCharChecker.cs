﻿namespace System.Text.Patterns {
	internal sealed class AlternateCharChecker : Checker, IEquatable<AlternateCharChecker> {
		internal readonly Func<Char, Boolean> Left;
		internal readonly Func<Char, Boolean> Right;

		internal AlternateCharChecker(String Name, Func<Char, Boolean> Left, Func<Char, Boolean> Right) : base(Name) {
			this.Left = Left;
			this.Right = Right;
		}

		internal override Boolean CheckHeader(ref Source Source) => Source.EOF ? false : (Left(Source.Peek()) ? true : Right(Source.Peek()));

		internal override void Consume(ref Source Source, ref Result Result) {
			if (Source.EOF) {
				Result.Error = new EndOfSourceError(Expected: ToString());
			} else {
				if (Left(Source.Peek()) || Right(Source.Peek())) {
					Source.Position++;
					Result.Length++;
					Result.Error = null;
				} else {
					Result.Error = new ConsumeFailedError(Expected: ToString());
				}
			}
		}

		internal override void Neglect(ref Source Source, ref Result Result) {
			if (Source.EOF) {
				Result.Error = new EndOfSourceError(Expected: ToString());
			} else {
				if (!Left(Source.Peek()) && !Right(Source.Peek())) {
					Source.Position++;
					Result.Length++;
					Result.Error = null;
				} else {
					Result.Error = new NeglectFailedError(Neglected: ToString());
				}
			}
		}

		public override Boolean Equals(Object obj) {
			switch (obj) {
			case AlternateCharChecker other:
				return Equals(other);
			case String other:
				return Equals(other);
			default:
				return false;
			}
		}

		public override Boolean Equals(ReadOnlySpan<Char> other) => other.Length != 1 ? false : (Left(other[0]) || Right(other[0]));

		public override Boolean Equals(String other) => other.Length != 1 ? false : (Left(other[0]) || Right(other[0]));

		public Boolean Equals(AlternateCharChecker other) => Left.Equals(other.Left) && Right.Equals(other.Right);

		public override Int32 GetHashCode() => Left.GetHashCode() | Right.GetHashCode();

		public override String ToString() => $"┋{Name}┋";
	}
}