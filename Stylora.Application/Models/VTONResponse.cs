namespace Stylora.Application.Models;

public class VTONResponse
{
    // The resulting try-on image as a Base64 string
    public string ResultBase64 { get; set; }
}

// Helper class for Gradio API payload
public class GradioPayload
{
    // This index must match the primary function in the Gradio app
    public int fn_index { get; set; } = 0;

    // The data array must strictly match the order of inputs on the Gradio UI
    // For IDM-VTON, this is typically [garment_image, person_image, cloth_description]
    public List<string> data { get; set; }
}

// Helper class for the Gradio API response
public class GradioResult
{
    // The Gradio API response contains a data array; the first element is the result image
    public List<string> data { get; set; }
}