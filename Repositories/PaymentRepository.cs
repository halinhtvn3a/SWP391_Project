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
        private readonly PaymentDAO _paymentDao = null;
        public PaymentRepository()
        {
            if (_paymentDao == null)
            {
                _paymentDao = new PaymentDAO();
            }
        }
        public Payment AddPayment(Payment Payment) => _paymentDao.AddPayment(Payment);

        public void DeletePayment(string id) => _paymentDao.DeletePayment(id);

        public Payment GetPayment(string id) => _paymentDao.GetPayment(id);

        public List<Payment> GetPayments() => _paymentDao.GetPayments();

        //public Payment UpdatePayment(string id, Payment Payment) => PaymentDAO.UpdatePayment(id, Payment);

        public List<Payment> SearchByDate(DateTime start, DateTime end) => _paymentDao.SearchByDate(start, end);
    }
}
