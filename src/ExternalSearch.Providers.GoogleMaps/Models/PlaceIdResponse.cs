using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps.Models
{
    public class PlaceIdCode
    {

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }
    }

    public class PlaceIdResponse
    {

        [JsonProperty("candidates")]
        public List<PlaceIdCode> Candidates { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}