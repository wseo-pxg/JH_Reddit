using DataService;
using DataService.Contract;
using DataService.Model;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace DataService.Repository;

public class DataStorage : IDataStorage
{
  private ConcurrentQueue<RedditPost> _postsQueue = new();
  private ConcurrentDictionary<string, SubredditInfo> _subredditInfos = new();
  private Timer? _timer = null;

  public DataStorage()
  {
    StartAsync(CancellationToken.None);
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

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
      if (!_postsQueue.IsEmpty)
      {
        HashSet<SubredditInfo> updatedSubreddits = new();
        foreach (RedditPost post in _postsQueue)
        {
          if (!_subredditInfos.TryGetValue(post.Subreddit, out SubredditInfo? subredditInfo))
          {
            subredditInfo = new SubredditInfo(post.Subreddit);
            _subredditInfos[post.Subreddit] = subredditInfo;
          }

          updatedSubreddits.Add(subredditInfo);
          subredditInfo.Store(post);
        }

        if (updatedSubreddits.Any())
        {
          foreach (SubredditInfo updatedSubreddit in updatedSubreddits)
          {
            updatedSubreddit.UpdateStatistics();
          }
        }
      }
    }
    finally
    {
      _timer?.Change(1000, Timeout.Infinite);
    }
  }

  public void Store(RedditPost post)
  {
    if (post != null)
    {
      _postsQueue.Enqueue(post);
    }
  }

  public SubredditInfo? GetSubredditInfo(string subreddit)
  {
    if (string.IsNullOrWhiteSpace(subreddit))
    {
      throw new ArgumentNullException(nameof(subreddit));
    }

    if (_subredditInfos.ContainsKey(subreddit))
    {
      return _subredditInfos[subreddit];
    }

    return null;
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

