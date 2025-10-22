namespace Stylora.Application.Models;

public class VTONRequest
{
    // These will be base64-encoded images
    // The format should include the data URI prefix (e.g., "data:image/png;base64,...")
    public string GarmentBase64 { get; set; }
    public string PersonBase64 { get; set; }
    public string ClothDescription { get; set; }
}