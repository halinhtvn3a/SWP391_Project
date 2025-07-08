using BusinessObjects;
using DAOs;
using DAOs.Helper;
using DAOs.Models;
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
    public class NewsService
    {
        private readonly NewsRepository newsRepository = null;

        public NewsService()
        {
            if (newsRepository == null)
            {
                newsRepository = new NewsRepository();
            }
        }

        public async Task<(List<News>, int total)> GetNews(PageResult pageResult, string searchQuery = null) => await newsRepository.GetNews(pageResult, searchQuery);

        public async Task<(List<News>, int total)> GetNews(PageResult pageResult, bool IsHomepageSlideshow, string status, string searchQuery = null) => await newsRepository.GetNews(pageResult, IsHomepageSlideshow, status, searchQuery);

        public News GetNew(string id) => newsRepository.GetNew(id);
        public News AddNew(NewsModel news) => newsRepository.AddNew(news);
        public News UpdateNew(string id, NewsModel news) => newsRepository.UpdateNew(id, news);
        public void DeleteNew(string id) => newsRepository.DeleteNew(id);

        public List<News> ShowSlideShowImage() => newsRepository.ShowSlideShowImage();

        public async Task<ResponseModel> GetNewsResponse(PageResult pageResult, string searchQuery = null)
        {
            try
            {
                var (news, total) = await GetNews(pageResult, searchQuery);
                var response = new PagingResponse<News>
                {
                    Data = news,
                    Total = total
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> GetNewsResponse(PageResult pageResult, bool isHomepageSlideshow, string status, string searchQuery = null)
        {
            try
            {
                var (news, total) = await GetNews(pageResult, isHomepageSlideshow, status, searchQuery);
                var response = new PagingResponse<News>
                {
                    Data = news,
                    Total = total
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel ShowSlideShowImageResponse()
        {
            try
            {
                var images = ShowSlideShowImage();
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(images) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel GetNewResponse(string id)
        {
            try
            {
                var _news = GetNew(id);
                if (_news == null)
                    return new ResponseModel { Status = "Error", Message = "News not found." };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(_news) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> AddNewResponseAsync(NewsModel newsModel)
        {
            try
            {
                if (newsModel == null)
                    return new ResponseModel { Status = "Error", Message = "News information is required." };

                // Handle file upload
                if (newsModel.NewsImage != null && newsModel.NewsImage.Length > 0)
                {
                    var file = newsModel.NewsImage;
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    await using var stream = file.OpenReadStream();
                    var task = new FirebaseStorage("court-callers.appspot.com")
                            .Child("NewsImages")
                            .Child(fileName)
                            .PutAsync(stream);
                    var downloadUrl = await task;
                    newsModel.Image = downloadUrl;
                }

                var newNews = AddNew(newsModel);
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(newNews) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> UpdateNewResponseAsync(string id, NewsModel newsModel)
        {
            try
            {
                if (newsModel == null)
                    return new ResponseModel { Status = "Error", Message = "News information is required." };

                var existing = GetNew(id);
                if (existing == null)
                    return new ResponseModel { Status = "Error", Message = "News not found." };

                // Handle file upload if new image provided
                if (newsModel.NewsImage != null && newsModel.NewsImage.Length > 0)
                {
                    var file = newsModel.NewsImage;
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    await using var stream = file.OpenReadStream();
                    var task = new FirebaseStorage("court-callers.appspot.com")
                            .Child("NewsImages")
                            .Child(fileName)
                            .PutAsync(stream);
                    var downloadUrl = await task;
                    newsModel.Image = downloadUrl;
                }

                var updated = UpdateNew(id, newsModel);
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(updated) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public ResponseModel DeleteNewResponse(string id)
        {
            try
            {
                var existing = GetNew(id);
                if (existing == null)
                    return new ResponseModel { Status = "Error", Message = "News not found." };

                DeleteNew(id);
                return new ResponseModel { Status = "Success", Message = "News deleted successfully." };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }
    }
}
