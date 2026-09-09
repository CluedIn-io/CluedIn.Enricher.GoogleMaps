using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps.Models.Place;

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
    [JsonProperty("business_status")]
    [JsonPropertyName("business_status")]
    public string BusinessStatus { get; set; }
        
    [JsonProperty("formatted_address")]
    [JsonPropertyName("formatted_address")]
    public string FormattedAddress { get; set; }
        
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
        
    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }
        
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
        
    [JsonProperty("types")]
    [JsonPropertyName("types")]
    public List<string> Types { get; set; }
        
    [JsonProperty("user_ratings_total")]
    [JsonPropertyName("user_ratings_total")]
    public int UserRatingsTotal { get; set; }
}

public class PlaceIdResponse
{
    [JsonProperty("html_attributions")]
    [JsonPropertyName("html_attributions")]
    public List<object> HtmlAttributions { get; set; }
        
    [JsonProperty("results")]
    [JsonPropertyName("results")]
    public List<Result> Results { get; set; }
        
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