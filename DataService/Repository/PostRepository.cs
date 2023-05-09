using DataService.Contract;
using DataService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Repository;

public class PostRepository : IPostRepository
{
  private IDataStorage _storage;
  public PostRepository(IDataStorage storage) => (_storage) = (storage);

  public SubredditInfo? GetInfo(string subreddit)
  {
    if (string.IsNullOrWhiteSpace(subreddit)) throw new ArgumentNullException(nameof(subreddit));
    return _storage.GetSubredditInfo(subreddit);
  }

  public void Store(string subreddit, List<RedditPost> posts)
  {
    if (string.IsNullOrWhiteSpace(subreddit)) throw new ArgumentNullException(nameof(subreddit));
    if (posts == null || !posts.Any()) return;

    foreach (var post in posts)
    {
      _storage.Store(post);
    }
  }
}
