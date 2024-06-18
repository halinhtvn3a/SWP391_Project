using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Models;
using BusinessObjects;
using DAOs.Helper;
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
        public List<decimal> ShowPrice(string branchId)
        {
            var weekdayPricing = _dbContext.Prices.FirstOrDefault(p => p.BranchId == branchId && p.IsWeekend == false);
            var weekendPricing = _dbContext.Prices.FirstOrDefault(p => p.BranchId == branchId && p.IsWeekend);
            
            decimal weekdayPrice = weekdayPricing?.SlotPrice ?? 0;
            decimal weekendPrice = weekendPricing?.SlotPrice ?? 0;
            var hehe = new List<decimal>();
            hehe.Add(weekdayPrice);
            hehe.Add(weekendPrice);

            return hehe;
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

        public async Task<List<Price>> SortPrice(string? sortBy, bool isAsc, PageResult pageResult)
        {
            IQueryable<Price> query = _dbContext.Prices;

            switch (sortBy?.ToLower())
            {
                case "priceid":
                    query = isAsc ? query.OrderBy(m => m.PriceId) : query.OrderByDescending(m => m.PriceId);
                    break;
                case "branchid":
                    query = isAsc ? query.OrderBy(m => m.BranchId) : query.OrderByDescending(m => m.BranchId);
                    break;
                case "slotprice":
                    query = isAsc ? query.OrderBy(m => m.SlotPrice) : query.OrderByDescending(m => m.SlotPrice);
                    break;
                case "isweekend":
                    query = isAsc ? query.OrderBy(m => m.IsWeekend) : query.OrderByDescending(m => m.IsWeekend);
                    break;
                default:
                    break;
            }

            Pagination pagination = new Pagination(_dbContext);
            List<Price> prices = await pagination.GetListAsync<Price>(query, pageResult);
            return prices;
        }
    }
}
