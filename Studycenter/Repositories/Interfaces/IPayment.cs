using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IPayment
    {
        Task<IEnumerable<DtoPaymentResponse>> GetAllPaymentsAsync();
        Task<DtoPaymentResponse> GetPaymentByIdAsync(int id);
        Task<DtoPaymentResponse> AddPaymentAsync(DtoPaymentRequest paymentDto, ClaimsPrincipal userPrincipal);
        //Task<DtoPaymentResponse> UpdatePaymentAsync(int id, DtoPaymentRequest paymentDto);
        //Task<bool> DeletePaymentAsync(int id);
    }
}

