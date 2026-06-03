namespace HeThongDatLichVaKhamBenh.Models.ViewModels.MoMo;

public class MoMoCreatePaymentRequest
{
    public string partnerCode { get; set; } = string.Empty;
    public string partnerName { get; set; } = string.Empty;
    public string storeId { get; set; } = string.Empty;
    public string requestId { get; set; } = string.Empty;
    public long amount { get; set; }
    public string orderId { get; set; } = string.Empty;
    public string orderInfo { get; set; } = string.Empty;
    public string redirectUrl { get; set; } = string.Empty;
    public string ipnUrl { get; set; } = string.Empty;
    public string requestType { get; set; } = string.Empty;
    public string extraData { get; set; } = string.Empty;
    public string lang { get; set; } = "vi";
    public string signature { get; set; } = string.Empty;
}

public class MoMoQueryStatusRequest
{
    public string partnerCode { get; set; } = string.Empty;
    public string requestId { get; set; } = string.Empty;
    public string orderId { get; set; } = string.Empty;
    public string signature { get; set; } = string.Empty;
    public string lang { get; set; } = "vi";
}
