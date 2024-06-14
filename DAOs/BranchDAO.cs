using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;
using DAOs.Models;

namespace DAOs
{
    public class BranchDAO
    {
        private readonly CourtCallerDbContext _courtCallerDbContext = null;

        public BranchDAO()
        {
            if (_courtCallerDbContext == null)
            {
                _courtCallerDbContext = new CourtCallerDbContext();
            }
        }

        public List<Branch> GetBranches() => _courtCallerDbContext.Branches.ToList();
        

        public async Task<List<Branch>> GetBranches(PageResult pageResult)
        {
            var query = _courtCallerDbContext.Branches.AsQueryable();
            Pagination pagination = new Pagination(_courtCallerDbContext);
            List<Branch> Branches = await pagination.GetListAsync<Branch>(query, pageResult);
            return Branches;
        }

        public Branch GetBranch(string id)
        {
            return _courtCallerDbContext.Branches.FirstOrDefault(m => m.BranchId.Equals(id));
        }

        public Branch AddBranch(BranchModel branchModel)
        {
            Branch branch = new Branch()
            {
                BranchId = "B" + (_courtCallerDbContext.Branches.Count() + 1).ToString("D5"),
                BranchAddress = branchModel.BranchAddress,
                BranchName = branchModel.BranchName,
                BranchPhone = branchModel.BranchPhone,
                Description = branchModel.Description,
                BranchPicture = branchModel.BranchPicture,
                OpenTime = branchModel.OpenTime,
                CloseTime = branchModel.CloseTime,
                OpenDay = branchModel.OpenDay,
                Status = branchModel.Status
            };
            _courtCallerDbContext.Branches.Add(branch);
            _courtCallerDbContext.SaveChanges();
            return branch;
        }

        public Branch UpdateBranch(string id, BranchModel branchModel)
        {
            Branch oBranch = GetBranch(id);
            if (oBranch != null)
            {
                oBranch.BranchAddress = branchModel.BranchAddress;
                oBranch.Description = branchModel.Description;
                oBranch.BranchName = branchModel.BranchName;
                oBranch.BranchPhone = branchModel.BranchPhone;
                oBranch.BranchPicture = branchModel.BranchPicture;
                oBranch.OpenTime = branchModel.OpenTime;
                oBranch.CloseTime = branchModel.CloseTime;
                oBranch.OpenDay = branchModel.OpenDay;
                oBranch.Status = branchModel.Status;
                _courtCallerDbContext.Update(oBranch);
                _courtCallerDbContext.SaveChanges();
            }
            return oBranch;
        }


        public void DeleteBranch(string id)
        {
            Branch oBranch = GetBranch(id);
            if (oBranch != null)
            {
                oBranch.Status = "Inactive";
                _courtCallerDbContext.Update(oBranch);
                _courtCallerDbContext.SaveChanges();
            }
        }

        public List<Branch> GetBranchesByStatus(string status)
        {
            return _courtCallerDbContext.Branches.Where(m => m.Status == status).ToList();
        }

        public List<Branch> GetBranchesByCourtId(string courtId)
        {
            return _courtCallerDbContext.Branches.Where(m => m.Courts.Any(c => c.CourtId == courtId)).ToList();
        }

        public List<Branch> SortBranchByPrice(decimal minPrice, decimal maxPrice)
        {
            return _courtCallerDbContext.Branches.Where(m => m.Prices.Any(c =>
                c.SlotPrice >= minPrice && c.SlotPrice <= maxPrice
            )).ToList();
        }


    }
}
