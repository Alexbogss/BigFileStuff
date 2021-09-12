using System.Collections.Generic;
using System.IO;

namespace BigFileStuff.GeneratorUtility.Utilities
{
    public class CliValidationResult
    {
        public bool Success { get; set; }

        public List<string> Errors { get; set; }
    }

    public static class CliArgsValidator
    {
        public static CliValidationResult Validate(CommandLineOptions options)
        {
            var success = true;
            var errors = new List<string>();

            var size = options.Size;
            if (size <= 0)
            {
                success = false;
                errors.Add($"Size value must be positive");
            }

            if (size.ToString().Length > 13)
            {
                success = false;
                errors.Add($"Size value too big");
            }

            var path = options.Path;
            if (File.Exists(path))
            {
                success = false;
                errors.Add($"File exists on selected path");
            }

            return new CliValidationResult
            {
                Success = success,
                Errors = errors
            };
        }
    }
}
