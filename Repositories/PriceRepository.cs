using BusinessObjects.Models;
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
        public Price AddPrice(Price Price) => _priceDao.AddPrice(Price);

        public void DeletePrice(string id) => _priceDao.DeletePrice(id);

        public Price GetPrice(string id) => _priceDao.GetPrice(id);

        public Price UpdatePrice(string id, Price Price) => _priceDao.UpdatePrice(id, Price);

        public List<Price> GetPriceByBranch(string branchId) => _priceDao.GetPriceByBranch(branchId);
    }
}
