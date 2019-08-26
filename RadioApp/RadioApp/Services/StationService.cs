using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using RadioApp.Dto;
using RadioApp.Models;
using RadioApp.Services.Api;
using Splat;

namespace RadioApp.Services
{
    public class StationService : IStationService
    {
        private readonly IShoutCastService _shoutCastService;
        private readonly HttpClient _httpClient;
        

        public StationService(IShoutCastService shoutCastService = null)
        {
            _shoutCastService = shoutCastService ?? Locator.CurrentMutable.GetService<IShoutCastService>();
            _httpClient = new HttpClient();
        }

        public async Task<List<StationModel>> Get(string filter = null)
        {
            var payload = await _shoutCastService.GetTop500(Constants.ShoutCastDevId);
            var stationDto = DeserializeShoutCastStations(payload, "stationlist");
            var tuneInSettings = stationDto.TuneIn;

            return stationDto.Stations
                             .Select(x => new StationModel(x, tuneInSettings.BaseM3U))
                             //.Where(x => string.IsNullOrWhiteSpace(filter) || x.Name.ToLowerInvariant().Contains(filter.ToLowerInvariant()))
                             .ToList();
        }

        public async Task<string> GetStreamUrl(string url)
        {
            var m3UContent = new M3uContent();

            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)await request.GetResponseAsync();

            var stream = response.GetResponseStream();
            var playlist = m3UContent.GetFromStream(stream);

            if (!playlist.PlaylistEntries.Any())
                return null;

            var entry = playlist.PlaylistEntries[0];

            return entry.Path;
        }

        private StationListDto DeserializeShoutCastStations(string xDocument, string xRootName)
        {
            var stations = new StationListDto();
            try
            {
                var xRoot = new XmlRootAttribute()
                {
                    ElementName = xRootName,
                    IsNullable = true
                };

                var serializer = new XmlSerializer(typeof(StationListDto), xRoot);

                using (TextReader reader = new StringReader(xDocument))
                {
                    stations = (StationListDto)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return stations;
        }
    }
}
