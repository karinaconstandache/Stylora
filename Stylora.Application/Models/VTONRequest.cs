using System.Text.Json.Serialization;

namespace Stylora.Application.Models;

public class VTONRequest
{
    // These will be base64-encoded images
    // The format should include the data URI prefix (e.g., "data:image/png;base64,...")
    [JsonPropertyName("garmentBase64")]
    public required string GarmentBase64 { get; set; }
    
    [JsonPropertyName("personBase64")]
    public required string PersonBase64 { get; set; }
    
    [JsonPropertyName("clothDescription")]
    public string ClothDescription { get; set; } = string.Empty;
}