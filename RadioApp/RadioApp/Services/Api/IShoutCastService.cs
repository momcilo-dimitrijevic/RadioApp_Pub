using System.Threading.Tasks;
using Refit;

namespace RadioApp.Services.Api
{
    public interface IShoutCastService
    {
        [Get("/legacy/Top500")]
        Task<string> GetTop500(string k);
    }
}
