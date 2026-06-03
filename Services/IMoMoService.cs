using HeThongDatLichVaKhamBenh.Models.ViewModels.MoMo;

namespace HeThongDatLichVaKhamBenh.Services;

public interface IMoMoService
{
    Task<MoMoCreatePaymentResponse?> CreatePaymentAsync(string orderId, string orderInfo, long amount);
    Task<MoMoQueryStatusResponse?> QueryPaymentAsync(string orderId, string requestId);
}
