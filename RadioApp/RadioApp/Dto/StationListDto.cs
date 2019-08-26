using System.Collections.Generic;
using System.Xml.Serialization;

namespace RadioApp.Dto
{
    public class StationListDto
    {
        public StationListDto()
        {
            Stations = new List<StationDto>();
        }

        [XmlElement("station")]
        public List<StationDto> Stations { get; set; }

        [XmlElement("tunein")]
        public TuneInDto TuneIn { get; set; }
    }
}
