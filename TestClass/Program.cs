using System;
using System.IO;
using KodiMovieNfoLibrary;
using Nito.AsyncEx;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace TestClass
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }

        private static async void MainAsync(string[] args)
        {
            //TMDbClient client = new TMDbClient("125e475654c781837d575da127e4d7bd");
            //Movie movie = await client.GetMovieAsync(258509, MovieMethods.Credits |
            //    MovieMethods.AlternativeTitles |
            //    MovieMethods.Images |
            //    MovieMethods.Keywords |
            //    MovieMethods.ReleaseDates |
            //    MovieMethods.Changes |
            //    MovieMethods.Releases |
            //    MovieMethods.Reviews |
            //    MovieMethods.Similar);

            //Console.WriteLine(movie.Title);

            //var r = RootMovie.Convert(movie);

            var rd = File.ReadAllText(@"Q:\MyMovies.Library\004\MyMovies.L\Legend (1985)\movie.nfo");
            var r = rd.XmlDeserializeFromString<RootMovie>();
            var client = new TMDbClient("125e475654c781837d575da127e4d7bd");
            var movie = await client.GetMovieAsync(r.TmdbId, MovieMethods.Credits |
                                                             MovieMethods.AlternativeTitles |
                                                             MovieMethods.Images |
                                                             MovieMethods.Keywords |
                                                             MovieMethods.ReleaseDates |
                                                             MovieMethods.Changes |
                                                             MovieMethods.Releases |
                                                             MovieMethods.Reviews |
                                                             MovieMethods.Similar);

            r = RootMovie.Convert(movie);

            if (r.Genre.StartsWith("TMDbLib", StringComparison.InvariantCultureIgnoreCase)) r.Genre = "";
            //var st = @"<movie><title>Who knows</title><originaltitle>Who knows for real</originaltitle><sorttitle>Who knows 1</sorttitle><set>Who knows trilogy</set><rating>6.100000</rating><year>2008</year><top250>0</top250><votes>50</votes><outline>A look at the role of the Buckeye State in the 2004 Presidential Election.</outline><plot>A look at the role of the Buckeye State in the 2004 Presidential Election.</plot><tagline></tagline><runtime>90</runtime><thumb>http://ia.ec.imdb.com/media/imdb/01/I/25/65/31/10f.jpg</thumb><mpaa>Not available</mpaa><playcount>0</playcount><id>tt0432337</id><filenameandpath>c:\Dummy_Movie_Files\Movies\...So Goes The Nation.avi</filenameandpath><trailer></trailer><genre></genre><credits></credits><fileinfo><streamdetails/></fileinfo><studio>Dummy Pictures</studio><director>Adam Del Deo</director>"
            //         +
            //         @"<actor><name>Paul Begala</name><role>Himself</role></actor><actor><name>George W. Bush</name><role>Himself</role></actor><actor><name>Mary Beth Cahill</name><role>Herself</role></actor><actor><name>Ed Gillespie</name><role>Himself</role></actor><actor><name>John Kerry</name><role>Himself</role></actor></movie>";
            //var r = st.XmlDeserializeFromString<RootMovie>();

            var us = "";
            if (r.Actors != null)
            {
                var q = r.Actors.Clone();
                r.Actors = null;
                var u = q as ActorName[];
                us = u.XmlSerializeToString()
                    .Replace("<ActorName>", "<actor>")
                    .Replace("</ActorName>", "</actor>")
                    .Replace("<ArrayOfActorName>\r\n", "")
                    .Replace("\r\n</ArrayOfActorName>", "")
                     + "\r\n";
            }
            Console.WriteLine(r.XmlSerializeToString().Replace("</movie>", us + "</movie>"));


            Console.ReadKey();
        }
    }
}