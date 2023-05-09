using DataService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Contract;

public interface IPostRepository
{
  void Store(string subreddit, List<RedditPost> posts);

  SubredditInfo? GetInfo(string subreddit);
}
