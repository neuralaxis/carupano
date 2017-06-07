using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Model
{
    public class RepositoryModel
    {
        public Type Type {get;}
        public ReadModelModel Model { get; }
        public IEnumerable<QueryHandlerModel> QueryHandlers { get; }

        public RepositoryModel(Type repoType, ReadModelModel model, IEnumerable<QueryHandlerModel> queries)
        {
            Type = repoType;
            Model = model;
            QueryHandlers = queries;
        }
    }
}
