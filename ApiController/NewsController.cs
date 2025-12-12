using Backend.Dto;
using Backend.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.ApiController {
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase {
        private readonly INewsRepository _repo;
        public NewsController(INewsRepository repo) {
            _repo = repo;
        }
        // GET api/news
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET api/news/3
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) {
            var news = await _repo.GetByIdAsync(id);
            if(news == null) {
                return NotFound();
            }

            return Ok(news);
        }
        // POST api/news/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] News req) {
            if(string.IsNullOrWhiteSpace(req.Title) ||
                string.IsNullOrWhiteSpace(req.Content)) {
                return BadRequest("Title and Content are required.");
            }

            var news = await _repo.CreateAsync(new News {
                Title = req.Title.Trim(),
                Content = req.Content,
                ImgUrl = req.ImgUrl ?? "",
                IsPublished = req.IsPublished,
                Date = req.Date,
            });

            return Ok(news);
        }

        // POST api/news/update
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] News req) {
            if(req.Id <= 0)
                return BadRequest("Invalid id.");

            if(string.IsNullOrWhiteSpace(req.Title) ||
                string.IsNullOrWhiteSpace(req.Content)) {
                return BadRequest("Title and Content are required.");
            }

            var updated = await _repo.UpdateAsync(req);
            if(updated == null)
                return NotFound();

            return Ok(updated);
        }
        [HttpPost("{id}/publish")]
        public async Task<IActionResult> UpdatePublishStatus( int id, [FromBody] PublishNewsReq req
        ) {
            var success = await _repo.UpdatePublishStatusAsync(id, req.IsPublished);
            return Ok();
        }
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> Delete(int id) {
            if(id <= 0)
                return BadRequest("Invalid id.");

            var success = await _repo.DeleteAsync(id);
            return success ? Ok() : NotFound();
        }
    }
}
