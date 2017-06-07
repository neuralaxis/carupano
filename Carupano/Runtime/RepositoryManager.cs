using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.Runtime
{
    using Model;
    public class RepositoryManager
    {
        readonly IEnumerable<RepositoryModel> Repositories;
        readonly IServiceProvider Services;
        public RepositoryManager(IEnumerable<RepositoryModel> repos, IServiceProvider services)
        {
            Repositories = repos;
            Services = services;
        }

        public void RegisterRepositoryServices()
        {

        }
    }
}
