using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace RedditHelper.Workers;


public class RedditWorkerPool : IRedditWorkerPool
{
  private ConcurrentDictionary<string, IRedditWorker> _postWorkers = new();
  private IRedditWorkerFactory _workerFactory;
  private ILogger<RedditWorkerPool> _logger;

  public RedditWorkerPool(ILogger<RedditWorkerPool> logger, IRedditWorkerFactory workerFactory)
  {
    _workerFactory = workerFactory;
    _logger = logger;
  }

  public IRedditWorker AddWorker(string subreddit)
  {
    if (string.IsNullOrWhiteSpace(subreddit))
    {
      throw new ArgumentNullException(nameof(subreddit));
    }

    if (!_postWorkers.TryGetValue(subreddit, out IRedditWorker? worker))
    {
      worker = _workerFactory.CreateWorker(subreddit);
      _postWorkers.TryAdd(subreddit, worker);
      worker.StartAsync(CancellationToken.None).Wait();
    }
    
    return worker;
  }

  public IRedditWorker? GetWorker(string subreddit)
  {
    if (string.IsNullOrWhiteSpace(subreddit))
    {
      throw new ArgumentNullException(nameof(subreddit));
    }

    _postWorkers.TryGetValue(subreddit, out IRedditWorker? worker);
    return worker;
  }

  public void RemoveWorker(string subreddit)
  {
    if (string.IsNullOrWhiteSpace(subreddit))
    {
      throw new ArgumentNullException(nameof(subreddit));
    }

    if (_postWorkers.TryGetValue(subreddit, out IRedditWorker? worker))
    {
      worker.StopAsync(CancellationToken.None).Wait();
      _postWorkers.TryRemove(subreddit, out _);
    }
  }
}
