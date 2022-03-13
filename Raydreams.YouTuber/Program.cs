using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Html.DataScraping.MultimediaScraping;
using Aspose.Html.DataScraping.MultimediaScraping.YouTube;
using CommandLine;
using Raydreams.YouTuber;

namespace Raydreams.YouTube
{
    /// <summary>Simple CL console app to download a collection of YouTube videos specified in the input file</summary>
    public class YouTuber
    {
        #region [ Fields ]

        /// <summary>Root path to the users desktop folder at least on the Mac</summary>
        public static readonly string DesktopPath = Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory );

        /// <summary>Where videYos are downloaded to</summary>
        public const string DownloadFolder = "Downloads";

        /// <summary>Folder on the desktop were all the app data is stored</summary>
        public const string AppFolder = "YouTuber";

        #endregion [ Fields ]

        static void Main( string[] args )
        {
            // get command line options
            CommandLineOptions options = new CommandLineOptions();
            var op = Parser.Default.ParseArguments<CommandLineOptions>( args ).WithParsed( o => options = o );

            YouTuber app = new YouTuber( options );
            app.Run();

            Console.WriteLine( "Done" );
        }

        /// <summary>Constructor</summary>
        /// <param name="options"></param>
        public YouTuber(CommandLineOptions options)
        {
            if (options != null)
            {
                this.InputList = options.InputFile;
                this.BaseFilename = options.BaseFilename;
                this.SequenceStart = options.SequenceStart;
            }
        }

        #region [ Properties ]

        /// <summary>Input data file with one video URL per line</summary>
        public string InputList { get; set; } = "myvideos.txt";

        /// <summary>The base file name for each video</summary>
        public string BaseFilename { get; set; } = "video";

        /// <summary>The integer to start counting from</summary>
        public int SequenceStart { get; set; } = 1;

        /// <summary>List of Video IDs</summary>
        protected List<String> Videos { get; set; }

        #endregion [ Properties ]

        #region [ Methods ]

        /// <summary>Run the app</summary>
        public void Run()
        {
            try
            {
                // need to check all the folders exist

                // load the input data
                this.LoadData( Path.Combine( DesktopPath, AppFolder, InputList ) );

                if ( this.Videos.Count < 1 )
                    return;

                int count = 0;

                // dl each video
                foreach ( string url in Videos )
                {
                    try
                    {
                        this.DownloadVideo( url, $"{BaseFilename}-{this.SequenceStart + count}" );
                    }
                    catch
                    {
                        continue;
                    }

                    ++count;
                }
            }
            catch ( System.Exception exp )
            {
                Console.WriteLine(exp.Message);
            }

        }

        /// <summary>Load all the video IDs to download</summary>
        /// <param name="path"></param>
        public void LoadData(string path)
        {
            if ( !File.Exists( path ) )
                return;

            // init the list to load
            this.Videos = new List<string>();

            // read everything
            var lines = File.ReadAllLines( path ).ToList();

            // parse all the input
            // later extract just the ID from a complete URL
            foreach (string line in lines)
            {
                if ( String.IsNullOrWhiteSpace( line ) )
                    continue;

                this.Videos.Add( line.Trim() );
            }
        }

        /// <summary>Actually DL the video sync for now</summary>
        /// <param name="vid">YourYube Video ID ONLY</param>
        /// <param name="filename">Name of the output file</param>
        public void DownloadVideo(string vid, string filename)
        {
            var url = new Uri( $"https://www.youtube.com/watch?v={vid}" );

            if ( String.IsNullOrWhiteSpace(filename) || !url.IsAbsoluteUri )
                return;

            // Initialize an instance of the MultimediaScraper class
            using var multimediaScraper = new MultimediaScraper();

            // Create a multimedia object that includes information from the URL
            using var multimedia = multimediaScraper.GetMultimedia( url.AbsoluteUri );

            // Get a VideoInfo object
            var videoInfo = multimedia.CollectVideoInfo();

            // Cast a videoInfo to YouTubeVideoInfo type
            var youTubeVideoInfo = videoInfo as YouTubeVideoInfo;

            // log nothing
            if ( youTubeVideoInfo == null )
                return;
            
            // Get the first element from the formats collection with minimal bitrate and present audio and video codecs
            var format = youTubeVideoInfo.Formats.OrderBy( f => f.Bitrate ).First( f => f.AudioCodec != null && f.VideoCodec != null );

            // Get the extension for the output file
            var ext = String.IsNullOrEmpty( format.Extension ) ? "mp4" : format.Extension;

            // Get the full file path for the output file
            var filePath = Path.Combine( DesktopPath, AppFolder, DownloadFolder, $"{filename}.{ext}" );

            // Download YouTube video
            multimedia.Download( format, filePath );

            // create a log statement for later
            string log = $"Title:{youTubeVideoInfo.Title}; Description:{youTubeVideoInfo.Description}; Duration:{youTubeVideoInfo.Duration}; Thumbnails Count:{youTubeVideoInfo.Thumbnails.Count}; Formats count: {0}, { youTubeVideoInfo.Formats.Count} ";

            // later return the info
        }

        #endregion [ Methods ]
    }
}
