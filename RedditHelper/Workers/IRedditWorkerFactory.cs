using Microsoft.Extensions.Hosting;

namespace RedditHelper.Workers;

public interface IRedditWorkerFactory
{
  IRedditWorker CreateWorker(string subreddit);
}
