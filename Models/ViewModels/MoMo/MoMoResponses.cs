namespace HeThongDatLichVaKhamBenh.Models.ViewModels.MoMo;

public class MoMoCreatePaymentResponse
{
    public string partnerCode { get; set; } = string.Empty;
    public string orderId { get; set; } = string.Empty;
    public string requestId { get; set; } = string.Empty;
    public long amount { get; set; }
    public long responseTime { get; set; }
    public string message { get; set; } = string.Empty;
    public int resultCode { get; set; }
    public string payUrl { get; set; } = string.Empty;
    public string qrCodeUrl { get; set; } = string.Empty;
    public string deeplink { get; set; } = string.Empty;
    public string signature { get; set; } = string.Empty;
}

public class MoMoQueryStatusResponse
{
    public string partnerCode { get; set; } = string.Empty;
    public string orderId { get; set; } = string.Empty;
    public string requestId { get; set; } = string.Empty;
    public string extraData { get; set; } = string.Empty;
    public long amount { get; set; }
    public long transId { get; set; }
    public string payType { get; set; } = string.Empty;
    public int resultCode { get; set; }
    public string message { get; set; } = string.Empty;
    public long responseTime { get; set; }
    public string signature { get; set; } = string.Empty;
}
