using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentService
    {
        private readonly PaymentRepository PaymentRepository = null;
        private readonly BookingRepository bookingRepository = null;
        private readonly VnpayService vnpayService = null;
        private readonly ILogger<VnpayService> _logger;
        public PaymentService()
        {
            if (PaymentRepository == null)
            {
                PaymentRepository = new PaymentRepository();
            }
            if (bookingRepository == null)
                bookingRepository = new BookingRepository();
            if (vnpayService == null)
                vnpayService = new VnpayService(_logger);

        }
        public Payment AddPayment(Payment Payment) => PaymentRepository.AddPayment(Payment);
        public void DeletePayment(string id) => PaymentRepository.DeletePayment(id);
        public Payment GetPayment(string id) => PaymentRepository.GetPayment(id);
        public List<Payment> GetPayments() => PaymentRepository.GetPayments();
        //public Payment UpdatePayment(string id, Payment Payment) => PaymentRepository.UpdatePayment(id, Payment);

        public List<Payment> SearchByDate(DateTime start, DateTime end) => PaymentRepository.SearchByDate(start, end);

        public async Task<string> ProcessBookingPayment(string bookingId)
        {
            var bookings = await bookingRepository.GetBooking(bookingId);
            if (bookings == null)
            {
                return null;
            }

            var paymentURL = vnpayService.CreatePaymentUrl(bookings.TotalPrice,"ok",bookings.BookingId);


            return paymentURL;
        }

    }
}
