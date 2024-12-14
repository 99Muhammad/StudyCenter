using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPayment _paymentService;

        public PaymentController(IPayment paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DtoPaymentResponse>>> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DtoPaymentResponse>> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [Authorize(Roles ="Student")]
        [HttpPost]
        public async Task<ActionResult<DtoPaymentResponse>> AddPayment(DtoPaymentRequest paymentDto)
        {
            if (paymentDto == null)
            {
                return BadRequest("Payment data is required.");
            }

            var createdPaymentDto = await _paymentService.AddPaymentAsync(paymentDto, User);
            return CreatedAtAction(nameof(GetPaymentById), new { id = createdPaymentDto.PaymentId }, createdPaymentDto);
        }

        /*[HttpPut("{id}")]
        public async Task<ActionResult<DtoPaymentResponse>> UpdatePayment(int id, DtoPaymentRequest paymentDto)
        {
            if (paymentDto == null)
            {
                return BadRequest("Payment data is required.");
            }
            var updatedPaymentDto = await _paymentService.UpdatePaymentAsync(id, paymentDto);
            if (updatedPaymentDto == null) return NotFound();
            return Ok(updatedPaymentDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }*/
    }
}
