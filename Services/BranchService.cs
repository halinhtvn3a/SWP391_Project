using BusinessObjects;
using DAOs.Models;
using DAOs.Helper;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BranchService
    {
        private readonly BranchRepository _branchRepository = null;
        public BranchService()
        {
            if (_branchRepository == null)
            {
                _branchRepository = new BranchRepository();
            }
        }
        public Branch AddBranch(BranchModel branchModel) => _branchRepository.AddBranch(branchModel);
        public void DeleteBranch(string id) => _branchRepository.DeleteBranch(id);
        public Branch GetBranch(string id) => _branchRepository.GetBranch(id);
        public async Task<(List<Branch>, int total)> GetBranches(PageResult pageResult, string searchQuery = null) => await _branchRepository.GetBranches(pageResult, searchQuery);
        public Branch UpdateBranch(string id, BranchModel branchModel) => _branchRepository.UpdateBranch(id, branchModel);

        public List<Branch> GetBranchesByStatus(string status) => _branchRepository.GetBranchesByStatus(status);

        public List<Branch> GetBranchByPrice(decimal minPrice, decimal maxPrice) =>
            _branchRepository.GetBranchByPrice(minPrice, maxPrice);

        public List<Branch> GetBranchesByCourtId(string courtId) => _branchRepository.GetBranchesByCourtId(courtId);

        public Branch GetLastBranch(string userId) => _branchRepository.GetLastBranch(userId);

        public async Task<List<Branch>> SortBranch(string? sortBy, bool isAsc, PageResult pageResult) => await _branchRepository.SortBranch(sortBy, isAsc, pageResult);
    }
}
