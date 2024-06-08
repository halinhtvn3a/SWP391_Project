using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DAOs.Helper
{
    public class GeocodingService
    {
        private const string GeocodingApiUrl = "https://maps.googleapis.com/maps/api/geocode/json";

        public static async Task<LocationModel> GetGeolocationAsync(string address)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var requestUri = $"{GeocodingApiUrl}?address={Uri.EscapeDataString(address)}&key=";

                HttpResponseMessage response = await httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var geocodingResponse = JsonConvert.DeserializeObject<GeocodingResponse>(content);

                if (geocodingResponse.Status == "OK")
                {
                    var location = geocodingResponse.Results.First();
                    return new LocationModel
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    };
                }

                throw new Exception("Unable to geocode address");
            }
        }
    }

    public class GeocodingResponse
    {
        public string Status { get; set; }
        public IEnumerable<LocationModel> Results { get; set; }

    }

}
