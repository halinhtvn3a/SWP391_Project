using BusinessObjects;
using DAOs.Models;
using DAOs.Helper;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Services
{
    public class CourtService
    {
        private readonly CourtRepository _courtRepository = null;
        public CourtService()
        {
            if (_courtRepository == null)
            {
                _courtRepository = new CourtRepository();
            }
        }
        public Court AddCourt(CourtModel courtModel) => _courtRepository.AddCourt(courtModel);
        public void DeleteCourt(string id) => _courtRepository.DeleteCourt(id);
        public Court GetCourt(string id) => _courtRepository.GetCourt(id);
        public async Task<(List<Court>, int total)> GetCourts(PageResult pageResult, string searchQuery = null) => await _courtRepository.GetCourts(pageResult, searchQuery);
        public Court UpdateCourt(string id, CourtModel courtModel) => _courtRepository.UpdateCourt(id, courtModel);

        public int GetNumberOfCourtsByBranchId(string branchId) => _courtRepository.GetNumberOfCourtsByBranchId(branchId);
        public List<Court> GetCourtsByStatus(string status) => _courtRepository.GetCourtsByStatus(status);

        public async Task<List<Court>> SortCourt(string? sortBy, bool isAsc, PageResult pageResult) => await _courtRepository.SortCourt(sortBy, isAsc, pageResult);

        public void MaintainCourt(string courtId)
        {
            _courtRepository.MaintainCourt(courtId);

        }

        public List<Court> AvailableCourts(SlotModel slotModel) => _courtRepository.AvailableCourts(slotModel);

        public async Task<(List<Court>, int total)> GetCourtsByBranchId(string branchId, PageResult pageResult, string searchQuery = null)
=> await _courtRepository.GetCourtsByBranchId(branchId, pageResult, searchQuery);

        public async Task<ResponseModel> GetCourtsResponse(PageResult pageResult, string searchQuery = null)
        {
            try
            {
                var (courts, total) = await GetCourts(pageResult, searchQuery);
                var response = new PagingResponse<Court>
                {
                    Data = courts,
                    Total = total
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel GetCourtResponse(string id)
        {
            try
            {
                var court = GetCourt(id);
                if (court == null)
                    return new ResponseModel { Status = "Error", Message = "Court not found." };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(court) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel AddCourtResponse(CourtModel courtModel)
        {
            try
            {
                var court = AddCourt(courtModel);
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(court) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel UpdateCourtResponse(string id, CourtModel courtModel)
        {
            try
            {
                var court = GetCourt(id);
                if (court == null || id != court.CourtId)
                    return new ResponseModel { Status = "Error", Message = "Invalid court ID." };
                var updated = UpdateCourt(id, courtModel);
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(updated) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel DeleteCourtResponse(string id)
        {
            try
            {
                var court = GetCourt(id);
                if (court == null)
                    return new ResponseModel { Status = "Error", Message = "Court not found." };
                DeleteCourt(id);
                return new ResponseModel { Status = "Success", Message = "Court deleted successfully." };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel GetCourtsByStatusResponse(string status)
        {
            try
            {
                var courts = GetCourtsByStatus(status);
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(courts) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> SortCourtResponse(string sortBy, bool isAsc, PageResult pageResult)
        {
            try
            {
                var courts = await SortCourt(sortBy, isAsc, pageResult);
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(courts) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel GetNumberOfCourtsByBranchIdResponse(string branchId)
        {
            try
            {
                if (string.IsNullOrEmpty(branchId))
                    return new ResponseModel { Status = "Error", Message = "Branch ID is required." };
                var count = GetNumberOfCourtsByBranchId(branchId);
                return new ResponseModel { Status = "Success", Message = count.ToString() };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel AvailableCourtsResponse(SlotModel slotModel)
        {
            try
            {
                var courts = AvailableCourts(slotModel);
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(courts) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> GetCourtsByBranchIdResponse(string branchId, PageResult pageResult, string searchQuery = null)
        {
            try
            {
                var (courts, total) = await GetCourtsByBranchId(branchId, pageResult, searchQuery);
                var response = new PagingResponse<Court>
                {
                    Data = courts,
                    Total = total
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }
    }

}
