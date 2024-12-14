using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Services
{
    public class PaymentService : IPayment
    {
        private readonly StudyCenterDbContext _context;

        public PaymentService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<DtoPaymentResponse> AddPaymentAsync(DtoPaymentRequest paymentDto, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }
            var payment = new Payment
            {
                StudentId = student.StudentId,
                CourseId = paymentDto.CourseId,
                Date = DateTime.Now,
                Amount = paymentDto.Amount,
                Method = paymentDto.Method,
                Status = paymentDto.Status,
                ReferenceNumber = paymentDto.ReferenceNumber,
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return new DtoPaymentResponse
            {
                PaymentId = payment.PaymentId,
                StudentId = payment.StudentId,
                CourseId = payment.CourseId,
                Date = payment.Date,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                ReferenceNumber = payment.ReferenceNumber
            };
        }

        public async Task<IEnumerable<DtoPaymentResponse>> GetAllPaymentsAsync()
        {
            var payments = await _context.Payments.ToListAsync();
            return payments.Select(p => new DtoPaymentResponse
            {
                PaymentId = p.PaymentId,
                StudentId = p.StudentId,
                CourseId = p.CourseId,
                Date = p.Date,
                Amount = p.Amount,
                Method = p.Method,
                Status = p.Status,
                ReferenceNumber = p.ReferenceNumber
            });
        }

        public async Task<DtoPaymentResponse> GetPaymentByIdAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                return new DtoPaymentResponse
                {
                    PaymentId = payment.PaymentId,
                    StudentId = payment.StudentId,
                    CourseId = payment.CourseId,
                    Date = payment.Date,
                    Amount = payment.Amount,
                    Method = payment.Method,
                    Status = payment.Status,
                    ReferenceNumber = payment.ReferenceNumber
                };
            }
            return null;
        }

      /*  public async Task<DtoPaymentResponse> UpdatePaymentAsync(int id, DtoPaymentRequest paymentDto)
        {
            var existingPayment = await _context.Payments.FindAsync(id);
            if (existingPayment != null)
            {
                existingPayment.StudentId = paymentDto.StudentId;
                existingPayment.CourseId = paymentDto.CourseId;
                existingPayment.Date = paymentDto.Date;
                existingPayment.Amount = paymentDto.Amount;
                existingPayment.Method = paymentDto.Method;
                existingPayment.Status = paymentDto.Status;
                existingPayment.ReferenceNumber = paymentDto.ReferenceNumber;

                await _context.SaveChangesAsync();

                return new DtoPaymentResponse
                {
                    PaymentId = existingPayment.PaymentId,
                    StudentId = existingPayment.StudentId,
                    CourseId = existingPayment.CourseId,
                    Date = existingPayment.Date,
                    Amount = existingPayment.Amount,
                    Method = existingPayment.Method,
                    Status = existingPayment.Status,
                    ReferenceNumber = existingPayment.ReferenceNumber
                };
            }
            return null;
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }*/
    }
}

