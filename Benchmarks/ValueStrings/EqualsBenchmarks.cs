﻿using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Stringier;

namespace Benchmarks.ValueStrings {
	[SimpleJob(RuntimeMoniker.NetCoreApp31)]
	[SimpleJob(RuntimeMoniker.CoreRt31)]
	[SimpleJob(RuntimeMoniker.Mono)]
	[MemoryDiagnoser]
	public class EqualsBenchmarks {
		[Params(new Char[] { }, new Char[] { 'h', 'e', 'l', 'l', 'o' }, new Char[] { 'h', 'e', 'l', 'l', 'o', ' ', 'w', 'o', 'r', 'l', 'd' })]
		public Char[] Source { get; set; }

		[Params(new Char[] { }, new Char[] { 'h', 'e', 'l', 'l', 'o' }, new Char[] { 'h', 'e', 'l', 'l', 'o', ' ', 'w', 'o', 'r', 'l', 'd' })]
		public Char[] Other { get; set; }

		public String StrSrc { get; set; }

		public ValueString ValSrc { get; set; }

		public String StrOth { get; set; }

		public ValueString ValOth { get; set; }

		[GlobalSetup]
		public void GlobalSetup() {
			StrSrc = new String(Source);
			ValSrc = new ValueString(Source);
			StrOth = new String(Other);
			ValOth = new ValueString(Source);
		}

		[Benchmark]
		public void String() => StrSrc.Equals(StrOth);

		[Benchmark]
		public void ValueString() => ValSrc.Equals(ValOth);
	}
}
