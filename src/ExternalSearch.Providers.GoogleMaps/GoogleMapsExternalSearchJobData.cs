using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps
{
    public class GoogleMapsExternalSearchJobData : CrawlJobData
    {
        public GoogleMapsExternalSearchJobData(IDictionary<string, object> configuration)
        {
            ApiToken = GetValue<string>(configuration, Constants.KeyName.ApiToken);
            AcceptedEntityType = GetValue<string>(configuration, Constants.KeyName.AcceptedEntityType);
            OrgNameKey = GetValue<string>(configuration, Constants.KeyName.OrgNameKey);
            OrgAddressKey = GetValue<string>(configuration, Constants.KeyName.OrgAddressKey);
            OrgZipCodeKey = GetValue<string>(configuration, Constants.KeyName.OrgZipCodeKey);
            OrgCityKey = GetValue<string>(configuration, Constants.KeyName.OrgCityKey);
            OrgStateKey = GetValue<string>(configuration, Constants.KeyName.OrgStateKey);
            OrgCountryKey = GetValue<string>(configuration, Constants.KeyName.OrgCountryKey);
            LocationAddressKey = GetValue<string>(configuration, Constants.KeyName.LocationAddressKey);
            UserAddressKey = GetValue<string>(configuration, Constants.KeyName.UserAddressKey);
            PersonAddressKey = GetValue<string>(configuration, Constants.KeyName.PersonAddressKey);
            PersonAddressCityKey = GetValue<string>(configuration, Constants.KeyName.PersonAddressCityKey);
            LatitudeKey = GetValue<string>(configuration, Constants.KeyName.LatitudeKey);
            LongitudeKey = GetValue<string>(configuration, Constants.KeyName.LongitudeKey);
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                { Constants.KeyName.ApiToken, ApiToken },
                { Constants.KeyName.AcceptedEntityType, AcceptedEntityType },
                { Constants.KeyName.OrgNameKey, OrgNameKey },
                { Constants.KeyName.OrgAddressKey, OrgAddressKey },
                { Constants.KeyName.OrgZipCodeKey, OrgZipCodeKey },
                { Constants.KeyName.OrgCityKey, OrgCityKey },
                { Constants.KeyName.OrgStateKey, OrgStateKey },
                { Constants.KeyName.OrgCountryKey, OrgCountryKey },
                { Constants.KeyName.LocationAddressKey, LocationAddressKey },
                { Constants.KeyName.UserAddressKey, UserAddressKey },
                { Constants.KeyName.PersonAddressKey, PersonAddressKey },
                { Constants.KeyName.PersonAddressCityKey, PersonAddressCityKey },
                { Constants.KeyName.LatitudeKey, LatitudeKey },
                { Constants.KeyName.LongitudeKey, LongitudeKey }
            };
        }

        public string ApiToken { get; set; }
        public string AcceptedEntityType { get; set; }
        public string OrgNameKey { get; set; }
        public string OrgAddressKey { get; set; }
        public string OrgZipCodeKey { get; set; }
        public string OrgCityKey { get; set; }
        public string OrgStateKey { get; set; }
        public string OrgCountryKey { get; set; }
        public string LocationAddressKey { get; set; }
        public string UserAddressKey { get; set; }
        public string PersonAddressKey { get; set; }
        public string PersonAddressCityKey { get; set; }
        public string LatitudeKey { get; set; }
        public string LongitudeKey { get; set; }
    }
}
