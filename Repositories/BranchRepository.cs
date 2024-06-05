using BusinessObjects;
using DAOs;
using DAOs.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BranchRepository
    {
        private readonly BranchDAO _branchDao = null;
        public BranchRepository()
        {
            if (_branchDao == null)
            {
                _branchDao = new BranchDAO();
            }
        }
        public Branch AddBranch(Branch Branch) => _branchDao.AddBranch(Branch);

        public void DeleteBranch(string id) => _branchDao.DeleteBranch(id);

        public Branch GetBranch(string id) => _branchDao.GetBranch(id);

        public async Task<List<Branch>> GetBranches(PageResult pageResult) => await _branchDao.GetBranches(pageResult);

        public Branch UpdateBranch(string id, Branch Branch) => _branchDao.UpdateBranch(id, Branch);

        public List<Branch> GetBranchesByStatus(string status) => _branchDao.GetBranchesByStatus(status);
    }
}
