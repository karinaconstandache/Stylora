public class HuggingFaceConfiguration
{
    public const string SectionName = "HuggingFace";
    
    public string Token { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
}