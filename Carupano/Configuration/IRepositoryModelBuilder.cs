using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Configuration
{
    using Model;
    public interface IRepositoryModelBuilder
    {
        RepositoryModel Build();
    }
}
