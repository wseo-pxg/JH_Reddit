using Microsoft.Extensions.Hosting;

namespace RedditHelper.Workers;

public interface IRedditWorker : IHostedService, IDisposable
{

}
