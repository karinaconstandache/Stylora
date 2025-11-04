using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stylora.Application.Models;
using System.Text.Json;

[ApiController]
[Route("[controller]")]
public class VTONController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VTONController> _logger;
    private readonly HuggingFaceConfiguration _huggingFaceConfig;

    public VTONController(
        IHttpClientFactory httpClientFactory, 
        ILogger<VTONController> logger,
        IOptions<HuggingFaceConfiguration> huggingFaceConfig)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _huggingFaceConfig = huggingFaceConfig.Value;
        
        // Log configuration info (without exposing the token)
        _logger.LogInformation("HuggingFace Configuration - Model: {ModelName}, API URL: {ApiUrl}", 
            _huggingFaceConfig.ModelName, _huggingFaceConfig.ApiUrl);
        
        // Set up the HTTP client with authorization header
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _huggingFaceConfig.Token);
    }

    [HttpPost("try-on")]
    public async Task<IActionResult> TryOn([FromBody] VTONRequest request)
    {
        try 
        {
            _logger.LogInformation("Received VTON request");
            
            // Validate input
            if (request == null)
            {
                _logger.LogWarning("Request is null");
                return BadRequest(new { Error = "Request cannot be null" });
            }

            if (string.IsNullOrEmpty(request.GarmentBase64) || string.IsNullOrEmpty(request.PersonBase64))
            {
                _logger.LogWarning("Missing required image data");
                return BadRequest(new { Error = "Both garment and person images are required" });
            }

            _logger.LogInformation("Garment image size: {Size} chars", request.GarmentBase64.Length);
            _logger.LogInformation("Person image size: {Size} chars", request.PersonBase64.Length);
            _logger.LogInformation("Cloth description: {Description}", request.ClothDescription);
            // 1. Construct the Gradio Payload
            _logger.LogInformation("Constructing Gradio payload");
            var payload = new GradioPayload
            {
                fn_index = 1,
                data = new List<string>
                {
                    request.GarmentBase64,
                    request.PersonBase64,
                    request.ClothDescription ?? ""
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            _logger.LogInformation("Calling Hugging Face API at: {ApiUrl}", _huggingFaceConfig.ApiUrl);
            _logger.LogInformation("Payload: {Payload}", jsonPayload);
            
            // 2. Call the Hugging Face Gradio API
            var response = await _httpClient.PostAsync(_huggingFaceConfig.ApiUrl, content);
            
            _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("External API call failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                return StatusCode((int)response.StatusCode, new { Error = "External API call failed.", Details = errorContent });
            }

            // 3. Deserialize the result and return the Base64 image
            var jsonResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Received response from API");
            var gradioResult = JsonSerializer.Deserialize<GradioResult>(jsonResponse);

            if (gradioResult?.data != null && gradioResult.data.Count > 0)
            {
                _logger.LogInformation("Successfully processed VTON request");
                return Ok(new VTONResponse
                {
                    ResultBase64 = gradioResult.data[0]
                });
            }

            _logger.LogWarning("Failed to parse result from VTON API");
            return BadRequest(new { Error = "Failed to parse result from VTON API." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing VTON request");
            return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
        }
    }
}