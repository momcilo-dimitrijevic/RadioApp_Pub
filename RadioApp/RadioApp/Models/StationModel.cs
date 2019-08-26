using System;
using System.Collections.Generic;
using System.Text;
using RadioApp.Dto;

namespace RadioApp.Models
{
    public class StationModel
    {
        public StationModel(StationDto dto, string playlistFormat)
        {
            BitRate = dto.BitRate;
            CurrentTrack = dto.CurrentTrack;
            Genre = dto.Genre;
            StationId = dto.Id;
            MediaType = dto.MediaType;
            Name = dto.Name;
            M3Url = $"http://yp.shoutcast.com/{playlistFormat}?id={StationId}";
            LogoUrl = dto.LogoUrl;
        }

        public long Id { get; set; }
        public long StationId { get; set; }
        public int Order { get; set; }
        public string M3Url { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string MediaType { get; set; }
        public int BitRate { get; set; }
        public string Genre { get; set; }
        public string CurrentTrack { get; set; }
        public bool Selected { get; set; }
        public string LogoUrl { get; set; }
    }
}
