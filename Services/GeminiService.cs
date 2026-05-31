using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HeThongDatLichVaKhamBenh.Services;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiService> _logger;

    public GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<GeminiSuggestionResponse?> SuggestSpecialtiesAsync(string symptoms, IEnumerable<SpecialtyInfo> availableSpecialties)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Gemini API Key is not configured.");
            return null;
        }

        var specialtyList = string.Join("\n", availableSpecialties.Select(x => $"- ID: {x.Id}, Name: {x.Name}, Default Duration: {x.AverageMinutes} mins"));

        var systemInstruction = @"Bạn là trợ lý y tế phân tích triệu chứng để điều hướng bệnh nhân đến đúng chuyên khoa.
TUYỆT ĐỐI KHÔNG đưa ra chẩn đoán y khoa chính thức. Chỉ sử dụng triệu chứng để gợi ý chuyên khoa.
Nếu có dấu hiệu nguy hiểm khẩn cấp (như khó thở nặng, đau ngực dữ dội, v.v.), hãy đặt HasUrgentWarning = true và thêm cảnh báo vào Message.
Bạn CHỈ ĐƯỢC PHÉP chọn chuyên khoa từ danh sách được cung cấp. Nếu không chắc chắn, hãy chọn chuyên khoa Nội khoa (hoặc chuyên khoa chung nhất có thể).
Kết quả trả về PHẢI là JSON đúng định dạng sau:
{
  ""Suggestions"": [
    {
      ""SpecialtyId"": ""Mã chuyên khoa"",
      ""SpecialtyName"": ""Tên chuyên khoa"",
      ""EstimatedMinutes"": Thời_gian_khám_dự_kiến_bằng_số,
      ""Reason"": ""Lý do ngắn gọn dựa trên triệu chứng (VD: Có liên quan đến đau đầu, chóng mặt...)""
    }
  ],
  ""HasUrgentWarning"": true/false,
  ""Message"": ""Thông điệp hỗ trợ ngắn gọn cho bệnh nhân.""
}";

        var prompt = $"Danh sách chuyên khoa khả dụng:\n{specialtyList}\n\nTriệu chứng của bệnh nhân: {symptoms}\n\nHãy phân tích và trả về 1 chuyên khoa phù hợp nhất (hoặc tối đa 2 chuyên khoa nếu triệu chứng thực sự đan xen) dưới dạng JSON.";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            systemInstruction = new
            {
                parts = new[] { new { text = systemInstruction } }
            },
            generationConfig = new
            {
                responseMimeType = "application/json"
            }
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={apiKey}";

        try
        {
            var response = await _httpClient.PostAsync(url, jsonContent);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API Error: {StatusCode} {Error}", response.StatusCode, error);
                return null;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var resultDocument = JsonDocument.Parse(responseJson);
            
            var root = resultDocument.RootElement;
            if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var content = candidates[0].GetProperty("content");
                if (content.TryGetProperty("parts", out var parts) && parts.GetArrayLength() > 0)
                {
                    var text = parts[0].GetProperty("text").GetString();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        text = text.Replace("```json", "").Replace("```", "").Trim();
                        var result = JsonSerializer.Deserialize<GeminiSuggestionResponse>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return result;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while calling Gemini API.");
        }

        return null;
    }
}
