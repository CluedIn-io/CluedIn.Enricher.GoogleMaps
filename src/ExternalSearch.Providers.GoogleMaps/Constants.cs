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
        public const string Instruction = """
            [
              {
                "type": "bulleted-list",
                "children": [
                  {
                    "type": "list-item",
                    "children": [
                      {
                        "text": "Add the business domain to specify the golden records you want to enrich. Only golden records belonging to that business domain will be enriched."
                      }
                    ]
                  },
                  {
                    "type": "list-item",
                    "children": [
                      {
                        "text": "Add the vocabulary keys to provide the input for the enricher to search for additional information. For example, if you provide the website vocabulary key for the Web enricher, it will use specific websites to look for information about companies. In some cases, vocabulary keys are not required. If you don't add them, the enricher will use default vocabulary keys."
                      }
                    ]
                  },
                  {
                    "type": "list-item",
                    "children": [
                      {
                        "text": "Add the API key to enable the enricher to retrieve information from a specific API. For example, the Vatlayer enricher requires an access key to authenticate with the Vatlayer API."
                      }
                    ]
                  }
                ]
              }
            ]
            """;

        public struct KeyName
        {
            public const string ApiToken = "apiToken";
            public const string AcceptedEntityType = "acceptedEntityType";
            public const string ControlFlag = "controlFlag";
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
            Token = new List<Control>
            {
                new()
                {
                    DisplayName = "API Key",
                    Type = "password",
                    IsRequired = true,
                    Name = KeyName.ApiToken,
                    Help = "The key to authenticate access to the Google Maps Platform API.",
                    ValidationRules = new List<Dictionary<string, string>>()
                    {
                        new() {
                            { "regex", "\\s" },
                            { "message", "Spaces are not allowed" }
                        }
                    },
                },
                new()
                {
                    DisplayName = "Accepted Business Domain",
                    Type = "entityTypeSelector",
                    IsRequired = true,
                    Name = KeyName.AcceptedEntityType,
                    Help = "The business domain that defines the golden records you want to enrich (e.g., /Organization)."
                },
                new()
                {
                    DisplayName = "Vocabulary Key used to control whether it should be enriched",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.ControlFlag,
                    Help = "The vocabulary key that determines whether to enrich the golden record. If the value is True, the golden record will be enriched."
                },
                new()
                {
                    DisplayName = "Organization Name Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.OrgNameKey,
                    Help = "The vocabulary key that contains the names of companies you want to enrich (e.g., organization.name)."
                },
                new()
                {
                    DisplayName = "Organization Address Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.OrgAddressKey,
                    Help = "The vocabulary key that contains the addresses of companies you want to enrich (e.g., organization.address)."
                },
                 new()
                {
                    DisplayName = "Organization City Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.OrgCityKey,
                    Help = "The vocabulary key that contains the cities of companies you want to enrich (e.g., organization.city)."
                },
                new()
                {
                    DisplayName = "Organization Zip Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.OrgZipCodeKey,
                    Help = "The vocabulary key that contains the ZIP Codes of companies you want to enrich (e.g., organization.address.zipCode)."
                },
                new()
                {
                    DisplayName = "Organization State Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.OrgStateKey,
                    Help = "The vocabulary key that contains the states of companies you want to enrich (e.g., organization.address.state)."
                },
                new()
                {
                    DisplayName = "Organization Country Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.OrgCountryKey,
                    Help = "The vocabulary key that contains the countries of companies you want to enrich (e.g., organization.country)."
                },
                new()
                {
                    DisplayName = "Location Address Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.LocationAddressKey,
                    Help = "The vocabulary key that contains the addresses of locations you want to enrich (e.g., location.fullAddress)."
                },
                new()
                {
                    DisplayName = "User Address Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.UserAddressKey,
                    Help = "The vocabulary key that contains the addresses of users you want to enrich (e.g., user.home.address)."
                },
                new()
                {
                    DisplayName = "Person Address Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.PersonAddressKey,
                    Help = "The vocabulary key that contains the addresses of persons you want to enrich (e.g., person.home.address)."
                },
                new()
                {
                    DisplayName = "Person Address City Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.PersonAddressCityKey,
                    Help = "The vocabulary key that contains the city addresses of persons you want to enrich (e.g., person.home.address.city)."
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

        public static Guide Guide { get; set; } = new Guide
        {
            Instructions = Instruction
        };
        public static IntegrationType IntegrationType { get; set; } = IntegrationType.Enrichment;
    }
}