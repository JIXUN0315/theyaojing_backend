using System.Text.Json;
using Backend.Dto;
using Backend.Repository;
using Backend.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.ApiController;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogPostRepository _repository;

    public BlogController(IBlogPostRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateBlogPostViewModel req)
    {

        var post = new BlogPostDto(req);
        var id = await _repository.CreateAsync(post);
        return Ok(new { id });
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var post = await _repository.GetAllAsync();
        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var post = await _repository.GetByIdAsync(id);
        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpPut("{id}")]
    [Authorize]

    public async Task<IActionResult> Update(int id, [FromBody] BlogPostDto post)
    {
        post.Id = id;
        await _repository.UpdateAsync(post);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]

    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("featured")]
    public async Task<IActionResult> Featured() {
        var post = await _repository.GetAllAsync();
        post = post.Where(x => x.IsFeatured).OrderBy(x => x.Date).Take(3).ToList();
        if(post == null)
            return NotFound();

        return Ok(post);
    }
    [HttpPost("{id}/publish")]
    public async Task<IActionResult> UpdatePublishStatus(int id, [FromBody] PublishNewsReq req
        ) {
        var success = await _repository.UpdatePublishStatusAsync(id, req.IsPublished);
        return Ok();
    }
}