using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raydreams.YouTuber
{
    public static class Extensions
    {
        /// <summary>Given a string in the format ?key1=value1,key2=value2,key3=value3 splits into a dictionary</summary>
        public static Dictionary<string, string> PairsToDictionary( this string str )
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            // any characters needing to be removed
            str = str.Trim().TrimStart( new char[] { '?' } );

            List<string> pairs = str.ToString().Split( '&', StringSplitOptions.None ).ToList();

            pairs.ForEach( p => {
                string[] parts = p.Split( '=', StringSplitOptions.None );

                if ( parts != null && parts.Length > 0 && !String.IsNullOrWhiteSpace( parts[0] ) )
                {
                    parts[1] = ( parts.Length < 2 || String.IsNullOrWhiteSpace( parts[1] ) ) ? String.Empty : parts[1].Trim();

                    string key = parts[0].Trim();

                    if ( !results.ContainsKey(key) )
                        results.Add( key, parts[1] );
                }
            });

            return results;
        }
    }
}
