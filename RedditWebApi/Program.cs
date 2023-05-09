using RedditHelper;
using RedditHelper.Workers;
using DataService;
using DataService.Contract;
using DataService.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IRedditWorkerFactory, RedditPollWorkerFactory>();
builder.Services.AddSingleton<IRedditWorkerPool, RedditWorkerPool>();
builder.Services.AddSingleton<IDataApi, DataApi>();
builder.Services.AddSingleton<IDataStorage, DataStorage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
