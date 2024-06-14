using DAOs.Models;
using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PriceRepository
    {
        private readonly PriceDAO _priceDao = null;
        public PriceRepository()
        {
            if (_priceDao == null)
            {
                _priceDao = new PriceDAO();
            }
        }
        public decimal ShowPrice(string branchId , DateOnly slotDate) => _priceDao.ShowPrice(branchId, slotDate);
        public Price AddPrice(Price price) => _priceDao.AddPrice(price);
        public void DeletePrice(string id) => _priceDao.DeletePrice(id);

        public Price GetPrice(string id) => _priceDao.GetPrice(id);

        public Price UpdatePrice(string id, Price price) => _priceDao.UpdatePrice(id, price);

        public List<Price> GetPriceByBranch(string branchId) => _priceDao.GetPriceByBranch(branchId);
    }
}
