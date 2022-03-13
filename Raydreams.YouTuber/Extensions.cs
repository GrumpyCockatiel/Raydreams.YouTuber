using System;
using System.Collections.Generic;
using System.Text;

namespace Raydreams.YouTuber
{
    public static class Extensions
    {
        /// <summary>Given a string in the format key1=value1,key2=value2,key3=value3 splits into a dictionary</summary>
        /// <param name="stripQuotes">If true will remove quotes from around the value in cases of key="myvalue"</param>
        public static Dictionary<string, string> PairsToDictionary( this string str, bool stripQuotes = true )
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            StringBuilder temp = new StringBuilder();

            foreach ( char c in str )
            {
                if ( c == '?' )
                    continue;
                else if ( c == '&' )
                {
                    string[] parts = temp.ToString().Split( '=', StringSplitOptions.None );

                    if ( parts != null && parts.Length > 0 && !String.IsNullOrWhiteSpace( parts[0] ) )
                    {
                        parts[1] = ( parts.Length < 2 || String.IsNullOrWhiteSpace( parts[1] ) ) ? String.Empty : parts[1].Trim();

                        if ( stripQuotes )
                            parts[1] = parts[1].Replace( "\"", "" );

                        results.Add( parts[0].Trim(), parts[1] );
                    }

                    temp = new StringBuilder();
                }
                else
                {
                    temp.Append( c );
                }
            }

            return results;
        }
    }
}
