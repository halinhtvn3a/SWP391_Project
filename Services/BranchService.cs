using BusinessObjects;
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
        private readonly BranchRepository BranchRepository = null;
        public BranchService()
        {
            if (BranchRepository == null)
            {
                BranchRepository = new BranchRepository();
            }
        }
        public Branch AddBranch(Branch Branch) => BranchRepository.AddBranch(Branch);
        public void DeleteBranch(string id) => BranchRepository.DeleteBranch(id);
        public Branch GetBranch(string id) => BranchRepository.GetBranch(id);
        public async Task<List<Branch>> GetBranches(PageResult pageResult) => await BranchRepository.GetBranches(pageResult);
        public Branch UpdateBranch(string id, Branch Branch) => BranchRepository.UpdateBranch(id, Branch);
    }
}
