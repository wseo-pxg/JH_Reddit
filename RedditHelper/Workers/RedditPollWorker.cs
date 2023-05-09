using DataService;
using DataService.Model;
using System.Collections.Concurrent;
using System.Data;

namespace RedditHelper.Workers;


public class RedditPollWorker : IRedditWorker
{
  private Timer? _timer = null;
  private string? _subreddit;
  private IDataApi _dataApi;

  public RedditPollWorker(string subreddit, IDataApi dataApi)
  {
    if (string.IsNullOrWhiteSpace(subreddit))
    {
      throw new AbandonedMutexException(nameof(subreddit));
    }

    _subreddit = subreddit;
    _dataApi = dataApi;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    //_timer = new Timer(DoWork, null, TimeSpan.Zero, Timeout.Infinite);
    _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));

    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _timer?.Change(Timeout.Infinite, Timeout.Infinite);
    return Task.CompletedTask;
  }

  private void DoWork(object? state)
  {
    try
    {
      _timer?.Change(Timeout.Infinite, Timeout.Infinite);
      List<RedditPost> posts = Subreddit.FetchPosts(_subreddit!, "top").Result;
      posts.AddRange(Subreddit.FetchPosts(_subreddit!, "new").Result);

      if (posts != null && posts.Any())
      {
        _dataApi.Post.Store(_subreddit!, posts);
      }
    }
    finally
    {
      _timer?.Change(3000, Timeout.Infinite);
    }
  }

  #region IDisposable
  private bool disposedValue;
  public void Dispose() => Dispose(disposing: true);

  protected void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      if (disposing)
      {
        _timer?.Dispose();
        _timer = null;
      }

      disposedValue = true;
    }
  }
  #endregion IDisposable
}
