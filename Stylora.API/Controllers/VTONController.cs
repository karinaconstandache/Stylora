using Microsoft.AspNetCore.Mvc;
using Stylora.Application.Models;
using System.Text.Json;

[ApiController]
[Route("[controller]")]
public class VTONController : ControllerBase
{
    private readonly HttpClient _httpClient;
    // Base URL for the Hugging Face Space
    private const string VtonApiUrl = "https://yisol-idm-vton.hf.space/run/predict";

    public VTONController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpPost("try-on")]
    public async Task<IActionResult> TryOn([FromBody] VTONRequest request)
    {
        // 1. Construct the Gradio Payload
        var payload = new GradioPayload
        {
            fn_index = 0,
            data = new List<string>
            {
                request.GarmentBase64,
                request.PersonBase64,
                request.ClothDescription ?? ""
            }
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

        // 2. Call the Hugging Face Gradio API
        var response = await _httpClient.PostAsync(VtonApiUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, new { Error = "External API call failed.", Details = errorContent });
        }

        // 3. Deserialize the result and return the Base64 image
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var gradioResult = JsonSerializer.Deserialize<GradioResult>(jsonResponse);

        if (gradioResult?.data != null && gradioResult.data.Count > 0)
        {
            return Ok(new VTONResponse
            {
                ResultBase64 = gradioResult.data[0]
            });
        }

        return BadRequest(new { Error = "Failed to parse result from VTON API." });
    }
}