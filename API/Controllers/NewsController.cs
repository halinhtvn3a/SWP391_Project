using BusinessObjects;
using DAOs.Helper;
using DAOs.Models;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly NewsService newsService;

        public NewsController()
        {
            newsService = new NewsService();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<News>>> GetNews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var news = await newsService.GetNews(pageResult, searchQuery);
            return Ok(news);
        }

        [HttpGet("NewsPage")]
        public async Task<ActionResult<IEnumerable<News>>> GetNews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] bool IsHomepageSlideshow = true, [FromQuery] string status = "Active", [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var news = await newsService.GetNews(pageResult, searchQuery);
            return Ok(news);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<News>> GetNew(string id)
        {
            var news = newsService.GetNew(id);

            if (news == null)
            {
                return NotFound();
            }

            return news;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNew(string id, News news)
        {
            var updatedNews = newsService.UpdateNew(id, news);
            return Ok(updatedNews);
        }

        [HttpPost]
        public async Task<ActionResult<News>> PostNew(NewsModel newsModel)
        {

            var file = newsModel.NewsImage;

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            using (var stream = file.OpenReadStream())
            {
                var task = new FirebaseStorage("court-callers.appspot.com")
                    .Child("NewsImages")
                    .Child(fileName)
                    .PutAsync(stream);

                var downloadUrl = await task;
                newsModel.Image = downloadUrl; // Directly assign the URL
            }

            var newNews = newsService.AddNew(newsModel);
            return CreatedAtAction("GetNew", new { id = newNews.NewId }, newNews);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNew(string id)
        {
            newsService.DeleteNew(id);
            return NoContent();
        }


    }
}
