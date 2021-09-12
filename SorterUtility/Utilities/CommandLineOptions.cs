using CommandLine;

namespace BigFileStuff.SorterUtility.Utilities
{
    public sealed class CommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "File path to sort")]
        public string Path { get; set; }
    }
}
