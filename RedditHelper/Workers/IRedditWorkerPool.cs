namespace RedditHelper.Workers;

public interface IRedditWorkerPool
{
  IRedditWorker AddWorker(string subreddit);
  IRedditWorker? GetWorker(string subreddit);
  void RemoveWorker(string subreddit);
}