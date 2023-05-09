using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataService.Model;
using Newtonsoft.Json.Linq;

namespace RedditHelper;

public class Subreddit
{
  private static ConcurrentDictionary<string, string> _tokenCache = new();

  private static string _clientId;
  private static string _clientSecret;
  private static string _username;
  private static string _password;

  static Subreddit()
  {
    _clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID")?? throw new Exception("Reddit client id is not defined");
    _clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET") ?? throw new Exception("Reddit client secret is not defined");
    _username = Environment.GetEnvironmentVariable("REDDIT_USERNAME") ?? throw new Exception("Reddit username is not defined");
    _password = Environment.GetEnvironmentVariable("REDDIT_PASSWORD") ?? throw new Exception("Reddit password is not defined");
  }

  public static async Task<List<RedditPost>> FetchPosts(string subreddit, string sort = "top", int limit = 10)
  {
    if (string.IsNullOrWhiteSpace(subreddit))
    {
      throw new ArgumentNullException(nameof(subreddit));
    }

    string? accessToken = await AccessToken.GetRedditAccessTokenAsync(_clientId, _clientSecret, _username, _password);
    if (accessToken == null)
    {
      // return 403;
    }

    // Step 2: Use the access token to get posts from the subreddit
    using HttpClient client = new();
    client.DefaultRequestHeaders.UserAgent.ParseAdd("JH Reddit App");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    string url = $"https://oauth.reddit.com/r/{subreddit}/{sort}";
    var result = await client.GetAsync(url);
    var json = await result.Content.ReadAsStringAsync();

    // Process the JSON data to get the posts
    JObject data = JObject.Parse(json);

    List<RedditPost> posts = new();
    foreach (var post in data["data"]["children"])
    {
      posts.Add(new RedditPost()
      {
        Subreddit = post["data"]["subreddit"].ToString(),
        Id = post["data"]["id"].ToString(),
        Author = post["data"]["author"].ToString(),
        Name = post["data"]["name"].ToString(),
        Title = post["data"]["title"].ToString(),
        UpVoteRatio = (double)post["data"]["upvote_ratio"],
        Ups = (int)post["data"]["ups"],
        Downs = (int)post["data"]["downs"],
      });
    }

    return posts;
  }
}