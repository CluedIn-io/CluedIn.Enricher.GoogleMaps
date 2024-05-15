using System;
using System.Collections.Generic;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps
{
    public static class Constants
    {
        public const string ComponentName = "GoogleMaps";
        public const string ProviderName = "Google Maps";
        public static readonly Guid ProviderId = Guid.Parse("7999344b-2ee6-462a-886a-a630b169117c");

        public struct KeyName
        {
            public const string ApiToken = "apiToken";
            public const string AcceptedEntityType = "acceptedEntityType";
            public const string OrgNameKey = "orgNameKey";
            public const string OrgAddressKey = "orgAddressKey";
            public const string OrgZipCodeKey = "orgZipCodeKey";
            public const string OrgCityKey = "orgCityKey";
            public const string OrgStateKey = "orgStateKey";
            public const string OrgCountryKey = "orgCountryKey";
            public const string LocationAddressKey = "locationAddressKey";
            public const string UserAddressKey = "userAddressKey";
            public const string PersonAddressKey = "personAddressKey";
            public const string PersonAddressCityKey = "personAddressCityKey";
            public const string LatitudeKey = "latitudeKey";
            public const string LongitudeKey = "longitudeKey";
        }

        public static string About { get; set; } = "Google Maps is a web mapping platform and consumer application offered by Google. It offers satellite imagery, aerial photography, street maps, 360° interactive panoramic views of streets, real-time traffic conditions, and route planning for traveling by foot, car, air and public transportation.";
        public static string Icon { get; set; } = "Resources.Google_Maps_icon_2020.svg";
        public static string Domain { get; set; } = "N/A";

        public static AuthMethods AuthMethods { get; set; } = new AuthMethods
        {
            token = new List<Control>()
            {
                new Control()
                {
                    displayName = "Api Key",
                    type = "input",
                    isRequired = true,
                    name = KeyName.ApiToken
                },
                new Control()
                {
                    displayName = "Accepted Entity Type",
                    type = "input",
                    isRequired = false,
                    name = KeyName.AcceptedEntityType
                },
                new Control()
                {
                    displayName = "Organization Name vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgNameKey
                },
                new Control()
                {
                    displayName = "Organization Address vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgAddressKey
                },
                 new Control()
                {
                    displayName = "Organization City vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgCityKey
                },
                new Control()
                {
                    displayName = "Organization Zip vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgZipCodeKey
                },
                new Control()
                {
                    displayName = "Organization State vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgStateKey
                },
                new Control()
                {
                    displayName = "Organization Country vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgCountryKey
                },
                new Control()
                {
                    displayName = "Location Address vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.LocationAddressKey
                },
                new Control()
                {
                    displayName = "User Address vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.UserAddressKey
                },
                new Control()
                {
                    displayName = "Person Address vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.PersonAddressKey
                },
                new Control()
                {
                    displayName = "Person Address City vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.PersonAddressCityKey
                },
                new Control()
                {
                    displayName = "Latitude vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.LatitudeKey
                },
                new Control()
                {
                    displayName = "Longitude vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.LongitudeKey
                },
            }
        };

        public static IEnumerable<Control> Properties { get; set; } = new List<Control>()
        {
            // NOTE: Leaving this commented as an example - BF
            //new()
            //{
            //    displayName = "Some Data",
            //    type = "input",
            //    isRequired = true,
            //    name = "someData"
            //}
        };

        public static Guide Guide { get; set; } = null;
        public static IntegrationType IntegrationType { get; set; } = IntegrationType.Enrichment;
    }
}