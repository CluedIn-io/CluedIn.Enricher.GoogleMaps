using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps.Vocabularies
{
    public class CompanyDetailsVocabulary : SimpleVocabulary
    {
        public CompanyDetailsVocabulary()
        {
            this.VocabularyName = "GoogleMaps Organization";
            this.KeyPrefix = "googleMaps.Organization";
            this.KeySeparator = ".";
            this.Grouping = EntityType.Organization;

            this.AddGroup("CompanyDetails Organization Details", group =>
            {
                this.AddressComponents = group.Add(new VocabularyKey("AddressComponents", VocabularyKeyDataType.Json, VocabularyKeyVisibility.Hidden));
                this.PostalCode = group.Add(new VocabularyKey("PostalCode", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.StreetNumber = group.Add(new VocabularyKey("StreeNumber", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.StreetName = group.Add(new VocabularyKey("StreetName", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.CityName = group.Add(new VocabularyKey("CityName", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.CountryCode = group.Add(new VocabularyKey("CountryCode", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.AdrAddress = group.Add(new VocabularyKey("AdrAddress", VocabularyKeyDataType.Html, VocabularyKeyVisibility.Visible));
                this.FormattedAddress = group.Add(new VocabularyKey("FormattedAddress", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.FormattedPhoneNumber = group.Add(new VocabularyKey("FormattedPhoneNumber", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.Geometry = group.Add(new VocabularyKey("Geometry", VocabularyKeyDataType.GeographyCoordinates, VocabularyKeyVisibility.Visible));
                this.Icon = group.Add(new VocabularyKey("Icon", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.Id = group.Add(new VocabularyKey("Id", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.InternationalPhoneNumber = group.Add(new VocabularyKey("InternationalPhoneNumber", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.Name = group.Add(new VocabularyKey("Name", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.OpeningHours = group.Add(new VocabularyKey("OpeningHours", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.PlaceId = group.Add(new VocabularyKey("PlaceId", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.PlusCode = group.Add(new VocabularyKey("PlusCode", VocabularyKeyDataType.Json, VocabularyKeyVisibility.Hidden));
                this.Rating = group.Add(new VocabularyKey("Rating", VocabularyKeyDataType.Integer, VocabularyKeyVisibility.Hidden));
                this.Reference = group.Add(new VocabularyKey("Reference", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.Reviews = group.Add(new VocabularyKey("Reviews", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.Scope = group.Add(new VocabularyKey("Scope", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.Types = group.Add(new VocabularyKey("Types", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.Url = group.Add(new VocabularyKey("Url", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.UserRatingsTotal = group.Add(new VocabularyKey("UserRatingsTotal", VocabularyKeyDataType.Integer, VocabularyKeyVisibility.Hidden));
                this.UtcOffset = group.Add(new VocabularyKey("UtcOffset", VocabularyKeyDataType.Integer, VocabularyKeyVisibility.Hidden));
                this.Vicinity = group.Add(new VocabularyKey("Vicinity", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.Website = group.Add(new VocabularyKey("Website", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));

            });

            this.AddMapping(this.FormattedAddress, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Address);
            this.AddMapping(this.StreetNumber, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressStreetNumber);
            this.AddMapping(this.StreetName, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressStreetName);
            this.AddMapping(this.CityName, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCity);
            this.AddMapping(this.CountryCode, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode);
            this.AddMapping(this.PostalCode, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressZipCode);
            this.AddMapping(this.FormattedPhoneNumber, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.PhoneNumber);
        }

        public VocabularyKey AddressComponents { get; set; }
        public VocabularyKey PostalCode { get; set; }
        public VocabularyKey StreetNumber { get; set; }
        public VocabularyKey StreetName { get; set; }
        public VocabularyKey CityName { get; set; }
        public VocabularyKey CountryCode { get; set; }
        public VocabularyKey AdrAddress { get; set; }
        public VocabularyKey FormattedAddress { get; set; }
        public VocabularyKey FormattedPhoneNumber { get; set; }
        public VocabularyKey Geometry { get; set; }
        public VocabularyKey Icon { get; set; }
        public VocabularyKey Id { get; set; }
        public VocabularyKey InternationalPhoneNumber { get; set; }
        public VocabularyKey Name { get; set; }
        public VocabularyKey OpeningHours { get; set; }
        public VocabularyKey PlaceId { get; set; }
        public VocabularyKey PlusCode { get; set; }
        public VocabularyKey Rating { get; set; }
        public VocabularyKey Reference { get; set; }
        public VocabularyKey Reviews { get; set; }
        public VocabularyKey Scope { get; set; }
        public VocabularyKey Types { get; set; }
        public VocabularyKey Url { get; set; }
        public VocabularyKey UserRatingsTotal { get; set; }
        public VocabularyKey UtcOffset { get; set; }
        public VocabularyKey Vicinity { get; set; }
        public VocabularyKey Website { get; set; }
    }

}