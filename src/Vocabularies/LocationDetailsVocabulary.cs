
using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps.Vocabularies
{
    public class LocationDetailsVocabulary : SimpleVocabulary
    {
        public LocationDetailsVocabulary()
        {
            this.VocabularyName = "GoogleMaps Location";
            this.KeyPrefix = "googleMaps.Location";
            this.KeySeparator = ".";
            this.Grouping = EntityType.Location;

            this.AddGroup("Location Details", group =>
            {

                this.FormattedAddress = group.Add(new VocabularyKey("formattedAddress", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.Geometry = group.Add(new VocabularyKey("Geometry", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                this.Latitude = group.Add(new VocabularyKey("Latitude", VocabularyKeyDataType.GeographyCoordinates, VocabularyKeyVisibility.Hidden));
                this.Longitude = group.Add(new VocabularyKey("Longitude", VocabularyKeyDataType.GeographyCoordinates, VocabularyKeyVisibility.Hidden));
                this.Name = group.Add(new VocabularyKey("namE", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.NameStreet = group.Add(new VocabularyKey("NameStreet", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.NumberStreet = group.Add(new VocabularyKey("NumberStreet", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.NameCity = group.Add(new VocabularyKey("NameCity", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.CodeCountry = group.Add(new VocabularyKey("CodeCountry", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.CodePostal = group.Add(new VocabularyKey("CodePostal", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.AdministrativeArea = group.Add(new VocabularyKey("AdministrativeArea", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                this.ComponentsAddress = group.Add(new VocabularyKey("componentsAddress", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            });

            this.AddMapping(this.FormattedAddress, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInLocation.Address);
            this.AddMapping(this.Latitude, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInLocation.AddressLattitude);
            this.AddMapping(this.Longitude, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInLocation.AddressLongitude);

        }


        public VocabularyKey FormattedAddress { get; set; }
        public VocabularyKey Geometry { get; set; }
        public VocabularyKey Name { get; set; }
        public VocabularyKey NameStreet { get; set; }
        public VocabularyKey NumberStreet { get; set; }
        public VocabularyKey NameCity { get; set; }
        public VocabularyKey CodeCountry { get; set; }
        public VocabularyKey CodePostal { get; set; }
        public VocabularyKey AdministrativeArea { get; set; }
        public VocabularyKey ComponentsAddress { get; set; }
        public VocabularyKey Latitude { get; set; }
        public VocabularyKey Longitude { get; set; }

    }

}