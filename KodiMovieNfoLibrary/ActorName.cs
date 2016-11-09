using System.Xml.Serialization;

namespace KodiMovieNfoLibrary
{
    [XmlRoot("actor")]
    public class ActorName
    {
        public ActorName()
        {
        }

        public ActorName(string n, string r)
        {
            Name = n;
            Role = r;
        }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("role")]
        public string Role { get; set; }
    }
}