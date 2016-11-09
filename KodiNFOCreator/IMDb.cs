using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

/*******************************************************************************
 * Free ASP.net IMDb Scraper API for the new IMDb Template.
 * Author: Abhinay Rathore
 * Website: http://www.AbhinayRathore.com
 * Blog: http://web3o.blogspot.com
 * More Info: http://web3o.blogspot.com/2010/11/aspnetc-imdb-scraping-api.html
 * Last Updated: Feb 20, 2013
 *******************************************************************************/

namespace LegeDoos.KodiNFOCreator.IMDb_Scraper
{
    public class ImDb
    {
        private readonly string _askSearch = "http://www.ask.com/web?q=imdb+";
        private readonly string _bingSearch = "http://www.bing.com/search?q=imdb+";

        //Search Engine URLs
        private readonly string _googleSearch = "http://www.google.com/search?q=imdb+";

        //Constructor for multiple search
        public ImDb()
        {
            InitSearchResults();
        }

        //constructor for url
        public ImDb(string url, bool simple, bool placeHolder = false) : this()
        {
            var imdbUrl = GetImDbUrl(url);
            Status = false;
            if (!string.IsNullOrEmpty(imdbUrl))
            {
                ParseImDbPage(imdbUrl, false, true);
            }
        }

        //Constructor for one url
        public ImDb(string movieName, bool getExtraInfo = true) : this()
        {
            var imdbUrl = GetImDbUrl(Uri.EscapeUriString(movieName));
            Status = false;
            if (!string.IsNullOrEmpty(imdbUrl))
            {
                ParseImDbPage(imdbUrl, getExtraInfo);
            }
        }

        public bool Status { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Year { get; set; }
        public string Rating { get; set; }
        public ArrayList Genres { get; set; }
        public ArrayList Directors { get; set; }
        public ArrayList Writers { get; set; }
        public ArrayList Cast { get; set; }
        public ArrayList Producers { get; set; }
        public ArrayList Musicians { get; set; }
        public ArrayList Cinematographers { get; set; }
        public ArrayList Editors { get; set; }
        public string MpaaRating { get; set; }
        public string ReleaseDate { get; set; }
        public string Plot { get; set; }
        public ArrayList PlotKeywords { get; set; }
        public string Poster { get; set; }
        public string PosterLarge { get; set; }
        public string PosterFull { get; set; }
        public string Runtime { get; set; }
        public string Top250 { get; set; }
        public string Oscars { get; set; }
        public string Awards { get; set; }
        public string Nominations { get; set; }
        public string Storyline { get; set; }
        public string Tagline { get; set; }
        public string Votes { get; set; }
        public ArrayList Languages { get; set; }
        public ArrayList Countries { get; set; }
        public ArrayList ReleaseDates { get; set; }
        public ArrayList MediaImages { get; set; }
        public ArrayList RecommendedTitles { get; set; }
        public string ImdbUrl { get; set; }
        public Dictionary<string, ImDb> SearchResults { get; private set; } //url, title

        //Partial title search
        public void SearchTitles(string partialTitle, string searchEngine = "google", bool reset = true)
        {
            //todo: minimum aantal karakters?
            if (reset)
            {
                InitSearchResults();
            }
            var url = _googleSearch + partialTitle; //default to Google search
            if (searchEngine.ToLower().Equals("bing")) url = _bingSearch + partialTitle;
            if (searchEngine.ToLower().Equals("ask")) url = _askSearch + partialTitle;
            var html = GetUrlData(url);
            var imdbUrls = MatchAllToList(@"(http://www.imdb.com/title/tt\d{7}/)", html);
            if (imdbUrls.Count > 0)
            {
                foreach (var s in imdbUrls)
                {
                    //todo: performance: query page only once
                    SearchResults.Add(s, new ImDb(s));
                }
            }
            else if (searchEngine.ToLower().Equals("google")) //if Google search fails
                SearchTitles(partialTitle, "bing", false); //search using Bing
            else if (searchEngine.ToLower().Equals("bing")) //if Bing search fails
                SearchTitles(partialTitle, "ask", false); //search using Ask
        }

        //Get IMDb URL from search results
        private string GetImDbUrl(string movieName, string searchEngine = "google")
        {
            var url = _googleSearch + movieName; //default to Google search
            if (searchEngine.ToLower().Equals("bing")) url = _bingSearch + movieName;
            if (searchEngine.ToLower().Equals("ask")) url = _askSearch + movieName;
            var html = GetUrlData(url);
            var imdbUrls = MatchAll(@"(http://www.imdb.com/title/tt\d{7}/)", html);
            if (imdbUrls.Count > 0)
                return (string) imdbUrls[0]; //return first IMDb result
            if (searchEngine.ToLower().Equals("google")) //if Google search fails
                return GetImDbUrl(movieName, "bing"); //search using Bing
            if (searchEngine.ToLower().Equals("bing")) //if Bing search fails
                return GetImDbUrl(movieName, "ask"); //search using Ask
            return string.Empty;
        }

        //Parse IMDb page data
        private void ParseImDbPage(string imdbUrl, bool getExtraInfo, bool getSimpleInfo = false)
        {
            var html = GetUrlData(imdbUrl + "combined");
            Id = Match(@"<link rel=""canonical"" href=""http://www.imdb.com/title/(tt\d{7})/combined"" />", html);
            if (!string.IsNullOrEmpty(Id))
            {
                Status = true;
                Title = Match(@"<title>(IMDb \- )*(.*?) \(.*?</title>", html, 2);
                Year = Match(@"<title>.*?\(.*?(\d{4}).*?\).*?</title>", html);
                if (getSimpleInfo)
                    return;
                OriginalTitle = Match(@"title-extra"">(.*?)<", html);
                Rating = Match(@"<b>(\d.\d)/10</b>", html);
                Genres = MatchAll(@"<a.*?>(.*?)</a>", Match(@"Genre.?:(.*?)(</div>|See more)", html));
                Directors = MatchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>",
                    Match(@"Directed by</a></h5>(.*?)</table>", html));
                Writers = MatchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>",
                    Match(@"Writing credits</a></h5>(.*?)</table>", html));
                Producers = MatchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>",
                    Match(@"Produced by</a></h5>(.*?)</table>", html));
                Musicians = MatchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>",
                    Match(@"Original Music by</a></h5>(.*?)</table>", html));
                Cinematographers = MatchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>",
                    Match(@"Cinematography by</a></h5>(.*?)</table>", html));
                Editors = MatchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>",
                    Match(@"Film Editing by</a></h5>(.*?)</table>", html));
                Cast = MatchAll(@"<td class=""nm""><a.*?href=""/name/.*?/"".*?>(.*?)</a>",
                    Match(@"<h3>Cast</h3>(.*?)</table>", html));
                Plot = Match(@"Plot:</h5>.*?<div class=""info-content"">(.*?)(<a|</div)", html);
                PlotKeywords = MatchAll(@"<a.*?>(.*?)</a>",
                    Match(@"Plot Keywords:</h5>.*?<div class=""info-content"">(.*?)</div", html));
                ReleaseDate =
                    Match(
                        @"Release Date:</h5>.*?<div class=""info-content"">.*?(\d{1,2} (January|February|March|April|May|June|July|August|September|October|November|December) (19|20)\d{2})",
                        html);
                Runtime = Match(@"Runtime:</h5><div class=""info-content"">(\d{1,4}) min[\s]*.*?</div>", html);
                Top250 = Match(@"Top 250: #(\d{1,3})<", html);
                Oscars = Match(@"Won (\d+) Oscars?\.", html);
                if (string.IsNullOrEmpty(Oscars) && "Won Oscar.".Equals(Match(@"(Won Oscar\.)", html))) Oscars = "1";
                Awards = Match(@"(\d{1,4}) wins", html);
                Nominations = Match(@"(\d{1,4}) nominations", html);
                Tagline = Match(@"Tagline:</h5>.*?<div class=""info-content"">(.*?)(<a|</div)", html);
                MpaaRating = Match(@"MPAA</a>:</h5><div class=""info-content"">Rated (G|PG|PG-13|PG-14|R|NC-17|X) ",
                    html);
                Votes = Match(@">(\d+,?\d*) votes<", html);
                Languages = MatchAll(@"<a.*?>(.*?)</a>", Match(@"Language.?:(.*?)(</div>|>.?and )", html));
                Countries = MatchAll(@"<a.*?>(.*?)</a>", Match(@"Country:(.*?)(</div>|>.?and )", html));
                Poster = Match(@"<div class=""photo"">.*?<a name=""poster"".*?><img.*?src=""(.*?)"".*?</div>", html);
                if (!string.IsNullOrEmpty(Poster) && Poster.IndexOf("media-imdb.com") > 0)
                {
                    Poster = Regex.Replace(Poster, @"_V1.*?.jpg", "_V1._SY200.jpg");
                    PosterLarge = Regex.Replace(Poster, @"_V1.*?.jpg", "_V1._SY500.jpg");
                    PosterFull = Regex.Replace(Poster, @"_V1.*?.jpg", "_V1._SY0.jpg");
                }
                else
                {
                    Poster = string.Empty;
                    PosterLarge = string.Empty;
                    PosterFull = string.Empty;
                }
                ImdbUrl = "http://www.imdb.com/title/" + Id + "/";
                if (getExtraInfo)
                {
                    var plotHtml = GetUrlData(imdbUrl + "plotsummary");
                    Storyline = Match(@"<p class=""plotpar"">(.*?)(<i>|</p>)", plotHtml);
                    ReleaseDates = GetReleaseDates();
                    MediaImages = GetMediaImages();
                    RecommendedTitles = GetRecommendedTitles();
                }
            }
        }

        //Get all release dates
        private ArrayList GetReleaseDates()
        {
            var list = new ArrayList();
            var releasehtml = GetUrlData("http://www.imdb.com/title/" + Id + "/releaseinfo");
            foreach (string r in MatchAll(@"<tr>(.*?)</tr>", Match(@"Date</th></tr>\n*?(.*?)</table>", releasehtml)))
            {
                var rd =
                    new Regex(@"<td>(.*?)</td>\n*?.*?<td align=""right"">(.*?)</td>", RegexOptions.Multiline).Match(r);
                list.Add(StripHtml(rd.Groups[1].Value.Trim()) + " = " + StripHtml(rd.Groups[2].Value.Trim()));
            }
            return list;
        }

        //Get all media images
        private ArrayList GetMediaImages()
        {
            var list = new ArrayList();
            var mediaurl = "http://www.imdb.com/title/" + Id + "/mediaindex";
            var mediahtml = GetUrlData(mediaurl);
            var pagecount =
                MatchAll(@"<a href=""\?page=(.*?)"">", Match(@"<span style=""padding: 0 1em;"">(.*?)</span>", mediahtml))
                    .Count;
            for (var p = 1; p <= pagecount + 1; p++)
            {
                mediahtml = GetUrlData(mediaurl + "?page=" + p);
                foreach (
                    Match m in
                        new Regex(@"src=""(.*?)""", RegexOptions.Multiline).Matches(
                            Match(@"<div class=""thumb_list"" style=""font-size: 0px;"">(.*?)</div>", mediahtml)))
                {
                    var image = m.Groups[1].Value;
                    list.Add(Regex.Replace(image, @"_V1\..*?.jpg", "_V1._SY0.jpg"));
                }
            }
            return list;
        }

        //Get Recommended Titles
        private ArrayList GetRecommendedTitles()
        {
            var list = new ArrayList();
            var recUrl = "http://www.imdb.com/widget/recommendations/_ajax/get_more_recs?specs=p13nsims%3A" + Id;
            var json = GetUrlData(recUrl);
            list = MatchAll(@"title=\\""(.*?)\\""", json);
            var set = new HashSet<string>();
            foreach (string rec in list) set.Add(rec);
            return new ArrayList(set.ToList());
        }

        /*******************************[ Helper Methods ]********************************/

        //Match single instance
        private string Match(string regex, string html, int i = 1)
        {
            return new Regex(regex, RegexOptions.Multiline).Match(html).Groups[i].Value.Trim();
        }

        //Match all instances and return as ArrayList
        private ArrayList MatchAll(string regex, string html, int i = 1)
        {
            var list = new ArrayList();
            foreach (Match m in new Regex(regex, RegexOptions.Multiline).Matches(html))
                list.Add(m.Groups[i].Value.Trim());
            return list;
        }

        //Match all instances and return as List
        private List<string> MatchAllToList(string regex, string html, int i = 1)
        {
            var retVal = new List<string>();
            foreach (Match m in new Regex(regex, RegexOptions.Multiline).Matches(html))
            {
                retVal.Add(m.Groups[i].Value.Trim());
            }
            return retVal.Distinct().ToList();
        }

        //Strip HTML Tags
        private static string StripHtml(string inputString)
        {
            return Regex.Replace(inputString, @"<.*?>", string.Empty);
        }

        //Get URL Data
        private string GetUrlData(string url)
        {
            var client = new WebClient();
            var r = new Random();
            //Random IP Address
            client.Headers["X-Forwarded-For"] = r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255) + "." +
                                                r.Next(0, 255);
            //Random User-Agent
            client.Headers["User-Agent"] = "Mozilla/" + r.Next(3, 5) + ".0 (Windows NT " + r.Next(3, 5) + "." +
                                           r.Next(0, 2) + "; rv:2.0.1) Gecko/20100101 Firefox/" + r.Next(3, 5) + "." +
                                           r.Next(0, 5) + "." + r.Next(0, 5);
            var datastream = client.OpenRead(url);
            var reader = new StreamReader(datastream);
            var sb = new StringBuilder();
            while (!reader.EndOfStream)
                sb.Append(reader.ReadLine());
            return sb.ToString();
        }

        //init dictionary searchresults
        private void InitSearchResults()
        {
            SearchResults = new Dictionary<string, ImDb>();
        }
    }
}