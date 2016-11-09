using System;
using System.Linq;
using System.Xml.Serialization;
using TMDbLib.Objects.Movies;

namespace KodiMovieNfoLibrary
{
    /*
<movie>
        <title>Who knows</title>
        <originaltitle>Who knows for real</originaltitle>
        <sorttitle>Who knows 1</sorttitle>
        <set>Who knows trilogy</set>
        <rating>6.100000</rating>
        <year>2008</year>
        <top250>0</top250>
        <votes>50</votes>
        <outline>A look at the role of the Buckeye State in the 2004 Presidential Election.</outline><!-- Should be short, will be displayed on a single line. -->
        <plot>A look at the role of the Buckeye State in the 2004 Presidential Election.</plot> <!-- Can contain more information on multiple lines, will be wrapped. -->
        <tagline></tagline>
        <runtime>90</runtime> //runtime in minutes
        <thumb>http://ia.ec.imdb.com/media/imdb/01/I/25/65/31/10f.jpg</thumb>
        <mpaa>Not available</mpaa>
        <playcount>0</playcount><!-- setting this to > 0 will mark the movie as watched if the "importwatchedstate" flag is set in advancedsettings.xml -->
        <id>tt0432337</id>
        <filenameandpath>c:\Dummy_Movie_Files\Movies\...So Goes The Nation.avi</filenameandpath>
        <trailer></trailer>
        <genre></genre>
        <credits></credits> <!-- Library exports uses this field for writers.-->
        <fileinfo>
            <streamdetails> <!-- While it is possible to manually set the information contained within the "streamdetails" tag,there is little point in doing so, as the software will always overwrite this data when it plays back the video file. In other words, no matter how many times you try to manually set it, it will be undone the moment the video is played.-->
            </streamdetails>
        </fileinfo>
        <studio>Dummy Pictures</studio>
        <director>Adam Del Deo</director>
        <actor>
            <name>Paul Begala</name>
            <role>Himself</role>
        </actor>
        <actor>
            <name>George W. Bush</name>
            <role>Himself</role>
        </actor>
        <actor>
            <name>Mary Beth Cahill</name>
            <role>Herself</role>
        </actor>
        <actor>
            <name>Ed Gillespie</name>
            <role>Himself</role>
        </actor>
        <actor>
            <name>John Kerry</name>
            <role>Himself</role>
        </actor>
    </movie>
     */

    [Serializable]
    [XmlRoot("movie")]
    public class RootMovie
    {


        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("originaltitle")]
        public string OriginalTitle { get; set; }

        [XmlElement("sorttitle")]
        public string SortTitle { get; set; }

        [XmlElement("set")]
        public string CollectionName { get; set; }

        [XmlElement("rating")]
        public double Rating { get; set; }

        [XmlElement("year")]
        public int ReleaseYear { get; set; }

        [XmlElement("top250")]
        public int Top250 { get; set; }

        [XmlElement("votes")]
        public int Votes { get; set; }

        [XmlElement("outline")]
        public string Outline { get; set; }

        [XmlElement("plot")]
        public string Plot { get; set; }

        [XmlElement("tagline")]
        public string TagLine { get; set; }

        [XmlElement("runtime")]
        public string Runtime { get; set; }

        [XmlElement("thumb")]
        public string PosterThumbnail { get; set; }

        [XmlElement("mpaa")]
        public string Mpaa { get; set; } = "Not Available";

        /// <summary>
        ///     setting this to > 0 will mark the movie as watched if the "importwatchedstate" flag is set in advancedsettings.xml
        /// </summary>
        [XmlElement("playcount")]
        public int PlayCount { get; set; }

        [XmlElement("id")]
        public string ImdbId { get; set; }

        [XmlElement("tmdbid")]
        public int TmdbId { get; set; }


        [XmlIgnore, XmlElement("filenameandpath")]
        public string FilenameAndPath { get; set; }

        [XmlElement("trailer")]
        public string Trailer { get; set; }

        [XmlElement("genre")]
        public string Genre { get; set; }

        [XmlElement("credits")]
        public string Credits { get; set; }

        [XmlElement("fileinfo")]
        public VideoStreamDetails StreamDetails { get; set; } = new VideoStreamDetails();
        [XmlElement("studio")]
        public string MovieStudio { get; set; }

        [XmlElement("director")]
        public string Director { get; set; }

        [XmlElement("actor")]
        public ActorName[] Actors;

        public void AddActors(string n, string r)
        {
            if (Actors == null)
            {
                Actors = new ActorName[1];
                Actors[0] = new ActorName(n, r);
            }
            else
            {
                var l = Actors.ToList();
                l.Add(new ActorName(n, r));
                Actors = l.ToArray();
            }
        }

        public static string ToStaticTitle(string s)
        {
            var l = s.Split(' ');
            if (l.Any() && l[0].Equals("the", StringComparison.InvariantCultureIgnoreCase))
            {
                var b = l.Skip(1).ToArray();
                return string.Join(" ", b) + ", The";
            }
            return "";
        }

        public static RootMovie Convert(Movie movie)
        {
            var rm = new RootMovie
            {
                Title = movie.Title,
                OriginalTitle = movie.OriginalTitle,
                SortTitle = ToStaticTitle(movie.Title),
                Rating = movie.VoteAverage,
                Top250 = 0,
                Votes = movie.VoteCount,
                Outline = movie.Overview,
                TagLine = movie.Tagline,
                Runtime = movie.Runtime?.ToString() ?? "0",
                PosterThumbnail = @"https://image.tmdb.org/t/p/w300_and_h450_bestv2"+ movie.PosterPath,
                ImdbId = movie.ImdbId,
                TmdbId = movie.Id,
                Credits = movie.Credits.Crew.FirstOrDefault(x => x.Job == "Writer")?.Name,
                MovieStudio = movie.ProductionCompanies.FirstOrDefault()?.Name,
                Director = movie.Credits.Crew.FirstOrDefault(x => x.Job == "Director")?.Name,
                Actors = movie.Credits.Cast.Select(x => new ActorName(x.Name, x.Character)).ToArray(),
                Genre = movie.Genres.FirstOrDefault()?.Name,
                ReleaseYear = movie.ReleaseDate?.Year ?? 0,
                CollectionName = movie.BelongsToCollection?.Name
            };



            return rm;
        }
    }
}