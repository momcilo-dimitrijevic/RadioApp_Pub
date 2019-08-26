using System.Xml.Serialization;

namespace RadioApp.Dto
{
    [XmlType("tunein")]
    public class TuneInDto
    {
        [XmlAttribute("base")]
        public string Base { get; set; }

        [XmlAttribute("base-m3u")]
        public string BaseM3U { get; set; }

        [XmlAttribute("base-xspf")]
        public string BaseXspf { get; set; }
    }
}
