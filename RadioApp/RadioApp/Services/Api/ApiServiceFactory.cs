using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Refit;

namespace RadioApp.Services.Api
{
    public class ApiServiceFactory
    {
        protected static HttpClient _client;
        protected static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient()
                    {
                        BaseAddress = new Uri(Constants.ShoutCastApiUrl)
                    };
                }
                return _client;
            }
        }
    }

    public class ApiServiceFactory<T> : ApiServiceFactory
    {
        public static T Instance = RestService.For<T>(Client,
            new RefitSettings
            {
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                }
            });
    }

    public static class ApiServiceFactory<T, U> where U : T, new()
    {
        public static T Instance = new U();
    }
}
