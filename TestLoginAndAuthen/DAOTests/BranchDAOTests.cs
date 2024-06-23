using BusinessObjects;
using DAOs;
using DAOs.Helper;
using DAOs.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.DAOsTests
{
    public class BranchDAOTests
    {
        private readonly Mock<DbSet<Branch>> mockSet;
        private readonly Mock<CourtCallerDbContext> mockContext;
        private readonly List<Branch> branchList;

        public BranchDAOTests()
        {
            // Initialize mock set and context
            mockSet = new Mock<DbSet<Branch>>();
            mockContext = new Mock<CourtCallerDbContext>();
            branchList = new List<Branch>
            {
                new Branch { BranchId = "B00001", BranchName = "Test Branch 1", BranchAddress = "Test Address 1", Status = "Active" },
                new Branch { BranchId = "B00002", BranchName = "Test Branch 2", BranchAddress = "Test Address 2", Status = "Active"  },
                new Branch { BranchId = "B00003", BranchName = "Test Branch 3", BranchAddress = "Test Address 3", Status = "Inactive"  }
            };

            var data = branchList.AsQueryable();

            mockSet.As<IQueryable<Branch>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Branch>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Branch>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Branch>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(c => c.Branches).Returns(mockSet.Object);
        }

        [Fact]
        public void GetBranches_ReturnsAllBranches()
        {
            var dao = new BranchDAO(mockContext.Object);

            var result = dao.GetBranches();

            Assert.Equal(3, result.Count);
            Assert.Equal(branchList, result);
        }

        [Theory]
        [InlineData("B00001")]
        [InlineData("B00002")]
        public void GetBranch_ReturnsBranch(string branchId)
        {
            var dao = new BranchDAO(mockContext.Object);

            var result = dao.GetBranch(branchId);

            Assert.NotNull(result);
            Assert.Equal(branchId, result.BranchId);
        }

        [Fact]
        public void GetBranch_ReturnsNull()
        {
            var dao = new BranchDAO(mockContext.Object);

            var result = dao.GetBranch("B00000");

            Assert.Null(result);
        }

        [Fact]
        public void AddBranch_ReturnsBranch()
        {
            var dao = new BranchDAO(mockContext.Object);
            var branchModel = new BranchModel
            {
                BranchName = "Test Branch 4",
                BranchAddress = "Test Address 4",
            };

            var result = dao.AddBranch(branchModel);

            Assert.NotNull(result);
            Assert.Equal("B00004", result.BranchId);
            Assert.Equal(branchModel.BranchName, result.BranchName);
        }

        //use DDT to test addbranch
        [Theory]
        [InlineData("Test Branch 4", "Test Address 4")]
        [InlineData("Test Branch 5", "Test Address 5")]
        public void AddBranchDDT_ReturnsBranch(string branchName, string branchAddress)
        {
            var dao = new BranchDAO(mockContext.Object);
            var branchModel = new BranchModel
            {
                BranchName = branchName,
                BranchAddress = branchAddress,
            };

            var result = dao.AddBranch(branchModel);

            Assert.NotNull(result);
            Assert.Equal("B00004", result.BranchId);
            Assert.Equal(branchModel.BranchName, result.BranchName);
            Assert.Equal(branchModel.BranchAddress, result.BranchAddress);
        }


        [Fact]
        public void UpdateBranch_ReturnsBranch()
        {
            var dao = new BranchDAO(mockContext.Object);
            var branchModel = new BranchModel
            {
                BranchName = "Test Branch 4",
            };

            var result = dao.UpdateBranch("B00001", branchModel);

            Assert.NotNull(result);
            Assert.Equal("B00001", result.BranchId);
            Assert.Equal(branchModel.BranchName, result.BranchName);
            Assert.Equal(branchModel.BranchAddress, result.BranchAddress);
        }

        [Fact]
        public void DeleteBranch_ReturnsBranch()
        {
            var dao = new BranchDAO(mockContext.Object);

            dao.DeleteBranch("B00001");

            var result = dao.GetBranch("B00001");
            Assert.Equal("Inactive", result.Status);
        }

        [Fact]
        public void DeleteBranch_ReturnsNull()
        {
            var dao = new BranchDAO(mockContext.Object);

            dao.DeleteBranch("B00000");

            var result = dao.GetBranch("B00000");
            Assert.Null(result);
        }

        [Fact]
        public void DeleteBranch_ReturnsBranchNotFound()
        {
            var dao = new BranchDAO(mockContext.Object);

            dao.DeleteBranch("B00004");

            var result = dao.GetBranch("B00004");
            Assert.Null(result);
        }

        [Theory]
        [InlineData("Active", 2)]
        [InlineData("Inactive", 1)]
        public void GetBranchesByStatusActive_ReturnsAllBranches(string status, int expectedNum)
        {
            var dao = new BranchDAO(mockContext.Object);

            var result = dao.GetBranchesByStatus(status);

            Assert.Equal(expectedNum, result.Count);
        }
    }
}
