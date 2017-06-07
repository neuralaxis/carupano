using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.Persistence
{
    public interface IRepository<TModel>
    {
        Task<TModel> QuerySingle<TQuery>(TQuery query);
        Task<IEnumerable<TModel>> QueryMany<TQuery>(TQuery query);
    }
}
