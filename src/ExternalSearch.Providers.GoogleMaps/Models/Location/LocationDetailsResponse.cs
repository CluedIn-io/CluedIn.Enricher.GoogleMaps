using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps.Models.Location;
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

public class Location
{

    [JsonProperty("lat")]
    [JsonPropertyName("lat")]
    public string Lat { get; set; }

    [JsonProperty("lng")]
    [JsonPropertyName("lng")]
    public string Lng { get; set; }
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

public class Southwest
{

    [JsonProperty("lat")]
    [JsonPropertyName("lat")]
    public string Lat { get; set; }

    [JsonProperty("lng")]
    [JsonPropertyName("lng")]
    public string Lng { get; set; }
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

public class Geometry
{

    [JsonProperty("location")]
    [JsonPropertyName("location")]
    public Location Location { get; set; }

    [JsonProperty("viewport")]
    [JsonPropertyName("viewport")]
    public Viewport Viewport { get; set; }
}

public class Result
{

[JsonProperty("address_components")]
[JsonPropertyName("address_components")]
public List<AddressComponent> AddressComponents { get; set; }

[JsonProperty("adr_address")]
[JsonPropertyName("adr_address")]
public string AdrAddress { get; set; }

[JsonProperty("formatted_address")]
[JsonPropertyName("formatted_address")]
public string FormattedAddress { get; set; }

[JsonProperty("geometry")]
[JsonPropertyName("geometry")]
public Geometry Geometry { get; set; }

[JsonProperty("name")]
[JsonPropertyName("name")]
public string Name { get; set; }
}

public class LocationDetailsResponse
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