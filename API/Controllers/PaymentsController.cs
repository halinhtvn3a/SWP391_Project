using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Services;
using BusinessObjects.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService paymentService = new PaymentService();
        private readonly TokenForPayment _tokenForPayment;

        public PaymentsController( TokenForPayment tokenForPayment)
        {
          
            _tokenForPayment = tokenForPayment;
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return paymentService.GetPayments();
        }

        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(string id)
        {
            var payment = paymentService.GetPayment(id);

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        // PUT: api/Payments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutPayment(string id, Payment payment)
        //{
        //    if (id != payment.PaymentId)
        //    {
        //        return BadRequest();
        //    }

        //    paymentService.Entry(payment).State = EntityState.Modified;

        //    try
        //    {
        //        await paymentService.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PaymentExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Payments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            paymentService.AddPayment(payment);

            return CreatedAtAction("GetPayment", new { id = payment.PaymentId }, payment);
        }

        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(string id)
        {
            var payment = paymentService.GetPayment(id);
            if (payment == null)
            {
                return NotFound();
            }

            paymentService.DeletePayment(id);

            return NoContent();
        }

        //private bool PaymentExists(string id)
        //{
        //    return paymentService.GetPayments().Any(e => e.PaymentId == id);
        //}

        [HttpGet("SearchByDate")]
        public async Task<ActionResult<IEnumerable<Payment>>> SearchByDate(DateTime start, DateTime end)
        {
            return paymentService.SearchByDate(start, end);
        }


        [HttpGet("GeneratePaymentToken/{bookingId}")]
        public IActionResult GeneratePaymentToken(string bookingId)
        {
            var token = _tokenForPayment.GenerateToken(bookingId);
            return Ok(new { token });
        }

        [HttpPost("ProcessPayment")]
        public async Task<ActionResult> ProcessPayment( string token)
        {
            //if (bookingId == null)
            //{
            //    return BadRequest(new ResponseModel
            //    {
            //        Status = "Error",
            //        Message = "Booking information is required."
            //    });
            //}
            var bookingId = _tokenForPayment.ValidateToken(token);
            var response = await paymentService.ProcessBookingPayment(bookingId);
            return Ok(response);
        }

    }
}
