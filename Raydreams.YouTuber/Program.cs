using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Html.DataScraping.MultimediaScraping;
using Aspose.Html.DataScraping.MultimediaScraping.YouTube;

namespace Raydreams.YouTube
{
    /// <summary></summary>
    public class YouTuber
    {
        #region [ Fields ]

        /// <summary>Root path to the users desktop folder at least on the Mac</summary>
        public static readonly string DesktopPath = Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory );

        /// <summary>Where videYos are downloaded to</summary>
        public const string DownloadFolder = "Downloads";

        /// <summary>Folder on the desktop were all the app data is stored</summary>
        public const string AppFolder = "YouTuber";

        /// <summary>Input data file with one video URL per line</summary>
        public const string InputList = "myvideos.txt";

        /// <summary>The base file name for each video</summary>
        public const string BaseFilename = "dungeonsdragons";

        #endregion [ Fields ]

        static void Main( string[] args )
        {
            YouTuber app = new YouTuber();
            app.Run();

            Console.WriteLine( "Done" );
        }

        #region [ Properties ]

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
                        this.DownloadVideo( url, $"{BaseFilename}-{count + 1}" );
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

            this.Videos = File.ReadAllLines(path).ToList();
        }

        /// <summary>Actually DL the video sync for now</summary>
        /// <param name="vid">YourYube Video ID ONLY</param>
        /// <param name="filename">Name of the output file</param>
        public void DownloadVideo(string vid, string filename)
        {
            // Expected video properties, for example, "Title" and "Duration" 
            //string expectedTitle = "HAPPY - Walk off the Earth Ft. Parachute";
            //int expectedDuration = 60 * 3 + 55;

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

            if ( youTubeVideoInfo != null )
            {
                // Get the first element from the formats collection with minimal bitrate and present audio and video codecs
                var format = youTubeVideoInfo.Formats.OrderBy( f => f.Bitrate ).First( f => f.AudioCodec != null && f.VideoCodec != null );

                // Get the extension for the output file
                var ext = string.IsNullOrEmpty( format.Extension ) ? "mp4" : format.Extension;

                // Get the full file path for the output file
                var filePath = Path.Combine( DesktopPath, filename + "." + ext );

                // Download YouTube video
                multimedia.Download( format, filePath );

                // Show YouTube video info
                Console.WriteLine( "Title: {0}", youTubeVideoInfo.Title );
                Console.WriteLine( "Description: {0}", youTubeVideoInfo.Description );
                Console.WriteLine( "Duration: {0}", youTubeVideoInfo.Duration );
                Console.WriteLine( "Thumbnails count: {0}", youTubeVideoInfo.Thumbnails.Count );
                Console.WriteLine( "Formats count: {0}", youTubeVideoInfo.Formats.Count );
            }
        }

        #endregion [ Methods ]
    }
}
