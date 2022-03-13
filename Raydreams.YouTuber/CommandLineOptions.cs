using System;
using CommandLine;

namespace Raydreams.YouTuber
{
    /// <summary>Options passed to the command line</summary>
    public class CommandLineOptions
    {
        /// <summary></summary>
        [Option( 'b', "base", Required = false, HelpText = "The base output filename to use" )]
        public string BaseFilename { get; set; }

        /// <summary></summary>
        [Option( 'i', "input", Required = false, HelpText = "Filename of the input file with a single column list of YouTube IDs" )]
        public string InputFile { get; set; }

        /// <summary></summary>
        [Option( 's', "seq", Required = false, HelpText = "Number sequence start suffixed to the output filename e.g. myvideo-#." )]
        public int SequenceStart { get; set; }
    }
}
