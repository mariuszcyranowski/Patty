using Microsoft.Extensions.CommandLineUtils;

namespace Patty.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "Patty",
                Description =
                    "Provided with set of samples and regexs called pattern fragments, builds up a list of patterns which matches these samples."
            };
            
            app.HelpOption("-?|-h|--help");
            var patternsFile = app.Option("-p |--patterns-file <filepath>", "File with pattern fragments to be used",
                CommandOptionType.SingleValue);
            
            var samples = app.Option("-s |--samples <filepath>", "Source of samples", CommandOptionType.SingleValue);
            
            app.OnExecute(() =>
            {
                if (!samples.HasValue() || !System.IO.File.Exists(samples.Value()))
                {
                    app.Error.WriteLine("sample file not exist or not provided");
                    
                    return 1;
                }
                
                var patty = new Patty();
                
                if (patternsFile.HasValue())
                {
                    if (System.IO.File.Exists(patternsFile.Value()))
                    {
                        using (var reader = new System.IO.StreamReader (patternsFile.Value())) {
                            string pattern;
                            while ((pattern = reader.ReadLine ()) != null) {
                                patty.AddPatternFragment(pattern);
                            }
                        }    
                    }
                    else
                    {
                        app.Error.WriteLine($"patterns file does not exists - {patternsFile.Value()}");
                    }
                }
                
                using (var reader = new System.IO.StreamReader (samples.Value())) {
                    string sample;
                    while ((sample = reader.ReadLine()) != null) {
                        patty.AddSample (sample);		
                    }
                }
                
                var result = patty.CompileAll();
                foreach (var item in result) {
                    app.Out.WriteLine(item);
                }
                
                return 0;
            });
 
            app.Execute(args);
        }
    }
}