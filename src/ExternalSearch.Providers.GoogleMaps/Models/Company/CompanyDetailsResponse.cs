using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps.Models.Company;
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class AddressComponent
{
    [JsonProperty("long_name")]
    [JsonPropertyName("long_name")]
    public string LongName { get; set; }

    [JsonProperty("short_name")]
    [JsonPropertyName("short_name")]
    public string ShortName { get; set; }

    [JsonProperty("types")]
    [JsonPropertyName("types")]
    public List<string> Types { get; set; }
}

public class Close
{
    [JsonProperty("date")]
    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonProperty("day")]
    [JsonPropertyName("day")]
    public int Day { get; set; }

    [JsonProperty("time")]
    [JsonPropertyName("time")]
    public string Time { get; set; }
}

public class CurrentOpeningHours
{
    [JsonProperty("open_now")]
    [JsonPropertyName("open_now")]
    public bool OpenNow { get; set; }

    [JsonProperty("periods")]
    [JsonPropertyName("periods")]
    public List<Period> Periods { get; set; }

    [JsonProperty("weekday_text")]
    [JsonPropertyName("weekday_text")]
    public List<string> WeekdayText { get; set; }
}

public class Geometry
{
    [JsonProperty("location")]
    [JsonPropertyName("location")]
    public Location Location { get; set; }

    [JsonProperty("viewport")]
    [JsonPropertyName("viewport")]
    public Viewport Viewport { get; set; }
}

public class Location
{
    [JsonProperty("lat")]
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonProperty("lng")]
    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}

public class Northeast
{
    [JsonProperty("lat")]
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonProperty("lng")]
    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}

public class Open
{
    [JsonProperty("date")]
    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonProperty("day")]
    [JsonPropertyName("day")]
    public int Day { get; set; }

    [JsonProperty("time")]
    [JsonPropertyName("time")]
    public string Time { get; set; }
}

public class OpeningHours
{
    [JsonProperty("open_now")]
    [JsonPropertyName("open_now")]
    public bool OpenNow { get; set; }

    [JsonProperty("periods")]
    [JsonPropertyName("periods")]
    public List<Period> Periods { get; set; }

    [JsonProperty("weekday_text")]
    [JsonPropertyName("weekday_text")]
    public List<string> WeekdayText { get; set; }
}

public class Period
{
    [JsonProperty("close")]
    [JsonPropertyName("close")]
    public Close Close { get; set; }

    [JsonProperty("open")]
    [JsonPropertyName("open")]
    public Open Open { get; set; }
}

public class Photo
{
    [JsonProperty("height")]
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonProperty("html_attributions")]
    [JsonPropertyName("html_attributions")]
    public List<string> HtmlAttributions { get; set; }

    [JsonProperty("photo_reference")]
    [JsonPropertyName("photo_reference")]
    public string PhotoReference { get; set; }

    [JsonProperty("width")]
    [JsonPropertyName("width")]
    public int Width { get; set; }
}

public class PlusCode
{
    [JsonProperty("compound_code")]
    [JsonPropertyName("compound_code")]
    public string CompoundCode { get; set; }

    [JsonProperty("global_code")]
    [JsonPropertyName("global_code")]
    public string GlobalCode { get; set; }
}

public class Result
{
    [JsonProperty("address_components")]
    [JsonPropertyName("address_components")]
    public List<AddressComponent> AddressComponents { get; set; }

    [JsonProperty("adr_address")]
    [JsonPropertyName("adr_address")]
    public string AdrAddress { get; set; }

    [JsonProperty("business_status")]
    [JsonPropertyName("business_status")]
    public string BusinessStatus { get; set; }

    [JsonProperty("current_opening_hours")]
    [JsonPropertyName("current_opening_hours")]
    public CurrentOpeningHours CurrentOpeningHours { get; set; }

    [JsonProperty("formatted_address")]
    [JsonPropertyName("formatted_address")]
    public string FormattedAddress { get; set; }

    [JsonProperty("formatted_phone_number")]
    [JsonPropertyName("formatted_phone_number")]
    public string FormattedPhoneNumber { get; set; }

    [JsonProperty("geometry")]
    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; set; }

    [JsonProperty("icon")]
    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    [JsonProperty("icon_background_color")]
    [JsonPropertyName("icon_background_color")]
    public string IconBackgroundColor { get; set; }

    [JsonProperty("icon_mask_base_uri")]
    [JsonPropertyName("icon_mask_base_uri")]
    public string IconMaskBaseUri { get; set; }

    [JsonProperty("international_phone_number")]
    [JsonPropertyName("international_phone_number")]
    public string InternationalPhoneNumber { get; set; }

    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonProperty("opening_hours")]
    [JsonPropertyName("opening_hours")]
    public OpeningHours OpeningHours { get; set; }

    [JsonProperty("photos")]
    [JsonPropertyName("photos")]
    public List<Photo> Photos { get; set; }

    [JsonProperty("place_id")]
    [JsonPropertyName("place_id")]
    public string PlaceId { get; set; }

    [JsonProperty("plus_code")]
    [JsonPropertyName("plus_code")]
    public PlusCode PlusCode { get; set; }

    [JsonProperty("rating")]
    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonProperty("reference")]
    [JsonPropertyName("reference")]
    public string Reference { get; set; }

    [JsonProperty("reviews")]
    [JsonPropertyName("reviews")]
    public List<Review> Reviews { get; set; }

    [JsonProperty("types")]
    [JsonPropertyName("types")]
    public List<string> Types { get; set; }

    [JsonProperty("url")]
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonProperty("user_ratings_total")]
    [JsonPropertyName("user_ratings_total")]
    public int UserRatingsTotal { get; set; }

    [JsonProperty("utc_offset")]
    [JsonPropertyName("utc_offset")]
    public int UtcOffset { get; set; }

    [JsonProperty("vicinity")]
    [JsonPropertyName("vicinity")]
    public string Vicinity { get; set; }

    [JsonProperty("website")]
    [JsonPropertyName("website")]
    public string Website { get; set; }

    [JsonProperty("wheelchair_accessible_entrance")]
    [JsonPropertyName("wheelchair_accessible_entrance")]
    public bool WheelchairAccessibleEntrance { get; set; }
}

public class Review
{
    [JsonProperty("author_name")]
    [JsonPropertyName("author_name")]
    public string AuthorName { get; set; }

    [JsonProperty("author_url")]
    [JsonPropertyName("author_url")]
    public string AuthorUrl { get; set; }

    [JsonProperty("language")]
    [JsonPropertyName("language")]
    public string Language { get; set; }

    [JsonProperty("original_language")]
    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; set; }

    [JsonProperty("profile_photo_url")]
    [JsonPropertyName("profile_photo_url")]
    public string ProfilePhotoUrl { get; set; }

    [JsonProperty("rating")]
    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonProperty("relative_time_description")]
    [JsonPropertyName("relative_time_description")]
    public string RelativeTimeDescription { get; set; }

    [JsonProperty("text")]
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonProperty("time")]
    [JsonPropertyName("time")]
    public int Time { get; set; }

    [JsonProperty("translated")]
    [JsonPropertyName("translated")]
    public bool Translated { get; set; }
}

public class CompanyDetailsResponse
{
    [JsonProperty("html_attributions")]
    [JsonPropertyName("html_attributions")]
    public List<object> HtmlAttributions { get; set; }

    [JsonProperty("result")]
    [JsonPropertyName("result")]
    public Result Result { get; set; }

    [JsonProperty("status")]
    [JsonPropertyName("status")]
    public string Status { get; set; }
}

public class Southwest
{
    [JsonProperty("lat")]
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonProperty("lng")]
    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}

public class Viewport
{
    [JsonProperty("northeast")]
    [JsonPropertyName("northeast")]
    public Northeast Northeast { get; set; }

    [JsonProperty("southwest")]
    [JsonPropertyName("southwest")]
    public Southwest Southwest { get; set; }
}