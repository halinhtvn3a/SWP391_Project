using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Models;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DAOs
{
    public class PriceDAO
    {
        private readonly CourtCallerDbContext _dbContext = null;

        public PriceDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new CourtCallerDbContext();
            }
        }

        public Price GetPrice(string id)
        {
            return _dbContext.Prices.FirstOrDefault(m => m.PriceId.Equals(id));
        }
        public Price AddPrice(Price price)
        {
            _dbContext.Prices.Add(price);
            _dbContext.SaveChanges();
            return price;
        }
        public decimal ShowPrice(string branchId, DateOnly slotDate)
        {
            bool isWeekend = slotDate.DayOfWeek == DayOfWeek.Saturday || slotDate.DayOfWeek == DayOfWeek.Sunday;


            var pricing = _dbContext.Prices
                .FirstOrDefault(p => p.BranchId == branchId && p.IsWeekend == isWeekend);

            return pricing?.SlotPrice ?? 0;
        }

        public Price UpdatePrice(string id, Price price)
        {
            Price oPrice = GetPrice(id);
            if (oPrice != null)
            {
                oPrice.IsWeekend = price.IsWeekend;
                oPrice.SlotPrice = price.SlotPrice;
                _dbContext.Update(oPrice);
                _dbContext.SaveChanges();
            }
            return oPrice;
        }

        public void DeletePrice(string id)
        {
            Price oPrice = GetPrice(id);
            if (oPrice != null)
            {
                _dbContext.Remove(oPrice);
                _dbContext.SaveChanges();
            }
        }

        public List<Price> GetPriceByBranch(string branchId)
        {
            return _dbContext.Prices.Where(m => m.BranchId.Equals(branchId)).ToList();
        }

        public Price GetPriceByBranchAndWeekend(string branchId, bool isWeekend)
        {
            return _dbContext.Prices.FirstOrDefault(m => m.BranchId.Equals(branchId) && m.IsWeekend == isWeekend);
        }
    }
}
