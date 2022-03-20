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
    /// <remarks>
    /// TO DO
    /// add a logger
    /// </remarks>
    public class YouTuber
    {
        #region [ Fields ]

        /// <summary>Root path to the users desktop folder at least on the Mac</summary>
        public static readonly string DesktopPath = Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory );

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
                // required
                this.InputList = options.InputFile;

                // default optional values set in CommandLineOptions
                this.BaseFilename = ( !String.IsNullOrWhiteSpace( options.BaseFilename ) ) ? options.BaseFilename : "video";
                this.SequenceStart = ( options.SequenceStart > -1 ) ? options.SequenceStart : 1;

                // determine the output folder
                if ( !String.IsNullOrWhiteSpace(options.Output) )
                {
                    if ( !Directory.Exists( options.Output.Trim() ) )
                        this.OutputFolder = new DirectoryInfo( Path.Combine( DesktopPath, AppFolder ) );
                    else
                        this.OutputFolder = new DirectoryInfo( options.Output.Trim() );
                }
            }
        }

        #region [ Properties ]

        /// <summary>Input data file with one video URL per line</summary>
        public string InputList { get; set; }

        /// <summary>Download folder</summary>
        public DirectoryInfo OutputFolder { get; protected set; } = new DirectoryInfo( Path.Combine( DesktopPath, AppFolder ) );

        /// <summary>The base file name for each video</summary>
        public string BaseFilename { get; set; } = "video";

        /// <summary>The integer to start counting from</summary>
        public int SequenceStart { get; set; } = 1;

        /// <summary>List of Video IDs</summary>
        protected List<Uri> Videos { get; set; }

        #endregion [ Properties ]

        #region [ Methods ]

        /// <summary>Run the app</summary>
        public void Run()
        {
            try
            {
                // load the input data into the Videos list
                this.LoadData( this.InputList );

                if ( this.Videos.Count < 1 )
                    return;

                int count = 0;

                // dl each video
                foreach ( Uri url in Videos )
                {
                    try
                    {
                        var info = this.DownloadVideo( url, $"{BaseFilename}-{this.SequenceStart + count}" );
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine( e.Message );
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
        public void LoadData( string path )
        {
            if ( String.IsNullOrWhiteSpace(path) )
                return;

            // init the list to load
            this.Videos = new List<Uri>();

            // is it a local file
            if ( File.Exists( path ) )
            {
                // read everything
                var lines = File.ReadAllLines( path ).ToList();

                // parse all the input
                foreach ( string line in lines )
                {
                    var uri = ExtractSimpleUrl( line );

                    if (uri != null)
                        this.Videos.Add( uri );
                }

                return;
            }

            // maybe a url itself
            var vid = ExtractSimpleUrl( path );

            if ( vid != null )
                this.Videos.Add( vid );

            return;
        }

        /// <summary>Actually DL the video sync for now</summary>
        /// <param name="vid">YourYube Video ID ONLY</param>
        /// <param name="filename">Name of the output file</param>
        public YouTubeVideoInfo DownloadVideo(Uri vid, string filename)
        {
            if ( String.IsNullOrWhiteSpace(filename) || !vid.IsAbsoluteUri )
                return null;

            // Initialize an instance of the MultimediaScraper class
            using MultimediaScraper multimediaScraper = new MultimediaScraper();

            // Create a multimedia object that includes information from the URL
            using Multimedia multimedia = multimediaScraper.GetMultimedia( vid.AbsoluteUri );

            // Get a VideoInfo object
            YouTubeVideoInfo info = multimedia.CollectVideoInfo() as YouTubeVideoInfo;

            // log nothing
            if ( info == null )
                return null;
            
            // Get the first element from the formats collection with minimal bitrate and present audio and video codecs
            var format = info.Formats.OrderBy( f => f.Bitrate ).First( f => f.AudioCodec != null && f.VideoCodec != null );

            // Get the extension for the output file
            var ext = String.IsNullOrEmpty( format.Extension ) ? "mp4" : format.Extension;

            // Get the full file path for the output file
            string filePath = Path.Combine( this.OutputFolder.FullName, $"{filename}.{ext}" );

            // create a log statement for later
            string log = $"Title:{info.Title}; Description:{info.Description}; Duration:{info.Duration}; Thumbnails Count:{info.Thumbnails.Count}; Formats count: {0}, { info.Formats.Count} ".Replace('\n', ' ');

            Console.WriteLine( log );

            // Download YouTube video
            multimedia.Download( format, filePath );

            return info;
        }

        /// <summary>Reduce a YouTube URL to its simple form</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Uri ExtractSimpleUrl(string path)
        {
            if ( String.IsNullOrWhiteSpace( path ) )
                return null;

            // extract the video ID from the URL
            var url = new Uri( path.Trim() );

            if ( !url.IsAbsoluteUri )
                return null;

            var queryDict = url.Query.PairsToDictionary();

            if ( !queryDict.ContainsKey( "v" ) )
                return null;

            string id = queryDict["v"];

            return new Uri( $"https://www.youtube.com/watch?v={id}" );
        }

        #endregion [ Methods ]
    }
}
