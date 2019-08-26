using System.Xml.Serialization;

namespace RadioApp.Dto
{
    [XmlType("station")]
    public class StationDto
    {
        [XmlAttribute("id")]
        public long Id { get; set; }

        [XmlAttribute("logo")]
        public string LogoUrl { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("mt")]
        public string MediaType { get; set; }

        [XmlAttribute("br")]
        public int BitRate { get; set; }

        [XmlAttribute("genre")]
        public string Genre { get; set; }

        [XmlAttribute("ct")]
        public string CurrentTrack { get; set; }
    }
}
