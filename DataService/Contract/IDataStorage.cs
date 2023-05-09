using DataService;
using DataService.Model;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace DataService.Contract;


public interface IDataStorage : IHostedService, IDisposable
{
  void Store(RedditPost post);

  SubredditInfo? GetSubredditInfo(string subreddit);
}

