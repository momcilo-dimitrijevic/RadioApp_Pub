using System.Collections.Generic;
using System.Threading.Tasks;
using RadioApp.Models;

namespace RadioApp.Services
{
    public interface IStationService
    {
        Task<List<StationModel>> Get(string filter = null);

        Task<string> GetStreamUrl(string url);
    }
}
