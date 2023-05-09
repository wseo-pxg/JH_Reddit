using Microsoft.AspNetCore.Mvc;
using RedditHelper;
using Newtonsoft.Json.Linq;
using RedditHelper.Workers;
using DataService.Model;
using DataService;

namespace RedditWebApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class MonitorController : ControllerBase
  {
    private static readonly HashSet<string> subreddits = new() { "gaming", "worldnews", "movies" };
  
    private readonly ILogger<MonitorController> _logger;
    private readonly IRedditWorkerPool _workerPool;
    private readonly IDataApi _dataApi;

    public MonitorController(ILogger<MonitorController> logger, IRedditWorkerPool workerPool, IDataApi dataApi) => (_logger, _workerPool, _dataApi) = (logger, workerPool, dataApi);

    [HttpGet(Name = "GetAvailableSubreddits")]
    public async Task<IEnumerable<string>> Get()
    {
      return subreddits;
    }

    [HttpGet("GetSubredditInfo/{subreddit}")]
    public async Task<IActionResult> GetSubredditData(string? subreddit)
    {
      if (string.IsNullOrWhiteSpace(subreddit))
      {
        return BadRequest("subreddit name is null.");
      }

      if (!subreddits.Contains(subreddit))
      {
        return BadRequest("Unsupported subreddit name.");
      }

      SubredditInfo? subredditInfo = _dataApi.Post.GetInfo(subreddit);
      if (subredditInfo == null) return NotFound();

      return Ok(subredditInfo);
    }

    [HttpPost("{subreddit}")]
    public async Task<IActionResult> Create(string subreddit)
    {
      if (string.IsNullOrWhiteSpace(subreddit))
      {
        return BadRequest("subreddit name is null.");
      }

      if (!subreddits.Contains(subreddit))
      {
        return BadRequest("Unsupported subreddit name.");
      }

      if (_workerPool.GetWorker(subreddit) != null)
      {
        return Ok();
      }

      IRedditWorker redditWorker = _workerPool.AddWorker(subreddit);
      if (redditWorker != null)
      {
        return Created("A new Reddit worker is created successfully.", null);
      }

      return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Failed to create subreddit monitor for {subreddit}" });
    }
  }
}