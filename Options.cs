using CommandLine;

namespace MMLib.Utils.ImageCompressor
{
    public class Options
    {
        [Option('s', "source", Required = true, HelpText = "Full path to the source folder.")]
        public string SourceFolder { get; set; }

        [Option('d', "destination", Required = false,
            HelpText = "Full path to the destination folder. (If is not entered, source files are overwritten.")]
        public string DestinationFolder { get; set; }

        [Option('p', "percentage", Required = false, HelpText = "Percentage of destination resize.", Default = 90)]
        public int Percentage { get; set; }
    }
}