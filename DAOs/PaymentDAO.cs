using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication2.Data;

namespace DAOs
{
    public class PaymentDAO
    {
        private readonly CourtCallerDbContext dbContext = null;

        public PaymentDAO()
        {
            if (dbContext == null)
            {
                dbContext = new CourtCallerDbContext();
            }
        }

        public List<Payment> GetPayments()
        {
            return dbContext.Payments.ToList();
        }

        public Payment GetPayment(string id)
        {
            return dbContext.Payments.FirstOrDefault(m => m.PaymentId.Equals(id));
        }

        public Payment AddPayment(Payment Payment)
        {
            dbContext.Payments.Add(Payment);
            dbContext.SaveChanges();
            return Payment;
        }

        //public Payment UpdatePayment(string id, Payment Payment)
        //{
        //    Payment oPayment = GetPayment(id);
        //    if (oPayment != null)
        //    {
        //        oPayment.PaymentName = Payment.PaymentName;
        //        oPayment.IsNatural = Payment.IsNatural;
        //        dbContext.Update(oPayment);
        //        dbContext.SaveChanges();
        //    }
        //    return oPayment;
        //}

        public void DeletePayment(string id)
        {
            Payment oPayment = GetPayment(id);
            if (oPayment != null)
            {
                oPayment.PaymentStatus = "Cancel";
                dbContext.Update(oPayment);
                dbContext.SaveChanges();
            }
        }
    }
}
