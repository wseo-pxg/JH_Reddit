using DataService.Contract;
using DataService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService;

public class DataApi : IDataApi
{
  IDataStorage _storage;
  public DataApi(IDataStorage storage) => _storage = storage;
  public IPostRepository Post => _post ??= new PostRepository(_storage);
  private IPostRepository? _post;
}
