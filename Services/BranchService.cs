using BusinessObjects;
using DAOs.Models;
using DAOs.Helper;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Storage;
using Newtonsoft.Json;
using System.Text.Json;
using System.IO;

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

        public async Task<(List<Branch>, int total)> GetBranches(PageResult pageResult, string status = "Active", string searchQuery = null) => await _branchRepository.GetBranches(pageResult, status, searchQuery);
        public Branch AddBranch(BranchModel branchModel) => _branchRepository.AddBranch(branchModel);
        public void DeleteBranch(string id) => _branchRepository.DeleteBranch(id);
        public Branch GetBranch(string id) => _branchRepository.GetBranch(id);
        public async Task<(List<Branch>, int total)> GetBranches(PageResult pageResult, string searchQuery = null) => await _branchRepository.GetBranches(pageResult, searchQuery);
        public Branch UpdateBranch(string id, PutBranch branchModel) => _branchRepository.UpdateBranch(id, branchModel);

        public List<Branch> GetBranchesByStatus(string status) => _branchRepository.GetBranchesByStatus(status);

        public List<Branch> GetBranchByPrice(decimal minPrice, decimal maxPrice) =>
            _branchRepository.GetBranchByPrice(minPrice, maxPrice);

        public List<Branch> GetBranchesByCourtId(string courtId) => _branchRepository.GetBranchesByCourtId(courtId);

        public Branch GetLastBranch(string userId) => _branchRepository.GetLastBranch(userId);

        public async Task<List<Branch>> SortBranch(string? sortBy, bool isAsc, PageResult pageResult) => await _branchRepository.SortBranch(sortBy, isAsc, pageResult);

        public async Task<(List<BranchDistance>, int total)> SortBranchByDistance(LocationModel user, PageResult pageResult) => await _branchRepository.SortBranchByDistance(user, pageResult);

        public async Task<(List<Branch>, int total)> GetBranchByPrice(decimal minPrice, decimal maxPrice, PageResult pageResult) => await _branchRepository.GetBranchByPrice(minPrice, maxPrice, pageResult);

        public async Task<(List<Branch>, int total)> GetBranchByRating(int rating, PageResult pageResult) => await _branchRepository.GetBranchByRating(rating, pageResult);

        public async Task<ResponseModel> AddBranchResponseAsync(BranchModel branchModel)
        {
            try
            {
                // Validate input
                if (branchModel == null)
                {
                    return new ResponseModel { Status = "Error", Message = "Branch information is required." };
                }

                // Handle image upload if provided
                if (branchModel.BranchPictures != null && branchModel.BranchPictures.Any())
                {
                    var imageUrls = new List<string>();
                    foreach (var file in branchModel.BranchPictures)
                    {
                        if (file.Length == 0) continue;
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        await using var stream = file.OpenReadStream();
                        var task = new FirebaseStorage("court-callers.appspot.com")
                                    .Child("BranchImage")
                                    .Child(fileName)
                                    .PutAsync(stream);
                        var downloadUrl = await task;
                        imageUrls.Add(downloadUrl);
                    }
                    branchModel.BranchPicture = System.Text.Json.JsonSerializer.Serialize(imageUrls);
                }

                var branch = AddBranch(branchModel);
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(branch) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> GetBranchesResponse(PageResult pageResult, string searchQuery = null)
        {
            try
            {
                var (branches, total) = await GetBranches(pageResult, searchQuery);
                var response = new PagingResponse<Branch>
                {
                    Data = branches,
                    Total = total
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> GetBranchesResponse(PageResult pageResult, string status, string searchQuery = null)
        {
            try
            {
                var (branches, total) = await GetBranches(pageResult, status, searchQuery);
                var response = new PagingResponse<Branch>
                {
                    Data = branches,
                    Total = total
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel GetBranchResponse(string id)
        {
            try
            {
                var branch = GetBranch(id);
                if (branch == null)
                    return new ResponseModel { Status = "Error", Message = "Branch not found." };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(branch) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel DeleteBranchResponse(string id)
        {
            try
            {
                var branch = GetBranch(id);
                if (branch == null)
                    return new ResponseModel { Status = "Error", Message = "Branch not found." };
                DeleteBranch(id);
                return new ResponseModel { Status = "Success", Message = "Branch deleted successfully." };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel UpdateBranchResponse(string id, PutBranch branchModel)
        {
            try
            {
                var branch = GetBranch(id);
                if (branch == null)
                    return new ResponseModel { Status = "Error", Message = "Branch not found." };

                var updated = UpdateBranch(id, branchModel);
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(updated) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }
    }
}
