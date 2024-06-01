using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PaymentRepository
    {
        private readonly PaymentDAO PaymentDAO = null;
        public PaymentRepository()
        {
            if (PaymentDAO == null)
            {
                PaymentDAO = new PaymentDAO();
            }
        }
        public Payment AddPayment(Payment Payment) => PaymentDAO.AddPayment(Payment);

        public void DeletePayment(string id) => PaymentDAO.DeletePayment(id);

        public Payment GetPayment(string id) => PaymentDAO.GetPayment(id);

        public List<Payment> GetPayments() => PaymentDAO.GetPayments();

        //public Payment UpdatePayment(string id, Payment Payment) => PaymentDAO.UpdatePayment(id, Payment);

        public List<Payment> SearchByDate(DateTime start, DateTime end) => PaymentDAO.SearchByDate(start, end);
    }
}
