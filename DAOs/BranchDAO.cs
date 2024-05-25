using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class BranchDAO
    {
        private readonly CourtCallerDbContext dbContext = null;

        public BranchDAO()
        {
            if (dbContext == null)
            {
                dbContext = new CourtCallerDbContext();
            }
        }

        public List<Branch> GetBranches()
        {
            return dbContext.Branches.ToList();
        }

        public Branch GetBranch(string id)
        {
            return dbContext.Branches.FirstOrDefault(m => m.BranchId.Equals(id));
        }

        public Branch AddBranch(Branch Branch)
        {
            dbContext.Branches.Add(Branch);
            dbContext.SaveChanges();
            return Branch;
        }

        public Branch UpdateBranch(string id, Branch Branch)
        {
            Branch oBranch = GetBranch(id);
            if (oBranch != null)
            {
                oBranch.Address = Branch.Address;
                oBranch.Description = Branch.Description;
                oBranch.Picture = Branch.Picture;
                oBranch.OpenTime = Branch.OpenTime;
                oBranch.CloseTime = Branch.CloseTime;
                oBranch.OpenDay = Branch.OpenDay;
                dbContext.Update(oBranch);
                dbContext.SaveChanges();
            }
            return oBranch;
        }

        public void DeleteBranch(string id)
        {
            Branch oBranch = GetBranch(id);
            if (oBranch != null)
            {
                oBranch.Status = false;
                dbContext.Update(oBranch);
                dbContext.SaveChanges();
            }
        }
    }
}
