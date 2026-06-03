using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HeThongDatLichVaKhamBenh.Models;
using HeThongDatLichVaKhamBenh.Models.ViewModels.MoMo;
using Microsoft.Extensions.Options;

namespace HeThongDatLichVaKhamBenh.Services;

public class MoMoService : IMoMoService
{
    private readonly MoMoSettings _settings;
    private readonly HttpClient _httpClient;

    public MoMoService(IOptions<MoMoSettings> options, HttpClient httpClient)
    {
        _settings = options.Value;
        _httpClient = httpClient;
    }

    public async Task<MoMoCreatePaymentResponse?> CreatePaymentAsync(string orderId, string orderInfo, long amount)
    {
        string requestId = Guid.NewGuid().ToString();
        string rawHash = $"accessKey={_settings.AccessKey}&amount={amount}&extraData=&ipnUrl={_settings.IpnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={_settings.PartnerCode}&redirectUrl={_settings.ReturnUrl}&requestId={requestId}&requestType=captureWallet";
        string signature = ComputeHmacSha256(rawHash, _settings.SecretKey);

        var request = new MoMoCreatePaymentRequest
        {
            partnerCode = _settings.PartnerCode,
            partnerName = "Test MoMo",
            storeId = "MomoTestStore",
            requestId = requestId,
            amount = amount,
            orderId = orderId,
            orderInfo = orderInfo,
            redirectUrl = _settings.ReturnUrl,
            ipnUrl = _settings.IpnUrl,
            requestType = "captureWallet",
            extraData = "",
            lang = "vi",
            signature = signature
        };

        var jsonOptions = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        var content = new StringContent(JsonSerializer.Serialize(request, jsonOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_settings.Endpoint}/create", content);
        var responseString = await response.Content.ReadAsStringAsync();
        
        try 
        {
            return JsonSerializer.Deserialize<MoMoCreatePaymentResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch 
        {
            // Fallback object to carry the raw error string
            return new MoMoCreatePaymentResponse { resultCode = -1, message = responseString };
        }
    }

    public async Task<MoMoQueryStatusResponse?> QueryPaymentAsync(string orderId, string requestId)
    {
        string rawHash = $"accessKey={_settings.AccessKey}&orderId={orderId}&partnerCode={_settings.PartnerCode}&requestId={requestId}";
        string signature = ComputeHmacSha256(rawHash, _settings.SecretKey);

        var request = new MoMoQueryStatusRequest
        {
            partnerCode = _settings.PartnerCode,
            requestId = requestId,
            orderId = orderId,
            signature = signature,
            lang = "vi"
        };

        var jsonOptions = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        var content = new StringContent(JsonSerializer.Serialize(request, jsonOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_settings.Endpoint}/query", content);
        var responseString = await response.Content.ReadAsStringAsync();

        try 
        {
            return JsonSerializer.Deserialize<MoMoQueryStatusResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    private string ComputeHmacSha256(string message, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        byte[] hashBytes;

        using (var hmac = new HMACSHA256(keyBytes))
        {
            hashBytes = hmac.ComputeHash(messageBytes);
        }

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
