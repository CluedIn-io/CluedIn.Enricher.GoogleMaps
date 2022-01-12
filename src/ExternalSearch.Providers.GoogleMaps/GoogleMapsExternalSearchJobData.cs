using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps
{
    public class GoogleMapsExternalSearchJobData : CrawlJobData
    {
        public GoogleMapsExternalSearchJobData(IDictionary<string, object> configuration)
        {
            ApiToken = GetValue<string>(configuration, GoogleMapsConstants.KeyName.ApiToken);
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                { GoogleMapsConstants.KeyName.ApiToken, ApiToken }
            };
        }

        public string ApiToken { get; set; }
    }
}
