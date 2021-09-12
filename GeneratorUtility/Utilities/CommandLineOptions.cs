using CommandLine;

namespace BigFileStuff.GeneratorUtility.Utilities
{
    public sealed class CommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "Generated file path")]
        public string Path { get; set; }

        [Option(Default = 2000000000L, HelpText = "Size of generated file", Required = false)]
        public long Size { get; set; }
    }
}
