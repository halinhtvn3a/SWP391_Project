using BusinessObjects;
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
        public PaymentService()
        {
            if (PaymentRepository == null)
            {
                PaymentRepository = new PaymentRepository();
            }
        }
        public Payment AddPayment(Payment Payment) => PaymentRepository.AddPayment(Payment);
        public void DeletePayment(string id) => PaymentRepository.DeletePayment(id);
        public Payment GetPayment(string id) => PaymentRepository.GetPayment(id);
        public List<Payment> GetPayments() => PaymentRepository.GetPayments();
        //public Payment UpdatePayment(string id, Payment Payment) => PaymentRepository.UpdatePayment(id, Payment);
        public List<Payment> SearchByDate(DateTime start, DateTime end) => PaymentRepository.SearchByDate(start, end);
    }
}
