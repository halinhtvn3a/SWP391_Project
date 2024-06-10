using BusinessObjects.Models;
using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PriceService
    {
        private readonly PriceRepository _priceRepository = null;
        public PriceService()
        {
            if (_priceRepository == null)
            {
                _priceRepository = new PriceRepository();
            }
        }
        public Price AddPrice(Price Price) => _priceRepository.AddPrice(Price);
        public void DeletePrice(string id) => _priceRepository.DeletePrice(id);
        public Price GetPrice(string id) => _priceRepository.GetPrice(id);
        public Price UpdatePrice(string id, Price Price) => _priceRepository.UpdatePrice(id, Price);
        public List<Price> GetPriceByBranch(string branchId) => _priceRepository.GetPriceByBranch(branchId);

    }
}
