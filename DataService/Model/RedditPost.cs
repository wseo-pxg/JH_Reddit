using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Model;

public class RedditPost
{
  public string Subreddit { get; set; } = default!;
  public string Id { get; set; } = default!;
  public string Name { get; set; } = default!;
  public string Title { get; set; } = default!;
  public string Author { get; set; } = default!;
  public double UpVoteRatio { get; set; }
  public int Ups { get; set; }
  public int Downs { get; set; }
}
