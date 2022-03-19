using System;
using CommandLine;

namespace Raydreams.YouTuber
{
    /// <summary>Options passed to the command line</summary>
    public class CommandLineOptions
    {

        /// <summary>input</summary>
        [Option( 'i', "input", Required = true, HelpText = "Either the complete filepath to an input file with a single column list of YouTube video URLs or single URL itself." )]
        public string InputFile { get; set; }

        /// <summary>output</summary>
        [Option( 'o', "output", Required = false, HelpText = "Full local file path to a folder to write videos to. If not set then will be written to the user's desktop." )]
        public string Output { get; set; }

        /// <summary>Base filename</summary>
        [Option( 'b', "base", Required = false, Default = "outvideo", HelpText = "The base output filename to use" )]
        public string BaseFilename { get; set; }

        /// <summary>Sequence start suffix</summary>
        [Option( 's', "seq", Required = false, Default = 1, HelpText = "Number sequence start suffixed to the output filename e.g. myvideo-#." )]
        public int SequenceStart { get; set; }
    }
}
