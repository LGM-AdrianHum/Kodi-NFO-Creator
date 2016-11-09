using System.Xml.Serialization;

namespace KodiMovieNfoLibrary
{
    [XmlRoot("fileinfo")]
    public class VideoStreamDetails
    {
        [XmlElement("streamdetails")]
        public string StreamDetails { get; set; }
    }
}