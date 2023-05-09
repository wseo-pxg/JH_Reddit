using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Model;

public class SubredditInfo
{
  public string Name { get; private set; }
  public RedditPost? MostUpVotePost { get; private set; }
  public string? MostPostedUser { get; private set; }
  public int MostPostedCountByUser { get; private set; }
  public Dictionary<string, RedditPost> Posts { get; private set; } = new();

  public SubredditInfo(string subreddit)
  {
    if (string.IsNullOrWhiteSpace(subreddit)) throw new ArgumentNullException(nameof(subreddit));
    Name = subreddit;
  }

  public void Store(RedditPost post)
  {
    Posts[post.Name] = post;
  }

  public void UpdateStatistics()
  {
    if (!Posts.Any())
    {
      return;
    }

    MostUpVotePost = Posts.Values.MaxBy(p => p.Ups)!;
    MostPostedUser = Posts.Values
                          .GroupBy(x => x.Author)
                          .OrderByDescending(g => g.Count())
                          .FirstOrDefault()?.First().Author;
    MostPostedCountByUser = Posts.Values
                          .GroupBy(x => x.Author)
                          .Max(g => g.Count());
  }
}
