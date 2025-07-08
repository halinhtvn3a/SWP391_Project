using BusinessObjects;
using DAOs.Helper;
using DAOs.Models;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;
using Newtonsoft.Json;

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
        public async Task<IActionResult> GetNews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await newsService.GetNewsResponse(pageResult, searchQuery);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("NewsPage")]
        public async Task<IActionResult> GetNewsPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] bool IsHomepageSlideshow = true, [FromQuery] string status = "Active", [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await newsService.GetNewsResponse(pageResult, IsHomepageSlideshow, status, searchQuery);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("SlideShowImage")]
        public IActionResult ShowSlideShowImage()
        {
            var response = newsService.ShowSlideShowImageResponse();
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetNew(string id)
        {
            var response = newsService.GetNewResponse(id);
            if (response.Status == "Success")
                return Ok(response);
            return NotFound(response);
        }

        [HttpPut("EditNews")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutNew(string id, NewsModel news)
        {
            var response = await newsService.UpdateNewResponseAsync(id, news);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostNew(NewsModel newsModel)
        {
            var response = await newsService.AddNewResponseAsync(newsModel);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteNew(string id)
        {
            var response = newsService.DeleteNewResponse(id);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }
    }
}
