using BusinessObjects;
using DAOs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;

namespace Services
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepository = null;
        private readonly BookingRepository _bookingRepository = null;
        private readonly VnpayService _vnpayService = null;
        private readonly ILogger<VnpayService> _logger;
        public PaymentService()
        {
            if (_paymentRepository == null)
            {
                _paymentRepository = new PaymentRepository();
            }
            if (_bookingRepository == null)
            {
                _bookingRepository = new BookingRepository();
            }
            if (_vnpayService == null)
            {
                _vnpayService = new VnpayService(_logger, _bookingRepository, _paymentRepository);
            }

        }
        public Payment AddPayment(Payment Payment) => _paymentRepository.AddPayment(Payment);
        public void DeletePayment(string id) => _paymentRepository.DeletePayment(id);
        public Payment GetPayment(string id) => _paymentRepository.GetPayment(id);
        public List<Payment> GetPayments() => _paymentRepository.GetPayments();
        //public Payment UpdatePayment(string id, Payment Payment) => PaymentRepository.UpdatePayment(id, Payment);

        public List<Payment> SearchByDate(DateTime start, DateTime end) => _paymentRepository.SearchByDate(start, end);

        public async Task<string> ProcessBookingPayment(string bookingId)
        {
            var bookings = await _bookingRepository.GetBooking(bookingId);
            if (bookings == null)
            {
                return null;
            }

            var paymentURL = _vnpayService.CreatePaymentUrl(bookings.TotalPrice,"ok",bookings.BookingId);


            return paymentURL;
        }
        public async Task<List<Payment>> SortPayment(string? sortBy, bool isAsc, PageResult pageResult) => await _paymentRepository.SortPayment(sortBy, isAsc, pageResult);
    }
}
