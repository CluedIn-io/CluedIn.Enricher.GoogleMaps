﻿using System.Collections.Generic;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps
{
    public static class GoogleMapsConstants
    {
        public const string ProviderName = "GoogleMaps";

        public struct KeyName
        {
            public const string ApiToken = "apiToken";

        }

        public static string About { get; set; } = "Google Maps is a web mapping platform and consumer application offered by Google. It offers satellite imagery, aerial photography, street maps, 360° interactive panoramic views of streets, real-time traffic conditions, and route planning for traveling by foot, car, air and public transportation.";
        public static string Icon { get; set; } = "Resources.Google_Maps_icon_2020.svg";
        public static string Domain { get; set; } = "N/A";

        public static AuthMethods AuthMethods { get; set; } = new AuthMethods
        {
            token = new List<Control>()
            {
                new()
                {
                    displayName = "Api Key",
                    type = "input",
                    isRequired = true,
                    name = "apiKey"
                }
            }
        };

        public static IEnumerable<Control> Properties { get; set; } = new List<Control>()
        {
            new()
            {
                displayName = "Some Data",
                type = "input",
                isRequired = true,
                name = "someData"
            }
        };

        public static Guide Guide { get; set; } = null;
        public static IntegrationType IntegrationType { get; set; } = IntegrationType.Cloud;
    }
}