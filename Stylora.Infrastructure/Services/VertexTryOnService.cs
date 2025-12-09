using System;
using System.IO; // Added for File check
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2; 
using Stylora.Domain.Interfaces;

namespace Stylora.Infrastructure.Services
{
    public class VertexTryOnService : IVirtualTryOnService
    {
        private readonly HttpClient _httpClient;
        // Verify this is your exact Project ID
        private const string ProjectId = "gen-lang-client-0932195243"; 
        private const string Location = "us-central1"; 
        private const string ModelId = "virtual-try-on-preview-08-04";

        public VertexTryOnService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> ExecuteTryOnAsync(byte[] personImage, byte[] garmentImage)
        {
            // --- DEBUG: Verify Key File Exists ---
            var keyPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            if (string.IsNullOrEmpty(keyPath) || !File.Exists(keyPath))
            {
                throw new FileNotFoundException($"GCP Key not found at: {Path.GetFullPath(keyPath ?? "null")}. Did you set 'Copy to Output Directory' to 'Copy Always'?");
            }
            // -------------------------------------

            // 1. Authenticate (CORRECTED METHOD)
            // Vertex AI requires the "Cloud Platform" scope, not an OIDC token.
            var credential = GoogleCredential.FromFile(keyPath)
                .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

            // Get the actual OAuth 2.0 token
            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            // 2. Prepare URL
            var url = $"https://{Location}-aiplatform.googleapis.com/v1/projects/{ProjectId}/locations/{Location}/publishers/google/models/{ModelId}:predict";

            // 3. Prepare Payload
            var requestBody = new
            {
                instances = new[]
                {
                    new
                    {
                        personImage = new { image = new { bytesBase64Encoded = Convert.ToBase64String(personImage) } },
                        productImages = new[]
                        {
                            new { image = new { bytesBase64Encoded = Convert.ToBase64String(garmentImage) } }
                        }
                    }
                },
                parameters = new { sampleCount = 1 }
            };

            // 4. Send Request
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(requestMessage);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Vertex AI Error: {response.StatusCode} - {responseString}");
            }

            // 5. Parse Result
            using var doc = JsonDocument.Parse(responseString);
            
            // Safety check: Ensure the response actually has predictions
            if (!doc.RootElement.TryGetProperty("predictions", out var predictions))
            {
                 throw new Exception($"API returned success but no predictions. Response: {responseString}");
            }

            var base64Result = predictions[0]
                                  .GetProperty("bytesBase64Encoded")
                                  .GetString();

            return Convert.FromBase64String(base64Result);
        }
    }
}