using System;
using System.Linq;
using Xunit;

namespace Patty.Tests
{
	public class Tests
	{
		private readonly Patty patty;

		public Tests()
		{
			patty = new Patty ();
		}

		/// <summary>
		/// As default patty counts occurences of each unique sample
		/// </summary>
		[Fact]
		public void SummrizeSamples() {
			patty.AddSample ("a");
			patty.AddSample ("a");
			patty.AddSample ("b");
			var result = patty.CompileAll ();

			Assert.Equal(2, result.Count);
			Assert.Contains("a", result);
			Assert.Contains("b", result);

			Assert.Equal(2, patty.Samples.Keys.Count);
			Assert.Contains("a", patty.Samples.Keys);
			Assert.Contains("b", patty.Samples.Keys);

			Assert.Equal(2, patty.Samples["a"].SamplesCount);
			Assert.Equal(1, patty.Samples["b"].SamplesCount);
		}

		public void test1() {
			patty.AddSample ("a");
			patty.AddSample ("b");
			patty.AddPatternFragment ("a+");
			patty.AddPatternFragment ("b+");
			patty.CompileAll ();
			
			Assert.Contains("a+", patty.Samples["a"].CompatibleFragments);
			Assert.Equal(1, 	  patty.Samples["a"].CompatibleFragments.Count);
			
			Assert.Contains("b+", patty.Samples["b"].CompatibleFragments);
			Assert.Equal(1,       patty.Samples["b"].CompatibleFragments.Count);

		}

		public void test2() {
			patty.AddSample ("abb");
			patty.AddPatternFragment ("b+");
			patty.AddPatternFragment ("[ab]");
			patty.AddPatternFragment ("not used");
			var result = patty.CompileAll ();
			
			Assert.Equal (2, patty.Samples ["abb"].CompatibleFragments.Count);
			Assert.Equal("[ab]b+", patty.Samples["abb"].Compile());

			Assert.Equal(1, result.Count);
			Assert.Equal("[ab]b+", result.Single());
		}

		/// <summary>
		/// Pattern matching uses each pattern at most once to compile
		/// resulting pattern, so it can be called 'single pass' strategy
		/// </summary>
		[Fact]
		public void SinglePassMatching() {
			patty.AddSample ("ababab");
			patty.AddPatternFragment ("[ab]");
			patty.AddPatternFragment ("a+");
			patty.AddPatternFragment ("b+");
			var result = patty.CompileAll ();
			Assert.Equal("[ab]b+a+bab", result.Single());
		}

		/// <summary>
		// Example run
		/// </summary>
		[Fact]
		public void ExampleUsage() {
			patty.AddSample ("a");
			patty.AddSample ("a");
			patty.AddSample ("b");
			patty.AddSample ("c");
			patty.AddSample ("d");
			patty.AddSample ("x");
			patty.AddSample ("ad");
			patty.AddPatternFragment ("[ab]");
			patty.AddPatternFragment ("[cdx]");
			var result = patty.CompileAll ();
			
			Assert.Equal(3, result.Count);
			Assert.Contains("[ab][cdx]", result);
			Assert.Contains("[cdx]", result);
			Assert.Contains("[ab]", result);

			Assert.Contains("[ab]", patty.Samples.First().Value.CompatibleFragments);
			Assert.Contains("[cdx]", patty.Samples.Last().Value.CompatibleFragments);
		}
		
		[Fact]
		public void SuffixesMissingPatternWithPartOfSample() {
			patty.AddSample ("abc");
			patty.AddPatternFragment ("[ab]+");
			var result = patty.CompileAll ();
			Assert.Equal("[ab]+c", result.Single());
		}
		
		[Fact]
		public void PrefixesMissingPatternWithPartOfSample() {
			patty.AddSample ("cab");
			patty.AddPatternFragment ("[ab]+");
			var result = patty.CompileAll ();
			Assert.Equal("c[ab]+", result.Single());
		}
		
		[Fact]
		public void FullfilsMissingPatternWithPartOfSampleInTheMiddleOfCompiledPattern() {
			patty.AddSample ("abccx");
			patty.AddPatternFragment ("[ab]+");
			patty.AddPatternFragment ("x$");
			var result = patty.CompileAll ();
			Assert.Equal("[ab]+ccx$", result.Single());
		}

		[Fact]
		public void mathTest() {
			var a = new { i = 1, l = 2 };
			var b = new { i = 2, l = 1 };
			var c = new { i = 3, l = 2 };
			var d = new { i = 0, l = 1 };
			var e = new { i = 2, l = 3 };

			// 01234
			// -aa--
			// --b--
			// ---cc
			// d----
			// -eee-

			Assert.True (Intersects (a.i, a.l, b.i, b.l));
			Assert.True (Intersects (b.i, b.l, a.i, a.l));
			Assert.True (Intersects (c.i, c.l, e.i, e.l));
			Assert.True (Intersects (a.i, a.l, e.i, e.l));
			Assert.True (Intersects (e.i, e.l, a.i, a.l));

			Assert.False (Intersects (c.i, c.l, d.i, d.l));
			Assert.False (Intersects (d.i, d.l, c.i, c.l));
			Assert.False (Intersects (b.i, b.l, c.i, c.l));
			Assert.False (Intersects (c.i, c.l, a.i, a.l));
		}

		private bool Intersects(int xi, int xl, int yi, int yl) {
			var xmin = Math.Min (xi, xi + xl - 1);
			var xmax = Math.Max (xi, xi + xl - 1);
			var ymin = Math.Min (yi, yi + yl - 1);
			var ymax = Math.Max (yi, yi + yl - 1);
			return (xmin <= ymax && xmax >= ymin);

		}
	}
}