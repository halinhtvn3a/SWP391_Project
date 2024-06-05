using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;

namespace DAOs
{
    public class BranchDAO
    {
        private readonly CourtCallerDbContext DbContext = null;

        public BranchDAO()
        {
            if (DbContext == null)
            {
                DbContext = new CourtCallerDbContext();
            }
        }

        public async Task<List<Branch>> GetBranches(PageResult pageResult)
        {
            var query = DbContext.Branches.AsQueryable();
            Pagination pagination = new Pagination(DbContext);
            List<Branch> Branches = await pagination.GetListAsync<Branch>(query, pageResult);
            return Branches;
        }

        public Branch GetBranch(string id)
        {
            return DbContext.Branches.FirstOrDefault(m => m.BranchId.Equals(id));
        }

        public Branch AddBranch(Branch Branch)
        {
            DbContext.Branches.Add(Branch);
            DbContext.SaveChanges();
            return Branch;
        }

        public Branch UpdateBranch(string id, Branch Branch)
        {
            Branch oBranch = GetBranch(id);
            if (oBranch != null)
            {
                oBranch.BranchAddress = Branch.BranchAddress;
                oBranch.Description = Branch.Description;
                oBranch.BranchName = Branch.BranchName;
                oBranch.BranchPhone = Branch.BranchPhone;
                oBranch.BranchPicture = Branch.BranchPicture;
                oBranch.OpenTime = Branch.OpenTime;
                oBranch.CloseTime = Branch.CloseTime;
                oBranch.OpenDay = Branch.OpenDay;
                DbContext.Update(oBranch);
                DbContext.SaveChanges();
            }
            return oBranch;
        }

        public void DeleteBranch(string id)
        {
            Branch oBranch = GetBranch(id);
            if (oBranch != null)
            {
                oBranch.Status = "Cancel";
                DbContext.Update(oBranch);
                DbContext.SaveChanges();
            }
        }

            public List<Branch> GetBranchesByStatus(string status)
        {
            return DbContext.Branches.Where(m => m.Status == status).ToList();
        }

    }
}
