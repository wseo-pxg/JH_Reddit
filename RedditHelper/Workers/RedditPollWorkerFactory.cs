using DataService;

namespace RedditHelper.Workers;

public class RedditPollWorkerFactory : IRedditWorkerFactory
{
  private IDataApi _dataApi;
  public RedditPollWorkerFactory(IDataApi dataApi) => (_dataApi) = dataApi;
  public IRedditWorker CreateWorker(string subreddit)
  {
    if (string.IsNullOrWhiteSpace(subreddit))
    {
      throw new ArgumentNullException(nameof(subreddit));
    }

    return new RedditPollWorker(subreddit, dataApi: _dataApi);
  }
}
