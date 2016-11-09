using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace LegeDoos.KodiNFOCreator
{
    internal class MovieScraperOMdb : MovieScraper
    {
        public MovieScraperOMdb()
        {
            InitSearchResults();
        }

        public override void SearchForTitlePart(string titlePart)
        {
            InitSearchResults();
            var request = CreateSearchRequest(titlePart);
            var result = MakeRequest(request);
            if (result != null)
            {
                ProcessResponse(result);
            }
        }

        #region.ServiceMethods

        private string CreateSearchRequest(string queryString)
        {
            var request = "http://www.omdbapi.com/?s={0}&r=xml";
            return string.Format(request, queryString);
        }

        private XDocument MakeRequest(string requestUrl)
        {
            try
            {
                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                var response = request.GetResponse() as HttpWebResponse;
                var xDoc = XDocument.Load(new StreamReader(response.GetResponseStream()));
                return xDoc;
            }
            catch (Exception e)
            {
                //throw new InvalidDataException();
                return null;
            }
        }

        private void ProcessResponse(XDocument response)
        {
            var rows = response.Descendants().Where(d => d.Name == "Movie");
            foreach (var element in rows)
            {
                var id = element.Attribute("imdbID").Value;
                if (id != string.Empty)
                {
                    var i = new MovieInfo(id);
                    i.Title = GetValue(element, "Title");
                    i.Year = GetValue(element, "Year");
                    i.Type = GetValue(element, "Type");
                    AddSearchResult(i.DisplayTitle, i);
                }
            }
        }

        private string GetValue(XElement element, string p)
        {
            var retVal = "";
            try
            {
                retVal = element.Attribute(p).Value;
            }
            catch (Exception)
            {
                retVal = "";
            }
            return retVal;
        }

        #endregion
    }
}