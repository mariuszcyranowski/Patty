using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Patty
{
    /// <summary>
    /// Provided with set of samples and regexs called pattern fragments, builds up
    /// list of patterns which matches these samples. 
    /// </summary>
	public class Patty
    {
        List<Regex> fragments = new List<Regex>();
        List<string> samples = new List<string>();
        public IDictionary<string, PatternMatch> Samples { get; private set; }
        
        public void AddPatternFragment(string fragment)
        {
            fragments.Add (new Regex(fragment, RegexOptions.Compiled | RegexOptions.CultureInvariant));
        }

        public void AddSample(string sample) 
        {
            samples.Add (sample);
        }

        /// <summary>
        /// Compiles patterns from availible pattern fragments for each sample
        /// </summary>
        /// <returns></returns>
        public IList<string> CompileAll() 
        {
            var result = new Dictionary<string, PatternMatch> ();
            foreach (var sample in samples) {
                if (!result.ContainsKey (sample)) {
                    result.Add (sample, new PatternMatch (sample));
                } else {
                    result [sample].SamplesCount += 1;
                }
                foreach (var fragment in fragments) {
                    var pm = result [sample];
                    pm.Use (fragment);
                }
            }
            Samples = result;

//            var combinedPatterns = result.Values.Select (x => x.GetCombinedPattern ()).Distinct ();
//            var grouping = result.GroupBy (x => x.Value.GetCombinedPattern ());
//            var counted = grouping.Select (x => new { 
//                x.Key, 
//                Count = x.Count ()
//            });
//            var padMax = counted.Select (x => Math.Floor(Math.Log10(x.Count) + 1)).Max ();
//            return counted
//                .OrderByDescending (x => x.Count)
//                .Select (x => string.Format ("{0} {1}", x.Count.ToString ().PadLeft ((int)padMax, '0'), x.Key))
//                .ToList ();

            return result.Values.Select (x => x.Compile()).Distinct().OrderBy(x => x).ToList();
        }
    }
}