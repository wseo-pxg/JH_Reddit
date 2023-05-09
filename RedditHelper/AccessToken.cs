using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace RedditHelper;

public class AccessToken
{
  private static ConcurrentDictionary<string, string> _tokenCache = new();

  public static async Task<string> GetRedditAccessTokenAsync(string clientId, string clientSecret, string username, string password)
  {
    if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException(nameof(clientId));
    if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentNullException(nameof(clientSecret));
    if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));
    if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

    string tokenKey = $"{clientId}:{username}";
    if (_tokenCache.ContainsKey(tokenKey))
    {
      return _tokenCache[tokenKey];
    }

    using HttpClient client = new ();
    client.BaseAddress = new Uri("https://www.reddit.com/");

    string accessTokenUrl = "api/v1/access_token";

    client.DefaultRequestHeaders.Authorization = new ("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));
    client.DefaultRequestHeaders.UserAgent.ParseAdd("JH Reddit App");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

    var postData = new Dictionary<string, string>
        {
            {"grant_type", "password"},
            {"username", username},
            {"password", password}
        };

    var response = await client.PostAsync(accessTokenUrl, new FormUrlEncodedContent(postData));
    string responseBody = await response.Content.ReadAsStringAsync();

    JObject json = JObject.Parse(responseBody);
    string? accessToken = json["access_token"]?.Value<string>();
    if (accessToken != null)
    {
      _tokenCache[tokenKey] = accessToken;
      return accessToken;
    }

    throw new Exception("Failed to get access token.");
  }
}