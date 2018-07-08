using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Patty
{
    public class PatternMatch 
    {
        string sample;
        string compiledPattern;

        public PatternMatch (string sample)
        {
            this.sample = sample;
            SamplesCount = 1;
            CompatibleFragments = new List<string> ();
        }

        public long SamplesCount { get; set; }
        public IList<string> CompatibleFragments { get; private set; }
        
        private readonly Dictionary<string, Tuple<int, int>> positionsOfFragments 
            = new Dictionary<string, Tuple<int, int>>();

        /// <summary>
        /// Try to use fragment with the sample 
        /// </summary>
        public void Use (Regex fragment)
        {
            var matches = fragment.Matches (sample);
            foreach (Match m in matches) {
                if (m.Success && CanBeUsed(m.Index, m.Length)) {
                    Reset();
                    positionsOfFragments.Add (fragment.ToString(), Tuple.Create (m.Index, m.Length));
                    CompatibleFragments.Add (fragment.ToString());
                    break;
                }	
            }
        }

        private void Reset()
        {
            compiledPattern = null;
        }

        private bool CanBeUsed(int index, int length) {
            return !positionsOfFragments.Values.Any (x => Intersects (index, length, x.Item1, x.Item2));
        }

        private bool Intersects(int xi, int xl, int yi, int yl) {
            var xmin = Math.Min (xi, xi + xl - 1);
            var xmax = Math.Max (xi, xi + xl - 1);
            var ymin = Math.Min (yi, yi + yl - 1);
            var ymax = Math.Max (yi, yi + yl - 1);
            return xmin <= ymax && xmax >= ymin;

        }
        public string Compile()
        {
            if (!string.IsNullOrEmpty(compiledPattern))
                return compiledPattern;

            var sampleCurrentIndex = 0;
            var compiledPatternBuilder = new StringBuilder();
            
            using (var fragmentsEnumerator =
                positionsOfFragments.OrderBy(x => x.Value.Item1).GetEnumerator())
            {
                while (sampleCurrentIndex < sample.Length)
                {
                    if (fragmentsEnumerator.MoveNext())
                    {
                        int fragmentUsageIndex = fragmentsEnumerator.Current.Value.Item1;
                        int fragmentUsageLength = fragmentsEnumerator.Current.Value.Item2;
                        
                        if (fragmentUsageIndex > sampleCurrentIndex)
                        {
                            var samplePartWithoutMatch = sample.Substring(sampleCurrentIndex, 
                                fragmentUsageIndex - sampleCurrentIndex);
                            compiledPatternBuilder.Append(samplePartWithoutMatch);
                        }

                        compiledPatternBuilder.Append(fragmentsEnumerator.Current.Key);
                        sampleCurrentIndex = fragmentUsageIndex + fragmentUsageLength;
                    }
                    else
                    {
                        compiledPatternBuilder.Append(sample.Substring(sampleCurrentIndex));
                        sampleCurrentIndex = sample.Length;
                    }
                }
            }

            compiledPattern = compiledPatternBuilder.ToString();

            return compiledPattern;
        }
    }
}